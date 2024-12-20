using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MovePlate : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region Attributes
    [Header("Sprites")]
    [SerializeField] Sprite selectedMove;
    [SerializeField] Sprite attackSprite;
    [SerializeField] SpriteRenderer spriteRenderer;
    public Sprite NormalSprite;
    [Header("Integers")]
    public int matrixX;
    public int matrixY;
    [Header("Photon")]
    private PhotonView photonView;
    [Header("GameObjects")]
    public GameObject controller;
    [Header("Chessman")]
    public Chessman reference = null;
    [Header("Booleans")]
    public bool attack = false;
    #endregion
    
    #region Unity Methods
    public void Start()
    {
        NormalSprite = spriteRenderer.sprite;
        if (attack) gameObject.GetComponent<SpriteRenderer>().sprite = attackSprite; photonView = GetComponent<PhotonView>();
    }
    #endregion

    #region Sprite Setters
    public void SetNormalSprite()
    {
        spriteRenderer.sprite = NormalSprite;
    }
    public void SetSelectedSprite()
    {
        spriteRenderer.sprite = selectedMove;
    }
    #endregion

    #region Piece Methods
    public PieceType GetPieceTypeOnThisPlate()
    {
        PieceType type = PieceType.Pawn;
        GameObject pieceOnthisPlate = Game.Get().GetPosition(matrixY, matrixX);
        if (pieceOnthisPlate) type = pieceOnthisPlate.GetComponent<Chessman>().type;
        return type;
    }
    public void MovePiece()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            Game.Get().IncreaseStamina();
            photonView.RPC("OnClickRPC", RpcTarget.AllBuffered);
        }
    }
    #endregion

    #region Reference Methods
    public void SetReference(Chessman obj)
    {
        reference = obj;
    }
    public Chessman GetReference()
    {
        return reference;
    }
    #endregion

    #region Mouse Methods
    public void OnMouseUp()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(true);
            SetSelectedSprite();//Update selected move sprite
            if (PVPManager.manager.selectedMove) PVPManager.manager.selectedMove.SetNormalSprite();
            PVPManager.manager.selectedMove = GetComponent<MovePlate>();
        }
    }
    #endregion

    #region Coords Methods
    public void SetCoords(int y, int x)//
    {
        matrixY = y;
        matrixX = x;
    }
    #endregion

    #region Instantiate Methods
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        attack = (bool)data[4];
        PlayerType ptype = (PlayerType)data[0];
        PieceType type = (PieceType)data[1];
        SetCoords((int)data[2], (int)data[3]);
        reference = Chessman.GetPiece(ptype, type, (int)data[5]);
    }
    #endregion

    #region Pun calls
    [PunRPC]
    public void OnClickRPC()
    {
        if (attack)
        {
            Debug.Log(reference.type + " Ref Type" + PVPManager.manager.tempPieceOpp);
            if (true/*PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer && PVPManager.manager.tempPieceOpp == PieceType.Pawn ||
                (PhotonNetwork.LocalPlayer != Game.Get()._currnetTurnPlayer && PVPManager.manager.MyAttackedPiece != null && PVPManager.manager.MyAttackedPiece.type == PieceType.Pawn)*/)
            {
                Debug.Log("IN Pawn Condition");
                Game.Get().SetPositionsEmpty(reference.GetYboard(), reference.GetXboard());
                reference.SetXBoard(matrixX);
                reference.SetYBoard(matrixY);
                reference.SetCoords();
                Game.Get().SetPosition(reference);
                Debug.Log("IN Position");
                reference.DestroyMovePlates();
                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurn();
                Game.Get().DestroyPieceObjectForPawnTaken(PVPManager.manager.oppPieceType);
            }
            else
            {
                Debug.Log("NOT IN Pawn Condition");
                reference.DestroyMovePlates();
                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().SetPVPMode(true);
                bool localplayerTurn = Game.Get().isLocalPlayerTurn;
                if (reference.playerType == PlayerType.White) PVPManager.Get().SetData(new Vector2(reference.GetYboard(), reference.GetXboard()), new Vector2(matrixY, matrixX), localplayerTurn, false);
                else
                {
                    PVPManager.Get().SetData(new Vector2(matrixY, matrixX), new Vector2(reference.GetYboard(), reference.GetXboard()), localplayerTurn, true);
                    Debug.Log("else --------- else");
                }
                reference.DestroyMovePlates();
                Debug.Log("================================== pvp start here =============================");
                Debug.Log("*****White Kings " + Chessman.GetPiecesOfPlayer(PlayerType.White).FindAll(x => x.type == PieceType.King).Count);
                Debug.Log("*****Black Kings " + Chessman.GetPiecesOfPlayer(PlayerType.Black).FindAll(x => x.type == PieceType.King).Count);
            }
        }
        else
        {
            print("PieceType else pawn");
            if (reference.type == PieceType.Pawn)
            {
                PawnClass pawn = reference.GetComponent<PawnClass>();
                if (pawn != null) pawn.SetMoved(true);
            }
            Game.Get().SetPositionsEmpty(reference.GetXboard(),
            reference.GetYboard());
            reference.SetXBoard(matrixX);
            reference.SetYBoard(matrixY);
            reference.SetCoords();
            Game.Get().SetPosition(reference);
            if (reference.type == PieceType.Pawn)
            {
                if ((reference.playerType == PlayerType.White && reference.GetYboard() == Game.Get().GetMaxY() - 1) || (reference.playerType == PlayerType.Black && reference.GetYboard() == 0))
                {
                    if (Game.Get().isLocalPlayerTurn)
                    {
                        if (Game.Get().DestroyedObjects.Count > 0) Game.Get().ShowReviveOption(reference);
                        else Game.Get().NextTurn();
                    }
                }
                else Game.Get().NextTurn();
            }
            else Game.Get().NextTurn();
            reference.DestroyMovePlates();
        }
    }
    #endregion
}