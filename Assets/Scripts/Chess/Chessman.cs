////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Chessman.cs
//FileType: C# Source file
//Description : This script is used to handle "Chess Piece" in the chess mode
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

/// <summary>
/// Chess piece type - King, Queen etc...
/// </summary>
public enum PieceType
{
    Queen,
    King,
    Rook,
    Knight,
    Bishop,
    Pawn
}
/// <summary>
/// Chess player type - Black / White
/// </summary>
public enum PlayerType
{
    Black,
    White,
    None

    
}

/// <summary>
/// Interface for healthbar update
/// </summary>
public interface IHealthBar
{
    public void UpdateHealth(float _health);
}
/// <summary>
/// Interface for saving  pvp mode - attack position (poker game attack position on body)
/// </summary>
public interface ISaveHighLowLeftRightMedle
{
    public void SaveHighLowLeftRightMedle(float high, float low, float left, float right, float medle);
}

public class Chessman : MonoBehaviour, IPunInstantiateMagicCallback, IHealthBar, ISaveHighLowLeftRightMedle
{
    public GameObject controller;               //controller - "Game" script reference. "Game" script handles chess game.
    public GameObject movePlate;                //Move plate - used to highlight available attack positions 

    public PieceType type;                      //Type of this chess piece
    public PlayerType playerType;               //Player type who owns this piece (black or white)
    private PhotonView photonView;              //Used to handle multiplayer mode functionalities and sync this piece on network

    public int xBoard = -1;                     //X-Position coordinates on board for this piece
    public int yBoard = -1;                     //Y-Position coordinates on board for this piece
    public bool isFirstmove = true;             //Used to decide if it's first move for this piece

    public string player;                       //Text string of player type (black / white)
    public static List<Chessman> pieces = new List<Chessman>(); //Static list of current pieces

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;   //chess  piece sprites (Black set)
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;   //chess piece sprites (White set)

    public CharacterData character;             //Character scriptable data for this piece
    public float high, low, left, right, medle; 
    [SerializeField]
    float scaleValue;                           //Used to change scale of chess piece 
    /// <summary>
    /// Used to save and load character's data - health , speed ,stamina 
    /// </summary>
    public class CharacterRuntimeData
    {
        public int health;
        public float stamina;
        public float speed;

        public Dictionary<string,int> attack_data = new Dictionary<string, int>();

        public void SaveData(string key, int val){
            if(attack_data.ContainsKey(key))
                attack_data[key] = val;
            else
                attack_data.Add(key,val);
        }

        public int GetData(string key){
            if(attack_data.ContainsKey(key))
                return attack_data[key];
            else
                return 0;
        }

    }

    public CharacterRuntimeData pData;                  //Instance of character data

    public int PieceIndex;                              //Used as identification of piece

    //private Vector3 localScale;
    [SerializeField] private GameObject healthBar;      //Healthbar object displayed on this chess piece


    //public bool isFristMovePawn=true;
    public PieceType myPiece, opponentpiece;            //Player and  Opponent piece types

    public bool AlreadyPlayedPvP = false;               //Used to decide if player has played poker game already or not

    public List<SpellCard> cards;                       //List of spell cards that can be used in poker battle
    /// <summary>
    /// Sets photonview reference , spell card deck from data
    /// </summary>
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        healthBar.GetComponent<Image>().fillAmount = 1;
        AlreadyPlayedPvP = false;

        cards = new List<SpellCard>();

        string[] deckStr = PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"] == null ?null :  PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"].ToString().Split('_');
        foreach (var item in deckStr)
        {
            cards.Add(GameData.Get().GetPet(System.Convert.ToInt32(item)));
        }
    }
    /// <summary>
    /// Updates piece's healt
    /// </summary>
    private void Update()
    {
        UpdateHealth(pData.health);
    }

    /// <summary>
    /// Updates healthbar sprite
    /// </summary>
    /// <param name="_health">health value</param>
    public void UpdateHealth(float _health)
    {
        healthBar.GetComponent<Image>().fillAmount = _health / character.health;
    }
    /// <summary>
    /// Used to save attack data
    /// </summary>
    public void SaveHighLowLeftRightMedle(float _high, float _low, float _left, float _right, float _medle)
    {
        //Debug.Log($"<color=yellow> {name} health {character.health} </color>  high {high}" +
        //  $"low {low} left {left} right {right} medle {medle}");

        high = _high;
        left = _left;
        low = _low;
        left = _left;
        right = _right;
        medle = _medle;

        //Debug.Log("====================================================================================================================");

        //Debug.Log($"<color=yellow> {name} health {character.health} </color>  high {high}" +
        //  $"low {low} left {left} right {right} medle {medle}");
    }

    /// <summary>
    /// Sets info for the network object after instantiation
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

       // Debug.Log("OnPhotonInstantiate");
        object[] data = info.photonView.InstantiationData;
        transform.SetParent(Game.Get().Board.transform);
        transform.localScale = scaleValue * Vector3.one;
        name = data[0].ToString();
        type = (PieceType)data[1];
        playerType = (PlayerType)data[2];
        SetXBoard((int)data[3]);
        SetYBoard((int)data[4]);
        character = GameData.Get().GetCharacter(data[6].ToString());
        pData = new CharacterRuntimeData();
        pData.health = character.health;
        //pData.stamina = character.stamina;
        //characterHealth = character.health;
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
        isFirstmove = true;
    }
    /// <summary>
    /// Update data after instantiation
    /// </summary>
    /// <param name="data">Instantiation data</param>
    public void OnInstantiate(object[] data)
    {

       // Debug.Log("OnInstantiate");

        transform.SetParent(Game.Get().Board.transform);
        name = data[0].ToString();
        type = (PieceType)data[1];
        playerType = (PlayerType)data[2];
        SetXBoard((int)data[3]);
        SetYBoard((int)data[4]);
        character = GameData.Get().GetCharacter(data[6].ToString());
        pData = new CharacterRuntimeData();
        pData.health = character.health;
        //pData.stamina = character.stamina;
        //characterHealth = character.health;
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
        isFirstmove = true;

    }

    /// <summary>
    /// Destroys piece from static pieces data at destroy times
    /// </summary>
    private void OnDestroy()
    {
        pieces.Remove(this);
    }
    /// <summary>
    /// Activate piece on board
    /// </summary>
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        player = playerType == PlayerType.Black ? "black" : "white";

        switch (type)
        {
            case PieceType.Queen: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_queen : white_queen; break;
            case PieceType.Knight: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_knight : white_knight; break;
            case PieceType.Bishop: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_bishop : white_bishop; break;
            case PieceType.King: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_king : white_king; break;
            case PieceType.Rook: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_rook : white_rook; break;
            case PieceType.Pawn: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_pawn : white_pawn; break;
        }
        if(type == PieceType.Pawn) ShowIndicator();
    }
    /// <summary>
    /// Get sprite from sprite renderer component
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite()
    {
        return this.GetComponent<SpriteRenderer>().sprite;
    }

    /// <summary>
    /// Set position coordinate with respect to board
    /// </summary>
    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        
        x *= 0.72f;
        y *= 0.72f;
        // x *= 0.63f;
        // y *= 0.62f;
      
        x +=  -2.72f;
         y += -2.66f;
    
        //x += -6.0f;
        //y += -6.0f;

        //x += -1f;
        //y += -0.85f;
       // if(xBoard == 0 && yBoard < 4)
       // {
            this.transform.position = Game.Get().boardPositions.Find(x => x.xBoard == xBoard && x.yboard == yBoard).boardPoint.position;
       // }
       // else 
      //  {
           // this.transform.position = new Vector3(x,y,-1.0f);
       // }      
    }
    /// <summary>
    /// Get X position on board
    /// </summary>
    /// <returns>X position  on board</returns>
    public int GetXboard()
    {
        return xBoard;
    }
    /// <summary>
    /// Get Y position on board
    /// </summary>
    /// <returns>Y position  on board</returns>
    public int GetYboard()
    {
        return yBoard;
    }
    /// <summary>
    /// Set X board position
    /// </summary>
    /// <param name="x">X value</param>
    public void SetXBoard(int x)
    {
        xBoard = x;
    }
    /// <summary>
    /// Set Y board position
    /// </summary>
    /// <param name="y">Y value</param>
    public void SetYBoard(int y)
    {
        yBoard = y;
    }
    /// <summary>
    /// Highlights available movement positions on boards for the selected chess piece 
    /// </summary>
    private void OnMouseUp()
    {
        if (!Game.Get().IsGameOver() && Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn)
        {
            
            if(type == PieceType.King && Game.Get().checkForKing()){
                photonView.RPC("DestroyMovePlatesRPC", RpcTarget.AllViaServer);
                photonView.RPC("InitiateMovePlatesRPC", RpcTarget.AllViaServer, type);
                if (PVPManager.manager.moveChoiceConfirmation.gameObject.activeSelf)
                {
                    PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(false);
                    PVPManager.manager.selectedMove = null;
                }
            }else if(type != PieceType.King){
                photonView.RPC("DestroyMovePlatesRPC", RpcTarget.AllViaServer);
                photonView.RPC("InitiateMovePlatesRPC", RpcTarget.AllViaServer, type);
                if (PVPManager.manager.moveChoiceConfirmation.gameObject.activeSelf)
                {
                    PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(false);
                    PVPManager.manager.selectedMove = null;
                }
            }
            
        }
    }
    /// <summary>
    /// Destroy highlighted position (move plates) objects
    /// </summary>
    public void DestroyMovePlates()
    {
        photonView.RPC("DestroyMovePlatesRPC", RpcTarget.AllViaServer);

    }
    /// <summary>
    /// Set opponent piece type in other player device
    /// </summary>
    /// <param name="type"></param>
    [PunRPC]
    public void SetPieceType(PieceType type)
    {
        PVPManager.manager.opponentpiece = type;
    }
    bool isPlateInstantiated = false;         // Bool to check if plate is instantiated or not
                                              
    /// <summary>
    /// Instantiate move plates on available movement positions for the given input piecetype
    /// </summary>
    /// <param name="type">Type of chess piece</param>
    public void InitiateMovePlates(PieceType type)
    {
        //Debug.Log("InitiateMovePlates");

        switch (type)
        {
            case PieceType.Queen:
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);

                break;
            case PieceType.Knight:
                LMovePlate();
                break;
            case PieceType.Bishop:
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case PieceType.King:
                SurroundMovePlate();
                break;
            case PieceType.Rook:
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case PieceType.Pawn:
                Vector2 piecePosition = new Vector2(xBoard, yBoard);
                Game sc = controller.GetComponent<Game>();
                if (playerType == PlayerType.Black)
                {
                  //  if(GameManager.instace.isFristMovePawn)
                    if (sc.GetPosition(xBoard,yBoard).GetComponent<Chessman>().isFirstmove)
                    {
                        sc.GetPosition(xBoard,yBoard).GetComponent<Chessman>().isFirstmove = false;
                        // Debug.LogError("*****Black Two by Two X :" + xBoard + " Y :" + yBoard);
                        //   Debug.LogError("*****FIRST PAWN " + GameManager.instace.isFristMovePawn);
                        if ((sc.GetPosition(xBoard, yBoard - 2) == null || sc.GetPosition(xBoard, yBoard - 2).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard, yBoard - 1) == null)
                            PawnMovePlateBlack(xBoard, yBoard - 2, piecePosition);
                        if (sc.GetPosition(xBoard, yBoard - 1) == null || sc.GetPosition(xBoard, yBoard - 1).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard, yBoard - 1, piecePosition);
                        if ((sc.GetPosition(xBoard - 2, yBoard) == null || sc.GetPosition(xBoard - 2, yBoard).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard - 1, yBoard) == null)
                            PawnMovePlateBlack(xBoard - 2, yBoard, piecePosition);
                        if (sc.GetPosition(xBoard - 1, yBoard) == null || sc.GetPosition(xBoard - 1, yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard - 1, yBoard, piecePosition);
                    }
                    else
                    {

                        //     Debug.LogError("*****Black One by One X :" + xBoard + " Y :" + yBoard);
                        //       Debug.LogError("*****Black One to One");
                        if (sc.GetPosition(xBoard, yBoard - 1) == null || sc.GetPosition(xBoard, yBoard - 1).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard, yBoard - 1, piecePosition, true);
                        if (sc.GetPosition(xBoard - 1, yBoard) == null || sc.GetPosition(xBoard - 1, yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard - 1, yBoard, piecePosition);
                    }

                }
                else
                {


                   // if (GameManager.instace.isFristMovePawn)
                    if(sc.GetPosition(xBoard,yBoard).GetComponent<Chessman>().isFirstmove)
                        {
                        sc.GetPosition(xBoard,yBoard).GetComponent<Chessman>().isFirstmove = false;

                        //    Debug.LogError("*****White Two by Two X :" + xBoard + " Y :" + yBoard);
                        //    Debug.LogError("*****FIRST PAWN " + GameManager.instace.isFristMovePawn);
                        if ((sc.GetPosition(xBoard, yBoard + 2) == null || sc.GetPosition(xBoard, yBoard + 2).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard, yBoard + 1) == null)
                            PawnMovePlate(xBoard, yBoard + 2, piecePosition);
                        if (sc.GetPosition(xBoard, yBoard + 1) == null || sc.GetPosition(xBoard, yBoard + 1).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard, yBoard + 1, piecePosition);
                        if ((sc.GetPosition(xBoard + 2, yBoard) == null || sc.GetPosition(xBoard + 2, yBoard).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard + 1, yBoard) == null)
                            PawnMovePlate(xBoard + 2, yBoard, piecePosition);
                        if (sc.GetPosition(xBoard + 1, yBoard) == null || sc.GetPosition(xBoard + 1, yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard + 1, yBoard, piecePosition);
                    }
                    else
                    {

                        //   Debug.LogError("*****White One by One X :" + xBoard + " Y :" + yBoard);
                        //     Debug.LogError("*****White One to One");
                        if (sc.GetPosition(xBoard, yBoard + 1) == null || sc.GetPosition(xBoard, yBoard + 1).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard, yBoard + 1, piecePosition, true);
                        if (sc.GetPosition(xBoard + 1, yBoard) == null || sc.GetPosition(xBoard + 1, yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard + 1, yBoard, piecePosition);
                    }
                }

                break;
        }

        if (isPlateInstantiated)
        {
            SetLastPieceInfo(type);
            isPlateInstantiated = false;
        }
    }
    /// <summary>
    /// Used to keep temporary type of last piece
    /// </summary>
    private void SetLastPieceInfo(PieceType type)
    {
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            // Debug.Log("LAST PIECE "+)
            PVPManager.manager.tempPiece = type;
            // photonView.RPC("SetPieceType",RpcTarget.Others,type);
        }
    }
    /// <summary>
    /// Logic to spawn move plate with respect to input data
    /// </summary>
    /// <param name="xIncrement">plus x position</param>
    /// <param name="yIncrement">plus y position</param>
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
       // Debug.Log("LineMovePlate");

        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
            isPlateInstantiated = true;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            Chessman chessman = sc.GetPosition(x,y).GetComponent<Chessman>();
            PieceType pieceType = chessman.type;
            PVPManager.manager.SetOpponentAttackPieceInfo(pieceType,chessman);
            MovePlateAttackSpawn(x, y);
            isPlateInstantiated = true;
        }
    }
    /// <summary>
    /// Used for generating move plates for Knight
    /// </summary>
    public void LMovePlate()
    {
       // Debug.Log("LMovePlate");

        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }
    /// <summary>
    /// Move plate generation for King
    /// </summary>
    public void SurroundMovePlate()
    {
       // Debug.Log("LMovePlate");

        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }
    /// <summary>
    /// Used for other move plate generation methods
    /// </summary>
    public void PointMovePlate(int x, int y)
    {
        //Debug.Log($"PointMovePlate x is {x} y is {y}");

        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                Chessman chessman = sc.GetPosition(x,y).GetComponent<Chessman>();
                PieceType pieceType =chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType,chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
            }
        }
    }
    /// <summary>
    /// Pawn move plates generation logic
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="y">y position</param>
    /// <param name="pieceVector">piece position</param>
    /// <param name="isAttackSibling">bool to check if attacks sibling</param>
    public void PawnMovePlate(int x, int y, Vector2 pieceVector, bool isAttackSibling = false)
    {
        //Debug.Log($"PawnMovePlate x {x} and y {y}");

        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            //OLD Working
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
               // Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            }
            if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) != null &&
               sc.GetPosition(x, y).GetComponent<Chessman>().player != player)//&& isAttackSibling )
            {
                //if((x) == (pieceVector.x) && y == (pieceVector.y))
                // {
                Chessman chessman = sc.GetPosition(x,y).GetComponent<Chessman>();
                PieceType pieceType = chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType,chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
                // }
               // Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} {y}   </color>");
            }
        }
    }
    /// <summary>
    /// Pawn move plates for black type
    /// </summary>
    public void PawnMovePlateBlack(int x, int y, Vector2 pieceVector, bool isAttackSibling = false)
    {
        //Debug.Log($"PawnMovePlate x {x} and y {y}");

        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            //OLD Working
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
               // Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            }
            if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) != null &&
               sc.GetPosition(x, y).GetComponent<Chessman>().player != player) //&& !isAttackSibling )
            {
                //if(x == (pieceVector.x) && y == (pieceVector.y))
                //{
                Chessman chessman = sc.GetPosition(x,y).GetComponent<Chessman>();
                PieceType pieceType = chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType,chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
               // Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} {y}   </color>");
                // }
            }
        }
    }
    /// <summary>
    /// Moveplate instantiation on network
    /// </summary>
    /// <param name="matrixX">x position</param>
    /// <param name="matrixY">y position</param>
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        //Debug.Log($"MovePlateSpawn x {matrixX} y {matrixY}");

        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        object[] data = new object[] { playerType, type, matrixX, matrixY, false, PieceIndex };
        Vector3 pos = Game.Get().boardPositions.Find(x => x.xBoard == matrixX && x.yboard == matrixY).boardPoint.position;
        pos.z = -3f;
        GameObject mp = PhotonNetwork.Instantiate("movePlate", pos, Quaternion.identity, 0, data);
    }
    /// <summary>
    /// Move plate spawn for "Attack" - when opponent piece can be attacked
    /// </summary>
    /// <param name="matrixX">x position</param>
    /// <param name="matrixY">y position</param>
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        //Debug.Log($"MovePlateAttackSpawn x {matrixX} y {matrixY}");

        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;
        Vector3 pos = Game.Get().boardPositions.Find(x => x.xBoard == matrixX && x.yboard == matrixY).boardPoint.position;
        pos.z = -3f;
        object[] data = new object[] { playerType, type, matrixX, matrixY, true, PieceIndex };
        GameObject mp = PhotonNetwork.Instantiate("movePlate", pos, Quaternion.identity, 0, data);
    }
    /// <summary>
    /// Get piece with respect to input data
    /// </summary>
    /// <param name="ptype">Player type (black / white)</param>
    /// <param name="type">Piece  type</param>
    /// <param name="ind">Piece index</param>
    /// <returns></returns>
    public static Chessman GetPiece(PlayerType ptype, PieceType type, int ind)
    {
        foreach (Chessman item in pieces)
        {
            if (item.playerType == ptype && item.type == type && item.PieceIndex == ind)
                return item;
        }
        return null;
    }
    /// <summary>
    /// Get piece by piece index
    /// </summary>
    /// <param name="ind">PieceIndex</param>
    /// <returns></returns>
    public static Chessman GetPiece(int ind)
    {
        foreach (Chessman item in pieces)
        {
            if (item.PieceIndex == ind)
                return item;
        }
        return null;
    }

    /// <summary>
    /// Get list of all pieces with respect to input player type-(black / white)
    /// </summary>
    /// <param name="ptype">Player type(Black/White)</param>
    /// <returns>List of pieces</returns>
    public static List<Chessman> GetPiecesOfPlayer(PlayerType ptype)
    {
        List<Chessman> res = new List<Chessman>();
        foreach (Chessman item in pieces)
        {
            if (item.playerType == ptype)
                res.Add(item);
        }
        return res;
    }
    /// <summary>
    /// RPC for move plates instantion- spawns plates in other player device 
    /// </summary>
    /// <param name="type"></param>
    #region Pun calls
    [PunRPC]
    public void InitiateMovePlatesRPC(PieceType type)
    {
        InitiateMovePlates(type);
    }
    /// <summary>
    /// Destroys  moveplates in devices
    /// </summary>
    [PunRPC]
    public void DestroyMovePlatesRPC()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    #endregion

    /// <summary>
    /// Condition check incase of check mates
    /// </summary>
    /// <returns></returns>
    public bool canDefendKing(){
        bool canDefend = false;
        Game sc = controller.GetComponent<Game>();
        List<Vector2> vecs = new List<Vector2>();
        GameObject[,] Absolute_past = sc.GetPresent();
        List<Chessman> prev_pieces = Chessman.GetCurrPieces();
        switch (type)
        {
            case PieceType.King:
                vecs = new List<Vector2>(){new Vector2(0,1),new Vector2(0,-1),new Vector2(-1,-1),new Vector2(-1,-0),new Vector2(-1,1),new Vector2(1,-1),new Vector2(1,0),new Vector2(1,1)};
                foreach (var item in vecs)
                {
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);
                    Chessman.SetCurrPieces(prev_pieces);

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    if (sc.FuturePositionOnBoard(x, y) && (sc.GetFuturePosition(x,y) == null || sc.GetFuturePosition(x,y)?.GetComponent<Chessman>().playerType != playerType))
                    {
                        if(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break;}
                    }
                    Chessman.SetCurrPieces(prev_pieces);
                    sc.SetPresent(prev_present);
                }
                break;
            case PieceType.Queen:
                vecs = new List<Vector2>(){new Vector2(1,1),new Vector2(1,-1),new Vector2(-1,1),new Vector2(-1,-1), new Vector2(1,0),new Vector2(0,1),new Vector2(-1,0),new Vector2(0,-1)};
                foreach (var item in vecs)
                {

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();

                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);

                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }

                    if(canDefend) break;
                    
                    sc.SetFuture(prev_present);
                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        List<Chessman> p = Chessman.GetCurrPieces();
                        p.Remove(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>());
                        Chessman.SetCurrPieces(p);
                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces);break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }    
                }
                break;
            case PieceType.Rook:
                vecs = new List<Vector2>(){new Vector2(1,0),new Vector2(0,1),new Vector2(-1,0),new Vector2(0,-1)};
                foreach (var item in vecs)
                {

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();

                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);

                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }

                    if(canDefend) break;

                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        sc.SetFuture(prev_present);
                        if(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces);break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }    
                }
                break;
            case PieceType.Bishop:
                vecs = new List<Vector2>(){new Vector2(1,1),new Vector2(1,-1),new Vector2(-1,1),new Vector2(-1,-1)};
                foreach (var item in vecs)
                {

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();

                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);
                        
                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }

                    if(canDefend) break;

                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        sc.SetFuture(prev_present);
                        if(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this,x,y);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }    
                }
                break;
            case PieceType.Knight:
                vecs = new List<Vector2>(){new Vector2(1,2),new Vector2(-1,2),new Vector2(2,1),new Vector2(2,-1),new Vector2(1,-2),new Vector2(-1,-2),new Vector2(-2,1),new Vector2(-2,-1)};
                foreach (var item in vecs)
                {
                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);

                    if (sc.FuturePositionOnBoard(x, y))
                    {
                        GameObject cp = sc.GetFuturePosition(x, y);
                        if(cp == null){
                            sc.SetFuturePosition(this,x,y);
                            sc.SetFuturePosition(null,xBoard,yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if(canDefend){sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces);break;}
                            sc.SetPresent(prev_present); 
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        else if (cp?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>().playerType != playerType){
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(x,y)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this,x,y);
                            sc.SetFuturePosition(null,xBoard,yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                            sc.SetPresent(prev_present); 
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                    }
                }
                break;
            case PieceType.Pawn:
            for (int i = 1; i <= 2; i++)
            {
                    //if(!GameManager.instace.isFristMovePawn && i > 1){
                   
                    if(!isFirstmove && i > 1)
                        {
                            break;
                }
                GameObject[,] prev_present = sc.GetPresent();
                sc.SetFuture(prev_present);
                if(playerType == PlayerType.Black){
                    if(sc.GetFuturePosition(xBoard-i,yBoard) == null){
                        sc.SetFuturePosition(this, xBoard-i,yBoard);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if(sc.GetFuturePosition(xBoard-i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                        if(sc.GetFuturePosition(xBoard - i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(xBoard - i,yBoard)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, xBoard-i,yBoard);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }else{
                    if(sc.GetFuturePosition(xBoard+i,yBoard) == null){
                        sc.SetFuturePosition(this, xBoard+i,yBoard);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if(sc.GetFuturePosition(xBoard+i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                        if(sc.GetFuturePosition(xBoard+i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(xBoard+i,yBoard)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, xBoard+i,yBoard);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }
            }

            for (int i = 1; i <= 2; i++)
            {
                    // if(!GameManager.instace.isFristMovePawn && i > 1){
                    
                    if(isFirstmove && i > 1)
                    {
                        break;
                }
                GameObject[,] prev_present = sc.GetPresent();
                sc.SetFuture(prev_present);
                if(playerType == PlayerType.Black){
                    if(sc.GetFuturePosition(xBoard,yBoard-i) == null){
                        sc.SetFuturePosition(this, xBoard,yBoard-i);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if(sc.GetFuturePosition(xBoard,yBoard-i)?.GetComponent<Chessman>().playerType != playerType){
                        if(sc.GetFuturePosition(xBoard,yBoard-i)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(xBoard,yBoard-i)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, xBoard,yBoard-i);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }else{
                    if(sc.GetFuturePosition(xBoard,yBoard+i) == null){
                        sc.SetFuturePosition(this, xBoard,yBoard-i);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if(sc.GetFuturePosition(xBoard,yBoard+i)?.GetComponent<Chessman>().playerType != playerType){
                        if(sc.GetFuturePosition(xBoard,yBoard+i)?.GetComponent<Chessman>().playerType != playerType){
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(xBoard,yBoard+i)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        } 
                        sc.SetFuturePosition(this, xBoard,yBoard+i);
                        sc.SetFuturePosition(null,xBoard,yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if(canDefend){sc.SetPresent(prev_present);Chessman.SetCurrPieces(prev_pieces); break;}
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }
            }
            break;
            
        }
        sc.SetPresent(Absolute_past);
        Chessman.SetCurrPieces(prev_pieces);
        return canDefend;
    }
    /// <summary>
    /// Check is opponent piece  is king
    /// </summary>
    /// <returns></returns>
    public bool IsCheckforKing(){
        bool isCheck = false;
        Game sc = controller.GetComponent<Game>();
        List<Vector2> vecs = new List<Vector2>();
        switch (type)
        {
            case PieceType.Pawn:
            
            for (int i = 1; i <= 2; i++)
            {
                    //if(!GameManager.instace.isFristMovePawn && i > 1){
                    if(!isFirstmove && i > 1) { 
                        break;
                }
                if(playerType == PlayerType.Black){
                    if(sc.GetPosition(xBoard-i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                        isCheck = sc.GetPosition(xBoard-i,yBoard)?.GetComponent<Chessman>().type == PieceType.King; 
                        if(isCheck) break;
                        break;
                    }
                }else{
                    if(sc.GetPosition(xBoard+i,yBoard)?.GetComponent<Chessman>().playerType != playerType){
                        isCheck = sc.GetPosition(xBoard+i,yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                        if(isCheck) break;
                        break;
                    }
                }
            }
            for (int i = 1; i <= 2; i++)
            {
                    //if(!GameManager.instace.isFristMovePawn && i > 1){
                    
                    if(isFirstmove && i > 1)
                    {
                        break;
                }
                if(playerType == PlayerType.Black){
                    if(sc.GetPosition(xBoard,yBoard-i)?.GetComponent<Chessman>().playerType != playerType){
                        isCheck = sc.GetPosition(xBoard-i,yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                        if(isCheck) break;
                        break;
                    }
                }else{
                    if(sc.GetPosition(xBoard,yBoard+i)?.GetComponent<Chessman>().playerType != playerType){
                        isCheck = sc.GetPosition(xBoard+i,yBoard)?.GetComponent<Chessman>().type == PieceType.King; 
                        if(isCheck) break;
                        break;
                    }
                }
            }

            break;
            case PieceType.Bishop:
            vecs = new List<Vector2>(){new Vector2(1,1),new Vector2(1,-1),new Vector2(-1,1),new Vector2(-1,-1)};
            foreach (var item in vecs)
            {
                int x = xBoard + (int)item.x;
                int y = yBoard + (int)item.y;

                while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
                {
                    
                    x += (int)item.x;
                    y += (int)item.y;
                    
                }

                if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                {
                    isCheck = sc.GetPosition(x, y)?.GetComponent<Chessman>().type == PieceType.King;
                    if(isCheck) break;
                }    
            }
            break;
            case PieceType.Rook:
            vecs = new List<Vector2>(){new Vector2(1,0),new Vector2(0,1),new Vector2(-1,0),new Vector2(0,-1)};
            foreach (var item in vecs)
            {
                int x = xBoard + (int)item.x;
                int y = yBoard + (int)item.y;

                while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
                {
                    
                    x += (int)item.x;
                    y += (int)item.y;
                    
                }

                if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                {
                    isCheck = sc.GetPosition(x, y)?.GetComponent<Chessman>().type == PieceType.King;
                    if(isCheck) break;
                }    
            }
            break;
            case PieceType.Knight:
            vecs = new List<Vector2>(){new Vector2(1,2),new Vector2(-1,2),new Vector2(2,1),new Vector2(2,-1),new Vector2(1,-2),new Vector2(-1,-2),new Vector2(-2,1),new Vector2(-2,-1)};
            foreach (var item in vecs)
            {
                int x = xBoard + (int)item.x;
                int y = yBoard + (int)item.y;
                if (sc.PositionOnBoard(x, y))
                {
                    GameObject cp = sc.GetPosition(x, y);
                    if (cp?.GetComponent<Chessman>().playerType != playerType)
                    {
                        isCheck = cp?.GetComponent<Chessman>().type == PieceType.King;
                        if(isCheck) break;
                    }
                }
            }
            break;
            case PieceType.Queen:
            vecs = new List<Vector2>(){new Vector2(1,1),new Vector2(1,-1),new Vector2(-1,1),new Vector2(-1,-1), new Vector2(1,0),new Vector2(0,1),new Vector2(-1,0),new Vector2(0,-1)};
            foreach (var item in vecs)
            {
                int x = xBoard + (int)item.x;
                int y = yBoard + (int)item.y;

                while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
                {
                    
                    x += (int)item.x;
                    y += (int)item.y;
                    
                }

                if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                {
                    isCheck = sc.GetPosition(x, y)?.GetComponent<Chessman>().type == PieceType.King;
                    if(isCheck) break;
                }    
            }
            break;
            case PieceType.King:
            vecs = new List<Vector2>(){new Vector2(0,1),new Vector2(0,-1),new Vector2(-1,-1),new Vector2(-1,-0),new Vector2(-1,1),new Vector2(1,-1),new Vector2(1,0),new Vector2(1,1)};
            foreach (var item in vecs)
            {
                int x = xBoard + (int)item.x;
                int y = yBoard + (int)item.y;
                if (sc.PositionOnBoard(x, y))
                {
                    GameObject cp = sc.GetPosition(x, y);
                    if (cp?.GetComponent<Chessman>().playerType != playerType)
                    {
                        isCheck = cp?.GetComponent<Chessman>().type == PieceType.King;
                        if(isCheck) break;
                    }
                }
            }
            break;
        }
        return isCheck;
    }
    /// <summary>
    /// Get list of current pieces on board
    /// </summary>
    /// <returns></returns>
    public static List<Chessman> GetCurrPieces(){
        List<Chessman> data = new List<Chessman>();
        foreach (var item in pieces)
        {
            data.Add(item);
        }
        return data;
    }
    /// <summary>
    /// Add board pieces in  list
    /// </summary>
    /// <param name="data"></param>
    public static void SetCurrPieces(List<Chessman> data){
        pieces = new List<Chessman>();
        foreach (var item in data)
        {
            pieces.Add(item);
        }        
    }
    /// <summary>
    /// Show healthbar
    /// </summary>
    public void OnMouseEnter()
    {
        healthBar.transform.parent.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hide healthbar
    /// </summary>
    public void OnMouseExit()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
    }
    /// <summary>
    /// Show plates indicator
    /// </summary>
    public void ShowIndicator()
    {
        SpriteRenderer currentSprite = Game.Get().plates[xBoard,yBoard].GetComponent<SpriteRenderer>();
        SpriteRenderer currentSpriteIndicator= Game.Get().platesIndicator[xBoard,yBoard].GetComponent<SpriteRenderer>();
        Color c = currentSprite.color;
        c.a = 1;
        currentSpriteIndicator.color = c;
    }
  
}
