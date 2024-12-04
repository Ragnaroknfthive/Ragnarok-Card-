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
public enum SpellCardPosition { None, petHomePlayer, petHomeOppoent, petBattlePlayer, petBattleOpponent }

public class Game : MonoBehaviour
{
    #region Variables
    [Header("Refactorized Variables")]
    public List<BoardPosition> boardPositions = new List<BoardPosition>();

    [Header("Cesar's Variables")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject chessBg;

    [Header("Listas")]
    public List<PlayerTrunName> _playerTurnList = new List<PlayerTrunName>();
    public List<int> PlayerStrengths = new List<int>(2);
    public List<string> PokerHandResults = new List<string>(2) { "", "" };
    public int MyHighCardValue = 0, OpponentHighCardValue = 0, MySecondHighCardValue = -1, OpponentSecondHighCardValue = -1;
    public List<int> MyHighCardList = new List<int>() { -1, -1, -1, -1, -1 };
    public List<int> OpponentHighCardList = new List<int>() { -1, -1, -1, -1, -1 };
    public List<Chessman> DestroyedObjects = new List<Chessman>();
    public List<Chessman> DestroyedObjectsOppo = new List<Chessman>();

    [Header("GameObject")]
    public GameObject chesspiece;
    public GameObject movePlate;
    public GameObject Board, RotatedBoardSpriteObject;
    public GameObject ColorPlate, ColorPlateIndicator;
    public GameObject ChessCanvas;
    public GameObject PVPCanvas;
    public GameObject board;
    public GameObject RematchPopUp, RematchYesNo;
    public GameObject PlayerTurnScreen;
    public GameObject NewBoard;
    public GameObject loadingScreen;
    public GameObject DeadPieceImage;
    public GameObject reviveLisItem;
    public GameObject PieceSelectionPanel;

    [Header("GameObjects")]
    [SerializeField] private GameObject[,] positions = new GameObject[6, 5];
    [SerializeField] private GameObject[,] Fpositions = new GameObject[6, 5];
    public GameObject[,] plates = new GameObject[6, 5];
    public GameObject[,] platesIndicator = new GameObject[6, 5];

    [Header("Chessman")]
    [SerializeField] private Chessman[] playerWhite = new Chessman[10];
    [SerializeField] private Chessman[] playerBlack = new Chessman[10];
    Chessman pawntobeRenewed;

    [Header("Transforms")]
    public Transform revivePr;
    public Transform myPieces, OppoPieces;

    [Header("Integers")]
    public int turn = 0;
    public int BetAmount = 0;
    public int localBetAmount = 0;
    public int MyStamina, OppoStamina;

    [Header("Floats")]
    public float HealthDemage = 0;

    [Header("Booleans")]
    public bool RestartButtonClicked;
    public bool isLocalPlayerTurn;
    private bool gameOver = false;
    public static bool setUpCalled;
    public bool IsDefender = false;
    public bool IsGameComplete = false;

    [Header("Strings")]
    private string currentPlayer = "white";
    public string LastAttackerColor = "";

    [Header("Texts")]
    public Text RematchTxt;
    public Text PlayerTurnScreenText;

    [Header("Data")]
    private PhotonView photonView;
    private static Game game;
    public CharacterData defChar;
    public Color BoardBlack, BoardWhite;

    [Header("Poker Variables")]
    public PlayerAction lastAction = PlayerAction.idle;
    public Player _currnetTurnPlayer;
    public Text WinnerTxT;
    public Text RestartTxT;
    public TMPro.TextMeshProUGUI playerName, opponentName;
    public Image playerProfileImage, opponentProfileImage;
    
    [Space]
    [Header("Chess Atributes")]
    public PlayerType MyType, OppoType;
    #endregion

    #region Class
    [System.Serializable]
    public class BoardPosition
    {
        public int xBoard;
        public int yboard;
        public Transform boardPoint;
    }
    public class PlayerTrunName
    {
        public Player _player;
        public bool _isTurn;
    }
    #endregion

    #region UnityCallbacks
    public void Awake()
    {
        game = this;
    }
    public void Start()
    {
        MyType = PhotonNetwork.LocalPlayer.IsMasterClient ? PlayerType.White : PlayerType.Black;
        OppoType = MyType == PlayerType.White ? PlayerType.Black : PlayerType.White;
        PlayerStrengths.Add(0);
        PlayerStrengths.Add(0);
        Renderer boardRenderer = board.GetComponent<Renderer>();
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;
        int plateRows = plates.GetLength(0);
        int plateCols = plates.GetLength(1);
        float plateWidth = boardWidth / plateCols;
        float plateHeight = boardHeight / plateRows;
        int cmasmas = 0;
        for (int i = 0; i < plateRows; i++)
        {
            for (int j = 0; j < plateCols; j++)
            {
                float plateX = ((i * plateWidth) - (boardWidth / 2) + (plateWidth / 2) + board.transform.position.x) + 0.270f;
                float plateY = ((j * plateHeight) - (boardHeight / 2) + (plateHeight / 2) + board.transform.position.y) + 0.270f;
                if (i == 0 && j == 0)
                {
                    //print("COLOR PLATE + X pos " + plateX + "Y pos " + plateY);
                }

                //print(Get().boardPositions[cmasmas].boardPoint.name);
                Vector3 pos1 = Get().boardPositions[cmasmas].boardPoint.position;
                cmasmas++;
                plateX = pos1.x;
                plateY = pos1.y;
                
                plates[i, j] = Instantiate(ColorPlate, new Vector3(plateX, plateY, -0.1f), Quaternion.identity);
                plates[i, j].transform.SetParent(board.transform);
                plates[i, j].transform.localScale = new Vector3(1, 1, 1f);
                plates[i, j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;
                SpriteRenderer sr = plates[i, j].GetComponent<SpriteRenderer>();
                
                platesIndicator[i, j] = Instantiate(ColorPlateIndicator, new Vector3(plateX, plateY - .05f, -0.1f), Quaternion.identity);
                platesIndicator[i, j].transform.SetParent(board.transform);
                platesIndicator[i, j].transform.localScale = new Vector3(1f, 1f, 1f);
                platesIndicator[i, j].GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? BoardBlack : BoardWhite;
                SpriteRenderer srIndicator = platesIndicator[i, j].GetComponent<SpriteRenderer>();
                
                
                
                if (sr.color == BoardBlack || sr.color == BoardWhite)
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
                if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    Vector3 pos = plates[i, j].transform.position;
                    pos.y = pos.x * (-1);
                    pos.x = pos.y * (-1);
                    plates[i, j].transform.position = new Vector3(plates[i, j].transform.position.x, plates[i, j].transform.position.y + .05f, plates[i, j].transform.position.z);
                    platesIndicator[i, j].transform.position = new Vector3(platesIndicator[i, j].transform.position.x, platesIndicator[i, j].transform.position.y + .05f, platesIndicator[i, j].transform.position.z);
                    LeanTween.rotate(plates[i, j], new Vector3(0, 0, -180), 0);
                    LeanTween.rotate(platesIndicator[i, j], new Vector3(0, 0, -180), 0);
                }

            }
        }
        isLocalPlayerTurn = (PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer == "white") || (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsMasterClient && currentPlayer == "white")
            {
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i], _isTurn = true });
            }
            else if (!PhotonNetwork.LocalPlayer.IsMasterClient && currentPlayer != "white")
            {
                _playerTurnList.Add(new PlayerTrunName { _player = PhotonNetwork.PlayerList[i], _isTurn = false });
            }
        }
        photonView = GetComponent<PhotonView>();
        photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
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
            RestartClicked();
        }
        if (PhotonNetwork.IsConnected && _currnetTurnPlayer != null && PhotonNetwork.LocalPlayer.NickName != _currnetTurnPlayer.NickName)
        {
            if (GameObject.FindObjectsOfType<MovePlate>() != null)
            {
                MovePlate[] movePlates = GameObject.FindObjectsOfType<MovePlate>();
                foreach (var item in movePlates)
                {
                    item.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }
    #endregion

    #region Numerators
    public IEnumerator SetLoadingScreenOnOff(bool isOn, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("DebugLog_21");
            //Debug.LogError("Loading screen set from here");
            photonView.RPC("SetLoadingScreenOff_RPC", RpcTarget.All, isOn);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    IEnumerator setPos()
    {
        Debug.Log("Spawning Black Pieces");
        playerBlack = new Chessman[] {
                Create("black_rook"     ,PieceType.Rook     ,PlayerType.Black, 0, 5 ,17),
                Create("black_bishop"   ,PieceType.Bishop   ,PlayerType.Black, 1, 5 ,20),
                Create("black_king"     ,PieceType.King     ,PlayerType.Black, 2, 5 ,21),
                Create("black_queen"    ,PieceType.Queen    ,PlayerType.Black, 3, 5 ,19),
                Create("black_knight"   ,PieceType.Knight   ,PlayerType.Black, 4, 5 ,23),

                Create("black_pawn"     ,PieceType.Pawn     ,PlayerType.Black, 0, 4 ,29),
                Create("black_pawn"     ,PieceType.Pawn     ,PlayerType.Black, 1, 4 ,24),
                Create("black_pawn"     ,PieceType.Pawn     ,PlayerType.Black, 2, 4 ,33),
                Create("black_pawn"     ,PieceType.Pawn     ,PlayerType.Black, 3, 4 ,31),
                Create("black_pawn"     ,PieceType.Pawn     ,PlayerType.Black, 4, 4 ,35)
            };
        yield return new WaitForSeconds(0.2f);
        Debug.Log("Spawning White Pieces");
        playerWhite = new Chessman[] {
                Create("white_knight"   ,PieceType.Knight   ,PlayerType.White, 0,0 ,4),
                Create("white_queen"    ,PieceType.Queen    ,PlayerType.White, 1,0 ,2),
                Create("white_king"     ,PieceType.King     ,PlayerType.White, 2,0 ,1),
                Create("white_bishop"   ,PieceType.Bishop   ,PlayerType.White, 3,0 ,8),
                Create("white_rook"     ,PieceType.Rook     ,PlayerType.White, 4,0 ,3),

                Create("white_pawn"     ,PieceType.Pawn     ,PlayerType.White, 0,1 ,11),
                Create("white_pawn"     ,PieceType.Pawn     ,PlayerType.White, 1,1 ,10),
                Create("white_pawn"     ,PieceType.Pawn     ,PlayerType.White, 2,1 ,9),
                Create("white_pawn"     ,PieceType.Pawn     ,PlayerType.White, 3,1 ,36),
                Create("white_pawn"     ,PieceType.Pawn     ,PlayerType.White, 4,1 ,37)
            };
        yield return new WaitForSeconds(0.2f);
        photonView.RPC("SetPosRPC", RpcTarget.All);
    }
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
    IEnumerator LeaveRoom()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MatchScene");
    }
    #endregion

    #region SettersAndGetters
    public void SetPresent(GameObject[,] data)
    {
        positions = new GameObject[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                positions[i, j] = data[i, j];
            }
        }
    }

    public GameObject[,] GetPresent()
    {
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
            for (int j = 0; j < data.GetLength(1); j++)
            {
                Fpositions[i, j] = data[i, j];
            }
        }
    }
    public void SetFuturePosition(Chessman obj, int y, int x)
    {
        Fpositions[y, x] = obj?.gameObject;
    }
    public void SetFuturePositionsEmpty(int y, int x)
    {
        Fpositions[y, x] = null;
    }
    public GameObject GetFuturePosition(int y, int x)
    {
        return Fpositions[y, x];
    }
    public bool FuturePositionOnBoard(int y, int x)
    {
        if (x < 0 || y < 0 || x >= Fpositions.GetLength(1) || y >= Fpositions.GetLength(0)) return false;
        return true;
    }
    #endregion

    #region Methods
    public GameObject GetPosition(int y, int x)
    {
        return positions[y, x];
    }
    public bool PositionOnBoard(int y, int x)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(1) || y >= positions.GetLength(0)) return false;
        return true;
    }
    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public bool IsGameOver()
    {
        return gameOver;
    }
    public void NextTurn()
    {
        if (IsGameOver())
        {
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }
        if (currentPlayer == "white") currentPlayer = "black";
        else currentPlayer = "white";
        if (isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.Log("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck) Debug.Log("Checkmate ================> " + IsCheckmate);
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
            else photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
        }
    }
    public void NextTurnContinue()
    {
        if (IsGameOver())
        {
            PVPManager.manager.TimerObject.SetActive(false);
            return;
        }
        if (currentPlayer == "white") currentPlayer = "black";
        else currentPlayer = "white";

        if (isMyTurn(currentPlayer))
        {
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
                photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer, true);
            }
        }
    }
    public void RestartClicked()
    {
        RestartButtonClicked = true;
        photonView.RPC("ShowRematch", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
    }
    public void RematchChoice(int i)
    {
        if (i == 0) photonView.RPC("RematchRejected", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        else photonView.RPC("RestartRPC", RpcTarget.AllBuffered);
    }
    public void GetExit()
    {
        StartCoroutine("LeaveRoom");
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
        if (PhotonNetwork.LocalPlayer.IsMasterClient) photonView.RPC("HandleWinRPC", RpcTarget.All, attackerWon, isAttackerMaster, p1Pos, p2Pos);
    }
    public void CloseClick()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
    public bool isMyTurn(string player)
    {
        return Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn;
    }
    public void UpdateLastAction(PlayerAction action)
    {
        PVPManager.Get().updatePlayerAction(action.ToString());
        PVPManager.Get().LastActionUpdated = true;
        photonView.RPC("UpdateLastAction_RPC", RpcTarget.All, (byte)action);
    }
    public void CheckWinner()
    {
        //StartCoroutine(CheckChessWin());
    }
    public void SetPositionsEmpty(int x, int y)
    {
        positions[y, x] = null;
        plates[y, x].GetComponent<SpriteRenderer>().color = (x + y) % 2 == 0 ? BoardBlack : BoardWhite;
        SpriteRenderer sr = plates[y, x].GetComponent<SpriteRenderer>();
        SpriteRenderer srIndicator = platesIndicator[y, x].GetComponent<SpriteRenderer>();
        if (sr.color == BoardBlack || sr.color == BoardWhite)
        {
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
            srIndicator.color = c;
        }
    }
    public int GetMaxY()
    {
        return positions.GetLength(1);
    }
    public void IncreaseStamina()
    {
        MyStamina++;
        MyStamina = Mathf.Clamp(MyStamina, 0, 10);
        photonView.RPC("IncreaseStaminaRPC", RpcTarget.Others);
    }
    public static Game Get()
    {
        return game;
    }
    public void WinPlayer(int i)
    {
        photonView.RPC("WinMatch", RpcTarget.All, i);
    }
    public Chessman Create(string name, PieceType type, PlayerType ptype, int x, int y, int id, string cid = null)
    {
        cid = getCharId(type);
        object[] cinit = new object[] { name, type, ptype, x, y, id, cid };
        GameObject obj = PhotonNetwork.Instantiate("chesspiece", new Vector3(0, 0, -1), Quaternion.identity, 0, cinit) as GameObject;
        return null;
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
                return "Human";
            default:
                return "Human";
        }
    }
    public void SetPosition(Chessman obj)
    {
        positions[obj.GetYboard(), obj.GetXboard()] = obj.gameObject;
        SpriteRenderer sr = plates[obj.GetYboard(), obj.GetXboard()].GetComponent<SpriteRenderer>();
        SpriteRenderer srInidcator = platesIndicator[obj.GetYboard(), obj.GetXboard()].GetComponent<SpriteRenderer>();
        plates[obj.GetYboard(), obj.GetXboard()].GetComponent<SpriteRenderer>().color = obj.character.tileColor;
        Color c = sr.color;
        Color indicator = c;
        c.a = .9f;
        indicator.a = 0;
        sr.color = indicator;
        srInidcator.color = c;
    }
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
    public void SwitchPlayerTurn_RPCCall()
    {
    }
    public void SetProfileImageForPlayer(bool isplayer, Player player)
    {
        ExitGames.Client.Photon.Hashtable _playerCustomProperties = player.CustomProperties;
        if (isplayer)
        {
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
    }
    void CheckChessWin()
    {
        bool won = false;
        foreach (var item in DestroyedObjects)
        {
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
        if (!won)
        {
            photonView.RPC("PlayerWon", RpcTarget.Others, -1);
        }
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
            Get().NextTurn();
        }
    }
    public void DestroyPieceObject(Chessman man)
    {
        man.transform.position = new Vector3(1000f, 1000f, 1000f);
        if (man.playerType == MyType)
        {
            DestroyedObjects.Add(man);
            bool won = false;
            foreach (var item in DestroyedObjects)
            {
                if (item.type == PieceType.King)
                {
                    won = true;
                    if (item.playerType == PlayerType.White) photonView.RPC("PlayerWon", RpcTarget.All, 1);
                    else photonView.RPC("PlayerWon", RpcTarget.All, 0);
                    Debug.Log("King Taken-Game Over");
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
    public void DestroyPieceObjectForPawnTaken(Chessman man)
    {
        if (PhotonNetwork.LocalPlayer != _currnetTurnPlayer)
        {
            if (PVPManager.manager.MyAttackedPiece)
            {
                man = PVPManager.manager.MyAttackedPiece;
            }
            else
            {
                Debug.Log("Man not found");
            }
        }
        man.transform.position = new Vector3(1000f, 1000f, 1000f);
        if (man.playerType == MyType)
        {
            Debug.Log("This should not be the case for Pawn taken");
            DestroyedObjects.Add(man);
            bool won = false;
            foreach (var item in DestroyedObjects)
            {
                if (item.type == PieceType.King)
                {
                    won = true;
                    if (item.playerType == PlayerType.White) photonView.RPC("PlayerWon", RpcTarget.All, 1);
                    else photonView.RPC("PlayerWon", RpcTarget.All, 0);
                    Debug.Log("King Taken-Game Over");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Else part");
            NextTurn();
            DestroyedObjectsOppo.Add(man);
        }
        UpdateOtherPlayerDeadPieces();
    }
    #endregion
    
    #region KingMethods
    public bool checkForKing()
    {
        List<Chessman> OppoPieces = Chessman.GetPiecesOfPlayer(OppoType);
        bool isCheck = false;
        foreach (var item in OppoPieces)
        {
            isCheck = item.IsCheckforKing();
            if (isCheck) { Debug.Log("Check by Opponent's " + item.type); break; }
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
            if (KingLives) { Debug.Log("King Defended by " + piece.type); break; }
        }
        return !KingLives;
    }
    #endregion

    #region RPC Calls
    [PunRPC]
    public void HandleWinRPC(bool attackerWon, bool isAttackerMaster, Vector2 p1Pos, Vector2 p2Pos)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) photonView.RPC("SetPVPModeRPC", RpcTarget.All, false);
        Chessman reference = GetPosition((int)p1Pos.y, (int)p1Pos.x).GetComponent<Chessman>();
        Chessman ob = GetPosition((int)p2Pos.y, (int)p2Pos.x).GetComponent<Chessman>();
        Debug.Log("attacker Won : " + attackerWon + ", attackerMaster :" + isAttackerMaster + ", p1pos : " + p1Pos + ", p2pos : " + p2Pos);
        if (attackerWon)
        {
            if (isAttackerMaster)
            {
                Get().SetPositionsEmpty(reference.GetXboard(),
                reference.GetYboard());
                reference.SetXBoard((int)p2Pos.x);
                reference.SetYBoard((int)p2Pos.y);
                reference.SetCoords();
                SetPosition(reference);
                DestroyPieceObject(ob);
            }
            else
            {
                Get().SetPositionsEmpty(ob.GetXboard(),
                ob.GetYboard());
                ob.SetXBoard((int)p1Pos.x);
                ob.SetYBoard((int)p1Pos.y);
                ob.SetCoords();
                SetPosition(ob);
                DestroyPieceObject(reference);
            }
        }
        else
        {
            if (isAttackerMaster)
            {
                Game.Get().SetPositionsEmpty(reference.GetXboard(),
                reference.GetYboard());
                DestroyPieceObject(reference);
            }
            else
            {
                Game.Get().SetPositionsEmpty(ob.GetXboard(),
                ob.GetYboard());
                DestroyPieceObject(ob);
            }
        }
        if (!IsGameComplete) photonView.RPC("SwitchCurrentPlayer", RpcTarget.AllBuffered, currentPlayer);
        else
        {
            StopCoroutine(PVPManager.Get().UpdateChessTurnTimer());
            PVPManager.Get().ChessTurnTimerText.gameObject.SetActive(false);
        }
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
    [PunRPC]
    public void DestroyPiece(int PieceIndex)
    {
        Chessman piece = Chessman.GetPiece(PieceIndex);
        List<Chessman> blacks = playerBlack.ToList();
        if (blacks.Contains(piece)) blacks.Remove(piece);
        playerBlack = blacks.ToArray();
        List<Chessman> whites = playerWhite.ToList();
        if (whites.Contains(piece)) whites.Remove(piece);
        playerWhite = whites.ToArray();
        if (PhotonNetwork.LocalPlayer.IsMasterClient) PhotonNetwork.Destroy(piece.GetComponent<PhotonView>());
    }
    [PunRPC]
    public void PlayerWon(int i)
    {
        if (i != -1) Winner(PhotonNetwork.PlayerList[i].NickName);
        else NextTurn();
    }
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
            PVPCanvas.SetActive(true);
            background.SetActive(true);
            ChessCanvas.SetActive(false);
            chessBg.SetActive(false);
            Board.SetActive(false);
        }
        else
        {
            PVPCanvas.SetActive(false);
            background.SetActive(false);
            ChessCanvas.SetActive(true);
            chessBg.SetActive(true);
            Board.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                playerName.text = PhotonNetwork.PlayerList[0].NickName;
                opponentName.text = PhotonNetwork.PlayerList[1].NickName;
                SetProfileImageForPlayer(true, PhotonNetwork.PlayerList[0]);
                SetProfileImageForPlayer(false, PhotonNetwork.PlayerList[1]);
            }
            else
            {
                playerName.text = PhotonNetwork.PlayerList[1].NickName;
                opponentName.text = PhotonNetwork.PlayerList[0].NickName;
                SetProfileImageForPlayer(true, PhotonNetwork.PlayerList[1]);
                SetProfileImageForPlayer(false, PhotonNetwork.PlayerList[0]);
            }
        }
    }
    [PunRPC]
    public void SwitchCurrentPlayer(string player)
    {
        currentPlayer = player;
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if (currentPlayer == "white") StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName, PhotonNetwork.PlayerList[0]));
        if (currentPlayer != "white") StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName, PhotonNetwork.PlayerList[1]));
        if (isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.Log("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck) Debug.Log("Checkmate ================> " + IsCheckmate);
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
        isLocalPlayerTurn = PhotonNetwork.PlayerList[currentPlayer == "white" ? 0 : 1].IsLocal;
        if (currentPlayer == "white") StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[0].NickName, PhotonNetwork.PlayerList[0]));
        if (currentPlayer != "white") StartCoroutine(COR_playerTurnNameShow(PhotonNetwork.PlayerList[1].NickName, PhotonNetwork.PlayerList[1]));
        if (isMyTurn(currentPlayer))
        {
            bool IsKinginCheck = checkForKing();
            bool IsCheckmate = IsKinginCheck ? IsCheckmateForKing() : false;
            Debug.Log("Checking for Check ================> " + IsKinginCheck);
            if (IsKinginCheck) Debug.Log("Checkmate ================> " + IsCheckmate);
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
            else NextTurn();
        }
    }
    [PunRPC]
    public void InitiateMovePlatesRPC()
    {

    }
    [PunRPC]
    public void UpdateLastAction_RPC(byte action)
    {
        lastAction = (PlayerAction)action;
    }
    [PunRPC]
    void RematchRejected(Player HeWhoGoes)
    {
        RematchYesNo.SetActive(false);
        RematchTxt.text = "Rematch request rejected.";
        StartCoroutine("LeaveRoom");
    }
    [PunRPC]
    void RestartRPC()
    {
        if (gameOver == true)
        {
            gameOver = false;
            setUpCalled = false;
            PhotonNetwork.CleanRpcBufferIfMine(photonView);
            PhotonNetwork.OpRemoveCompleteCache();
            SceneManager.LoadScene("Game");
        }
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
    public void IncreaseStaminaRPC()
    {
        OppoStamina++;
        OppoStamina = Mathf.Clamp(OppoStamina, 0, 10);
    }
    [PunRPC]
    public void SetLoadingScreenOff_RPC(bool isOn)
    {
        loadingScreen.SetActive(isOn);
    }
    [PunRPC]
    void SetPosRPC()
    {
        playerBlack = Chessman.GetPiecesOfPlayer(PlayerType.Black).ToArray();
        playerWhite = Chessman.GetPiecesOfPlayer(PlayerType.White).ToArray();
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            foreach (var item in playerBlack)
            {
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                item.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1]);
            }
            foreach (var item in playerWhite)
            {
                item.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            }
        }

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }
    [PunRPC]
    void WinMatch(int i)
    {
        if (i == 1)
        {
            foreach (var item in Chessman.GetPiecesOfPlayer(PlayerType.Black))
            {
                if (item.GetComponent<PhotonView>().IsMine) PhotonNetwork.Destroy(item.gameObject);
            }
        }
        else if (i == 2)
        {
            foreach (var item in Chessman.GetPiecesOfPlayer(PlayerType.White))
            {
                if (item.GetComponent<PhotonView>().IsMine) PhotonNetwork.Destroy(item.gameObject);
            }
        }
        StartCoroutine("CheckChessWin");
    }
    [PunRPC]
    public void CreateObj(object[] cinit)
    {
        GameObject ob = Instantiate(chesspiece, new Vector3(0f, 0f, -1f), Quaternion.identity);
        ob.GetComponent<Chessman>().OnInstantiate(cinit);
    }
    #endregion
}