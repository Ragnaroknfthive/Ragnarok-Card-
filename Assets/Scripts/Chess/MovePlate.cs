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
    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        photonView = GetComponent<PhotonView>();
    }

    public void OnMouseUp()
    {
        if (Game.Get().isLocalPlayerTurn)
        {
            PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(true);
            PVPManager.manager.selectedMove = GetComponent<MovePlate>();
        }
        //if(Game.Get().isLocalPlayerTurn)
        //    photonView.RPC("OnClickRPC",RpcTarget.AllBuffered);
        // controller = GameObject.FindGameObjectWithTag("GameController");

        // if(attack)
        // {
        //     Game.Get().SetPVPMode(true);
        //     reference.DestroyMovePlates();
        //     // GameObject cp = Game.Get().GetPosition(matrixX, matrixY);

        //     // if (cp.name == "white_king") Game.Get().Winner("black");
        //     // if (cp.name == "black_king") Game.Get().Winner("white");

        //     // Destroy(cp);

        //     //Handle the aftereffect of PVP
        // }else{
        //     Game.Get().SetPositionsEmpty(reference.GetXboard(),
        //     reference.GetYboard());

        //     reference.SetXBoard(matrixX);
        //     reference.SetYBoard(matrixY);
        //     reference.SetCoords();

        //     Game.Get().SetPosition(reference);

        //     Game.Get().NextTurn();

        //     reference.DestroyMovePlates();
        // }




    }

    public void MovePiece()
    {
        if (Game.Get().isLocalPlayerTurn){
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



        //Debug.Log("OnPhotonInstantiate is false here");
    }

    #region Pun calls

    [PunRPC]
    public void OnClickRPC()
    {
        if (attack)
        {
            //    Debug.LogError("******Piece Type  " + reference.type);
            reference.DestroyMovePlates();
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    DemoManager.instance.UpdateCards();
            //}
            PhotonNetwork.SendAllOutgoingCommands();
            Game.Get().SetPVPMode(true);
            //PVPManager.manager.SetChessSpriteForPVP();
            bool localplayerTurn = Game.Get().isLocalPlayerTurn;
            if (reference.playerType == PlayerType.White)
            {
                PVPManager.Get().SetData(new Vector2(reference.GetXboard(), reference.GetYboard()), new Vector2(matrixX, matrixY), localplayerTurn, false);
                //Debug.Log("if --------- if");
            }
            else
            {
                PVPManager.Get().SetData(new Vector2(matrixX, matrixY), new Vector2(reference.GetXboard(), reference.GetYboard()), localplayerTurn, true);
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


            //if (Chessman.GetPiecesOfPlayer(PlayerType.White).FindAll(x=>x.type==PieceType.King).Count==0 || Chessman.GetPiecesOfPlayer(PlayerType.Black).FindAll(x => x.type == PieceType.King).Count == 0) 
            //{

            //    Game.Get().CheckWinner();
            //}
            //else 
            //{
            //    //reference.DestroyMovePlates();
            //    Game.Get().SetPVPMode(true);



            //    bool localplayerTurn = Game.Get().isLocalPlayerTurn;
            //    if (reference.playerType == PlayerType.White)
            //    {
            //        PVPManager.Get().SetData(new Vector2(reference.GetXboard(), reference.GetYboard()), new Vector2(matrixX, matrixY), localplayerTurn);
            //        //Debug.Log("if --------- if");
            //    }
            //    else
            //    {
            //        PVPManager.Get().SetData(new Vector2(matrixX, matrixY), new Vector2(reference.GetXboard(), reference.GetYboard()), localplayerTurn);
            //        Debug.Log("else --------- else");
            //    }
            //    // GameObject cp = Game.Get().GetPosition(matrixX, matrixY);

            //    // if (cp.name == "white_king") Game.Get().Winner("black");
            //    // if (cp.name == "black_king") Game.Get().Winner("white");

            //    // Destroy(cp);

            //    //Handle the aftereffect of PVP

            //    Debug.Log("================================== pvp start here =============================");
            //}

        }
        else
        {

            if (FindObjectOfType<Game>()._currnetTurnPlayer == PhotonNetwork.LocalPlayer && reference.type == PieceType.Pawn)
            {
                GameManager.instace.isFristMovePawn = false;

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
