using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MovePlate : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject controller;

    Chessman reference = null;

    int matrixX;
    int matrixY;
    private PhotonView photonView;

    public bool attack = false;
    [SerializeField]
    Sprite selectedMove;
    [SerializeField]
    Sprite attackSprite;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    Sprite NormalSprite;

    public PieceType GetPieceTypeOnThisPlate()
    {
        PieceType type = PieceType.Pawn;
        GameObject pieceOnthisPlate = Game.Get().GetPosition(matrixX, matrixY);
        if (pieceOnthisPlate)
        {
            type = pieceOnthisPlate.GetComponent<Chessman>().type;
        }
        return type;
    }
    public void Start()
    {
        NormalSprite = spriteRenderer.sprite;
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = attackSprite;
        }
        photonView = GetComponent<PhotonView>();
    }
    public void SetNormalSprite()
    {
        spriteRenderer.sprite = NormalSprite;
    }
    public void SetSelectedSprite()
    {
        spriteRenderer.sprite = selectedMove;
    }
    public void OnMouseUp()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(true);
            SetSelectedSprite();//Update selected move sprite
            if (PVPManager.manager.selectedMove)
            {
                PVPManager.manager.selectedMove.SetNormalSprite();
            }
            PVPManager.manager.selectedMove = GetComponent<MovePlate>();
        }
    }

    public void MovePiece()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            Game.Get().IncreaseStamina();
            photonView.RPC("OnClickRPC", RpcTarget.AllBuffered);

        }

    }




    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(Chessman obj)
    {
        reference = obj;
    }

    public Chessman GetReference()
    {
        return reference;
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        attack = (bool)data[4];
        PlayerType ptype = (PlayerType)data[0];
        PieceType type = (PieceType)data[1];
        SetCoords((int)data[2], (int)data[3]);
        reference = Chessman.GetPiece(ptype, type, (int)data[5]);
    }

    #region Pun calls

    [PunRPC]
    public void OnClickRPC()
    {
        if (attack)
        {
            Debug.LogError(reference.type + " Ref Type" + PVPManager.manager.tempPieceOpp);
            if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer && PVPManager.manager.tempPieceOpp == PieceType.Pawn || (PhotonNetwork.LocalPlayer != Game.Get()._currnetTurnPlayer && PVPManager.manager.MyAttackedPiece != null && PVPManager.manager.MyAttackedPiece.type == PieceType.Pawn))
            {
                Debug.LogError("IN Pawn Condition");
                Game.Get().SetPositionsEmpty(reference.GetXboard(), reference.GetYboard());
                reference.SetXBoard(matrixX);
                reference.SetYBoard(matrixY);
                reference.SetCoords();
                Game.Get().SetPosition(reference);

                reference.DestroyMovePlates();

                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurn();
                Game.Get().DestroyPieceObjectForPawnTaken(PVPManager.manager.oppPieceType);

            }
            else
            {
                reference.DestroyMovePlates();
                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().SetPVPMode(true);
                bool localplayerTurn = Game.Get().isLocalPlayerTurn;
                if (reference.playerType == PlayerType.White)
                {
                    PVPManager.Get().SetData(new Vector2(reference.GetXboard(), reference.GetYboard()), new Vector2(matrixX, matrixY), localplayerTurn, false);
                }
                else
                {
                    PVPManager.Get().SetData(new Vector2(matrixX, matrixY), new Vector2(reference.GetXboard(), reference.GetYboard()), localplayerTurn, true);
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
            if (reference.type == PieceType.Pawn)
            {
                PawnClass pawn = reference.GetComponent<PawnClass>();
                if (pawn != null)
                {
                    pawn.SetMoved(true);
                }
            }

            Game.Get().SetPositionsEmpty(reference.GetXboard(),
            reference.GetYboard());

            reference.SetXBoard(matrixX);
            reference.SetYBoard(matrixY);
            reference.SetCoords();

            Game.Get().SetPosition(reference);

            if (reference.type == PieceType.Pawn)
            {
                //   Debug.LogError(reference.type + " - "+ reference.GetYboard()+" - "+ Game.Get().GetMaxY());
                if ((reference.playerType == PlayerType.White && reference.GetYboard() == Game.Get().GetMaxY() - 1) || (reference.playerType == PlayerType.Black && reference.GetYboard() == 0))
                {
                    if (Game.Get().isLocalPlayerTurn)
                    {
                        if (Game.Get().DestroyedObjects.Count > 0)
                            Game.Get().ShowReviveOption(reference);
                        else
                            Game.Get().NextTurn();
                    }
                }
                else
                {
                    Game.Get().NextTurn();
                }
            }
            else
            {
                Game.Get().NextTurn();
            }
            reference.DestroyMovePlates();
        }
    }



    #endregion
}
