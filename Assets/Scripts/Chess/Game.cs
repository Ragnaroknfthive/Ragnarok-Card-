////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Game.cs
//FileType: C# Source file
//Description : This script is used to handles chess board setup and other logic related to game
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Networking;
using System;

/// <summary>
///  Player actions in poker game
/// </summary>
public enum PlayerAction { idle, attack, counterAttack, defend, engage, brace }
/// <summary>
/// Spell card position types
/// </summary>
public enum SpellCardPosition { None, petHomePlayer, petHomeOppoent, petBattlePlayer, perBattleOpponent }
public class Game : MonoBehaviour
{
    public GameObject chesspiece;           //Chess piece prefab
    public GameObject movePlate;            //Sqare object used to display available moves for player's piece


    public GameObject Board,RotatedBoardSpriteObject;                               //Chess board gameobject, Rotated baord(for opponent view)
    public List<BoardPosition> boardPositions = new List<BoardPosition>();          //Chess board board positions class list which holds details of x and y coordinate and transform reference for that board position
    [SerializeField] private GameObject[,] positions = new GameObject[8,8];         //Position objects
    [SerializeField] private Chessman[] playerBlack = new Chessman[10];             //Black player pieces
    [SerializeField] private Chessman[] playerWhite = new Chessman[10];             //White player pieces

    [SerializeField] private GameObject[,] Fpositions = new GameObject[8,8];        //Used for chess game functionality 

    private string currentPlayer = "white";                                         //current player type string- white / black

    public bool isLocalPlayerTurn;                                                  //Used to check if it is local player turn or not in chess game
    public GameObject ChessCanvas;                                                  //Chess game UI canvas
    public GameObject PVPCanvas;                                                    //Poker game UI canvas 
    private PhotonView photonView;                                                  //Photon view on this object

    private bool gameOver = false;                                                  //Used to check game over for chess game
    private static Game game;
    public CharacterData defChar;

    public GameObject ColorPlate,ColorPlateIndicator;                               //Chess board box prefab and highlight object prefab            
    public GameObject[,] plates = new GameObject[8,8];                              //List of plates objects on board                          
    public GameObject[,] platesIndicator = new GameObject[8,8];                     //Highlight object for box
    public Color BoardBlack, BoardWhite;                                            //Board colors
    public GameObject board;                                                        //Chess board reference

    public GameObject RematchPopUp, RematchYesNo;                                   //Rematch popup gameobject and Rematch confirmation popup game object
    public Text RematchTxt;                                                         //Rematch text

    public bool RestartButtonClicked;                                              //Used to check if restart button click for chess board

    public static bool setUpCalled;                                                //Booleand used to make sure that setup functionality should be called once

    public GameObject PlayerTurnScreen;                                             //Player turn display canvas 
    public Text PlayerTurnScreenText;                                               //Player turn screen text object
                                                  
    public bool IsDefender = false;                                                 //Used to decide if player is defender or not
    public int turn = 0;
    
    public int BetAmount = 0;                                                       
    public int localBetAmount = 0;                                                  //Used to set hold reference of player bet amount value
    public PlayerAction lastAction = PlayerAction.idle;                             //Player's last action

    /// <summary>
    /// Not used in logic now
    /// </summary>
    [System.Serializable]
    public class PlayerTrunName                                                     
    {
        public Player _player;
        public bool _isTurn;
    }

    public List<PlayerTrunName> _playerTurnList = new List<PlayerTrunName>(); //Not in use now

    public Player _currnetTurnPlayer;                                         //Turn player reference - Photon network player
    public List<int> PlayerStrengths = new List<int>(2);                      //Used to hold player strength (Poker hand strength) list (player and opponent)
    public List<string> PokerHandResults = new List<string>(2) { "","" };    //Pokerhand result string..
    public int MyHighCardValue = 0, OpponentHighCardValue = 0, MySecondHighCardValue = -1, OpponentSecondHighCardValue = -1;
    //Player and Oppoent's  highest and second highest card values
    public List<int> MyHighCardList = new List<int>() { -1,-1,-1,-1,-1 };
    //List of high value cards in player hand
    public List<int> OpponentHighCardList = new List<int>() { -1,-1,-1,-1,-1 };
    //List of high value cards in opponent hand
    public GameObject loadingScreen;                                       //Loading screen object reference

    public List<Chessman> DestroyedObjects = new List<Chessman>();         //Destroyed/beaten chess pieces of player
    public List<Chessman> DestroyedObjectsOppo = new List<Chessman>();     //Destroyed/beaten chess pieces of opponent

    public PlayerType MyType, OppoType;                                    //Player's chess piece type black/ white

    public Text WinnerTxT;                                                 //Winner text object reference for chess game
    public Text RestartTxT;                                                //Restart game text

    public GameObject NewBoard;

    public int MyStamina, OppoStamina;                                      //Player amd Opponent stamina                                

    public Transform myPieces, OppoPieces;                                  //Player and Oppoent beaten pieces position parent transform
    public GameObject DeadPieceImage;                                       //Dead piece prefab

    public TMPro.TextMeshProUGUI playerName, opponentName;                  //Player and Opponent name text object reference for chess game screem
    public Image playerProfileImage, opponentProfileImage;                  //Player and Oppoent profile image in chess game
    public void IncreaseStamina()                                           //Increase player stamina
    {
        MyStamina++;
        MyStamina = Mathf.Clamp(MyStamina,0,10);
        photonView.RPC("IncreaseStaminaRPC",RpcTarget.Others);
    }
    /// <summary>
    /// RPC- stamina increase 
    /// </summary>
    [PunRPC]
    public void IncreaseStaminaRPC()
    {
        OppoStamina++;
        OppoStamina = Mathf.Clamp(OppoStamina,0,10);
    }
    /// <summary>
    /// Get Game script instance
    /// </summary>
    /// <returns></returns>
    public static Game Get()
    {
        return game;
    }
    /// <summary>
    /// Set instance
    /// </summary>
    private void Awake()
    {
        game = this;
    }
    /// <summary>
    /// Get last Y position on chess board
    /// </summary>
    /// <returns>Max Y position on board</returns>
    public int GetMaxY()
    {
        return positions.GetLength(1);
    }

    // Setup chessboard and start chess game
    void Start()
    {
        MyType = PhotonNetwork.LocalPlayer.IsMasterClient ? PlayerType.White : PlayerType.Black;
        OppoType = MyType == PlayerType.White ? PlayerType.Black : PlayerType.White;
        //PhotonNetwork.AutomaticallySyncScene = true;
        PlayerStrengths.Add(0);
        PlayerStrengths.Add(0);
        
        // Get board width and height
        Renderer boardRenderer = board.GetComponent<Renderer>();
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        // Calculate plate dimensions
        int plateRows = plates.GetLength(0);
        int plateCols = plates.GetLength(1);

        float plateWidth = boardWidth / plateCols;
        float plateHeight = boardHeight / plateRows;

        for(int i = 0 ; i < plateRows ; i++)
        {
            for(int j = 0 ; j < plateCols ; j++)
            {
                float plateX = ((i * plateWidth) - (boardWidth / 2) + (plateWidth  / 2) + board.transform.position.x)+0.270f;
                float plateY = ((j * plateHeight ) - (boardHeight / 2) + (plateHeight/ 2) + board.transform.position.y)+0.270f;
                if(i==0 && j == 0) 
                {
                    Debug.LogError("COLOR PLATE + X pos " + plateX + "Y pos " + plateY);
                }
               
                    Vector3 pos1 = Game.Get().boardPositions.Find(x => x.xBoard == i && x.yboard == j).boardPoint.position;
                    plateX = pos1.x;
                    plateY = pos1.y;
               
                plates[i,j] = Instantiate(ColorPlate,new Vector3(plateX,plateY,-0.1f),Quaternion.identity);
                plates[i,j].transform.SetParent(board.transform);
               // plates[i, j].transform.localScale = new Vector3(plateWidth, plateHeight, 1f);
                plates[i,j].transform.localScale = new Vector3(1,1,1f);
                plates[i,j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;

                SpriteRenderer sr = plates[i,j].GetComponent<SpriteRenderer>();

                //Indicators
                platesIndicator[i,j] = Instantiate(ColorPlateIndicator,new Vector3(plateX,plateY,-0.1f),Quaternion.identity);
                platesIndicator[i,j].transform.SetParent(board.transform);
                // plates[i, j].transform.localScale = new Vector3(plateWidth, plateHeight, 1f);
                platesIndicator[i,j].transform.localScale = new Vector3(8.8f,8.8f,8.8f);
                platesIndicator[i,j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;

                SpriteRenderer srIndicator = platesIndicator[i,j].GetComponent<SpriteRenderer>();

                //
                if(sr.color== BoardBlack|| sr.color == BoardWhite) 
                {
                    Color c = sr.color;
                    c.a = 0;
                    sr.color = c;
                    srIndicator.color = c;
                }
                else 
                {
                    Color c = sr.color;
                    c.a = .7f;
                    srIndicator.color = c;
                }
                if(!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    Vector3 pos = plates[i,j].transform.position;
                    pos.x= pos.x*(-1);
                    pos.y =pos.y*(-1);
                    LeanTween.rotate(plates[i,j],new Vector3(0,0,-180),0);
                    LeanTween.rotate(platesIndicator[i,j],new Vector3(0,0,-180),0);

                    // Debug.LogError("Updated");
                }
                
            }
        }


        isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");

        for(int i = 0 ; i < PhotonNetwork.PlayerList.Length ; i++)
        {
            if(PhotonNetwork.PlayerList[i].IsMasterClient && currentPlayer == "white")
            {
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i],_isTurn = true });
            }
            else if(!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white")
            {
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i],_isTurn = false });
            }
        }

        photonView = GetComponent<PhotonView>();
        photonView.RPC("SwitchCurrentPlayer",RpcTarget.AllBuffered,currentPlayer);
        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
        {

            //Camera Settings
            Camera.main.transform.Rotate(Vector3.forward,180f);
            RotatedBoardSpriteObject.transform.Rotate(Vector3.forward,-180);
            RotatedBoardSpriteObject.gameObject.SetActive(true);
            if(NewBoard)
            NewBoard.transform.Rotate(Vector3.forward,180f);
        }
        SetPVPMode(false);

        if(!setUpCalled && PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartCoroutine("setPos");
            setUpCalled = true;
        }

        DestroyedObjects = new List<Chessman>();

        StartCoroutine(SetLoadingScreenOnOff(false,1f));
        gameOver = false;
        MyStamina = 10;
        OppoStamina = 10;
        
    }
    /// <summary>
    /// Enable/Disable loading screen
    /// </summary>
    /// <param name="isOn">True- enable loaing screen, False- Disable loading screen</param>
    /// <param name="delay">waiting time to execute this function</param>
    /// <returns></returns>
    public IEnumerator SetLoadingScreenOnOff(bool isOn,float delay)
    {
        yield return new WaitForSeconds(delay);
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Loading screen set from here");
            photonView.RPC("SetLoadingScreenOff_RPC",RpcTarget.All,isOn);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    [PunRPC]
    public void SetLoadingScreenOff_RPC(bool isOn)
    {
        loadingScreen.SetActive(isOn);
    }

    public void WinPlayer(int i)
    {
        photonView.RPC("WinMatch",RpcTarget.All,i);
    }

    [PunRPC]
    void WinMatch(int i)
    {
        if(i == 1)
        {
            foreach(var item in Chessman.GetPiecesOfPlayer(PlayerType.Black))
            {
                //Destroy(item.gameObject);
                if(item.GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(item.gameObject);
                }
            }
        }
        else if(i == 2)
        {
            foreach(var item in Chessman.GetPiecesOfPlayer(PlayerType.White))
            {
                //Destroy(item.gameObject);
                if(item.GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(item.gameObject);
                }
            }
        }

        StartCoroutine("CheckChessWin");
    }

    IEnumerator setPos()
    {

        //if(!PhotonNetwork.LocalPlayer.IsMasterClient){
        Debug.Log("Spawning Black Pieces");
        playerBlack = new Chessman[] {
            Create("black_rook",PieceType.Rook,PlayerType.Black, 7, 6,17), 
            //Create("black_knight",PieceType.Knight,PlayerType.Black, 1, 7,18),
            Create("black_king",PieceType.King,PlayerType.Black, 7, 7,21),
            Create("black_bishop",PieceType.Bishop,PlayerType.Black, 7, 5,20),
            Create("black_queen",PieceType.Queen,PlayerType.Black, 6, 7,19),
            //Create("black_rook",PieceType.Rook,PlayerType.Black, 7, 7,22), 
            Create("black_knight",PieceType.Knight,PlayerType.Black, 5, 7,23), 
            //Create("black_bishop",PieceType.Bishop,PlayerType.Black, 5, 7,30),
            
            Create("black_pawn",PieceType.Pawn,PlayerType.Black, 5, 6,29),
            Create("black_pawn",PieceType.Pawn,PlayerType.Black, 6, 6,24),
            Create("black_pawn",PieceType.Pawn,PlayerType.Black, 4, 6,33),
            Create("black_pawn",PieceType.Pawn,PlayerType.Black, 6, 5,31),
            Create("black_pawn",PieceType.Pawn,PlayerType.Black, 6, 4,35)
        };
        //Create("black_pawn",PieceType.Pawn,PlayerType.Black, 5, 6,28), Create("black_pawn",PieceType.Pawn,PlayerType.Black, 4, 6,25), Create("black_pawn",PieceType.Pawn,PlayerType.Black, 3, 6,32),
        //Create("black_pawn",PieceType.Pawn,PlayerType.Black, 6, 6,27), Create("black_pawn",PieceType.Pawn,PlayerType.Black, 7, 6,26)};
        //}

        yield return new WaitForSeconds(0.2f);

        //if(PhotonNetwork.LocalPlayer.IsMasterClient){
        Debug.Log("Spawning White Pieces");
        playerWhite = new Chessman[] {
            Create("white_rook",PieceType.Rook,PlayerType.White, 0, 1,3),
            //Create("white_rook",PieceType.Rook,PlayerType.White, 7, 0,6),

            Create("white_knight",PieceType.Knight,PlayerType.White, 2, 0,4),
            //Create("white_knight",PieceType.Knight,PlayerType.White, 6, 0,7),

            Create("white_king",PieceType.King,PlayerType.White, 0, 0,1),
            Create("white_queen",PieceType.Queen,PlayerType.White, 1,0,2),


            Create("white_bishop",PieceType.Bishop,PlayerType.White, 0,2,8),
            //Create("white_bishop",PieceType.Bishop,PlayerType.White, 2, 0,5), 
            
            Create("white_pawn",PieceType.Pawn,PlayerType.White, 1, 1,11),
            Create("white_pawn",PieceType.Pawn,PlayerType.White, 2, 1,10),
            Create("white_pawn",PieceType.Pawn,PlayerType.White, 3, 1,9),
            Create("white_pawn",PieceType.Pawn,PlayerType.White, 1, 2,36),
            Create("white_pawn",PieceType.Pawn,PlayerType.White, 1, 3,37)
        };
        
        yield return new WaitForSeconds(0.2f);

        photonView.RPC("SetPosRPC",RpcTarget.All);
    }
    /// <summary>
    /// RPC- Set positiong of chess pieces 
    /// </summary>
    [PunRPC]
    void SetPosRPC()
    {
        playerBlack = Chessman.GetPiecesOfPlayer(PlayerType.Black).ToArray();
        playerWhite = Chessman.GetPiecesOfPlayer(PlayerType.White).ToArray();
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++");
        Debug.Log(playerBlack.Length + " - " + playerWhite.Length);
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++");

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            foreach(var item in playerBlack)
            {
                //item.gameObject.transform.Rotate(new Vector3(0f,0f,180f));
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,180f));
                item.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }
            foreach(var item in playerWhite)
            {
                //item.gameObject.transform.Rotate(new Vector3(0f,0f,180f));
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,180f));
                //item.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }

        }

        for(int i = 0 ; i < playerBlack.Length ; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }
    /// <summary>
    /// Get character id from chess piece type
    /// </summary>
    /// <param name="type">Chess piece type</param>
    /// <returns>id</returns>
    string getCharId(PieceType type)
    {
        switch(type)
        {
            case PieceType.King:
                return "Fire";
            case PieceType.Queen:
                return "Ether";
            case PieceType.Rook:
                return "Earth";
            case PieceType.Bishop:
                return "Water";
            case PieceType.Knight:
                return "Wind";
            case PieceType.Pawn:
                //string[] elems = new string[]{"Fire","Water","Earth","Wind"};
                return "Human";
            default:
                return "Human";
        }
    }
    /// <summary>
    /// Create chess piece
    /// </summary>
    /// <param name="name">Piece name</param>
    /// <param name="type">Piece type</param>
    /// <param name="ptype">Player type(Black/White)</param>
    /// <param name="x">X position on board</param>
    /// <param name="y">Y position on board</param>
    /// <param name="id">piece id</param>
    /// <param name="cid">character id</param>
    /// <returns></returns>
    public Chessman Create(string name,PieceType type,PlayerType ptype,int x,int y,int id,string cid = null)
    {
        cid = getCharId(type);
        object[] cinit = new object[] { name,type,ptype,x,y,id,cid };
        GameObject obj = PhotonNetwork.Instantiate("chesspiece",new Vector3(0,0,-1),Quaternion.identity,0,cinit) as GameObject;
        return null;
    }

   /// <summary>
   /// Set chess piece position
   /// </summary>
   /// <param name="obj">Chess piece script</param>
    public void SetPosition(Chessman obj)
    {
        positions[obj.GetXboard(),obj.GetYboard()] = obj.gameObject;
        SpriteRenderer sr = plates[obj.GetXboard(),obj.GetYboard()].GetComponent<SpriteRenderer>();
        SpriteRenderer srInidcator = platesIndicator[obj.GetXboard(),obj.GetYboard()].GetComponent<SpriteRenderer>();
        //if(sr.color.a == 0)
        //{
        //    Color c = sr.color;
        //    c.a = 1;
        //    sr.color = c;
        //}
        plates[obj.GetXboard(),obj.GetYboard()].GetComponent<SpriteRenderer>().color = obj.character.tileColor;
      //  if(sr.color.a != 0)
      //  {
            Color c = sr.color;
        Color indicator = c;
            c.a = .9f;
        indicator.a = 0;
            sr.color = indicator;
            srInidcator.color = c;
     //   }

    }
    /// <summary>
    /// Set empty position without chess piece
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    public void SetPositionsEmpty(int x,int y)
    {
        positions[x,y] = null;
        plates[x,y].GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? BoardBlack : BoardWhite;
        SpriteRenderer sr = plates[x,y].GetComponent<SpriteRenderer>();
        SpriteRenderer srIndicator = platesIndicator[x,y].GetComponent<SpriteRenderer>();
        if(sr.color == BoardBlack || sr.color == BoardWhite)
        {
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
            srIndicator.color = c;
        }
    }
    /// <summary>
    /// Get object for the given coordinates on board
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <returns></returns>
    public GameObject GetPosition(int x,int y)
    {
        return positions[x,y];
    }
    /// <summary>
    /// Check if position exists onboard or not
    /// </summary>
    public bool PositionOnBoard(int x,int y)
    {
        if(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }
    /// <summary>
    /// Set present objects on positions
    /// </summary>
    /// <param name="data">Gameobjects on board</param>
    public void SetPresent(GameObject[,] data)
    {
        positions = new GameObject[data.GetLength(0),data.GetLength(1)];
        for(int i = 0 ; i < data.GetLength(0) ; i++)
        {
            for(int j = 0 ; j < data.GetLength(1) ; j++)
            {
                positions[i,j] = data[i,j];
            }
        }
    }
    /// <summary>
    /// Get present objects on oboard
    /// </summary>
    /// <returns>Array of objects</returns>
    public GameObject[,] GetPresent()
    {
        GameObject[,] data = new GameObject[positions.GetLength(0),positions.GetLength(1)];
        for(int i = 0 ; i < positions.GetLength(0) ; i++)
        {
            for(int j = 0 ; j < positions.GetLength(1) ; j++)
            {
                data[i,j] = positions[i,j];
            }
        }
        return data;
    }
    /// <summary>
    /// Get future object list
    /// </summary>
    /// <returns></returns>
    public GameObject[,] GetFuture()
    {
        GameObject[,] data = new GameObject[Fpositions.GetLength(0),Fpositions.GetLength(1)];
        for(int i = 0 ; i < Fpositions.GetLength(0) ; i++)
        {
            for(int j = 0 ; j < Fpositions.GetLength(1) ; j++)
            {
                data[i,j] = Fpositions[i,j];
            }
        }
        return data;
    }
    /// <summary>
    /// Set future object list for chess board
    /// </summary>
    /// <param name="data"></param>
    public void SetFuture(GameObject[,] data)
    {
        Fpositions = new GameObject[data.GetLength(0),data.GetLength(1)];
        for(int i = 0 ; i < data.GetLength(0) ; i++)
        {
            for(int j = 0 ; j < data.GetLength(1) ; j++)
            {
                Fpositions[i,j] = data[i,j];
            }
        }
    }
    /// <summary>
    /// Set future positions for given chess piece
    /// </summary>
    /// <param name="obj">Chess piece</param>
    /// <param name="x">current x position</param>
    /// <param name="y">current y position</param>
    public void SetFuturePosition(Chessman obj,int x,int y)
    {
        Fpositions[x,y] = obj?.gameObject;
    }
    /// <summary>
    /// Set future x position empty for given x and y
    /// </summary>
    public void SetFuturePositionsEmpty(int x,int y)
    {
        Fpositions[x,y] = null;
    }
    /// <summary>
    /// Get single future position object with respect to given x and y position
    /// </summary>
    public GameObject GetFuturePosition(int x,int y)
    {
        return Fpositions[x,y];
    }
    /// <summary>
    /// Returns true if future position exists on board for the given x and y
    /// </summary>
    public bool FuturePositionOnBoard(int x,int y)
    {
        if(x < 0 || y < 0 || x >= Fpositions.GetLength(0) || y >= Fpositions.GetLength(1)) return false;
        return true;
    }

    /// <summary>
    /// Get current player name string
    /// </summary>
    /// <returns>Player name</returns>
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }
    /// <summary>
    /// Get chess game over boolean value
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        return gameOver;
    }
    /// <summary>
    /// Change turn in Chess game
    /// </summary>
    public void NextTurn()
    {
        if(IsGameOver())
        {
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }


        if(currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }

        if(isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if(IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if(IsCheckmate || IsKinginCheck)
            {
                if(MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,0);
                }
            }
            else
            {
                photonView.RPC("SwitchCurrentPlayer",RpcTarget.AllBuffered,currentPlayer);
            }
        }
    }
    /// <summary>
    /// Continue same player turn in chess game
    /// </summary>
    public void  NextTurnContinue()
    {
        if(IsGameOver())
        {
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }


        if(currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }

        if(isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if(IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if(IsCheckmate || IsKinginCheck)
            {
                if(MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,0);
                }
            }
            else
            {
                photonView.RPC("SwitchCurrentPlayer",RpcTarget.AllBuffered,currentPlayer,true);
            }
        }
    }
    //Allow Moveplate display only for local player
    public void Update()
    {
        if(gameOver && Input.GetMouseButtonDown(0) && !RestartButtonClicked)
        {
            RestartClicked();
        }
        if(PhotonNetwork.IsConnected && _currnetTurnPlayer != null && PhotonNetwork.LocalPlayer.NickName != _currnetTurnPlayer.NickName)
        {
            if(GameObject.FindObjectsOfType<MovePlate>() != null)
            {

                MovePlate[] movePlates = GameObject.FindObjectsOfType<MovePlate>();
                foreach(var item in movePlates)
                {
                    item.GetComponent<SpriteRenderer>().enabled = false;
                }

            }
        }

    }
    /// <summary>
    /// Restart Chess game functionality 
    /// </summary>
    public void RestartClicked()
    {
        RestartButtonClicked = true;
        photonView.RPC("ShowRematch",RpcTarget.AllBuffered,PhotonNetwork.LocalPlayer.NickName);
    }
    /// <summary>
    /// Rematch choice RPC trigger
    /// </summary>
    /// <param name="i">Confirmation choice 0- Rejected in case of Reject rematch</param>
    public void RematchChoice(int i)
    {
        if(i == 0)
        {
            photonView.RPC("RematchRejected",RpcTarget.AllBuffered,PhotonNetwork.LocalPlayer);
        }
        else
        {
            photonView.RPC("RestartRPC",RpcTarget.AllBuffered);
        }
    }
    /// <summary>
    /// RPC: Leave room Rematch rejected for chess game
    /// </summary>
    /// <param name="HeWhoGoes"></param>
    [PunRPC]
    void RematchRejected(Player HeWhoGoes)
    {
        RematchYesNo.SetActive(false);
        RematchTxt.text = "Rematch request rejected.";
        StartCoroutine("LeaveRoom");

    }
    /// <summary>
    /// Leave room coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator LeaveRoom()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
    /// <summary>
    /// Show rematch popup
    /// </summary>
    /// <param name="name"></param>
    [PunRPC]
    public void ShowRematch(string name)
    {
        if(PhotonNetwork.LocalPlayer.NickName == name)
        {
            RematchTxt.text = "Waiting for other player.";
            RematchPopUp.SetActive(true);
            RematchYesNo.SetActive(false);
        }
        else
        {
            RematchTxt.text = name + " Want's a Rematch do you want to play again?";
            RematchPopUp.SetActive(true);
            RematchYesNo.SetActive(true);
        }
        RestartButtonClicked = true;
    }

    /// <summary>
    /// RPC : Restart game
    /// </summary>
    [PunRPC]
    void RestartRPC()
    {
        if(gameOver == true)
        {
            gameOver = false;
            setUpCalled = false;
            PhotonNetwork.CleanRpcBufferIfMine(photonView);
            PhotonNetwork.OpRemoveCompleteCache();
            GameManager.instace.isFristMovePawn = true;
            SceneManager.LoadScene("Game");
        }
    }
    /// <summary>
    /// Set winner text for player in chess game
    /// </summary>
    /// <param name="playerWinner">player name</param>
    public void Winner(string playerWinner)
    {
        gameOver = true;
        WinnerTxT.enabled = true;
        WinnerTxT.text = playerWinner + " is the winner";
        RestartTxT.enabled = true;
        PVPManager.manager.TimerObject.SetActive(false);
    }
    /// <summary>
    /// RPC :Setup pvp mode - poker game setup
    /// </summary>
    /// <param name="b">True :Set Poker mode, False: Set chess mode</param>
    public void SetPVPMode(bool b)
    {
        photonView.RPC("SetPVPModeRPC",RpcTarget.All,b);
    }
    /// <summary>
    /// Trigger Chess mode winner RPC
    /// </summary>
    /// <param name="attackerWon">Is attacker won match</param>
    /// <param name="isAttackerMaster">If attacker is masterclient</param>
    /// <param name="p1Pos">player position</param>
    /// <param name="p2Pos">opponent position</param>
    public void HandleWin(bool attackerWon,bool isAttackerMaster,Vector2 p1Pos,Vector2 p2Pos)
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RPC("HandleWinRPC",RpcTarget.All,attackerWon,isAttackerMaster,p1Pos,p2Pos);
    }

    /// <summary>
    /// Close button click event (Leave room and show main menu)
    /// </summary>
    public void CloseClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }


    #region RPC Calls
    /// <summary>
    /// RPC for Handle win/lose condition in chess
    /// </summary>
    /// <param name="attackerWon">Is attacker won match</param>
    /// <param name="isAttackerMaster">If attacker is masterclient</param>
    /// <param name="p1Pos">player position</param>
    /// <param name="p2Pos">opponent position</param>
    [PunRPC]
    public void HandleWinRPC(bool attackerWon,bool isAttackerMaster,Vector2 p1Pos,Vector2 p2Pos)
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RPC("SetPVPModeRPC",RpcTarget.All,false);
        Chessman reference = GetPosition((int)p1Pos.x,(int)p1Pos.y).GetComponent<Chessman>();
        Chessman ob = GetPosition((int)p2Pos.x,(int)p2Pos.y).GetComponent<Chessman>();
        Debug.Log("attacker Won : " + attackerWon + ", attackerMaster :" + isAttackerMaster + ", p1pos : " + p1Pos + ", p2pos : " + p2Pos);
        if(attackerWon)
        {
            if(isAttackerMaster)
            {
                Game.Get().SetPositionsEmpty(reference.GetXboard(),
                reference.GetYboard());
                reference.SetXBoard((int)p2Pos.x);
                reference.SetYBoard((int)p2Pos.y);
                reference.SetCoords();
                SetPosition(reference);
                //Destroy(ob.gameObject);
                //if(ob.GetComponent<PhotonView>().IsMine)  
                DestroyPieceObject(ob);//PhotonNetwork.Destroy(ob.gameObject);
            }
            else
            {
                Game.Get().SetPositionsEmpty(ob.GetXboard(),
                ob.GetYboard());
                ob.SetXBoard((int)p1Pos.x);
                ob.SetYBoard((int)p1Pos.y);
                ob.SetCoords();
                SetPosition(ob);
                //Destroy(reference.gameObject);
                //PhotonNetwork.Destroy(reference.gameObject);
                DestroyPieceObject(reference);
            }
        }
        else
        {
            if(isAttackerMaster)
            {
                Game.Get().SetPositionsEmpty(reference.GetXboard(),
                reference.GetYboard());
                //if(reference.GetComponent<PhotonView>().IsMine)
                DestroyPieceObject(reference);//PhotonNetwork.Destroy(reference.gameObject);
                //Destroy(reference.gameObject);
            }
            else
            {
                Game.Get().SetPositionsEmpty(ob.GetXboard(),
                ob.GetYboard());
                DestroyPieceObject(ob);
                //PhotonNetwork.Destroy(ob.gameObject);
                //Destroy(ob.gameObject);
            }

        }

        if(!IsGameComplete)
            photonView.RPC("SwitchCurrentPlayer",RpcTarget.AllBuffered,currentPlayer);
        else
        {
            StopCoroutine(PVPManager.Get().UpdateChessTurnTimer());
            PVPManager.Get().ChessTurnTimerText.gameObject.SetActive(false);
        }
        //Debug.Log(Chessman.GetPiecesOfPlayer(PlayerType.White).Count+" - "+Chessman.GetPiecesOfPlayer(PlayerType.Black).Count);
        //    if(!IsGameComplete)
        //         NextTurn();

    }
    /// <summary>
    /// Destroy beaten piece object
    /// </summary>
    /// <param name="man">chess piece</param>
    public void DestroyPieceObject(Chessman man)
    {
        //  Debug.LogError("Adding : "+man.type+" to "+MyType);
        //   Debug.LogError("Adding : "+man.playerType+" to "+MyType);
        man.transform.position = new Vector3(1000f,1000f,1000f);

        if(man.playerType == MyType)
        {
            DestroyedObjects.Add(man);

            bool won = false;
            foreach(var item in DestroyedObjects)
            {
                //     Debug.LogError(item.playerType);
                if(item.type == PieceType.King)
                {
                    won = true;
                    if(item.playerType == PlayerType.White)
                    {
                        //Winner(PhotonNetwork.PlayerList[1].NickName);
                        photonView.RPC("PlayerWon",RpcTarget.All,1);
                    }
                    else
                    {
                        //Winner(PhotonNetwork.PlayerList[0].NickName);
                        photonView.RPC("PlayerWon",RpcTarget.All,0);

                    }
                    Debug.LogError("King Taken-Game Over");
                    break;
                }
            }
        
            IsGameComplete = won;
        }
        else
        {
            DestroyedObjectsOppo.Add(man);
        }

        UpdateDeadPieces();
    }
    /// <summary>
    /// Destroy piece object in case of pawn taken
    /// </summary>
    /// <param name="man">chess piece</param>
    public void DestroyPieceObjectForPawnTaken(Chessman man)
    {
        if(PhotonNetwork.LocalPlayer!=_currnetTurnPlayer)
        {
            if(PVPManager.manager.MyAttackedPiece)
            { 
                man = PVPManager.manager.MyAttackedPiece;
            }
            else 
            {
                Debug.LogError("Man not found");
            }
        }
        
        //  Debug.LogError("Adding : "+man.type+" to "+MyType);
        //   Debug.LogError("Adding : "+man.playerType+" to "+MyType);
        man.transform.position = new Vector3(1000f,1000f,1000f);

        if(man.playerType == MyType)
        {
            Debug.LogError("This should not be the case for Pawn taken");
            DestroyedObjects.Add(man);

            bool won = false;
            foreach(var item in DestroyedObjects)
            {
                //     Debug.LogError(item.playerType);
                if(item.type == PieceType.King)
                {
                    won = true;
                    if(item.playerType == PlayerType.White)
                    {
                        //Winner(PhotonNetwork.PlayerList[1].NickName);
                        photonView.RPC("PlayerWon",RpcTarget.All,1);
                    }
                    else
                    {
                        //Winner(PhotonNetwork.PlayerList[0].NickName);
                        photonView.RPC("PlayerWon",RpcTarget.All,0);

                    }
                    Debug.LogError("King Taken-Game Over");
                    break;
                }
            }

            //IsGameComplete = won;
        }
        else
        {
            Debug.LogError("Else part");
            
            DestroyedObjectsOppo.Add(man);
        }
       
       UpdateOtherPlayerDeadPieces();
       // UpdateDeadPieces();
    }
    /// <summary>
    /// Update beaten chess pieces
    /// </summary>
    public void UpdateDeadPieces()
    {
        foreach(Transform item in myPieces)
        {
            Destroy(item.gameObject);
        }
        foreach(Chessman item in DestroyedObjects)
        {
            GameObject o = Instantiate(DeadPieceImage,myPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
        photonView.RPC("UpdateDeadPiecesRPC",RpcTarget.Others);
    }
    /// <summary>
    /// Update other player beaten pieces
    /// </summary>
    public void UpdateOtherPlayerDeadPieces()
    {
        foreach(Transform item in OppoPieces)
        {
            Destroy(item.gameObject);
        }
        foreach(Chessman item in DestroyedObjectsOppo)
        {
            GameObject o = Instantiate(DeadPieceImage,OppoPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
        photonView.RPC("UpdateDeadPiecesMyRPC",RpcTarget.Others);
    }

    /// <summary>
    /// RPC : Update dead piece in network device
    /// </summary>
    [PunRPC]
    public void UpdateDeadPiecesRPC()
    {
        foreach(Transform item in OppoPieces)
        {
            Destroy(item.gameObject);
        }
        foreach(Chessman item in DestroyedObjectsOppo)
        {
            GameObject o = Instantiate(DeadPieceImage,OppoPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
    }
    /// <summary>
    /// Update dead piece in player's device
    /// </summary>
    [PunRPC]
    public void UpdateDeadPiecesMyRPC()
    {
        foreach(Transform item in myPieces)
        {
            Destroy(item.gameObject);
        }
        foreach(Chessman item in DestroyedObjects)
        {
            GameObject o = Instantiate(DeadPieceImage,myPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
    }

    public bool IsGameComplete = false;     // Used in chess game

    Chessman pawntobeRenewed;               //Pawn object reference for revive functionality
    public Transform revivePr;
    public GameObject reviveLisItem;        //Revive item
    public GameObject PieceSelectionPanel;  // Chess piece selection panel
    /// <summary>
    /// Show revive option for given chess piece
    /// </summary>
    /// <param name="pawn">Chess piece</param>
    public void ShowReviveOption(Chessman pawn)
    {
        pawntobeRenewed = pawn;
        foreach(Transform item in revivePr)
        {
            Destroy(item.gameObject);
        }
        int i = 0;
        foreach(var item in DestroyedObjects)
        {
            GameObject o = Instantiate(reviveLisItem,revivePr);
            o.GetComponentInChildren<Image>().sprite = item.GetSprite();
            o.GetComponent<RevivePieceItem>().id = i;
            i++;
        }
        PieceSelectionPanel.SetActive(true);

    }
    /// <summary>
    /// Change pawn to new item in case or revive functionality
    /// </summary>
    /// <param name="i">int piece id</param>
    public void ChangePawnToNewPiece(int i)
    {
        Chessman pawn = pawntobeRenewed;
        PieceSelectionPanel.SetActive(false);
        if(DestroyedObjects != null)
        {
            Chessman c = DestroyedObjects[i];

            Create("New_" + c.gameObject.name,c.type,c.playerType,pawn.GetXboard(),pawn.GetYboard(),30 + DestroyedObjects.Count);
            //Chessman.pieces.Remove(pawn);
            DestroyedObjects.Remove(c);
            photonView.RPC("DestroyPiece",RpcTarget.All,c.PieceIndex);
            photonView.RPC("DestroyPiece",RpcTarget.All,pawn.PieceIndex);
            photonView.RPC("SetPosRPC",RpcTarget.All);
            Game.Get().NextTurn();
        }
    }
    /// <summary>
    /// Destroy chess piece with ginve index
    /// </summary>
    /// <param name="PieceIndex">Index of chess piece</param>
    [PunRPC]
    public void DestroyPiece(int PieceIndex)
    {
        Chessman piece = Chessman.GetPiece(PieceIndex);
        List<Chessman> blacks = playerBlack.ToList();
        if(blacks.Contains(piece))
            blacks.Remove(piece);
        playerBlack = blacks.ToArray();

        List<Chessman> whites = playerWhite.ToList();
        if(whites.Contains(piece))
            whites.Remove(piece);
        playerWhite = whites.ToArray();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            PhotonNetwork.Destroy(piece.GetComponent<PhotonView>());

    }
    /// <summary>
    /// Not in use . but not removed to avoid any errors
    /// </summary>
    public void CheckWinner()
    {
        //StartCoroutine(CheckChessWin());
    }
    /// <summary>
    /// Not in use . but not removed to avoid any errors
    /// </summary>
    void CheckChessWin()
    {
        //yield return new WaitForSeconds(0.2f);

        //  Debug.LogError(playerWhite.ToList().FindAll(x=>x.type== PieceType.King).Count + " _ " + playerBlack.ToList().FindAll(x => x.type == PieceType.King).Count);
        //  Debug.LogError(DestroyedObjects.Count);
        bool won = false;
        foreach(var item in DestroyedObjects)
        {
            //    Debug.LogError(item.type);
            if(item.type == PieceType.King)
            {
                won = true;
                if(item.playerType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,0);
                }
                break;
            }
        }
      
        if(!won)
        {
            photonView.RPC("PlayerWon",RpcTarget.Others,-1);
        }
    }
    /// <summary>
    /// RPC  : winner , not used 
    /// </summary>
    /// <param name="i"></param>
    [PunRPC]
    public void PlayerWon(int i)
    {
        if(i != -1)
            Winner(PhotonNetwork.PlayerList[i].NickName);
        else
            NextTurn();
    }

    /// <summary>
    /// RPC : setup chess / poker canvas 
    /// </summary>
    /// <param name="b">True : show poker canvas,  False: show chess canvas</param>
    [PunRPC]
    public void SetPVPModeRPC(bool b)
    {
        if(b)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PVPManager.Get().player1.GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName;
                PVPManager.Get().player2.GetComponent<Text>().text = PhotonNetwork.PlayerList[1].NickName;
            }
            else
            {
                PVPManager.Get().player1.GetComponent<Text>().text = PhotonNetwork.PlayerList[1].NickName;
                PVPManager.Get().player2.GetComponent<Text>().text = PhotonNetwork.PlayerList[0].NickName;
            }
            ChessCanvas.SetActive(false);
            Game.Get().Board.SetActive(false);
            PVPCanvas.SetActive(true);
        }
        else
        {
            PVPCanvas.SetActive(false);
            ChessCanvas.SetActive(true);
            // GameManager.instace.isFristMovePawn = true;
            Game.Get().Board.SetActive(true);

            if(PhotonNetwork.IsMasterClient)
            {
                playerName.text = PhotonNetwork.PlayerList[0].NickName;
                opponentName.text = PhotonNetwork.PlayerList[1].NickName;
                SetProfileImageForPlayer(true,PhotonNetwork.PlayerList[0]); //Set player profile pic
                SetProfileImageForPlayer(false,PhotonNetwork.PlayerList[1]);// Set oppoenet profile pic
            }
            else
            {
                playerName.text = PhotonNetwork.PlayerList[1].NickName;
                opponentName.text = PhotonNetwork.PlayerList[0].NickName;
                SetProfileImageForPlayer(true,PhotonNetwork.PlayerList[1]); //Set player profile pic
                SetProfileImageForPlayer(false,PhotonNetwork.PlayerList[0]);// Set oppoenet profile pic
            }
        }
    }
    /// <summary>
    ///  Set profile image in chess UI 
    /// </summary>
    /// <param name="isplayer">True : for local player, False: opponent</param>
    /// <param name="player">Photon player</param>
    public void SetProfileImageForPlayer(bool isplayer,Player player) 
    {
        ExitGames.Client.Photon.Hashtable _playerCustomProperties = player.CustomProperties;
        if(isplayer)
        {  //SetProfileImage for player
            if(Convert.ToInt32(_playerCustomProperties[GameData.hasProfileConst].ToString()) == 1)
            {
                playerProfileImage.sprite = GameData.playerSprite;
            }
            else
            {
                int dummypicIndex = -1;
                dummypicIndex = Convert.ToInt32(_playerCustomProperties[GameData.dummyProfileImageIndex].ToString());
                if(dummypicIndex != -1)
                {
                    playerProfileImage.sprite = GameData.Get().DummyProfile[dummypicIndex];
                }
            }
        }
        else
        {
            Debug.LogError("Not Player");
            if(Convert.ToInt32(_playerCustomProperties[GameData.hasProfileConst].ToString()) == 1)
            {
                if(GameData.opponentSprite == GameData.Get().DummyProfile[0])
                {
                    Debug.LogError("Opponet has profile pic");
                    StartCoroutine(FetchOpponentProfilePicture(_playerCustomProperties[GameData.profileUrl].ToString()));
                }
                else 
                {
                  opponentProfileImage.sprite=  GameData.opponentSprite;
                    Debug.LogError("Opponet has profile pic" + "But not set");
                }
            }
            else
            {
                int dummypicIndex = -1;
                dummypicIndex = Convert.ToInt32(_playerCustomProperties[GameData.dummyProfileImageIndex].ToString());
                if(dummypicIndex != -1)
                {
                    opponentProfileImage.sprite = GameData.Get().DummyProfile[dummypicIndex];
                }
            }
        }
    }
    /// <summary>
    /// Not in use
    /// </summary>
    public void SwitchPlayerTurn_RPCCall()
    {
    }
    /// <summary>
    /// Switch current player in chess game
    /// </summary>
    /// <param name="player">player name string</param>
    [PunRPC]
    public void SwitchCurrentPlayer(string player)
    {

        currentPlayer = player;
        //isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if(currentPlayer == "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName,PhotonNetwork.PlayerList[0]));
        }
        if(currentPlayer != "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName,PhotonNetwork.PlayerList[1]));
        }

        if(isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if(IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if(IsCheckmate)
            {
                if(MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,0);
                }
            }
        }
    }
    /// <summary>
    /// RPC  : Switch current player
    /// </summary>
    /// <param name="player">player name</param>
    [PunRPC]
    public void SwitchCurrentPlayer(string player,bool continueTurn)
    {

        currentPlayer = player;
        //isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if(currentPlayer == "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName,PhotonNetwork.PlayerList[0]));
        }
        if(currentPlayer != "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName,PhotonNetwork.PlayerList[1]));
        }

        if(isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if(IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if(IsCheckmate)
            {
                if(MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon",RpcTarget.Others,0);
                }
            }
            else 
            {
                NextTurn();
            }
        }
    }
    /// <summary>
    /// Check for  king piece 
    /// </summary>
    public bool checkForKing()
    {
        List<Chessman> OppoPieces = Chessman.GetPiecesOfPlayer(OppoType);
        bool isCheck = false;
        foreach(var item in OppoPieces)
        {
            isCheck = item.IsCheckforKing();
            if(isCheck) { Debug.LogError("Check by Opponent's " + item.type); break; }
        }
        return isCheck;
    }
    /// <summary>
    /// Used to check checkmate
    /// </summary>
    /// <returns>True in case for checkmate</returns>
    public bool IsCheckmateForKing()
    {
        bool KingLives = true;
        List<Chessman> pieces = Chessman.GetPiecesOfPlayer(MyType);
        List<PieceType> types_lis = new List<PieceType>() { PieceType.King,PieceType.Queen,PieceType.Rook,PieceType.Bishop,PieceType.Knight,PieceType.Pawn };
        foreach(var Ptype in types_lis)
        {
            Chessman piece = pieces.Find((t) => t.type == Ptype);
            KingLives = piece.canDefendKing();
            if(KingLives) { Debug.LogError("King Defended by " + piece.type); break; }
        }
        return !KingLives;
    }
    /// <summary>
    /// Used to decide if it's localplayer turn in chess game
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool isMyTurn(string player)
    {
        return Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn;
    }
    /// <summary>
    /// Coroutine to show turn player name
    /// </summary>
    /// <param name="namePlayer">Player name</param>
    /// <param name="_player">Photon player</param>
    /// <returns></returns>
    private IEnumerator COR_playerTurnNameShow(string namePlayer,Player _player)
    {
        PieceSelectionPanel.SetActive(false);
        PlayerTurnScreen.SetActive(true);
        PlayerTurnScreenText.text = namePlayer + "turn";
        yield return new WaitForSecondsRealtime(1f);
        PlayerTurnScreen.SetActive(false);
        PlayerTurnScreenText.text = "";
        Debug.Log("------------------ Game Turn Change ----------------------");
        _currnetTurnPlayer = _player;
        isLocalPlayerTurn = _currnetTurnPlayer.IsLocal;
        //Info: Indicate player's turn;
        PVPManager.Get().p1Outline.gameObject.SetActive(_currnetTurnPlayer.IsLocal ? true : false);
        PVPManager.Get().p2Outline.gameObject.SetActive(_currnetTurnPlayer.IsLocal ? false : true);
        PVPManager.Get().chessTurnIndicator.gameObject.SetActive(_currnetTurnPlayer.IsLocal ? true : false);

        if(ChessCanvas.activeSelf)
        {
            if(_currnetTurnPlayer.IsLocal)
            { PVPManager.manager.StartChessTimer(); }
            else
            {
                if(PVPManager.manager.moveChoiceConfirmation.activeSelf)
                {
                    PVPManager.manager.moveChoiceConfirmation.SetActive(false);
                }
                PVPManager.manager.TimerObject.SetActive(false);
            }
        }

    }
    /// <summary>
    /// Not in use
    /// </summary>
    [PunRPC]
    public void InitiateMovePlatesRPC()
    {

    }
    /// <summary>
    /// Update last selected action by layer
    /// </summary>
    public void UpdateLastAction(PlayerAction action)
    {
        PVPManager.Get().updatePlayerAction(action.ToString());
        PVPManager.Get().LastActionUpdated = true;
        photonView.RPC("UpdateLastAction_RPC",RpcTarget.All,(byte)action);
    }
    /// <summary>
    /// RPC : Update action
    /// </summary>
    /// <param name="action"></param>
    [PunRPC]
    public void UpdateLastAction_RPC(byte action)
    {
        lastAction = (PlayerAction)action;
    }
    #endregion
    /// <summary>
    /// Fetch profile picture from Url and create sprite
    /// </summary>
    /// <param name="url">image url</param>
    private IEnumerator FetchOpponentProfilePicture(string url)
    {
        Debug.Log("Fetching profile picture from URL: " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed to download profile picture: " + www.error);
        }
        else
        {
            Debug.Log("Successfully downloaded profile picture.");
            Texture2D texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            GameData.opponentProfileTexture = texture2D; //((DownloadHandlerTexture)www.downloadHandler).texture;
            GameData.opponentSprite = GameData.SpriteFromTexture2D(texture2D);
            opponentProfileImage.sprite = GameData.opponentSprite;
        }
    }
}
/// <summary>
/// Board position class
/// </summary>
[System.Serializable]
public class BoardPosition 
{
  public  int xBoard,yboard;
    public Transform boardPoint;
}