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

public enum PlayerAction { idle, attack, counterAttack, defend, engage, brace }
public enum SpellCardPosition { None, petHomePlayer, petHomeOppoent, petBattlePlayer, perBattleOpponent }
public class Game : MonoBehaviour
{
    [Header("Cesar's Variables")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject chessBg;

    [Header("")]
    [Header("Other Variables")]
    public GameObject chesspiece;
    public GameObject movePlate;
    public GameObject Board, RotatedBoardSpriteObject;
    public List<BoardPosition> boardPositions = new List<BoardPosition>();
    [SerializeField] private GameObject[,] positions = new GameObject[8, 8];
    [SerializeField] private Chessman[] playerBlack = new Chessman[10];
    [SerializeField] private Chessman[] playerWhite = new Chessman[10];
    [SerializeField] private GameObject[,] Fpositions = new GameObject[8, 8];
    private string currentPlayer = "white";
    public bool isLocalPlayerTurn;
    public GameObject ChessCanvas;
    public GameObject PVPCanvas;
    private PhotonView photonView;
    private bool gameOver = false;
    private static Game game;
    public CharacterData defChar;
    public GameObject ColorPlate, ColorPlateIndicator;
    public GameObject[,] plates = new GameObject[8, 8];
    public GameObject[,] platesIndicator = new GameObject[8, 8];
    public Color BoardBlack, BoardWhite;
    public GameObject board;
    public GameObject RematchPopUp, RematchYesNo;
    public Text RematchTxt;
    public bool RestartButtonClicked;
    public static bool setUpCalled;
    public GameObject PlayerTurnScreen;
    public Text PlayerTurnScreenText;
    [Header(" ")]
    [Header("Poker Variables")]
    public string LastAttackerColor = "";
    public bool IsDefender = false;
    public int turn = 0;
    public float HealthDemage = 0;
    public int BetAmount = 0;
    public int localBetAmount = 0;
    public PlayerAction lastAction = PlayerAction.idle;
    [System.Serializable]
    public class PlayerTrunName
    {
        public Player _player;
        public bool _isTurn;
    }
    public List<PlayerTrunName> _playerTurnList = new List<PlayerTrunName>();
    public Player _currnetTurnPlayer;
    public List<int> PlayerStrengths = new List<int>(2);
    public List<string> PokerHandResults = new List<string>(2) { "", "" };
    public int MyHighCardValue = 0, OpponentHighCardValue = 0, MySecondHighCardValue = -1, OpponentSecondHighCardValue = -1;
    public List<int> MyHighCardList = new List<int>() { -1, -1, -1, -1, -1 };
    public List<int> OpponentHighCardList = new List<int>() { -1, -1, -1, -1, -1 };
    public GameObject loadingScreen;
    public List<Chessman> DestroyedObjects = new List<Chessman>();
    public List<Chessman> DestroyedObjectsOppo = new List<Chessman>();
    public PlayerType MyType, OppoType;
    public Text WinnerTxT;
    public Text RestartTxT;
    public GameObject NewBoard;
    public int MyStamina, OppoStamina;
    public Transform myPieces, OppoPieces;
    public GameObject DeadPieceImage;
    public TMPro.TextMeshProUGUI playerName, opponentName;
    public Image playerProfileImage, opponentProfileImage;
    public void IncreaseStamina()
    {
        Debug.Log("DebugLog_1: Stamina incresed");
        MyStamina++;
        MyStamina = Mathf.Clamp(MyStamina, 0, 10);
        photonView.RPC("IncreaseStaminaRPC", RpcTarget.Others);
    }
    [PunRPC]
    public void IncreaseStaminaRPC()
    {
        Debug.Log("DebugLog_2");
        OppoStamina++;
        OppoStamina = Mathf.Clamp(OppoStamina, 0, 10);
    }

    public static Game Get()
    {
        //Debug.Log("DebugLog_3");
        return game;
    }

    public void Awake()
    {
        Debug.Log("DebugLog_4");
        game = this;
    }

    public void Start()
    {
        Debug.Log("DebugLog_5");
        foreach (var item in PhotonNetwork.PlayerList)
        {

        }
        MyType = PhotonNetwork.LocalPlayer.IsMasterClient ? PlayerType.White : PlayerType.Black;
        OppoType = MyType == PlayerType.White ? PlayerType.Black : PlayerType.White;
        //PhotonNetwork.AutomaticallySyncScene = true;
        PlayerStrengths.Add(0);
        PlayerStrengths.Add(0);
        //if(PhotonNetwork.IsMasterClient) 
        //{
        //    playerName.text = PhotonNetwork.PlayerList[0].NickName;
        //    opponentName.text = PhotonNetwork.PlayerList[1].NickName;
        //}
        //else 
        //{
        //    playerName.text = PhotonNetwork.PlayerList[1].NickName;
        //    opponentName.text = PhotonNetwork.PlayerList[0].NickName;
        //}
        // Get board width and height
        Renderer boardRenderer = board.GetComponent<Renderer>();
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        // Calculate plate dimensions
        int plateRows = plates.GetLength(0);
        int plateCols = plates.GetLength(1);

        float plateWidth = boardWidth / plateCols;
        float plateHeight = boardHeight / plateRows;

        for (int i = 0; i < plateRows; i++)
        {
            Debug.Log("DebugLog_6");
            for (int j = 0; j < plateCols; j++)
            {
                Debug.Log("DebugLog_7");
                float plateX = ((i * plateWidth) - (boardWidth / 2) + (plateWidth / 2) + board.transform.position.x) + 0.270f;
                float plateY = ((j * plateHeight) - (boardHeight / 2) + (plateHeight / 2) + board.transform.position.y) + 0.270f;
                if (i == 0 && j == 0)
                {
                    Debug.Log("DebugLog_8");
                    Debug.LogError("COLOR PLATE + X pos " + plateX + "Y pos " + plateY);
                }
                //float plateX = ((i * plateWidth-.150f) - (boardWidth / 2) + (plateWidth - .150f / 2) + board.transform.position.x);// - 0.150f);  ;//-0.200f;
                //float plateY = ((j * plateHeight-0.300f) - (boardHeight / 2) + (plateHeight - 0.300f / 2) + board.transform.position.y) ;//- 0.300f); ;//-0.500f;
                //if(i == 0 && j < 4)
                // {
                Vector3 pos1 = Game.Get().boardPositions.Find(x => x.xBoard == i && x.yboard == j).boardPoint.position;
                plateX = pos1.x;
                plateY = pos1.y;
                //}
                plates[i, j] = Instantiate(ColorPlate, new Vector3(plateX, plateY, -0.1f), Quaternion.identity);
                plates[i, j].transform.SetParent(board.transform);
                // plates[i, j].transform.localScale = new Vector3(plateWidth, plateHeight, 1f);
                plates[i, j].transform.localScale = new Vector3(1, 1, 1f);
                plates[i, j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;

                SpriteRenderer sr = plates[i, j].GetComponent<SpriteRenderer>();

                //Indicators
                platesIndicator[i, j] = Instantiate(ColorPlateIndicator, new Vector3(plateX, plateY, -0.1f), Quaternion.identity);
                platesIndicator[i, j].transform.SetParent(board.transform);
                // plates[i, j].transform.localScale = new Vector3(plateWidth, plateHeight, 1f);
                platesIndicator[i, j].transform.localScale = new Vector3(8.8f, 8.8f, 8.8f);
                platesIndicator[i, j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;

                SpriteRenderer srIndicator = platesIndicator[i, j].GetComponent<SpriteRenderer>();

                //
                if (sr.color == BoardBlack || sr.color == BoardWhite)
                {
                    Color c = sr.color;
                    c.a = 0;
                    sr.color = c;
                    srIndicator.color = c;
                    Debug.Log("DebugLog_9");
                }
                else
                {
                    Debug.Log("DebugLog_10");
                    Color c = sr.color;
                    c.a = .7f;
                    srIndicator.color = c;
                }
                if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    Debug.Log("DebugLog_11");
                    Vector3 pos = plates[i, j].transform.position;
                    pos.x = pos.x * (-1);
                    pos.y = pos.y * (-1);
                    LeanTween.rotate(plates[i, j], new Vector3(0, 0, -180), 0);
                    LeanTween.rotate(platesIndicator[i, j], new Vector3(0, 0, -180), 0);

                    // Debug.LogError("Updated");
                }

            }
        }


        isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsMasterClient && currentPlayer == "white")
            {
                Debug.Log("DebugLog_12");
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i], _isTurn = true });
            }
            else if (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white")
            {
                Debug.Log("DebugLog_13");
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i], _isTurn = false });
            }
        }

        photonView = GetComponent<PhotonView>();
        photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("DebugLog_14");
            //Camera Settings
            Camera.main.transform.Rotate(Vector3.forward, 180f);
            RotatedBoardSpriteObject.transform.Rotate(Vector3.forward, -180);
            background.transform.Rotate(Vector3.forward, 180f);
            chessBg.transform.Rotate(Vector3.forward, 180f);
            RotatedBoardSpriteObject.gameObject.SetActive(true);
            if (NewBoard) NewBoard.transform.Rotate(Vector3.forward, 180f);
        }
        SetPVPMode(false);

        if (!setUpCalled && PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("DebugLog_15");
            StartCoroutine("setPos");
            setUpCalled = true;
        }

        DestroyedObjects = new List<Chessman>();

        StartCoroutine(SetLoadingScreenOnOff(false, 1f));
        gameOver = false;
        MyStamina = 10;
        OppoStamina = 10;
    }

    public void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0) && !RestartButtonClicked)
        {
            Debug.Log("DebugLog_16");
            RestartClicked();
        }
        if (PhotonNetwork.IsConnected && _currnetTurnPlayer != null && PhotonNetwork.LocalPlayer.NickName != _currnetTurnPlayer.NickName)
        {
            if (GameObject.FindObjectsOfType<MovePlate>() != null)
            {
                Debug.Log("DebugLog_17");
                MovePlate[] movePlates = GameObject.FindObjectsOfType<MovePlate>();
                foreach (var item in movePlates)
                {
                    Debug.Log("DebugLog_18");
                    item.GetComponent<SpriteRenderer>().enabled = false;
                }

            }
        }

    }

    public int GetMaxY()
    {
        Debug.Log("DebugLog_19");
        return positions.GetLength(1);
    }

    // Start is called before the first frame update

    public IEnumerator SetLoadingScreenOnOff(bool isOn, float delay)
    {
        Debug.Log("DebugLog_20");
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("DebugLog_21");
            Debug.LogError("Loading screen set from here");
            photonView.RPC("SetLoadingScreenOff_RPC", RpcTarget.All, isOn);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    [PunRPC]
    public void SetLoadingScreenOff_RPC(bool isOn)
    {
        Debug.Log("DebugLog_22");
        loadingScreen.SetActive(isOn);
    }

    public void WinPlayer(int i)
    {
        Debug.Log("DebugLog_23");
        photonView.RPC("WinMatch", RpcTarget.All, i);
    }

    [PunRPC]
    void WinMatch(int i)
    {
        Debug.Log("DebugLog_24");
        if (i == 1)
        {
            Debug.Log("DebugLog_25");
            foreach (var item in Chessman.GetPiecesOfPlayer(PlayerType.Black))
            {
                Debug.Log("DebugLog_26");
                //Destroy(item.gameObject);
                if (item.GetComponent<PhotonView>().IsMine)
                {
                    Debug.Log("DebugLog_27");
                    PhotonNetwork.Destroy(item.gameObject);
                }
            }
        }
        else if (i == 2)
        {
            Debug.Log("DebugLog_28");
            foreach (var item in Chessman.GetPiecesOfPlayer(PlayerType.White))
            {
                //Destroy(item.gameObject);
                if (item.GetComponent<PhotonView>().IsMine)
                {
                    Debug.Log("DebugLog_29");
                    PhotonNetwork.Destroy(item.gameObject);
                }
            }
        }

        StartCoroutine("CheckChessWin");
    }

    IEnumerator setPos()
    {
        Debug.Log("DebugLog_30");
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
        //Create("white_pawn",PieceType.Pawn,PlayerType.White, 5, 1,12), Create("white_pawn",PieceType.Pawn,PlayerType.White, 4, 1,13), Create("white_pawn",PieceType.Pawn,PlayerType.White, 3, 1,14),
        //Create("white_pawn",PieceType.Pawn,PlayerType.White, 6, 1,16), Create("white_pawn",PieceType.Pawn,PlayerType.White, 7, 1,15)};
        //}
        yield return new WaitForSeconds(0.2f);

        photonView.RPC("SetPosRPC", RpcTarget.All);
    }

    [PunRPC]
    void SetPosRPC()
    {
        Debug.Log("DebugLog_31");
        playerBlack = Chessman.GetPiecesOfPlayer(PlayerType.Black).ToArray();
        playerWhite = Chessman.GetPiecesOfPlayer(PlayerType.White).ToArray();
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++");
        Debug.Log(playerBlack.Length + " - " + playerWhite.Length);
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++");

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("DebugLog_32");
            foreach (var item in playerBlack)
            {
                Debug.Log("DebugLog_33");
                //item.gameObject.transform.Rotate(new Vector3(0f,0f,180f));
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                item.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }
            foreach (var item in playerWhite)
            {
                Debug.Log("DebugLog_34");
                //      Debug.LogError("name : "+item.name);
                //item.gameObject.transform.Rotate(new Vector3(0f,0f,180f));
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                //item.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }

        }

        for (int i = 0; i < playerBlack.Length; i++)
        {
            Debug.Log("DebugLog_35");
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);

            //Debug.Log(playerWhite[i].GetXboard());
        }
    }

    string getCharId(PieceType type)
    {
        switch (type)
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

    public Chessman Create(string name, PieceType type, PlayerType ptype, int x, int y, int id, string cid = null)
    {
        Debug.Log("DebugLog_36");
        cid = getCharId(type);
        object[] cinit = new object[] { name, type, ptype, x, y, id, cid };
        //photonView.RPC("CreateObj",RpcTarget.All,cinit);
        GameObject obj = PhotonNetwork.Instantiate("chesspiece", new Vector3(0, 0, -1), Quaternion.identity, 0, cinit) as GameObject;
        // obj.transform.SetParent(Board.transform);
        // Chessman cm = obj.GetComponent<Chessman>();
        // cm.name = name;
        // cm.type = type;
        // cm.playerType = ptype;
        // cm.SetXBoard(x);
        // cm.SetYBoard(y);
        // cm.Activate();
        return null;
    }

    [PunRPC]
    public void CreateObj(object[] cinit)
    {
        Debug.Log("DebugLog_37");
        GameObject ob = Instantiate(chesspiece, new Vector3(0f, 0f, -1f), Quaternion.identity);
        ob.GetComponent<Chessman>().OnInstantiate(cinit);
    }

    public void SetPosition(Chessman obj)
    {
        //Debug.Log("DebugLog_38");
        positions[obj.GetXboard(), obj.GetYboard()] = obj.gameObject;
        SpriteRenderer sr = plates[obj.GetXboard(), obj.GetYboard()].GetComponent<SpriteRenderer>();
        SpriteRenderer srInidcator = platesIndicator[obj.GetXboard(), obj.GetYboard()].GetComponent<SpriteRenderer>();
        //if(sr.color.a == 0)
        //{
        //    Color c = sr.color;
        //    c.a = 1;
        //    sr.color = c;
        //}
        plates[obj.GetXboard(), obj.GetYboard()].GetComponent<SpriteRenderer>().color = obj.character.tileColor;
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

    public void SetPositionsEmpty(int x, int y)
    {
        Debug.Log("DebugLog_40:Color plate change to red");
        positions[x, y] = null;
        plates[x, y].GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? BoardBlack : BoardWhite;
        SpriteRenderer sr = plates[x, y].GetComponent<SpriteRenderer>();
        SpriteRenderer srIndicator = platesIndicator[x, y].GetComponent<SpriteRenderer>();
        if (sr.color == BoardBlack || sr.color == BoardWhite)
        {
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
            srIndicator.color = c;
        }
    }

    public GameObject GetPosition(int x, int y)
    {
        //Debug.Log("DebugLog_41");
        return positions[x, y];
    }
    public bool PositionOnBoard(int x, int y)
    {
        Debug.Log("DebugLog_42");
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public void SetPresent(GameObject[,] data)
    {
        positions = new GameObject[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
        {
            Debug.Log("DebugLog_44");
            for (int j = 0; j < data.GetLength(1); j++)
            {
                positions[i, j] = data[i, j];
            }
        }
    }

    public GameObject[,] GetPresent()
    {
        Debug.Log("DebugLog_45");
        GameObject[,] data = new GameObject[positions.GetLength(0), positions.GetLength(1)];
        for (int i = 0; i < positions.GetLength(0); i++)
        {
            for (int j = 0; j < positions.GetLength(1); j++)
            {
                data[i, j] = positions[i, j];
            }
        }
        return data;
    }

    public GameObject[,] GetFuture()
    {
        Debug.Log("DebugLog_46");
        GameObject[,] data = new GameObject[Fpositions.GetLength(0), Fpositions.GetLength(1)];
        for (int i = 0; i < Fpositions.GetLength(0); i++)
        {
            for (int j = 0; j < Fpositions.GetLength(1); j++)
            {
                data[i, j] = Fpositions[i, j];
            }
        }
        return data;
    }
    public void SetFuture(GameObject[,] data)
    {
        Fpositions = new GameObject[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
        {
            Debug.Log("DebugLog_47");
            for (int j = 0; j < data.GetLength(1); j++)
            {
                Fpositions[i, j] = data[i, j];
            }
        }
    }
    public void SetFuturePosition(Chessman obj, int x, int y)
    {
        Debug.Log("DebugLog_48");
        Fpositions[x, y] = obj?.gameObject;
    }
    public void SetFuturePositionsEmpty(int x, int y)
    {
        Debug.Log("DebugLog_49");
        Fpositions[x, y] = null;
    }
    public GameObject GetFuturePosition(int x, int y)
    {
        Debug.Log("DebugLog_50");
        return Fpositions[x, y];
    }
    public bool FuturePositionOnBoard(int x, int y)
    {
        Debug.Log("DebugLog_51");
        if (x < 0 || y < 0 || x >= Fpositions.GetLength(0) || y >= Fpositions.GetLength(1)) return false;
        return true;
    }


    public string GetCurrentPlayer()
    {
        Debug.Log("DebugLog_52");
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        Debug.Log("DebugLog_53");
        return gameOver;
    }

    public void NextTurn()
    {
        if (IsGameOver())
        {
            Debug.Log("DebugLog_56");
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }


        if (currentPlayer == "white")
        {
            Debug.Log("DebugLog_54");
            currentPlayer = "black";
        }
        else
        {
            Debug.Log("DebugLog_55");
            currentPlayer = "white";
        }

        if (isMyTurn(currentPlayer))
        {
            Debug.Log("DebugLog_57");
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if (IsCheckmate || IsKinginCheck)
            {
                if (MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 0);
                }
            }
            else
            {
                photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
            }
        }
    }
    public void NextTurnContinue()
    {
        Debug.Log("DebugLog_58");
        if (IsGameOver())
        {
            Debug.Log("DebugLog_59");
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }


        if (currentPlayer == "white")
        {
            Debug.Log("DebugLog_60");
            currentPlayer = "black";
        }
        else
        {
            Debug.Log("DebugLog_61");
            currentPlayer = "white";
        }

        if (isMyTurn(currentPlayer))
        {
            Debug.Log("DebugLog_62");
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if (IsCheckmate || IsKinginCheck)
            {
                if (MyType == PlayerType.White)
                {
                    Debug.Log("DebugLog_63");
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 1);
                }
                else
                {
                    Debug.Log("DebugLog_64");
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 0);
                }
            }
            else
            {
                Debug.Log("DebugLog_65");
                photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer, true);
            }
        }
    }
    //Allow Moveplate display only for local player

    public void RestartClicked()
    {
        Debug.Log("DebugLog_66");
        RestartButtonClicked = true;
        photonView.RPC("ShowRematch", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
    }

    public void RematchChoice(int i)
    {
        if (i == 0)
        {
            Debug.Log("DebugLog_67");
            photonView.RPC("RematchRejected", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.Log("DebugLog_68");
            photonView.RPC("RestartRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RematchRejected(Player HeWhoGoes)
    {
        RematchYesNo.SetActive(false);
        RematchTxt.text = "Rematch request rejected.";
        StartCoroutine("LeaveRoom");

    }
    public void GetExit()
    {
        StartCoroutine("LeaveRoom");
    }
    IEnumerator LeaveRoom()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MatchScene");
    }

    [PunRPC]
    public void ShowRematch(string name)
    {
        if (PhotonNetwork.LocalPlayer.NickName == name)
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


    [PunRPC]
    void RestartRPC()
    {
        // foreach (var item in Chessman.GetPiecesOfPlayer(PlayerType.White))
        // {

        // }

        // foreach (var item in plates)
        // {
        //    Destroy(item);
        // }

        if (gameOver == true)
        {
            gameOver = false;
            setUpCalled = false;
            PhotonNetwork.CleanRpcBufferIfMine(photonView);
            PhotonNetwork.OpRemoveCompleteCache();
            GameManager.instace.isFristMovePawn = true;
            SceneManager.LoadScene("Game");
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        WinnerTxT.enabled = true;
        WinnerTxT.text = playerWinner + " is the winner";

        RestartTxT.enabled = true;

        PVPManager.manager.TimerObject.SetActive(false);


    }

    public void SetPVPMode(bool b)
    {
        photonView.RPC("SetPVPModeRPC", RpcTarget.All, b);
    }

    public void HandleWin(bool attackerWon, bool isAttackerMaster, Vector2 p1Pos, Vector2 p2Pos)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RPC("HandleWinRPC", RpcTarget.All, attackerWon, isAttackerMaster, p1Pos, p2Pos);
    }



    public void CloseClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }


    #region RPC Calls

    [PunRPC]
    public void HandleWinRPC(bool attackerWon, bool isAttackerMaster, Vector2 p1Pos, Vector2 p2Pos)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RPC("SetPVPModeRPC", RpcTarget.All, false);
        Chessman reference = GetPosition((int)p1Pos.x, (int)p1Pos.y).GetComponent<Chessman>();
        Chessman ob = GetPosition((int)p2Pos.x, (int)p2Pos.y).GetComponent<Chessman>();
        Debug.Log("attacker Won : " + attackerWon + ", attackerMaster :" + isAttackerMaster + ", p1pos : " + p1Pos + ", p2pos : " + p2Pos);
        if (attackerWon)
        {
            if (isAttackerMaster)
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
            if (isAttackerMaster)
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

        if (!IsGameComplete)
            photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
        else
        {
            StopCoroutine(PVPManager.Get().UpdateChessTurnTimer());
            PVPManager.Get().ChessTurnTimerText.gameObject.SetActive(false);
        }
        //Debug.Log(Chessman.GetPiecesOfPlayer(PlayerType.White).Count+" - "+Chessman.GetPiecesOfPlayer(PlayerType.Black).Count);
        //    if(!IsGameComplete)
        //         NextTurn();

    }


    public void DestroyPieceObject(Chessman man)
    {
        Debug.Log("DebugLog_69");
        //  Debug.LogError("Adding : "+man.type+" to "+MyType);
        //   Debug.LogError("Adding : "+man.playerType+" to "+MyType);
        man.transform.position = new Vector3(1000f, 1000f, 1000f);

        if (man.playerType == MyType)
        {
            Debug.Log("DebugLog_70");
            DestroyedObjects.Add(man);

            bool won = false;
            foreach (var item in DestroyedObjects)
            {
                //     Debug.LogError(item.playerType);
                if (item.type == PieceType.King)
                {
                    won = true;
                    if (item.playerType == PlayerType.White)
                    {
                        Debug.Log("DebugLog_71");
                        //Winner(PhotonNetwork.PlayerList[1].NickName);
                        photonView.RPC("PlayerWon", RpcTarget.All, 1);
                    }
                    else
                    {
                        Debug.Log("DebugLog_72");
                        //Winner(PhotonNetwork.PlayerList[0].NickName);
                        photonView.RPC("PlayerWon", RpcTarget.All, 0);

                    }
                    Debug.LogError("King Taken-Game Over");
                    break;
                }
            }

            IsGameComplete = won;
        }
        else
        {
            Debug.Log("DebugLog_73");
            DestroyedObjectsOppo.Add(man);
        }

        UpdateDeadPieces();
    }
    public void DestroyPieceObjectForPawnTaken(Chessman man)
    {
        Debug.Log("DebugLog_74");
        if (PhotonNetwork.LocalPlayer != _currnetTurnPlayer)
        {
            Debug.Log("DebugLog_75");
            if (PVPManager.manager.MyAttackedPiece)
            {
                Debug.Log("DebugLog_76");
                man = PVPManager.manager.MyAttackedPiece;
            }
            else
            {
                Debug.Log("DebugLog_77");
                Debug.LogError("Man not found");
            }
        }

        //  Debug.LogError("Adding : "+man.type+" to "+MyType);
        //   Debug.LogError("Adding : "+man.playerType+" to "+MyType);
        man.transform.position = new Vector3(1000f, 1000f, 1000f);

        if (man.playerType == MyType)
        {
            Debug.Log("DebugLog_78");
            Debug.LogError("This should not be the case for Pawn taken");
            DestroyedObjects.Add(man);

            bool won = false;
            foreach (var item in DestroyedObjects)
            {
                Debug.Log("DebugLog_79");
                //     Debug.LogError(item.playerType);
                if (item.type == PieceType.King)
                {
                    won = true;
                    if (item.playerType == PlayerType.White)
                    {
                        //Winner(PhotonNetwork.PlayerList[1].NickName);
                        photonView.RPC("PlayerWon", RpcTarget.All, 1);
                    }
                    else
                    {
                        //Winner(PhotonNetwork.PlayerList[0].NickName);
                        photonView.RPC("PlayerWon", RpcTarget.All, 0);

                    }
                    Debug.LogError("King Taken-Game Over");
                    break;
                }
            }

            //IsGameComplete = won;
        }
        else
        {
            Debug.Log("DebugLog_80");
            Debug.LogError("Else part");
            NextTurn();
            DestroyedObjectsOppo.Add(man);
        }

        UpdateOtherPlayerDeadPieces();
        // UpdateDeadPieces();
    }

    public void UpdateDeadPieces()
    {
        foreach (Transform item in myPieces)
        {
            Destroy(item.gameObject);
        }
        foreach (Chessman item in DestroyedObjects)
        {
            GameObject o = Instantiate(DeadPieceImage, myPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
        photonView.RPC("UpdateDeadPiecesRPC", RpcTarget.Others);
    }
    public void UpdateOtherPlayerDeadPieces()
    {
        foreach (Transform item in OppoPieces)
        {
            Destroy(item.gameObject);
        }
        foreach (Chessman item in DestroyedObjectsOppo)
        {
            GameObject o = Instantiate(DeadPieceImage, OppoPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
        photonView.RPC("UpdateDeadPiecesMyRPC", RpcTarget.Others);
    }


    [PunRPC]
    public void UpdateDeadPiecesRPC()
    {
        foreach (Transform item in OppoPieces)
        {
            Destroy(item.gameObject);
        }
        foreach (Chessman item in DestroyedObjectsOppo)
        {
            GameObject o = Instantiate(DeadPieceImage, OppoPieces);
            o.GetComponent<UnityEngine.UI.Image>().sprite = item.GetSprite();
        }
    }
    [PunRPC]
    public void UpdateDeadPiecesMyRPC()
    {
        foreach (Transform item in myPieces)
        {
            Destroy(item.gameObject);
        }
        foreach (Chessman item in DestroyedObjects)
        {
            GameObject o = Instantiate(DeadPieceImage, myPieces);
            o.GetComponent<Image>().sprite = item.GetSprite();
        }
    }

    public bool IsGameComplete = false;

    Chessman pawntobeRenewed;
    public Transform revivePr;
    public GameObject reviveLisItem;
    public GameObject PieceSelectionPanel;

    public void ShowReviveOption(Chessman pawn)
    {
        pawntobeRenewed = pawn;
        foreach (Transform item in revivePr)
        {
            Destroy(item.gameObject);
        }
        int i = 0;
        foreach (var item in DestroyedObjects)
        {
            GameObject o = Instantiate(reviveLisItem, revivePr);
            o.GetComponentInChildren<UnityEngine.UI.Image>().sprite = item.GetSprite();
            o.GetComponent<RevivePieceItem>().id = i;
            i++;
        }
        PieceSelectionPanel.SetActive(true);

    }

    public void ChangePawnToNewPiece(int i)
    {
        Chessman pawn = pawntobeRenewed;
        PieceSelectionPanel.SetActive(false);
        if (DestroyedObjects != null)
        {
            Chessman c = DestroyedObjects[i];

            Create("New_" + c.gameObject.name, c.type, c.playerType, pawn.GetXboard(), pawn.GetYboard(), 30 + DestroyedObjects.Count);
            //Chessman.pieces.Remove(pawn);
            DestroyedObjects.Remove(c);
            photonView.RPC("DestroyPiece", RpcTarget.All, c.PieceIndex);
            photonView.RPC("DestroyPiece", RpcTarget.All, pawn.PieceIndex);
            photonView.RPC("SetPosRPC", RpcTarget.All);
            Game.Get().NextTurn();
        }
    }

    [PunRPC]
    public void DestroyPiece(int PieceIndex)
    {
        Debug.Log("DebugLog_81");
        Chessman piece = Chessman.GetPiece(PieceIndex);
        List<Chessman> blacks = playerBlack.ToList();
        if (blacks.Contains(piece))
            blacks.Remove(piece);
        playerBlack = blacks.ToArray();

        List<Chessman> whites = playerWhite.ToList();
        if (whites.Contains(piece))
            whites.Remove(piece);
        playerWhite = whites.ToArray();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            PhotonNetwork.Destroy(piece.GetComponent<PhotonView>());

    }

    public void CheckWinner()
    {
        //StartCoroutine(CheckChessWin());
    }

    void CheckChessWin()
    {
        //yield return new WaitForSeconds(0.2f);

        //  Debug.LogError(playerWhite.ToList().FindAll(x=>x.type== PieceType.King).Count + " _ " + playerBlack.ToList().FindAll(x => x.type == PieceType.King).Count);
        //  Debug.LogError(DestroyedObjects.Count);
        bool won = false;
        foreach (var item in DestroyedObjects)
        {
            //    Debug.LogError(item.type);
            if (item.type == PieceType.King)
            {
                won = true;
                if (item.playerType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 0);
                }
                break;
            }
        }
        // if(playerWhite.ToList().FindAll(x=>x.type== PieceType.King).Count==0 ){

        // }else if(playerBlack.ToList().FindAll(x => x.type == PieceType.King).Count == 0)
        // {

        // }else{
        //     NextTurn();
        // }
        if (!won)
        {
            photonView.RPC("PlayerWon", RpcTarget.Others, -1);
        }
    }

    [PunRPC]
    public void PlayerWon(int i)
    {
        if (i != -1)
            Winner(PhotonNetwork.PlayerList[i].NickName);
        else
            NextTurn();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// PVP Mode

    [PunRPC]
    public void SetPVPModeRPC(bool b)
    {
        if (b)
        {
            if (PhotonNetwork.IsMasterClient)
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
            chessBg.SetActive(false);
            Board.SetActive(false);
            PVPCanvas.SetActive(true);
            background.SetActive(true);
        }
        else
        {
            PVPCanvas.SetActive(false);
            background.SetActive(false);
            ChessCanvas.SetActive(true);
            chessBg.SetActive(true);
            // GameManager.instace.isFristMovePawn = true;
            Board.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                playerName.text = PhotonNetwork.PlayerList[0].NickName;
                opponentName.text = PhotonNetwork.PlayerList[1].NickName;
                SetProfileImageForPlayer(true, PhotonNetwork.PlayerList[0]); //Set player profile pic
                SetProfileImageForPlayer(false, PhotonNetwork.PlayerList[1]);// Set oppoenet profile pic
            }
            else
            {
                playerName.text = PhotonNetwork.PlayerList[1].NickName;
                opponentName.text = PhotonNetwork.PlayerList[0].NickName;
                SetProfileImageForPlayer(true, PhotonNetwork.PlayerList[1]); //Set player profile pic
                SetProfileImageForPlayer(false, PhotonNetwork.PlayerList[0]);// Set oppoenet profile pic
            }
        }
    }
    public void SetProfileImageForPlayer(bool isplayer, Player player)
    {
        ExitGames.Client.Photon.Hashtable _playerCustomProperties = player.CustomProperties;
        if (isplayer)
        {  //SetProfileImage for player
            if (Convert.ToInt32(_playerCustomProperties[GameData.hasProfileConst].ToString()) == 1)
            {
                playerProfileImage.sprite = GameData.playerSprite;
            }
            else
            {
                int dummypicIndex = -1;
                dummypicIndex = Convert.ToInt32(_playerCustomProperties[GameData.dummyProfileImageIndex].ToString());
                if (dummypicIndex != -1)
                {
                    playerProfileImage.sprite = GameData.Get().DummyProfile[dummypicIndex];
                }
            }
        }
        else
        {
            Debug.LogError("Not Player");
            if (Convert.ToInt32(_playerCustomProperties[GameData.hasProfileConst].ToString()) == 1)
            {
                if (GameData.opponentSprite == GameData.Get().DummyProfile[0])
                {
                    Debug.LogError("Opponet has profile pic");
                    StartCoroutine(FetchOpponentProfilePicture(_playerCustomProperties[GameData.profileUrl].ToString()));
                }
                else
                {
                    opponentProfileImage.sprite = GameData.opponentSprite;
                    Debug.LogError("Opponet has profile pic" + "But not set");
                }
            }
            else
            {
                int dummypicIndex = -1;
                dummypicIndex = Convert.ToInt32(_playerCustomProperties[GameData.dummyProfileImageIndex].ToString());
                if (dummypicIndex != -1)
                {
                    opponentProfileImage.sprite = GameData.Get().DummyProfile[dummypicIndex];
                }
            }

        }
        //

    }
    public void SwitchPlayerTurn_RPCCall()
    {
    }
    [PunRPC]
    public void SwitchCurrentPlayer(string player)
    {

        currentPlayer = player;
        //isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if (currentPlayer == "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName, PhotonNetwork.PlayerList[0]));
        }
        if (currentPlayer != "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName, PhotonNetwork.PlayerList[1]));
        }

        if (isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if (IsCheckmate)
            {
                if (MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 0);
                }
            }
        }
    }
    [PunRPC]
    public void SwitchCurrentPlayer(string player, bool continueTurn)
    {

        currentPlayer = player;
        //isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if (currentPlayer == "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName, PhotonNetwork.PlayerList[0]));
        }
        if (currentPlayer != "white")
        {
            StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName, PhotonNetwork.PlayerList[1]));
        }

        if (isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.LogError("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck)
                Debug.LogError("Checkmate ================> " + IsCheckmate);
            if (IsCheckmate)
            {
                if (MyType == PlayerType.White)
                {
                    Winner(PhotonNetwork.PlayerList[1].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 1);
                }
                else
                {
                    Winner(PhotonNetwork.PlayerList[0].NickName);
                    photonView.RPC("PlayerWon", RpcTarget.Others, 0);
                }
            }
            else
            {
                NextTurn();
            }
        }
    }


    public bool checkForKing()
    {

        List<Chessman> OppoPieces = Chessman.GetPiecesOfPlayer(OppoType);
        bool isCheck = false;
        foreach (var item in OppoPieces)
        {
            isCheck = item.IsCheckforKing();
            if (isCheck) { Debug.LogError("Check by Opponent's " + item.type); break; }
        }
        return isCheck;
    }

    public bool IsCheckmateForKing()
    {
        bool KingLives = true;
        List<Chessman> pieces = Chessman.GetPiecesOfPlayer(MyType);
        List<PieceType> types_lis = new List<PieceType>() { PieceType.King, PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight, PieceType.Pawn };
        foreach (var Ptype in types_lis)
        {
            Chessman piece = pieces.Find((t) => t.type == Ptype);
            KingLives = piece.canDefendKing();
            if (KingLives) { Debug.LogError("King Defended by " + piece.type); break; }
        }
        return !KingLives;
    }

    public bool isMyTurn(string player)
    {
        return Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn;
    }

    private IEnumerator COR_playerTurnNameShow(string namePlayer, Player _player)
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

        if (ChessCanvas.activeSelf)
        {
            if (_currnetTurnPlayer.IsLocal)
            { PVPManager.manager.StartChessTimer(); }
            else
            {
                if (PVPManager.manager.moveChoiceConfirmation.activeSelf)
                {
                    PVPManager.manager.moveChoiceConfirmation.SetActive(false);
                }
                PVPManager.manager.TimerObject.SetActive(false);
            }
        }

    }

    [PunRPC]
    public void InitiateMovePlatesRPC()
    {

    }
    public void UpdateLastAction(PlayerAction action)
    {
        PVPManager.Get().updatePlayerAction(action.ToString());
        PVPManager.Get().LastActionUpdated = true;
        photonView.RPC("UpdateLastAction_RPC", RpcTarget.All, (byte)action);
    }
    [PunRPC]
    public void UpdateLastAction_RPC(byte action)
    {
        lastAction = (PlayerAction)action;
    }
    #endregion

    private IEnumerator FetchOpponentProfilePicture(string url)
    {
        Debug.Log("Fetching profile picture from URL: " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
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
[System.Serializable]
public class BoardPosition
{
    public int xBoard, yboard;
    public Transform boardPoint;
}