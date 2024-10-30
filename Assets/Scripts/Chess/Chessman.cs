using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public enum PieceType
{
    Queen,
    King,
    Rook,
    Knight,
    Bishop,
    Pawn
}
public enum PlayerType
{
    Black,
    White,
    None
}
public interface IHealthBar
{
    public void UpdateHealth(float _health);
}
public interface ISaveHighLowLeftRightMedle
{
    public void SaveHighLowLeftRightMedle(float high, float low, float left, float right, float medle);
}
public class Chessman : MonoBehaviour, IPunInstantiateMagicCallback, IHealthBar, ISaveHighLowLeftRightMedle
{
    #region Attributes
    [Header("GameObjects")]
    public GameObject controller;
    public GameObject movePlate;
    [SerializeField] private GameObject healthBar;

    [Header("Booleans")]
    [SerializeField] private bool pawn;
    public bool AlreadyPlayedPvP = false;
    bool isPlateInstantiated = false;

    [Header("References")]
    private PawnClass pawnClass;//PawnClass reference
    private PhotonView photonView;

    [Header("Integers")]
    public int xBoard = -1;
    public int yBoard = -1;
    public int PieceIndex;

    [Header("Floats")]
    public float high, low, left, right, medle;
    [SerializeField] float scaleValue;

    [Header("Sprites")]
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    [Header("Lists")]
    public static List<Chessman> pieces = new List<Chessman>();
    public List<SpellCard> cards;

    [Header("Strings")]
    public string player;

    [Header("Vector3")]
    private Vector3 localScale;

    [Header("Class References")]
    public CharacterData character;
    public CharacterRuntimeData pData;

    [Header("Enums")]
    public PieceType type;
    public PlayerType playerType;
    public PieceType myPiece, opponentpiece;
    #endregion

    #region Unity Methods
    private void Start()
    {
        pawnClass = GetComponent<PawnClass>();
        pawn = pawnClass.IsMoved();
        photonView = GetComponent<PhotonView>();
        healthBar.GetComponent<Image>().fillAmount = 1;
        AlreadyPlayedPvP = false;
        cards = new List<SpellCard>();
        string[] deckStr = PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"] == null ? null : PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"].ToString().Split('_');
        foreach (var item in deckStr)
        {
            cards.Add(GameData.Get().GetPet(System.Convert.ToInt32(item)));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Debug.Log(pawnClass.IsMoved());
        UpdateHealth(pData.health);
    }
    #endregion

    #region Pun calls
    [PunRPC]
    public void SetPieceType(PieceType type)
    {
        PVPManager.manager.opponentpiece = type;
    }
    [PunRPC]
    public void InitiateMovePlatesRPC(PieceType type)
    {
        InitiateMovePlates(type);
    }
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

    #region Class Methods
    public class CharacterRuntimeData
    {
        public int health;
        public float stamina;
        public float speed;
        public Dictionary<string, int> attack_data = new Dictionary<string, int>();
        public void SaveData(string key, int val)
        {
            if (attack_data.ContainsKey(key)) attack_data[key] = val;
            else attack_data.Add(key, val);
        }
        public int GetData(string key)
        {
            if (attack_data.ContainsKey(key)) return attack_data[key];
            else
                return 0;
        }
    }
    #endregion

    #region Board Methods
    public int GetXboard()
    {
        return xBoard;
    }
    public int GetYboard()
    {
        return yBoard;
    }
    public void SetXBoard(int x)
    {
        xBoard = x;
    }
    public void SetYBoard(int y)
    {
        yBoard = y;
    }
    #endregion

    #region Mouse Methods
    private void OnMouseUp()
    {
        if (!Game.Get().IsGameOver() && Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn)
        {
            if (type == PieceType.King && Game.Get().checkForKing())
            {
                photonView.RPC("DestroyMovePlatesRPC", RpcTarget.AllViaServer);
                photonView.RPC("InitiateMovePlatesRPC", RpcTarget.AllViaServer, type);
                if (PVPManager.manager.moveChoiceConfirmation.gameObject.activeSelf)
                {
                    PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(false);
                    PVPManager.manager.selectedMove = null;
                }
            }
            else if (type != PieceType.King)
            {
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
    public void OnMouseEnter()
    {
        healthBar.transform.parent.gameObject.SetActive(true);
    }
    public void OnMouseExit()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region Move Plate Methods
    public void DestroyMovePlates()
    {
        photonView.RPC("DestroyMovePlatesRPC", RpcTarget.AllViaServer);
    }
    public void InitiateMovePlates(PieceType type)
    {
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
                    if (!pawnClass.IsMoved())
                    {
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
                        Debug.Log("*****Black One by One X :" + xBoard + " Y :" + yBoard);
                        if (sc.GetPosition(xBoard, yBoard - 1) == null || sc.GetPosition(xBoard, yBoard - 1).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard, yBoard - 1, piecePosition, true);
                        if (sc.GetPosition(xBoard - 1, yBoard) == null || sc.GetPosition(xBoard - 1, yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard - 1, yBoard, piecePosition);
                    }
                }
                else
                {
                    if (!pawnClass.IsMoved())
                    {
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
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
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
            Chessman chessman = sc.GetPosition(x, y).GetComponent<Chessman>();
            PieceType pieceType = chessman.type;
            PVPManager.manager.SetOpponentAttackPieceInfo(pieceType, chessman);
            MovePlateAttackSpawn(x, y);
            isPlateInstantiated = true;
        }
    }
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }
    public void PointMovePlate(int x, int y)
    {
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
                Chessman chessman = sc.GetPosition(x, y).GetComponent<Chessman>();
                PieceType pieceType = chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType, chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
            }
        }
    }
    public void PawnMovePlate(int x, int y, Vector2 pieceVector, bool isAttackSibling = false)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
            }
            if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) != null && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
            {
                Chessman chessman = sc.GetPosition(x, y).GetComponent<Chessman>();
                PieceType pieceType = chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType, chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
            }
        }
    }
    public void PawnMovePlateBlack(int x, int y, Vector2 pieceVector, bool isAttackSibling = false)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
            }
            if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) != null &&
               sc.GetPosition(x, y).GetComponent<Chessman>().player != player) //&& !isAttackSibling )
            {
                Chessman chessman = sc.GetPosition(x, y).GetComponent<Chessman>();
                PieceType pieceType = chessman.type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType, chessman);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
            }
        }
    }
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
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
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
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
    #endregion

    #region King Methods
    public bool canDefendKing()
    {
        bool canDefend = false;
        Game sc = controller.GetComponent<Game>();
        List<Vector2> vecs = new List<Vector2>();
        GameObject[,] Absolute_past = sc.GetPresent();
        List<Chessman> prev_pieces = Chessman.GetCurrPieces();
        switch (type)
        {
            case PieceType.King:
                vecs = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, -0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1) };
                foreach (var item in vecs)
                {
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);
                    Chessman.SetCurrPieces(prev_pieces);
                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    if (sc.FuturePositionOnBoard(x, y) && (sc.GetFuturePosition(x, y) == null || sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType))
                    {
                        if (sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x, y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                    }
                    Chessman.SetCurrPieces(prev_pieces);
                    sc.SetPresent(prev_present);
                }
                break;
            case PieceType.Queen:
                vecs = new List<Vector2>() { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
                foreach (var item in vecs)
                {
                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();
                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if (canDefend) break;
                    sc.SetFuture(prev_present);
                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        List<Chessman> p = Chessman.GetCurrPieces();
                        p.Remove(sc.GetFuturePosition(x, y)?.GetComponent<Chessman>());
                        Chessman.SetCurrPieces(p);
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }
                break;
            case PieceType.Rook:
                vecs = new List<Vector2>() { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
                foreach (var item in vecs)
                {

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();

                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }

                    if (canDefend) break;

                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        sc.SetFuture(prev_present);
                        if (sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x, y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }
                break;
            case PieceType.Bishop:
                vecs = new List<Vector2>() { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
                foreach (var item in vecs)
                {

                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();

                    while (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y) == null)
                    {
                        sc.SetFuture(prev_present);
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        x += (int)item.x;
                        y += (int)item.y;
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                    if (canDefend) break;
                    if (sc.FuturePositionOnBoard(x, y) && sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                    {
                        sc.SetFuture(prev_present);
                        if (sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            List<Chessman> p = Chessman.GetCurrPieces();
                            p.Remove(sc.GetFuturePosition(x, y)?.GetComponent<Chessman>());
                            Chessman.SetCurrPieces(p);
                        }
                        sc.SetFuturePosition(this, x, y);
                        sc.SetFuturePosition(null, xBoard, yBoard);
                        sc.SetPresent(sc.GetFuture());
                        canDefend = !sc.checkForKing();
                        if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                        sc.SetPresent(prev_present);
                        Chessman.SetCurrPieces(prev_pieces);
                    }
                }
                break;
            case PieceType.Knight:
                vecs = new List<Vector2>() { new Vector2(1, 2), new Vector2(-1, 2), new Vector2(2, 1), new Vector2(2, -1), new Vector2(1, -2), new Vector2(-1, -2), new Vector2(-2, 1), new Vector2(-2, -1) };
                foreach (var item in vecs)
                {
                    int x = xBoard + (int)item.x;
                    int y = yBoard + (int)item.y;
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);
                    if (sc.FuturePositionOnBoard(x, y))
                    {
                        GameObject cp = sc.GetFuturePosition(x, y);
                        if (cp == null)
                        {
                            sc.SetFuturePosition(this, x, y);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        else if (cp?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if (sc.GetFuturePosition(x, y)?.GetComponent<Chessman>().playerType != playerType)
                            {
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(x, y)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this, x, y);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                    }
                }
                break;
            case PieceType.Pawn:
                for (int i = 1; i <= 2; i++)
                {
                    if (pawnClass.IsMoved() && i > 1)
                    {
                        break;
                    }
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);
                    if (playerType == PlayerType.Black)
                    {
                        if (sc.GetFuturePosition(xBoard - i, yBoard) == null)
                        {
                            sc.SetFuturePosition(this, xBoard - i, yBoard);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        if (sc.GetFuturePosition(xBoard - i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if (sc.GetFuturePosition(xBoard - i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                            {
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(xBoard - i, yBoard)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this, xBoard - i, yBoard);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                    }
                    else
                    {
                        if (sc.GetFuturePosition(xBoard + i, yBoard) == null)
                        {
                            sc.SetFuturePosition(this, xBoard + i, yBoard);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        if (sc.GetFuturePosition(xBoard + i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if (sc.GetFuturePosition(xBoard + i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                            {
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(xBoard + i, yBoard)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this, xBoard + i, yBoard);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                    }
                }
                for (int i = 1; i <= 2; i++)
                {
                    if (pawnClass.IsMoved() && i > 1) break;
                    GameObject[,] prev_present = sc.GetPresent();
                    sc.SetFuture(prev_present);
                    if (playerType == PlayerType.Black)
                    {
                        if (sc.GetFuturePosition(xBoard, yBoard - i) == null)
                        {
                            sc.SetFuturePosition(this, xBoard, yBoard - i);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        if (sc.GetFuturePosition(xBoard, yBoard - i)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if (sc.GetFuturePosition(xBoard, yBoard - i)?.GetComponent<Chessman>().playerType != playerType)
                            {
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(xBoard, yBoard - i)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this, xBoard, yBoard - i);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                    }
                    else
                    {
                        if (sc.GetFuturePosition(xBoard, yBoard + i) == null)
                        {
                            sc.SetFuturePosition(this, xBoard, yBoard - i);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
                            sc.SetPresent(prev_present);
                            Chessman.SetCurrPieces(prev_pieces);
                        }
                        if (sc.GetFuturePosition(xBoard, yBoard + i)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            if (sc.GetFuturePosition(xBoard, yBoard + i)?.GetComponent<Chessman>().playerType != playerType)
                            {
                                List<Chessman> p = Chessman.GetCurrPieces();
                                p.Remove(sc.GetFuturePosition(xBoard, yBoard + i)?.GetComponent<Chessman>());
                                Chessman.SetCurrPieces(p);
                            }
                            sc.SetFuturePosition(this, xBoard, yBoard + i);
                            sc.SetFuturePosition(null, xBoard, yBoard);
                            sc.SetPresent(sc.GetFuture());
                            canDefend = !sc.checkForKing();
                            if (canDefend) { sc.SetPresent(prev_present); Chessman.SetCurrPieces(prev_pieces); break; }
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
    public bool IsCheckforKing()
    {
        bool isCheck = false;
        Game sc = controller.GetComponent<Game>();
        List<Vector2> vecs = new List<Vector2>();
        switch (type)
        {
            case PieceType.Pawn:
                for (int i = 1; i <= 2; i++)
                {
                    if (pawnClass.IsMoved() && i > 1) break;
                    if (playerType == PlayerType.Black)
                    {
                        if (sc.GetPosition(xBoard - i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            isCheck = sc.GetPosition(xBoard - i, yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                            if (isCheck) break;
                            break;
                        }
                    }
                    else
                    {
                        if (sc.GetPosition(xBoard + i, yBoard)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            isCheck = sc.GetPosition(xBoard + i, yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                            if (isCheck) break;
                            break;
                        }
                    }
                }
                for (int i = 1; i <= 2; i++)
                {
                    if (pawnClass.IsMoved() && i > 1) break;
                    if (playerType == PlayerType.Black)
                    {
                        if (sc.GetPosition(xBoard, yBoard - i)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            isCheck = sc.GetPosition(xBoard - i, yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                            if (isCheck) break;
                            break;
                        }
                    }
                    else
                    {
                        if (sc.GetPosition(xBoard, yBoard + i)?.GetComponent<Chessman>().playerType != playerType)
                        {
                            isCheck = sc.GetPosition(xBoard + i, yBoard)?.GetComponent<Chessman>().type == PieceType.King;
                            if (isCheck) break;
                            break;
                        }
                    }
                }
                break;
            case PieceType.Bishop:
                vecs = new List<Vector2>() { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
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
                        if (isCheck) break;
                    }
                }
                break;
            case PieceType.Rook:
                vecs = new List<Vector2>() { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
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
                        if (isCheck) break;
                    }
                }
                break;
            case PieceType.Knight:
                vecs = new List<Vector2>() { new Vector2(1, 2), new Vector2(-1, 2), new Vector2(2, 1), new Vector2(2, -1), new Vector2(1, -2), new Vector2(-1, -2), new Vector2(-2, 1), new Vector2(-2, -1) };
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
                            if (isCheck) break;
                        }
                    }
                }
                break;
            case PieceType.Queen:
                vecs = new List<Vector2>() { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
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
                        if (isCheck) break;
                    }
                }
                break;
            case PieceType.King:
                vecs = new List<Vector2>() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, -0), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(1, 1) };
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
                            if (isCheck) break;
                        }
                    }
                }
                break;
        }
        return isCheck;
    }
    #endregion

    #region InstantiateAndDestroy
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
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
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
    }
    public void OnInstantiate(object[] data)
    {
        transform.SetParent(Game.Get().Board.transform);
        name = data[0].ToString();
        type = (PieceType)data[1];
        playerType = (PlayerType)data[2];
        SetXBoard((int)data[3]);
        SetYBoard((int)data[4]);
        character = GameData.Get().GetCharacter(data[6].ToString());
        pData = new CharacterRuntimeData();
        pData.health = character.health;
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
    }
    private void OnDestroy()
    {
        pieces.Remove(this);
    }
    public void SaveHighLowLeftRightMedle(float _high, float _low, float _left, float _right, float _medle)
    {
        high = _high;
        low = _low;
        left = _left;
        right = _right;
        medle = _medle;
    }
    #endregion

    #region Activate And ShowIndicator
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
        if (type == PieceType.Pawn) ShowIndicator();
    }
    public void ShowIndicator()
    {
        SpriteRenderer currentSprite = Game.Get().plates[xBoard, yBoard].GetComponent<SpriteRenderer>();
        SpriteRenderer currentSpriteIndicator = Game.Get().platesIndicator[xBoard, yBoard].GetComponent<SpriteRenderer>();
        Color c = currentSprite.color;
        c.a = 1;
        currentSpriteIndicator.color = c;
    }
    #endregion

    #region PieceGetters
    public static Chessman GetPiece(PlayerType ptype, PieceType type, int ind)
    {
        foreach (Chessman item in pieces)
        {
            if (item.playerType == ptype && item.type == type && item.PieceIndex == ind)
                return item;
        }
        return null;
    }
    public static Chessman GetPiece(int ind)
    {
        foreach (Chessman item in pieces)
        {
            if (item.PieceIndex == ind)
                return item;
        }
        return null;
    }
    public static List<Chessman> GetPiecesOfPlayer(PlayerType ptype)
    {
        List<Chessman> res = new List<Chessman>();
        foreach (Chessman item in pieces)
        {
            if (item.playerType == ptype) res.Add(item);
        }
        return res;
    }
    public static List<Chessman> GetCurrPieces()
    {
        List<Chessman> data = new List<Chessman>();
        foreach (var item in pieces)
        {
            data.Add(item);
        }
        return data;
    }
    #endregion

    #region SpriteGetter
    public Sprite GetSprite()
    {
        return this.GetComponent<SpriteRenderer>().sprite;
    }
    #endregion

    #region HealthMethod
    public void UpdateHealth(float _health)
    {
        healthBar.GetComponent<Image>().fillAmount = _health / character.health;
    }
    #endregion

    #region PositionMethods
    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;
        x *= 0.72f;
        y *= 0.72f;
        x += -2.72f;
        y += -2.66f;
        this.transform.position = Game.Get().boardPositions.Find(x => x.xBoard == xBoard && x.yboard == yBoard).boardPoint.position;
    }
    private void SetLastPieceInfo(PieceType type)
    {
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            PVPManager.manager.tempPiece = type;
        }
    }
    public static void SetCurrPieces(List<Chessman> data)
    {
        pieces = new List<Chessman>();
        foreach (var item in data)
        {
            pieces.Add(item);
        }
    }
    #endregion
}
