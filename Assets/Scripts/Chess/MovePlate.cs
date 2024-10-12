////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: MovePlate.cs
//FileType: C# Source file
//Description : This script is used to handles chess board moves
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using Photon.Pun;

public class MovePlate : MonoBehaviour, IPunInstantiateMagicCallback
{
    Chessman reference = null;                  //Chess piece reference 

    int matrixX;                                //X coordinate on board
    int matrixY;                                //Y coordinate on board
    private PhotonView photonView;              //Photon view - used for network sync
    
    public bool attack = false;                 //True : Incase if this attacked position
    [SerializeField]
    Sprite selectedMove;                        //Sprite for selected move
    [SerializeField]
    Sprite attackSprite;                       //Attacked move sprite
    [SerializeField]
    SpriteRenderer spriteRenderer;             //Sprite renderer reference
    Sprite NormalSprite;                       //Sprite for normal moveplate 
    /// <summary>
    /// Get piece type located on the current moveplate
    /// </summary>
    /// <returns></returns>
    public PieceType GetPieceTypeOnThisPlate()
    {
        PieceType type = PieceType.Pawn;
        GameObject pieceOnthisPlate = Game.Get().GetPosition(matrixX, matrixY);
        if (pieceOnthisPlate)
        {
            type = pieceOnthisPlate.GetComponent<Chessman>().type;
            //     Debug.LogError("ATTACKED PIECE " + type);
        }
        return type;
    }
    /// <summary>
    /// Set moveplate sprite and set photonview reference
    /// </summary>
    public void Start()
    {
        NormalSprite = spriteRenderer.sprite;
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = attackSprite;
           // gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        photonView = GetComponent<PhotonView>();
    }
    /// <summary>
    /// Set noraml sprite in moveplate
    /// </summary>
    public void SetNormalSprite() 
    {
        spriteRenderer.sprite = NormalSprite;
    }
    /// <summary>
    /// Set selected move sprite for current move plate
    /// </summary>
    public void SetSelectedSprite()
    {
        spriteRenderer.sprite = selectedMove;
    }
    /// <summary>
    /// Select  current move plate when user click on move plate
    /// </summary>
    public void OnMouseUp()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(true);
            SetSelectedSprite();//Update selected move sprite
            if(PVPManager.manager.selectedMove) 
            {
                PVPManager.manager.selectedMove.SetNormalSprite();
            }
            PVPManager.manager.selectedMove = GetComponent<MovePlate>();
        }
    }
    /// <summary>
    /// Move piece to this move plate
    /// </summary>
    public void MovePiece()
    {
        if (Game.Get().isLocalPlayerTurn){
            Game.Get().IncreaseStamina();
            photonView.RPC("OnClickRPC", RpcTarget.AllBuffered);
            
        }       
    }

    /// <summary>
    /// Set  coordinates for this plate
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }
    /// <summary>
    /// Set chess piece reference on this plate
    /// </summary>
    /// <param name="obj">chess piece</param>
    public void SetReference(Chessman obj)
    {
        reference = obj;
    }
    /// <summary>
    /// Get chess piece reference on this move plate
    /// </summary>
    /// <returns>Chess piece </returns>
    public Chessman GetReference()
    {
        return reference;
    }
    /// <summary>
    /// This functiona is called when move plate is instantiated on photon netwrok . After instantiate data set for this moveplate
    /// </summary>
    /// <param name="info">Photon information / data for instantiation</param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        attack = (bool)data[4];
        PlayerType ptype = (PlayerType)data[0];
        PieceType type = (PieceType)data[1];
        SetCoords((int)data[2], (int)data[3]);

        reference = Chessman.GetPiece(ptype, type, (int)data[5]);
        //Debug.Log("OnPhotonInstantiate is false here");
    }

    #region Pun calls
    /// <summary>
    /// RPC : called when user click on this Move plate. It triggers' piece movement in chess game.
    /// </summary>
    [PunRPC]
    public void OnClickRPC()
    {
        if (attack)  //If this click is for attack on the chess piece located on this moveplate
        {
            //    Debug.LogError("******Piece Type  " + reference.type);
           
            Debug.LogError(reference.type + " Ref Type" + PVPManager.manager.tempPieceOpp);
            if(PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer &&  PVPManager.manager.tempPieceOpp == PieceType.Pawn||(PhotonNetwork.LocalPlayer != Game.Get(). _currnetTurnPlayer && PVPManager.manager.MyAttackedPiece!=null && PVPManager.manager.MyAttackedPiece.type== PieceType.Pawn )) 
            {
                //no pvp battel
                //Continue turn
                Debug.LogError("IN Pawn Condition");
                //Game.Get().SetPositionsEmpty(reference.GetXboard(),reference.GetYboard());
                Game.Get().SetPositionsEmpty(reference.GetXboard(), reference.GetYboard());
                reference.SetXBoard(matrixX);
                reference.SetYBoard(matrixY);
                reference.SetCoords();
                Game.Get().SetPosition(reference);

                reference.DestroyMovePlates();
                
                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurnContinue();
                Game.Get().DestroyPieceObjectForPawnTaken(PVPManager.manager.oppPieceType);
                
            }
            else
            {
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    DemoManager.instance.UpdateCards();
                //}
                reference.DestroyMovePlates();
                PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().SetPVPMode(true);
                //PVPManager.manager.SetChessSpriteForPVP();
                bool localplayerTurn = Game.Get().isLocalPlayerTurn;
                if(reference.playerType == PlayerType.White)
                {
                    PVPManager.Get().SetData(new Vector2(reference.GetXboard(),reference.GetYboard()),new Vector2(matrixX,matrixY),localplayerTurn,false);
                    //Debug.Log("if --------- if");
                }
                else
                {
                    PVPManager.Get().SetData(new Vector2(matrixX,matrixY),new Vector2(reference.GetXboard(),reference.GetYboard()),localplayerTurn,true);
                    Debug.Log("else --------- else");
                }
                // GameObject cp = Game.Get().GetPosition(matrixX, matrixY);

                // if (cp.name == "white_king") Game.Get().Winner("black");
                // if (cp.name == "black_king") Game.Get().Winner("white");

                // Destroy(cp);

                //Handle the aftereffect of PVP
                reference.DestroyMovePlates();
                
                
                Debug.Log("================================== pvp start here =============================");
                Debug.Log("*****White Kings " + Chessman.GetPiecesOfPlayer(PlayerType.White).FindAll(x => x.type == PieceType.King).Count);
                Debug.Log("*****Black Kings " + Chessman.GetPiecesOfPlayer(PlayerType.Black).FindAll(x => x.type == PieceType.King).Count);
            }
        } 
        else  //Normal move
        {
            if (FindObjectOfType<Game>()._currnetTurnPlayer == PhotonNetwork.LocalPlayer && reference.type == PieceType.Pawn)
            {
                GameManager.instace.isFristMovePawn = false;
                reference.isFirstmove = false;

                //   Debug.LogError("is " + GameManager.instace.isFristMovePawn + " and " + reference.type.ToString());
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
