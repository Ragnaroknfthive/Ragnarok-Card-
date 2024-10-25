////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PVPManager.cs
//FileType: C# Source file
//Description : This script is used to handle Player vs Player some of chess events and most of poker battle related logic.
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
/// <summary>
/// stats  : Not useful now
/// </summary>
[System.Serializable]
public enum PVPStates
{
    AttackLoc,
    Attack,
    ExtraAttack,
    DefendLoc,
    Resolve,
    CounterAttack_reraise, Brace_fold, Defend_check, engage_call
}
/// <summary>
/// Player's choice data poker battle, attack location, defend location attack type etc..
/// </summary>
public struct PlayerChoice
{
    public AttackLocation attackLoc;
    public AttackType attack;
    public List<AttackType> ExtraAttack;
    public AttackLocation defendLoc;

}
/// <summary>
/// Contains most of player vs player battle functions and logic
/// </summary>
public class PVPManager : MonoBehaviour
{

    public PhotonView photonView;                   // Network photon view for this script
    public static PVPManager manager;               // Instance of this script
    public GameObject player1, player2;             // Player an Opponent player name text ojbect
    public Image p1Image, p2Image;                  // P1- Player , P2- Oppoent  sprites
    public Image p1Outline, p2Outline, chessTurnIndicator;
    // p1 outline- player turn indicator sprite(checkmark-green) in poker 
    // p2 outline- opponent turn indicator sprite(checkmark-green) in poker 
    // chessTurnIndicator - turn indicator checkmark sign for chess game

    public Text[] p1Name, p2Name;                   // Player and Opponent name text component

    public CharacterData p1Char, p2Char;           // Player and Opponent character data- scriptable object

    public GameObject ModePanel;                   // Attack UI parent panel which contains attack silders, buttons, body object to select attack or defend logication
    public GameObject waitPanel;                   // Small panel contains text to wait for your turn 
    public Text ModeTxT;                           // Text object to display text for attack, defend location choose indication 

    public PVPStates state;                        // PVP state - attack , defend ,attack location

    public bool isLocalPVPTurn;                     // Used to decide if current turn is local player turn or not
    public bool isLocalPVPFirstTurn = false;        // Used to check if local player turn is first turn in pvp ro not 

    public PlayerChoice player1Choice, player2Choice, playerChoice;   // Player attack choices
    public List<PlayerChoice> player1ExtraChoices, player2ExtraChoices;  //Not in use
    public List<int> opponetHandCardValues;         //Opponent - hand cards values
    public List<int> opponetHandCardColors;         //Opponent - card colors (card type - spade ,club,heart etc)
    public int StaGainedPerMatch = 10;              //Stamina gain value permatch



    public GameObject ChoiceDetails;                                         //Player choice details object parent
    public Text[] player1ChoiceTxt, player2ChoiceTxt;                       //Player's choice text - high,low ,middle etc

    public Slider P1HealthBar, P2HealthBar;                                 //P1- Player healthbar, P2- Opponent healthbar
    public Slider P1StaBar, P2StaBar, P1ChessStaminaBar, P2ChessStaminaBar;   //P1- Player staminabar, P2- Oppoent staminabar same of chess game screen -Chess staminabars

    public float P1StaVal, P2StaVal;                                        //Player stamina value, Opponent stamin value

    public Slider P1RageBar, P2RageBar;                                     //Player rage value, Opponent rage value
    public Text P1SpeedTxt, P2SpeedTxt;                                     //Player speed text , Opponent speed text  -used in poker game

    public float p1Speed, p2Speed;                                          //Player speed value, Opponet speed value

    public int P1HeavyComboIndex, P2HeavyComboIndex;                        //Not in Use now            
    public int P1SpeedComboIndex, P2SpeedComboIndex;                         //Not in Use now
    public int P1RemainingHandHealth, P2RemainingHandHealth, P2LastAttackValue = 0;
    //P1-Player remaining health, P2 remaining health - poker battle , P2LastAttackValue- Opponent last attack value

    //public int p1bar { get { return P1RemainingHandHealth; } set { P1RemainingHandHealth = value; } }
    public Text winTxt;                                                 //Text object used to display win text after poker battle
    public Image winIm;                                                 //Used to set Winner sprite        

    public Vector2 p1Pos, p2Pos;                                        //P1= player position on chessboard , P2- opponent position on chessboard- used when loading chess board after poker battle
    public Chessman p1Obj, p2Obj;                                       //P1- player piece ,P2- oppoent piece in chess 
    private float lowestSpeedNeeded = 1.5f;                             // Lowest speed set for attack - it used to decide if player can have extrac attack           



    bool isattackerMaster;                                             //Used to decide if the current attacker (player who removed chess pieces) is Master player (photon network master client) or not

    public Button skipTurn;                                             //Skip trun button
    public GameObject LocationChoices, AttackChoices, LocationChoiceHeading;  // Location and attack choices

    public Text P1StaTxt, P2StaTxt, P1HealthTxt, P2HealthTxt, P1RageTxt, P2RageTxt, P1ChessStaminaTxt, P2ChessStaminaTxt;
    //Player and oppoent stamina, health , Rage text display objects  -chess referes to text objects used to display in chess mode
    public Text MyManaBarTxt, OppoManaBarTxt;
    //Player and Opponent mana bar text - used to display mana count

    public Text P1ExtraDamageAnimationText, P2ExtraDamageAnimationText;
    //Extra damage animation text for player and oppoent

    public List<GameObject> _playersNameInPvP = new List<GameObject>();
    //List of player name text objects
    public float BatPoints = 0;                                     //Bat points in poker mode
    public float p1SliderAttack, p2SliderAttack;                    //player and oppoent attack value- set when player/opponent uses attack slider
    public Button EndTurnBtn;                                       //Button to end player's turn when player finish choosing attack and other options in poker 
    public Text BetTextObj, EndTurnTimerText, ChessTurnTimerText;   //Bet text object, End turn timer text reference (timer in seconds-30 second), timer text object reference for chess turn
    int choice = -1;                                               //choice to decide attack type 
    public GameObject PlayerCards, BoardCards, OpponetPlayerCards;   //Player, Board and Opponent cards container objects 
    public int rangeCounter = 0;                                    // Used to update rage values in poker game
    public SliderAttack lastAttackType = SliderAttack.nun;          // Attack type used to decide with respect to attack slider value choosen (Light/Medium/Heavy)
    public int PlayerChoiceOnce = -1, OpponentChoiceOne = -1, OpponentRangCounter = 0;
    //Player choice set one time used incase of reraise, opponet player rage counts
    public int[] opponentCardColor = new int[2] { -1, -1 };      // Opponent card color (heart , spades , clubs etc)
    public int[] opponentCardValue = new int[2] { -1, -1 };      // Opponent card value
    public List<Sprite> opponentCardSprite = new List<Sprite>();  // Opponent player card sprites
    public Card_SO[] deckFullList;                               // Card deck full list 
    public List<RectTransform> OpponentPlayerCardPositions;      // Opponent player card position transforms
    public GameObject BoardCardParent;                           // Parent object of obard card objects in poker screen

    public AttackSlider attackSlider;                           // Attack slider to choose attack value
    public int RagePointReward = 20;                            // Rage point reward value
    public AttackLocation opponentAttackLocation, opponentDefendLocation, playerAttackLocation, playerDefenceLocation;
    //Player and Opponent location references choosed for a poker turn
    public bool isAttackLocationSelected = false, isDefenceLocationSelected = false;
    //Boolean to decide if attack location is selected or not
    public int MyLastAttackAmount = 0;                          //Player's last attack amount in poker
    public int P1StartHealth = 100, P2StartHealth = 100;        //Player and Opponent start health at the time of poker game start
    public int myLocalBatAmount = 0;                            //Used to hold local bet amount value
    public Text resultText;                                     //Poker game detailed  result text : winner hand details, damange dealt  etc..
    public Text LocationChoiceText;                             //Text reference for location choice
    public GameObject choiceConfPopup;                          //Attack / Defand location choice confirmation popup object
    public int EndTurnTimer = 5, ChessTurnTimer = 30;           //Timer values for end/change turn  poker and chess
    public int OriginalTimerVal = 30, OriginalChessTimerValue = 30;  //Start value of timer
    public GameObject moveChoiceConfirmation;                   // Chess game move confirmation popup object
    public GameObject TimerObject;                              // Timer ojbect
    public Text p1AttackFor, p2AttackFor;                       // Attack for: amount in board  , "All in" text set in case of all-in
    public int AttackFor = 0;                                   // Slider value for  attack amount
    public int OpponentBestIndex = 0;                           // best index : index of best combination of opponent cards (pk-poker game code decides commbinations from given cards and findes best index 
    public GameObject extraDamageMessageP1, extraDamageMessageP2; //Extra damage message object for player and opponent
    public bool isAttackViaSpeedPoints = false;                 //Used to check if attack is done via speed points or not
    public float p1DamageCapacityReducedby = 0, p2DamageCapacityReducedby = 0, p1StaminRecoveryReducedBy = 0, p2StaminaRecovertReducedby = 0, p1DamageIncreasedby = 0, p2DamageIncreasedby = 0, p1SpeedSlowBy = 0, opponentAttackWeakPercentageVal = 0, p2SpeedSlowedBy;
    //Player and Opponent damange ,speed capacity increase/ decrease by this amount
    public float myExtraDamagePercentage, my_OpponetSpeedMakeSlowerPercentage, my_opponentAttackMadeWeakerPerntage, my_opponentStaminaLessRecovertPerncetage;
    //Player and Opponent extradamange percentage value
    public Text myExtraDamagePercentageTxt, my_opponentAttackMadeWeakerPerntageTxt, my_opponentStaminaLessRecovertPerncetageTxt, myOpponentAttackSpeedSlowTxt;
    //Text objects for player extra damage, stamina recovery percentages
    public Text p2ExtraDamagePercentageTxt, p2_opponentAttackMadeWeakerPerntageTxt, p2_opponentStaminaLessRecovertPerncetageTxt, p2OpponentAttackSpeedSlowTxt;
    //Text objects for opponent extra damage, stamina and other values
    public GameObject p1SpeedSlowObj, p2SpeedSlowObj;          //Player and Opponent slow speed details parent objects 
    public GameObject p1Weakness, p2Weakness;                  //Player and Oppoent weekness parent details

    public int myFoldAmount = 4;                              //Player's fold amount               
    public bool StartHandTurn;                                //Used to decied if player is attacker boolean value
    public bool EndTurnInvokedByClick = false;                //Used to decide if turn is ended due to timer or turn is ended by click on button
    public List<LocationObject> locationObjects = new List<LocationObject>();   //Reference object for locations on body part (high, low, middle, left ,right)

    public MovePlate selectedMove;                           //Selected moveplate in chess game

    public PieceType myPiece, opponentpiece, tempPiece, opponentPieceAttack, tempPieceOpp;
    //Player's chess piece type, opponent's piece type, temp piece - used to hold piece reference, temp piece(opponent player) 
    //Temp piece used to check if attacked piece and attacker piece in case of attacked by pawn
    public Chessman oppPieceType, MyAttackedPiece;
    //Attacked piece types
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;
    //Chess piece sprites - Black and White set
    public GameObject speedAttackChoices, speedAttackButton;                    //speed attack choices and speed attack button reference
    public Slider speedAttackSlider;                                            //Slider for speed attack
    public int isAttackedOnLeftSide = 0, isAttackedOnRightSide = 0, isAttackedInMiddle = 0, isAttackedHigh = 0, isAttackedLow = 0;
    public int isAttackedOnLeftSideOpponent = 0, isAttackedOnRightSideOpponent = 0, isAttackedInMiddleOpponent = 0, isAttackedHighOpponent = 0, isAttackedLowOpponent = 0;
    //Used to calculate total attack counts on different positions of body parts and then change body color with respect to hit counts:This tasks is not completed fully yet
    public TextMeshProUGUI debug;           //Debug text object for testing purpose

    public Slider MyManaBar, OppoManaBar;   //Mana slider object reference of player and opponent

    public int MaxManaBarVal;               //Max mana value for Player
    public int MyManabarVal;                //Player's mana value 
    public int OppoManabarVal;              //Opponent's mana value
    public GameObject DmgPref;              //Damage prefab 
    public int startNumCards = 3;           //Starting cards count
    public GameObject SpecialAttackButton;  //Special attack button reference

    public bool IsAttacker;                 //Used  to check if player is attacker

    public bool IsPetTurn = false;          //Used to check if it's time for pet turn

    public TextMeshProUGUI player1Bet, player2Bet;      //Player and Opponent bet amount text references
    public float player1BetAmt, player2BetAmt;          //Player and Opponent bet amounts

    public Image P1Shield, P2Shield;                    //Player and Opponent shield sprite object reference in poker game

    public TextMeshProUGUI P1LastAction, P2LastAction; //Player and Oppoent's last attack location
    public GameObject P1StaminPopup;                   //Stamina warning popup to inform player that stamina is low so he can use higher values for attack

    /// <summary>
    /// Set instance in awake method
    /// </summary>
    private void Awake()
    {
        manager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        int a = 2;
        a *= 2;
        Debug.Log("a is " + a);
        Debug.Log("<color=yellow> pvp manager start  </color>");
    }
    /// <summary>
    /// Not in use but kept to avoid any issues
    /// </summary>
    public void ShowSpeedAttackSlider()
    {
        //AttackChoices.SetActive(false);
        //speedAttackChoices.SetActive(true);
    }
    /// <summary>
    /// Show Location choices and set respective text for "Attack/Defence" location selection
    /// </summary>
    public void UpdateLocationChoices()
    {
        if (!isAttackLocationSelected && IsAttacker)
        {
            LocationChoiceHeading.GetComponent<Text>().text = "Choose Attack Location";
            LocationChoiceHeading.SetActive(true);
            LocationChoices.SetActive(true);
            isLocationChoose = true;
        }
        else if (!isDefenceLocationSelected && !IsAttacker)
        {
            LocationChoiceHeading.GetComponent<Text>().text = "Choose Defence Location";
            LocationChoiceHeading.SetActive(true);
            LocationChoices.SetActive(true);
            isLocationChoose = true;
        }
        else
        {
            //// EndTurnBtn.gameObject.SetActive(true);
            //// StartTimer();
        }
    }
    /// <summary>
    /// Start's chess timer for turn 
    /// </summary>
    public void StartChessTimer()
    {
        endChessTurn = false;
        ChessTurnTimer = OriginalChessTimerValue;
        ChessTimer = OriginalChessTimerValue;

        //Debug.LogError("Chess Timer STARTED FROM HERE");
        // if(!startChesstimer)
        // {
        TimerObject.SetActive(true);
        //StartCoroutine(UpdateChessTurnTimer());
        startChesstimer = true;
        ///}
    }
    // boolean to check if chess timer is started or not
    bool timerStarted = false;
    /// <summary>
    /// Update chess timer - chess timer countdown logic
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateChessTurnTimer()
    {
        timerStarted = true;
        //Debug.LogError("TICK");
        ChessTurnTimerText.text =

        ChessTurnTimer.ToString();

        while (ChessTurnTimer > 0)
        {
            yield return new WaitForSeconds(1);
            ChessTurnTimer -= 1;
            if (ChessTurnTimer < 0)
            {
                ChessTurnTimer = 0;
            }
            ChessTurnTimerText.text = ChessTurnTimer.ToString();
            //}
            if (ChessTurnTimer <= 0)
            {
                if (!endChessTurn)
                {
                    if (Game.Get().ChessCanvas.activeSelf)
                    {
                        Game.Get().NextTurn();
                    }
                    //StopCoroutine(UpdateChessTurnTimer());
                    ChessTurnTimer = 30;
                    timerStarted = false;
                    yield break;
                }
            }
        }
    }

    public float timer;   //Timer value reference     
    bool starttimer;      //Used to check if PVP turn timer is started or not

    public float ChessTimer; //Chess timer references 
    bool startChesstimer;    //Used to checks if chess timer is started or not

    private void Update()
    {
        if (DemoManager.instance._pokerButtons.activeSelf)
        {
            //if(p1Speed>0)
            //speedAttackButton.SetActive(true);
            if (P1RageBar.value > 50)
                SpecialAttackButton.SetActive(true);
        }
        else
        {
            SpecialAttackButton.SetActive(false);
            // speedAttackChoices.SetActive(false);
            //speedAttackButton.SetActive(false);
        }

        if (starttimer)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer < 0)
            {
                timer = 0;
            }
            EndTurnTimerText.text = Mathf.RoundToInt(timer).ToString();
            if (timer <= 0f)
            {
                if (!endTurn)
                {
                    OnClickEndTurn();

                    EndTurnTimer = 30;
                    timer = 30;
                    starttimer = false;
                    endTurnTimeStarted = false;

                }
            }
        }

        if (startChesstimer)
        {
            timerStarted = true;
            ChessTimer -= Time.unscaledDeltaTime;
            if (ChessTimer < 0)
            {
                ChessTimer = 0;
            }
            ChessTurnTimerText.text = Mathf.RoundToInt(ChessTimer).ToString();

            if (ChessTimer <= 0)
            {
                if (!endChessTurn)
                {
                    if (Game.Get().ChessCanvas.activeSelf)
                    {
                        Game.Get().NextTurn();
                    }
                    //StopCoroutine(UpdateChessTurnTimer());
                    ChessTurnTimer = 30;
                    ChessTimer = 30;
                    endChessTurn = true;
                    startChesstimer = false;
                    timerStarted = false;
                }
            }

        }
    }
    /// <summary>
    /// End turn timer logic
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateEndTurnTimer()
    {
        endTurnTimeStarted = true;
        EndTurnTimer = 30;
        EndTurnTimerText.text = EndTurnTimer.ToString();

        while (EndTurnTimer > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            EndTurnTimer -= 1;
            EndTurnTimerText.text = EndTurnTimer.ToString();
            //}
            if (EndTurnTimer <= 0)
            {
                if (!endTurn)
                {
                    OnClickEndTurn();
                    EndTurnTimer = 30;
                    endTurnTimeStarted = false;
                    yield break;
                }
            }
        }
    }
    /// <summary>
    /// Start poker turn timer
    /// </summary>
    public void StartTimer()
    {
        if (endTurnTimeStarted)
            return;
        endTurn = false;
        EndTurnTimer = OriginalTimerVal;
        endTurnTimeStarted = true;
        timer = 30f;
        starttimer = true;
    }
    /// <summary>
    ///  Move pieces in chess game after move is confirmed using confirmation popup
    /// </summary>
    public void ChessMoveConfirmed()
    {
        if (selectedMove != null)
        {

            Debug.Log("SELECTED MOVE TYPE " + selectedMove.GetReference().type);
            selectedMove.SetNormalSprite();
            selectedMove.GetPieceTypeOnThisPlate();
            // SetLastPieceInfo(tempPiece,tempPieceOpp);
            selectedMove.MovePiece();

            selectedMove = null;
            moveChoiceConfirmation.gameObject.SetActive(false);
            PVPManager.manager.endChessTurn = true;
            StopCoroutine(UpdateChessTurnTimer());
            timerStarted = false;
            //  Invoke("SetChessSpriteForPVP",0.5f);
        }
    }
    /// <summary>
    /// Set last piece info in chess game - player's piece , opponent piece etc..
    /// </summary>
    /// <param name="type">player's  piece type</param>
    /// <param name="oppPiece">opponent's piece type</param>
    private void SetLastPieceInfo(PieceType type, PieceType oppPiece)
    {
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            PVPManager.manager.myPiece = type;
            PVPManager.manager.opponentpiece = oppPiece;
            photonView.RPC("SetPieceType", RpcTarget.Others, type, oppPiece);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    /// <summary>
    /// Set opponent attacked piece info
    /// </summary>
    /// <param name="type">player's  piece type</param>
    /// <param name="oppPiece">opponent's piece type</param>
    public void SetOpponentAttackPieceInfo(PieceType type, Chessman oppPiece)
    {
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            Debug.Log("OPP PIECe TYPE " + type + " This Player is Turn Player-" + (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer));

            PVPManager.manager.tempPieceOpp = type;
            oppPieceType = oppPiece;
            photonView.RPC("SetAttackedPiece", RpcTarget.Others, oppPiece.GetXboard(), oppPiece.GetYboard());
            PhotonNetwork.SendAllOutgoingCommands();
            //  photonView.RPC("SetOpponentAttackPiecePieceType",RpcTarget.Others,type);
        }
    }
    /// <summary>
    /// Set player's attacked piece info using rpc
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    [PunRPC]
    public void SetAttackedPiece(int x, int y)
    {
        MyAttackedPiece = Game.Get().GetPosition(x, y).GetComponent<Chessman>();
    }
    /// <summary>
    /// RPC- Set player and oppoent piece type on player's devices
    /// </summary>
    /// <param name="type"></param>
    /// <param name="myType"></param>
    [PunRPC]
    public void SetPieceType(PieceType type, PieceType myType)
    {
        Debug.Log("MY TYPE " + myPiece);

        Debug.Log("OPP TYPE " + type);
        myPiece = myType;
        manager.opponentpiece = type;
    }
    /// <summary>
    /// RPC- Sets opponent's attacked piece type in oppoent device
    /// </summary>
    /// <param name="type"></param>
    [PunRPC]
    public void SetOpponentAttackPiecePieceType(PieceType type)
    {
        manager.myPiece = type;
        Debug.Log("OPP PIECe TYPE " + type + " This Player is Turn Player-" + (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer));
    }
    /// <summary>
    /// Set attack position in Poker game on body part - high ,low, middle , left , right etc.
    /// </summary>
    /// <param name="location">Location value</param>
    [PunRPC]
    public void SetOpponentAttackedSide_RPC(int location)
    {
        bool isSlowedDown = false;
        Chessman pObj = opponentObj;

        switch (location)
        {
            case ((int)AttackLocation.None):
                break;
            case ((int)AttackLocation.High):
                isAttackedHighOpponent = myObj.pData.GetData("high");
                isAttackedHighOpponent++;
                myObj.pData.SaveData("high", isAttackedHighOpponent);
                if (isAttackedHighOpponent >= 3)
                {
                    myExtraDamagePercentage += (isAttackedHighOpponent * 0.05f);
                    pObj.high = isAttackedHighOpponent;
                    myExtraDamagePercentageTxt.text = (myExtraDamagePercentage * 100).ToString() + " %";
                    photonView.RPC("ApplyAttackEffs", RpcTarget.Others, (int)AttackLocation.High);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                break;
            case ((int)AttackLocation.Low):
                isAttackedLowOpponent = myObj.pData.GetData("low");
                isAttackedLowOpponent++;
                myObj.pData.SaveData("low", isAttackedLowOpponent);
                if (isAttackedLowOpponent >= 3)
                {
                    my_OpponetSpeedMakeSlowerPercentage += (isAttackedLowOpponent * 0.05f);
                    pObj.low = isAttackedLowOpponent;
                    p2OpponentAttackSpeedSlowTxt.text = (my_OpponetSpeedMakeSlowerPercentage * 100).ToString() + " %";
                    isSlowedDown = true;
                    p2SpeedSlowedBy = (isAttackedLowOpponent * 0.05f);
                    photonView.RPC("ApplyAttackEffs", RpcTarget.Others, (int)AttackLocation.Low);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                break;
            case ((int)AttackLocation.Left):
                isAttackedOnLeftSideOpponent = myObj.pData.GetData("left");
                isAttackedOnLeftSideOpponent++;
                myObj.pData.SaveData("left", isAttackedOnLeftSideOpponent);
                if (isAttackedOnLeftSideOpponent >= 3)
                {
                    my_opponentAttackMadeWeakerPerntage += (isAttackedOnLeftSideOpponent * 0.05f);
                    pObj.left = isAttackedOnLeftSideOpponent;
                    my_opponentAttackMadeWeakerPerntageTxt.text = (my_opponentAttackMadeWeakerPerntage * 100).ToString() + " %";
                    photonView.RPC("ApplyAttackEffs", RpcTarget.Others, (int)AttackLocation.Left);
                    PhotonNetwork.SendAllOutgoingCommands();
                }

                break;
            case ((int)AttackLocation.Right):
                isAttackedOnRightSideOpponent = myObj.pData.GetData("right");
                isAttackedOnRightSideOpponent++;
                myObj.pData.SaveData("right", isAttackedOnRightSideOpponent);
                if (isAttackedOnRightSideOpponent >= 3)
                {
                    my_opponentAttackMadeWeakerPerntage += (isAttackedOnRightSide * 0.05f);
                    pObj.right = isAttackedOnRightSideOpponent;
                    my_opponentAttackMadeWeakerPerntageTxt.text = (my_opponentAttackMadeWeakerPerntage * 100).ToString() + " %";
                    photonView.RPC("ApplyAttackEffs", RpcTarget.Others, (int)AttackLocation.Right);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                break;
            case ((int)AttackLocation.Middle):
                isAttackedInMiddleOpponent = myObj.pData.GetData("middle");
                isAttackedInMiddleOpponent++;
                myObj.pData.SaveData("middle", isAttackedInMiddleOpponent);
                if (isAttackedInMiddleOpponent >= 3)
                {
                    my_opponentStaminaLessRecovertPerncetage += (isAttackedInMiddleOpponent * 0.1f);
                    pObj.medle = isAttackedInMiddleOpponent;
                    my_opponentStaminaLessRecovertPerncetageTxt.text = (my_opponentStaminaLessRecovertPerncetage * 100).ToString() + " %";
                    photonView.RPC("ApplyAttackEffs", RpcTarget.Others, (int)AttackLocation.Middle);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                break;
            default:
                break;
        }
        if (isSlowedDown)
        {
            LeanTween.scale(p2SpeedSlowObj.gameObject, Vector3.one, 0.3f);
            Invoke("ResetSpeed", 1.5f);
        }
        photonView.RPC("UpdateOpponentEffects_RPC", RpcTarget.Others, myExtraDamagePercentage, my_opponentAttackMadeWeakerPerntage, my_opponentStaminaLessRecovertPerncetage, my_OpponetSpeedMakeSlowerPercentage, isSlowedDown);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// Reset speed slow object scale
    /// </summary>
    public void ResetSpeed()
    {
        LeanTween.scale(p1SpeedSlowObj.gameObject, Vector3.zero, 0.3f);
        LeanTween.scale(p2SpeedSlowObj.gameObject, Vector3.zero, 0.3f);
    }
    /// <summary>
    /// RPC- Used to set opponent attack making weak percentage in player's device
    /// </summary>
    /// <param name="val">percentag value</param>
    [PunRPC]
    public void SetOpponentAttackWeakPercentage_RPC(float val)
    {
        opponentAttackWeakPercentageVal = val;
    }
    /// <summary>
    /// RPC- Upate effects on opponent device
    /// </summary>
    [PunRPC]
    public void UpdateOpponentEffects_RPC(float p2ExtraD, float weakAttack, float staminaLow, float speed_slow, bool isSlowedDown = false)
    {
        p2ExtraDamagePercentageTxt.text = (p2ExtraD * 100).ToString() + " %";
        p2_opponentAttackMadeWeakerPerntageTxt.text = (weakAttack * 100).ToString() + " %";
        p2_opponentStaminaLessRecovertPerncetageTxt.text = (staminaLow * 100).ToString() + " %";
        p2StaminaRecovertReducedby = staminaLow;
        myOpponentAttackSpeedSlowTxt.text = (speed_slow * 100).ToString() + " %";

        if (isSlowedDown)
        {
            LeanTween.scale(p1SpeedSlowObj.gameObject, Vector3.one, 0.3f);
            Invoke("ResetSpeed", 1.5f);
        }
    }
    /// <summary>
    /// Local method used to set opponent attacked location in player side
    /// </summary>
    /// <param name="location"></param>
    public void SetOpponentAttackedSide_Local(int location)
    {
        //Debug.LogError("opponent attacked : " + location);
        switch (location)
        {
            case ((int)AttackLocation.None):
                break;
            case ((int)AttackLocation.High):
                isAttackedHigh += 1;
                myObj.high = isAttackedHigh;
                break;
            case ((int)AttackLocation.Low):
                isAttackedLow += 1;
                myObj.low = isAttackedLow;
                break;
            case ((int)AttackLocation.Left):
                isAttackedOnLeftSide += 1;
                myObj.left = isAttackedOnLeftSide;
                break;
            case ((int)AttackLocation.Right):
                isAttackedOnRightSide += 1;
                myObj.right = isAttackedOnRightSide;
                break;
            case ((int)AttackLocation.Middle):
                isAttackedInMiddle += 1;
                myObj.medle = isAttackedInMiddle;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Reset attack locations
    /// </summary>
    public void ResetAttackLocation()
    {
        isAttackedHigh = 0;
        isAttackedLow = 0;
        isAttackedOnLeftSide = 0;
        isAttackedOnRightSide = 0;
        isAttackedInMiddle = 0;


        isAttackedHighOpponent = 0;
        isAttackedLowOpponent = 0;
        isAttackedOnLeftSideOpponent = 0;
        isAttackedOnRightSideOpponent = 0;
        isAttackedInMiddleOpponent = 0;

        myExtraDamagePercentageTxt.text = "0"; my_opponentAttackMadeWeakerPerntageTxt.text = "0"; my_opponentStaminaLessRecovertPerncetageTxt.text = "0"; myOpponentAttackSpeedSlowTxt.text = "0";
        p2ExtraDamagePercentageTxt.text = "0"; p2_opponentAttackMadeWeakerPerntageTxt.text = "0"; p2_opponentStaminaLessRecovertPerncetageTxt.text = "0"; p2OpponentAttackSpeedSlowTxt.text = "0";

        p1DamageIncreasedby = 0;
        p1DamageCapacityReducedby = 0;
        p1StaminRecoveryReducedBy = 0;
        p1SpeedSlowBy = 0;
        p2SpeedSlowedBy = 0f;

        p2StaminaRecovertReducedby = 0;

        p2DamageCapacityReducedby = 0;
        p2DamageIncreasedby = 0;
        opponentAttackWeakPercentageVal = 0;

        myExtraDamagePercentage = 0; my_OpponetSpeedMakeSlowerPercentage = 0; my_opponentAttackMadeWeakerPerntage = 0;
        my_opponentStaminaLessRecovertPerncetage = 0;

    }
    /// <summary>
    /// Discard selected move - when player reject's chess move from confirmation popup
    /// </summary>
    public void ChessMoveRejected()
    {
        selectedMove.SetNormalSprite();
        selectedMove = null;
        moveChoiceConfirmation.gameObject.SetActive(false);
    }
    //bool to check if end turn timer is started or not
    bool endTurnTimeStarted = false;

    /// <summary>
    ///  Update stamina, health, rage,speed text objects
    /// </summary>
    public void UpdateHMTxt()
    {
        //Clamp values to minimun and maximum
        if (P1StaVal > P1StaBar.maxValue)
        {
            P1StaVal = P1StaBar.maxValue;
        }
        else if (P1StaVal < 0)
        {
            P1StaVal = 0;
        }

        if (P2StaVal > P2StaBar.maxValue)
        { P2StaVal = P2StaBar.maxValue; }
        else if (P2StaVal < 0)
        {
            P2StaVal = 0;
        }
        //
        P1HealthTxt.text = P1HealthBar.value + " / " + P1HealthBar.maxValue;
        P2HealthTxt.text = P2HealthBar.value + " / " + P2HealthBar.maxValue;
        P1StaTxt.text = "Stamina " + MathF.Round(P1StaVal, 2) + " / " + P1StaBar.maxValue;
        P2StaTxt.text = "Stamina " + MathF.Round(P2StaVal, 2) + " / " + P2StaBar.maxValue;
        P1RageTxt.text = "Rage " + P1RageBar.value.ToString();//+//" / "+P1RageBar.maxValue;
        P2RageTxt.text = "Rage " + P2RageBar.value.ToString();//+" / "+P2RageBar.maxValue;
        P1SpeedTxt.text = p1Speed.ToString("F2");

        P2SpeedTxt.text = p2Speed.ToString("F2");
        P1StaBar.value = P1StaVal;
        P2StaBar.value = P2StaVal;
        P1ChessStaminaTxt.text = P1StaTxt.text;
        P2ChessStaminaTxt.text = P2StaTxt.text;
        P1ChessStaminaBar.value = P1StaBar.value;
        P2ChessStaminaBar.value = P2StaBar.value;

        //Debug.LogError("P1 STAMIN " + P1StaBar.value + " Time "+ DateTime.Now);
        //Debug.LogError("P2 STAMIN " + P2StaBar.value);
    }
    /// <summary>
    /// Update bet for local player
    /// </summary>
    /// <param name="s">bet amount</param>
    public void UpdateBetForPlayer(float s)
    {
        player1BetAmt = s;
        player1Bet.text = player1BetAmt.ToString();
        photonView.RPC("UpdateBetForPlayerRPC", RpcTarget.Others, s);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC-  Update bet amount for player on network
    /// </summary>
    /// <param name="s">bet amount</param>
    [PunRPC]
    public void UpdateBetForPlayerRPC(float s)
    {
        player2BetAmt = s;
        player2Bet.text = player2BetAmt.ToString();
    }
    /// <summary>
    /// Update player's action text in network
    /// </summary>
    /// <param name="s"></param>
    public void updatePlayerAction(string s)
    {
        P1LastAction.text = s;
        photonView.RPC("updatePLayerActionRPC", RpcTarget.Others, s);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC for action update
    /// </summary>
    /// <param name="s"></param>
    [PunRPC]
    public void updatePLayerActionRPC(string s)
    {
        P2LastAction.text = s;
    }
    /// <summary>
    /// Update manabar and text in playe and opponent UI
    /// </summary>
    public void UpdateManaTxt()
    {

        MyManaBar.value = MyManabarVal;
        OppoManaBar.value = OppoManabarVal;
        MyManaBarTxt.text = MyManabarVal + " / " + MaxManaBarVal;
        OppoManaBarTxt.text = OppoManabarVal + " / " + MaxManaBarVal;
    }
    /// <summary>
    ///  Set chess data and starts poker game
    /// </summary>
    /// <param name="posP1">player position (x,y)</param>
    /// <param name="posP2">opposite player position (x,y)</param>
    /// <param name="localplayerTurn">Boolean - True if local player's turn</param>
    /// <param name="isReverse">True- for black type piece</param>
    public void SetData(Vector2 posP1, Vector2 posP2, bool localplayerTurn, bool isReverse)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("CARD GENERATION CALL 2");
            DemoManager.instance.Generate3CardsStack();
        }
        isLocalPVPTurn = localplayerTurn;

        player1Choice.attack = AttackType.None;
        player1Choice.attackLoc = AttackLocation.None;
        player1Choice.defendLoc = AttackLocation.None;
        player1Choice.ExtraAttack = new List<AttackType>();
        player2Choice.attack = AttackType.None;
        player2Choice.attackLoc = AttackLocation.None;
        player2Choice.defendLoc = AttackLocation.None;
        player2Choice.ExtraAttack = new List<AttackType>();

        P1HeavyComboIndex = 0;
        P2HeavyComboIndex = 0;
        P1SpeedComboIndex = 0;
        P2SpeedComboIndex = 0;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //_playersNameInPvP[i].text = PhotonNetwork.PlayerList[i].NickName;

            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            {
                _playersNameInPvP[0].GetComponent<Text>().text = "Hero";//PhotonNetwork.PlayerList[i].NickName;
                //Debug.Log("if name change "+_playersNameInPvP[i].GetComponent<Text>().text);
            }
            else  //(PhotonNetwork.PlayerList[i] != PhotonNetwork.LocalPlayer)
            {
                _playersNameInPvP[1].GetComponent<Text>().text = "Opponent";
                //Debug.Log("else name change "+_playersNameInPvP[i].GetComponent<Text>().text);
            }
        }

        if (isLocalPVPTurn)
        {
            //SetModePanel();
            Game.Get().IsDefender = false;
            isLocalPVPFirstTurn = true;
        }
        else
        {
            Game.Get().IsDefender = true;
            isLocalPVPFirstTurn = false;
        }
        //IsPetTurn = isLocalPVPFirstTurn;
        isattackerMaster = isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            isattackerMaster = !isLocalPVPFirstTurn && !PhotonNetwork.LocalPlayer.IsMasterClient;

        //Info:Set laoding screen to avoid lag view and Reset Spell Cards
        Game.Get().loadingScreen.SetActive(true);
        SpellManager.instance.RemoveOldSpellData();
        SpellManager.instance.spawned_ids = new List<int>();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.RPC("SetDataRPC", RpcTarget.AllBuffered, posP1, posP2, isReverse);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    /// <summary>
    /// Update information text value in poker game - for example "All In" or "Counter attack for 20" etc..
    /// </summary>
    /// <param name="sliderVal">Attack slider value</param>
    /// <param name="isCounter">True- for counter attack</param>
    /// <param name="isAllin">True- If all in</param>
    [PunRPC]
    public void UpdateAttackForText(int sliderVal, bool isCounter = false, bool isAllin = false)
    {
        if (isAllin) { p2AttackFor.text = "All In"; }
        else
        {
            p2AttackFor.text = isCounter ? "Counter attack for " + sliderVal : "Attack For " + sliderVal;
        }

        p2AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);

        Invoke("ResetText", 3f);
    }
    /// <summary>
    /// Update total attack amount text in poker game
    /// </summary>
    /// <param name="sliderVal">attack slider value</param>
    [PunRPC]
    public void UpdatePotAmountForText(int sliderVal)
    {
        AttackFor += sliderVal;
        BetTextObj.text = "TOTAL ATTACK : " + AttackFor;
    }
    /// <summary>
    /// Update total attack amount text for allin
    /// </summary>
    [PunRPC]
    public void UpdatePotAmountForAllInText(int sliderVal)
    {
        AttackFor = sliderVal;
        BetTextObj.text = "TOTAL ATTACK : " + AttackFor;
    }
    /// <summary>
    /// Reset attack information text
    /// </summary>
    public void ResetText()
    {
        p2AttackFor.text = "";
        p1AttackFor.text = "";

        p1AttackFor.gameObject.transform.parent.localScale = Vector3.zero;
        p2AttackFor.gameObject.transform.parent.localScale = Vector3.zero;
    }
    /// <summary>
    /// RPC- Last attack amount
    /// </summary>
    public int LastAtkAmt;
    [PunRPC]
    public void UpdateLastAtkAmt(int f)
    {
        LastAtkAmt = f;
    }
    /// <summary>
    ///  Initiates attack when user click on "Attack" button after setting attack slider value 
    /// </summary>
    /// <param name="sliderAttack">slider attack value</param>
    /// <param name="StaminaConsumed">stamina consumed for this attack</param>
    /// <param name="action">Player's action</param>
    public void sliderAttackbuttonClick(int sliderAttack, float StaminaConsumed, PlayerAction action)
    {
        MyLastAttackAmount = sliderAttack;
        Game.Get().UpdateLastAction(action);
        photonView.RPC("UpdateLastAtkAmt", RpcTarget.All, sliderAttack);
        PhotonNetwork.SendAllOutgoingCommands();
        //LocationChoiceHeading.SetActive(false);
        if (AttackSlider.instance._sliderAttack == SliderAttack.HeavyAttack)
        {
            AttackChoices.SetActive(false);

            state = PVPStates.Attack;

            if (PhotonNetwork.IsMasterClient)
            {
                p1SliderAttack = sliderAttack;
            }
            else
            {
                p2SliderAttack = sliderAttack;
            }
        }
        else if (AttackSlider.instance._sliderAttack == SliderAttack.MediumAttack)
        {
            AttackChoices.SetActive(false);
            state = PVPStates.Attack;

            if (PhotonNetwork.IsMasterClient)
            {
                p1SliderAttack = sliderAttack;
            }
            else
            {
                p2SliderAttack = sliderAttack;
            }

        }
        else if (AttackSlider.instance._sliderAttack == SliderAttack.LightAttack)
        {
            AttackChoices.SetActive(false);
            state = PVPStates.Attack;

            if (PhotonNetwork.IsMasterClient)
            {
                p1SliderAttack = sliderAttack;
            }
            else
            {
                p2SliderAttack = sliderAttack;
            }
        }

        if (!isAttackLocationSelected && IsAttacker)
        {
            LocationChoiceHeading.GetComponent<Text>().text = "Choose Attack Location";
            LocationChoiceHeading.SetActive(true);
            LocationChoices.SetActive(true);
            isLocationChoose = true;

        }
        else if (!isDefenceLocationSelected && !IsAttacker)
        {
            LocationChoiceHeading.GetComponent<Text>().text = "Choose Defence Location";
            LocationChoiceHeading.SetActive(true);
            LocationChoices.SetActive(true);
            isLocationChoose = true;
        }
        else
        {
            OnClickEndTurn();
        }
        DeductStamina(StaminaConsumed);
    }
    /// <summary>
    /// Cast spell
    /// </summary>
    public void LaunchSpecialAttack()
    {
        StartCoroutine(SpellManager.instance.CastSpell(p1Char.SpecialAttack));
        SpecialAttackButton.SetActive(false);
        DeductRage(p1Char.SpecialAttackCost);
    }
    /// <summary>
    /// Poker- Deduct rage value and call RPC
    /// </summary>
    public void DeductRage(int v)
    {
        P1RageBar.value -= v;
        UpdateHMTxt();
        photonView.RPC("DeductRageRPC", RpcTarget.Others, v);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- used to update rage value in network devices
    /// </summary>
    /// <param name="i"></param>
    [PunRPC]
    public void DeductRageRPC(int i)
    {
        P2RageBar.value -= i;
        UpdateHMTxt();
    }
    /// <summary>
    /// Deduct stamina and call RPC for updating it in netwrok
    /// </summary>
    /// <param name="i"></param>
    public void DeductStamina(float i)
    {
        P1StaVal -= i;
        UpdateHMTxt();
        photonView.RPC("DeductStaminaRPC", RpcTarget.Others, i);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- used to update stamina
    /// </summary>
    [PunRPC]
    public void DeductStaminaRPC(float i)
    {
        P2StaVal -= i;
        UpdateHMTxt();
    }
    /// <summary>
    /// Deduct speed and update  RPC
    /// </summary>
    public void DeductSpeed(float i)
    {
        p1Speed -= i;
        p1Speed = Mathf.Clamp(p1Speed, 0, p1Speed);

        UpdateHMTxt();
        photonView.RPC("DeductSpeedRPC", RpcTarget.Others, i);
    }
    /// <summary>
    /// RPC- used to update speed
    /// </summary>
    [PunRPC]
    public void DeductSpeedRPC(float i)
    {
        p2Speed -= i;
        UpdateHMTxt();
    }
    /// <summary>
    /// RPC- to Set other player turn - Player's turn related details set in network 
    /// </summary>
    /// <param name="_player"></param>
    [PunRPC]
    public void RPC_otherplayerTurnPoker(Player _player)
    {
        //Debug.LogError("***LAST ACTION" + Game.Get().lastAction);
        LocationChoiceHeading.SetActive(false);
        LocationChoices.SetActive(false);
        AttackChoices.SetActive(false);
        choiceConfPopup.SetActive(false);
        if (PhotonNetwork.LocalPlayer.NickName == _player.NickName)//(Game.Get().IsDefender==false)// != Game.Get().GetCurrentPlayer())
        {
            if (isCheck)
            {
                float checkCost = 1f - p1StaminRecoveryReducedBy;
                Debug.LogError("check cost : " + checkCost + "stamina recovery reducedBy :" + p1StaminRecoveryReducedBy);
                if (checkCost < 0) checkCost = 0;

                P1StaVal += checkCost;
                //P1StaBar.value += checkCost;
                isCheck = false;
                UpdateHMTxt();
                Debug.LogError(PhotonNetwork.LocalPlayer.NickName + " " + _player.NickName);
                photonView.RPC("UpdateOpponentStamina", RpcTarget.Others, checkCost);
                PhotonNetwork.SendAllOutgoingCommands();
            }

            DemoManager.instance._pokerButtons.SetActive(false);
            PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
            PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(false);
            PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
            PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(false);
            PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);

            AttackChoices.SetActive(false);
            LocationChoices.SetActive(false);
            // ModePanel.SetActive(false);
            PlayerCards.SetActive(true);
            waitPanel.SetActive(true); //Debug.LogError("**ATTACKER_TURN NO_"+Game.Get().turn);
            IsPetTurn = false;
        }
        else
        {
            if (Game.Get().turn < 8)
            {

                StartTimer();
                EndTurnBtn.gameObject.SetActive(true);
            }
            AttackChoices.SetActive(false);
            LocationChoices.SetActive(false);

            ModePanel.SetActive(true);
            // PlayerCards.SetActive(false);
            waitPanel.SetActive(false);
            if (Game.Get().turn < 2)
            {
                PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
                PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(false);

            }
            PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(true);
            PokerButtonManager.instance.call_Engauge.gameObject.SetActive(true);
            //  PokerButtonManager.instance.call_Engauge.interactable = Game.Get().BetAmount <= P1HealthBar.value ? true:false;
            PokerButtonManager.instance.call_Engauge.interactable = P2LastAttackValue <= P1HealthBar.value ? true : false;

            PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(true);
            // }
            if (Game.Get().turn >= 1)
            {
                // PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(true);

                if (Game.Get().lastAction == PlayerAction.attack || Game.Get().lastAction == PlayerAction.counterAttack)
                {
                    //Debug.LogError("***LAST ATTACK TYPE 1" + Game.Get().lastAction);
                    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(true);
                    PokerButtonManager.instance.call_Engauge.gameObject.SetActive(P2LastAttackValue <= P1HealthBar.value);
                    PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(true);
                    PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(false);
                }
                if (Game.Get().lastAction == PlayerAction.defend)
                {
                    //Debug.LogError("***LAST ATTACK TYPE 2" + Game.Get().lastAction);
                    PokerButtonManager.instance.bet_attack.gameObject.SetActive(true);
                    PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(true);
                    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
                    PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(false);

                }

                if (Game.Get().lastAction == PlayerAction.engage)//(Game.Get().turn == 2 || Game.Get().turn == 4 || Game.Get().turn == 6 || Game.Get().turn == 8) &&
                {
                    //Debug.LogError("***LAST ATTACK TYPE 3" + Game.Get().lastAction);
                    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
                    PokerButtonManager.instance.bet_attack.gameObject.SetActive(true);
                    PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(true);
                    PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(false);
                }
            }
            //All In
            if (P2LastAttackValue >= P1RemainingHandHealth) // if(Game.Get().BetAmount * 2 > P1RemainingHandHealth)
            {
                PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                if (!PokerButtonManager.instance.bet_attack.gameObject.activeSelf)
                {
                    if (P2LastAttackValue >= P1RemainingHandHealth)
                    {
                        //Debug.LogError("ALL IN TRUE FROM HERE " + P2LastAttackValue + " Remaining Health " + P1RemainingHandHealth);
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
                        Debug.LogError("AllIn From If PArt");
                    }
                    else
                    {
                        //Debug.LogError("ALL IN SET FALSE FROM HERE ");
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (P2RemainingHandHealth <= 0)
                    {

                        PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
                        PokerButtonManager.instance.call_Engauge.gameObject.SetActive(true);
                        PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    }
                    else if (P2LastAttackValue >= P1RemainingHandHealth)
                    {
                        PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
                        PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
                        PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    }
                    else if (P2RemainingHandHealth > 0 && P2LastAttackValue >= P1RemainingHandHealth)
                    {
                        PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
                        PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
                        PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (P2RemainingHandHealth <= 0)
                {
                    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);

                    //PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
                    Debug.LogError("AllIn From Else PArt");
                }
                else
                {
                    PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
                }
            }
            if (PokerButtonManager.instance.bet_attack.gameObject.activeSelf)
            {
                PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
            }
            if (PokerButtonManager.instance.allIn_btn.gameObject.activeSelf)
            {
                PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
            }
            DemoManager.instance._pokerButtons.SetActive(true);
            if (P1RageBar.value > 50)
                SpecialAttackButton.SetActive(true);
            //Debug.LogError("**DEFENDER NO_" + Game.Get().turn);

            UpdateHMTxt();
            if (PokerButtonManager.instance.allIn_btn.gameObject.activeSelf)
            {
                PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
                PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
            }
            IsPetTurn = true;
            SpellManager.PetAlreadyAttacked = false;
            if (!SpellManager.PetAlreadyAttacked)
            {
                SpellManager.instance.PetAttack();
                SpellManager.PetAlreadyAttacked = true;
            }

        }

        if (_player != PhotonNetwork.LocalPlayer)
        {
            //isLocalPVPTurn = true;
            SpellManager.PetAlreadyAttacked = false;
        }
        else
        {
            // Debug.Log("you are not turn player please wait");
        }
    }
    /// <summary>
    /// RPC- update stamina in opponent
    /// </summary>
    /// <param name="val"></param>
    [PunRPC]
    public void UpdateOpponentStamina(float val)
    {
        // float checkCost = 1f;
        P2StaBar.value += val;
        P2StaVal += val;
        UpdateHMTxt();
        // isCheck = false;
    }

    /// <summary>
    /// RPC- Set player's  attack choice on network
    /// </summary>
    /// <param name="c"></param>
    [PunRPC]
    public void RPC_SetOpponentAttackChoice(int c)
    {
        OpponentChoiceOne = c;
    }

    int LastGameTurn = -1; // Not used


    List<int> ListOfCounterAttacks = new List<int>(); //Not used 

    /// <summary>
    /// RPC- Poker game  logic Turn, River etc..
    /// </summary>
    [PunRPC]
    public void RPC_UpdateTurn()
    {
        Debug.Log("LAST ATTACK TYPE " + Game.Get().lastAction + PVPManager.manager.isattackerMaster);
        if (PhotonNetwork.LocalPlayer.NickName == Game.Get()._currnetTurnPlayer.NickName && Game.Get().turn == 7 && P1RemainingHandHealth > 0 && MyLastBatAmount >= P2RemainingHandHealth && !isAutoTurn)
        {
            Debug.Log("LOCAL PLAYER ALL IN RAISED ");
        }
        else if (PhotonNetwork.LocalPlayer.NickName != Game.Get()._currnetTurnPlayer.NickName && P2LastAttackValue >= P1RemainingHandHealth && Game.Get().turn == 7 && P2RemainingHandHealth > 0 && !isAutoTurn)
        {

            Debug.Log("Other PLAYER ALL IN OPPONENT " + P2LastAttackValue);
        }
        else
        {
            Game.Get().turn += 1;
        }

        if (Game.Get().lastAction == PlayerAction.counterAttack || (Game.Get().lastAction == PlayerAction.attack && Game.Get().turn % 2 == 0 && Game.Get().turn > 1))
        {
            Game.Get().turn -= 1;
            return;
        }
        foreach (var item in FindObjectsOfType<BattleCardDisplay>())
        {
            item.ResetAttack();
        }

        //1
        if (Game.Get().turn == 2)
        {

            for (int i = 0; i < DemoManager.instance.board_cards.Count; i++)
            {
                DemoManager.instance.board_cards[i].gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(40 * i, 0, 0);
                DemoManager.instance.board_cards[i].gameObject.SetActive(true);
            }

            //AddMana();
            AddStamina();
            //SpellManager.instance.DrawCard();

        }
        if (Game.Get().turn == 4)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("CARD GENERATION CALL  4rth Card");
                DemoManager.instance.Generate3CardsStack();
            }
            //AddMana();
            AddStamina();
            //SpellManager.instance.DrawCard();

        }
        if (Game.Get().turn == 6)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("CARD GENERATION CALL 5th Card");
                DemoManager.instance.Generate3CardsStack();
            }
            // AddMana();
            AddStamina();
        }
        if (Game.Get().turn == 8)
        {
            //AddMana();
            AddStamina();

            for (int i = 0; i < DemoManager.instance.player_cards.Length; i++)
            {
                if (DemoManager.instance.player_cards[i] != null && DemoManager.instance.player_cards[i].Count > 0)
                {
                    PokerHand myHand = DemoManager.instance.CompareHandWithStrength(i);
                    //Debug.LogError("***PLAYER NO " + i + " RESULT" + myHand.strength);
                    Game.Get().PokerHandResults[0] = myHand.printResult();
                    Game.Get().PlayerStrengths[0] = myHand.strength;
                    //Game.Get().MyHighCardValue= (Int32) myHand.highCard.Find(x => x.cardValue == myHand.highCard.Max(x=>x.cardValue)).cardValue;
                    //  Debug.LogError("MY HIGH CARD VAL " + Game.Get().MyHighCardValue);
                    photonView.RPC("RPC_SetOthersPlayerStrength", RpcTarget.Others, myHand.strength, myHand.printResult(), Game.Get().MyHighCardValue, Game.Get().MySecondHighCardValue, Game.Get().MyHighCardList.ToArray());
                    PhotonNetwork.SendAllOutgoingCommands();
                    if (!isAttackLocationSelected && IsAttacker)
                    {
                        tempChoiceNo = 0;
                        ConfirmChoice();
                    }
                    if (!isDefenceLocationSelected && !IsAttacker)
                    {
                        tempChoiceNo = 1;
                        ConfirmChoice();
                    }
                    Invoke("DisplayResult", 1f);
                }
            }
        }
    }
    /// <summary>
    /// Add +1 mana
    /// </summary>
    public void AddMana()
    {
        MaxManaBarVal += 1;
        MaxManaBarVal = Mathf.Clamp(MaxManaBarVal, 0, 9);
        MyManaBar.maxValue = MaxManaBarVal;
        OppoManaBar.maxValue = MaxManaBarVal;
        MyManabarVal = MaxManaBarVal;
        OppoManabarVal = MaxManaBarVal;
        UpdateManaTxt();
    }
    /// <summary>
    /// Add stamina
    /// </summary>
    public void AddStamina()
    {
        float val = 1f - (p1StaminRecoveryReducedBy);
        P1StaVal += val;
        //Debug.LogError("Stamina recovery : " + val + " - " + p1StaminRecoveryReducedBy);
        P1StaVal = Mathf.Clamp(P1StaVal, 0, P1StaBar.maxValue);
        UpdateHMTxt();
        photonView.RPC("AddStaminaRPC", RpcTarget.Others, val);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- Add stamina on network
    /// </summary>
    /// <param name="i"></param>
    [PunRPC]
    public void AddStaminaRPC(float i)
    {
        P2StaVal += i;
        P2StaVal = Mathf.Clamp(P2StaVal, 0, P2StaBar.maxValue);
        UpdateHMTxt();
    }


    /// <summary>
    /// Deduct mana
    /// </summary>
    /// <param name="i"></param>
    public void DeductMana(int i)
    {
        MyManabarVal -= i;
        UpdateManaTxt();
        photonView.RPC("UpdateManaRPC", RpcTarget.Others, i);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC - Duduct mana on network
    /// </summary>
    /// <param name="i"></param>
    [PunRPC]
    public void UpdateManaRPC(int i)
    {
        OppoManabarVal -= i;
        UpdateManaTxt();
    }

    /// <summary>
    /// RPC- Reset turn count (used for poker game)
    /// </summary>
    [PunRPC]
    public void RPC_ResetTurn()
    {
        Game.Get().turn = 0;
        ListOfCounterAttacks = new List<int>();
        //
    }
    /// <summary>
    /// Displays details of Poker game result with Player and Opponent hand details
    /// </summary>
    public void DisplayResult()
    {
        for (int s = 0; s < Game.Get().PlayerStrengths.Count; s++)
        {
            if (s == 0)
            {
                Debug.LogError("***MY STRENGTH " + Game.Get().PlayerStrengths[0]);

            }
            if (s == 1)
            {
                Debug.LogError("***OPPONENT STRENGTH " + Game.Get().PlayerStrengths[1]);
            }

            resultText.text = "Your Hand :" + Game.Get().PokerHandResults[0]
                            + ", Opponent Hand :" + Game.Get().PokerHandResults[1];

        }

        if (!StartHandTurn)
        {
            resultText.text += "\nYou Defended : " + playerDefenceLocation.ToString() + ", Opponent Attacked : " + opponentAttackLocation.ToString();
        }
        else
        {
            resultText.text += "\nYou Attacked : " + playerAttackLocation.ToString() + ", Opponent defended : " + opponentDefendLocation.ToString();
        }


        if (playerDefenceLocation == opponentAttackLocation || playerAttackLocation == opponentDefendLocation)
        {
            if (StartHandTurn)
            {
                DeductSpeed(p1Speed);
                DeductStamina((P1StaVal * 0.75f));
            }
            resultText.text += "\n Damage Dealt = 0";
            StartCoroutine(ShowShieldEff());
        }
        else
        {
            if (!StartHandTurn)
            {
                LocationObject.GetLocation(OpponentChoiceOne).damaged_amt++;
            }
        }
        StartCoroutine("ResultRPCLocal");
    }
    /// <summary>
    /// Show shield effect in poker game on player's UI
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowShieldEff()
    {

        if (StartHandTurn)
        {
            GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
            o.GetComponent<Projectile>().DealDamage = false;
            o.GetComponent<Projectile>().target = P2Shield.gameObject;
            LeanTween.color(P2Shield.GetComponent<RectTransform>(), Color.white, 0.3f);
            yield return new WaitForSeconds(1.3f);
            LeanTween.color(P2Shield.GetComponent<RectTransform>(), new Color(0f, 0f, 0f, 0f), 0.3f);
        }
        else
        {
            GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
            o.GetComponent<Projectile>().DealDamage = false;
            o.GetComponent<Projectile>().target = P1Shield.gameObject;
            LeanTween.color(P1Shield.GetComponent<RectTransform>(), Color.white, 0.3f);
            yield return new WaitForSeconds(1.3f);
            LeanTween.color(P1Shield.GetComponent<RectTransform>(), new Color(0f, 0f, 0f, 0f), 0.3f);
        }
    }
    /// <summary>
    /// Checks  different condition to decide "Win" / "Lose" in poker game
    /// </summary>
    /// <returns></returns>
    public IEnumerator ResultRPCLocal()
    {
        yield return new WaitWhile(() => SpellManager.IsPetAttacking);
        ResetText();
        //speedAttackButton.SetActive(false);
        SpecialAttackButton.SetActive(false);
        //bool isLocationMatch = false;
        bool isPlayer1Win = false;
        bool isTie = false;
        if (Game.Get().PlayerStrengths[0] > Game.Get().PlayerStrengths[1])
        {
            isPlayer1Win = WinMethod();
            //..MathF.Round(p1Speed,2);
        }
        else if (Game.Get().PlayerStrengths[0] < Game.Get().PlayerStrengths[1])
        {
            LoseMethod();
            // Game.Get().NextTurn();
        }
        else if (Game.Get().PlayerStrengths[0] == Game.Get().PlayerStrengths[1])
        {
            CardValue mCard = (CardValue)Enum.Parse(typeof(CardValue), Game.Get().MyHighCardValue.ToString());
            CardValue oCard = (CardValue)Enum.Parse(typeof(CardValue), Game.Get().OpponentHighCardValue.ToString());
            Debug.LogError("MY HIGH CARD " + mCard);
            Debug.LogError("OPPONANT HIGH CARD " + oCard);

            if (Game.Get().MyHighCardValue > Game.Get().OpponentHighCardValue)
            {
                isPlayer1Win = WinMethod();
            }
            else if (Game.Get().MyHighCardValue < Game.Get().OpponentHighCardValue)
            {
                LoseMethod();
            }
            else if (Game.Get().MyHighCardValue == Game.Get().OpponentHighCardValue)
            {
                if (Game.Get().PlayerStrengths[0] == 1)
                {
                    if (Game.Get().MySecondHighCardValue > Game.Get().OpponentSecondHighCardValue)
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MySecondHighCardValue < Game.Get().OpponentSecondHighCardValue)
                    {
                        LoseMethod();
                    }
                    else
                    {
                        if (Game.Get().MyHighCardList[1] > Game.Get().OpponentHighCardList[1])
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else if (Game.Get().MyHighCardList[1] < Game.Get().OpponentHighCardList[1])
                        {
                            LoseMethod();
                        }
                        else
                        {
                            int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                            int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                            if (AnyHighCardGreaterThenMyAllCards == 1)
                            {
                                LoseMethod();
                            }
                            else if (AnyHighCardGreaterThenOpponentsAllCards == 1)
                            {
                                isPlayer1Win = WinMethod();
                            }
                            else
                            {
                                isPlayer1Win = TieMethod();
                                isTie = true;
                            }
                        }
                    }
                }
                else if (Game.Get().PlayerStrengths[0] == 2)
                {
                    if (Game.Get().MySecondHighCardValue > Game.Get().OpponentSecondHighCardValue)
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MySecondHighCardValue < Game.Get().OpponentSecondHighCardValue)
                    {
                        LoseMethod();
                    }
                    else
                    {
                        if (Game.Get().MyHighCardList[1] > Game.Get().OpponentHighCardList[1])
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else if (Game.Get().MyHighCardList[1] < Game.Get().OpponentHighCardList[1])
                        {
                            LoseMethod();
                        }
                        else
                        {
                            int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                            int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                            if (AnyHighCardGreaterThenMyAllCards >= 1)
                            {
                                LoseMethod();
                            }
                            else if (AnyHighCardGreaterThenOpponentsAllCards >= 1)
                            {
                                isPlayer1Win = WinMethod();
                            }
                            else
                            {
                                isPlayer1Win = TieMethod();
                                isTie = true;
                            }

                        }
                    }
                }
                else if (Game.Get().PlayerStrengths[0] == 3)
                {
                    if (Game.Get().MySecondHighCardValue > Game.Get().OpponentSecondHighCardValue)
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MySecondHighCardValue < Game.Get().OpponentSecondHighCardValue)
                    {
                        LoseMethod();
                    }
                    else
                    {
                        if (Game.Get().MyHighCardList[1] > Game.Get().OpponentHighCardList[1])
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else if (Game.Get().MyHighCardList[1] < Game.Get().OpponentHighCardList[1])
                        {
                            LoseMethod();
                        }
                        else
                        {
                            int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                            int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                            if (AnyHighCardGreaterThenMyAllCards >= 1)
                            {
                                LoseMethod();
                            }
                            else if (AnyHighCardGreaterThenOpponentsAllCards >= 1)
                            {
                                isPlayer1Win = WinMethod();
                            }
                            else
                            {
                                isPlayer1Win = TieMethod();
                                isTie = true;
                            }

                        }

                    }
                }
                else if (Game.Get().PlayerStrengths[0] == 7)
                {
                    if (Game.Get().MySecondHighCardValue > Game.Get().OpponentSecondHighCardValue)
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MySecondHighCardValue < Game.Get().OpponentSecondHighCardValue)
                    {
                        LoseMethod();
                    }
                    else
                    {
                        if (Game.Get().MyHighCardList[0] > Game.Get().OpponentHighCardList[0])
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else if (Game.Get().MyHighCardList[1] < Game.Get().OpponentHighCardList[1])
                        {
                            LoseMethod();
                        }
                        else
                        {
                            int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                            int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                            if (AnyHighCardGreaterThenMyAllCards >= 1)
                            {
                                LoseMethod();
                            }
                            else if (AnyHighCardGreaterThenOpponentsAllCards >= 1)
                            {
                                isPlayer1Win = WinMethod();
                            }
                            else
                            {
                                isPlayer1Win = TieMethod();
                                isTie = true;
                            }

                        }

                    }
                }
                else if (Game.Get().PlayerStrengths[0] == 6)
                {
                    if (Game.Get().MySecondHighCardValue > Game.Get().OpponentSecondHighCardValue)
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MySecondHighCardValue < Game.Get().OpponentSecondHighCardValue)
                    {
                        LoseMethod();
                    }
                    else
                    {
                        int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                        int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                        if (AnyHighCardGreaterThenMyAllCards >= 1)
                        {
                            LoseMethod();
                        }
                        else if (AnyHighCardGreaterThenOpponentsAllCards >= 1)
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else
                        {
                            isPlayer1Win = TieMethod();
                            isTie = true;
                        }


                    }
                }
                else if (Game.Get().PlayerStrengths[0] == 0)
                {
                    //High Card Comparison
                    if (Game.Get().MyHighCardList[0] > Game.Get().OpponentHighCardList[0])
                    {
                        isPlayer1Win = WinMethod();
                    }
                    else if (Game.Get().MyHighCardList[0] < Game.Get().OpponentHighCardList[0])
                    {
                        LoseMethod();
                    }
                    else
                    {
                        if (Game.Get().MyHighCardList[1] > Game.Get().OpponentHighCardList[1])
                        {
                            isPlayer1Win = WinMethod();
                        }
                        else if (Game.Get().MyHighCardList[1] < Game.Get().OpponentHighCardList[1])
                        {
                            LoseMethod();
                        }
                        else
                        {
                            int AnyHighCardGreaterThenOpponentsAllCards = Game.Get().MyHighCardList.FindAll(x => x > Game.Get().OpponentHighCardList.Find(x => Game.Get().OpponentHighCardList.Any(y => y > x) == false)).Count;
                            int AnyHighCardGreaterThenMyAllCards = Game.Get().OpponentHighCardList.FindAll(x => x > Game.Get().MyHighCardList.Find(x => Game.Get().MyHighCardList.Any(y => y > x) == false)).Count;
                            if (AnyHighCardGreaterThenMyAllCards >= 1)
                            {
                                LoseMethod();
                            }
                            else if (AnyHighCardGreaterThenOpponentsAllCards >= 1)
                            {
                                isPlayer1Win = WinMethod();
                            }
                            else
                            {
                                isPlayer1Win = TieMethod();
                                isTie = true;
                            }

                        }
                    }
                }
                else
                {
                    isPlayer1Win = TieMethod();
                    isTie = true;
                }
            }
        }
        isResultScreenOn = true;
        PokerButtonManager.instance.SetAllButtonsOff();
        if (isTie)
        {
            StartCoroutine(CheckWinNew(3f));
        }
        //Debug.LogError("PLAYER WIN :" + isPlayer1Win);
        StopTimer();
        //StartCoroutine(CheckWinNew());
    }
    /// <summary>
    /// Set's "Tie" text in result and reset health values
    /// </summary>
    /// <returns></returns>
    public bool TieMethod()
    {
        bool isPlayer1Win;
        P1HealthBar.value = P1StartHealth;
        P2HealthBar.value = P2StartHealth;
        isPlayer1Win = true;
        BetTextObj.text = "TIE";

        DemoManager.instance.HighLightWinnerHand(true);
        UpdateHMTxt();

        return isPlayer1Win;
    }
    /// <summary>
    /// Set text on loser player device in poker game and lose condition related settings 
    /// </summary>
    private void LoseMethod()
    {
        bool isLocationMatch = false;
        if (isDefenceLocationSelected)//Game.Get().lastAction == PlayerAction.defend &&
        {
            if (playerDefenceLocation == opponentAttackLocation)
            {
                isLocationMatch = true;
            }
            else
            {
                isLocationMatch = false;
            }
        }
        //Debug.LogError("***MY HEALTH DEC");
        BetTextObj.text = "YOU LOSE";
        Invoke("CheckForEffects", 1.5f);
        Debug.Log("LOCATION MATCH " + isLocationMatch);
        DemoManager.instance.HighLightWinnerHand(false);

    }
    /// <summary>
    /// Used check for effect of attack - Attacker's attack location and Defender's defence location decides effects
    /// </summary>
    public void CheckForEffects()
    {
        ManangeAttackLocationEffects();
        ChekcForWeaknessFactor();
    }
    /// <summary>
    /// Returns true if local playerwin , highlights winner hand on poker
    /// </summary>
    /// <returns>Ture if localplay win poker game</returns>
    public bool WinMethod()
    {
        bool isPlayer1Win;
        bool isLocationMatch = false;
        //Debug.LogError("***OPPONENT HEALTH DEC");
        BetTextObj.text = "YOU WIN";
        DemoManager.instance.HighLightWinnerHand(true);
        isPlayer1Win = true;

        if (isAttackLocationSelected) //Game.Get().lastAction == PlayerAction.defend &&
        {
            if (opponentDefendLocation == playerAttackLocation)
            {
                isLocationMatch = true;
            }
            else
            {
                isLocationMatch = false;
            }
        }

        return isPlayer1Win;
    }
    /// <summary>
    /// Checks for weaknees factor of player weakness against opponent and exceutes damange functions
    /// </summary>
    private void ChekcForWeaknessFactor()
    {
        CharacterType myCharWeakAgainst = CharacterType.Earth, opponent = CharacterType.Earth;
        if (Game.Get().IsDefender)
        {
            myCharWeakAgainst = p2Char.weakAgainst;
            opponent = p1Char.type;
        }
        else
        {
            myCharWeakAgainst = p1Char.weakAgainst;
            opponent = p2Char.type;
        }

        int player1weaknessfactor = myCharWeakAgainst == opponent ? (int)(0.2f * AttackFor) : 0;
        //

        //Debug.LogError("ATTACK FOR " + AttackFor + "  w factor " + ((int)(0.2f * AttackFor)));
        Debug.Log("P1 weak agains p2 " + myCharWeakAgainst.ToString() + "  ----" + opponent.ToString() + " --Extra damamge" + player1weaknessfactor);


        //Check if opponent attack has weak effect 
        int player2weakEffectFactor = (int)(opponentAttackWeakPercentageVal * AttackFor);


        //Location Effect
        int extraDamageForHighLimbAttack = (int)(((isAttackedHigh * 0.05f)) * AttackFor);

        //Debug.LogError("Damage new Effs : " + player2weakEffectFactor + " - " + extraDamageForHighLimbAttack);

        Debug.Log("Less DAMAGE opponent Weakness % val___" + player2weakEffectFactor);
        Debug.Log("EXTRA DAMAGE High limb succeesfull attack by opponent %___" + extraDamageForHighLimbAttack);
        Debug.Log("Total amount differenc " + (-player1weaknessfactor + player2weakEffectFactor - extraDamageForHighLimbAttack));
        //
        if (myCharWeakAgainst == opponent)
        {
            int total = (-player1weaknessfactor + player2weakEffectFactor - extraDamageForHighLimbAttack);
            StartCoroutine(UpdateRemainingHandHealthPlus(total));
            //  UpdateRemainingHandHealth(player1weaknessfactor);
            ShowExtraDamageMessage(0);
            Debug.Log("EXTRA DAMAGE Weakness 20%___" + player1weaknessfactor);
        }
        else
        {
            StartCoroutine(UpdateRemainingHandHealthPlus((+player2weakEffectFactor - extraDamageForHighLimbAttack)));
        }
    }

    /// <summary>
    ///  Applies effect of with resepct to attack locations in poker
    /// </summary>
    /// <param name="loc"></param>
    [PunRPC]
    public void ApplyAttackEffs(int loc)
    {
        switch ((AttackLocation)loc)
        {
            case AttackLocation.None:
                break;
            case AttackLocation.High:

                //  int player1weaknessfactor = (int)(((isAttackedHigh* 0.05f)+p1DamageIncreasedby) * AttackFor);
                //  UpdateRemainingHandHealth(player1weaknessfactor);
                //  ShowExtraDamageMessage(0);
                // Debug.Log("EXTRA DAMAGE %___" + player1weaknessfactor);
                p1DamageIncreasedby += (isAttackedHigh * 0.05f);

                break;
            case AttackLocation.Low:

                float effectFactor = (isAttackedLow * 0.05f) + p1SpeedSlowBy;

                p1SpeedSlowBy += (isAttackedLow * 0.05f);
                float speedSlowedDownVal = effectFactor * p1Speed;
                p1Speed -= speedSlowedDownVal;
                if (p1Speed < 0) p1Speed = 0;
                P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString("F2");
                photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);
                PhotonNetwork.SendAllOutgoingCommands();

                Debug.Log("Speed SLOWDOWN 5%___" + speedSlowedDownVal + " %---" + (p1SpeedSlowBy * 100));
                break;
            case AttackLocation.Left:
                p1DamageCapacityReducedby += (isAttackedOnLeftSide * 0.05f);
                Debug.Log("Damage Capacity Reduced by ___" + " %---" + (p1DamageCapacityReducedby * 100));
                //Reduce damage capacity by 5%  
                photonView.RPC("SetOpponentAttackWeakPercentage_RPC", RpcTarget.Others, p1DamageCapacityReducedby);
                PhotonNetwork.SendAllOutgoingCommands();
                break;
            case AttackLocation.Right:
                p1DamageCapacityReducedby += (isAttackedOnRightSide * 0.05f);
                Debug.Log("Damage Capacity Reduced by ___" + " %---" + (p1DamageCapacityReducedby * 100));
                photonView.RPC("SetOpponentAttackWeakPercentage_RPC", RpcTarget.Others, p1DamageCapacityReducedby);
                PhotonNetwork.SendAllOutgoingCommands();
                //photonView.RPC(Up)
                //Reduce damage capacity by 5%
                break;
            case AttackLocation.Middle:
                p1StaminRecoveryReducedBy += ((float)isAttackedInMiddle * 0.05f);
                Debug.Log("Damage Capacity Reduced by ___" + " %---" + (p1StaminRecoveryReducedBy * 100f));
                //Reduce damage capacity by 5%  
                //Slow down stamina recovery by 5%
                break;
            default:
                break;
        }
    }
    /// <summary>
    ///  Triggers oppoent attacked side rpc from player in case fo attack and defend location are different
    /// </summary>
    private void ManangeAttackLocationEffects()
    {
        if (playerDefenceLocation == opponentAttackLocation)
        {
            return;
        }

        photonView.RPC("SetOpponentAttackedSide_RPC", RpcTarget.Others, (int)opponentAttackLocation);
        PhotonNetwork.SendAllOutgoingCommands();
    }


    /// <summary>
    ///  Trigger win/lose coroutine and starts chess game if poker game is over
    /// </summary>
    public void ResumeChessGame()
    {
        StartCoroutine(CheckWinNew(1f));
    }
    //bool used to check if poker game result screen is on or not
    public bool isResultScreenOn = false;
    //bool used to reset pvp data
    public bool isReset = false;

    /// <summary>
    /// RPC - reset
    /// </summary>
    /// <param name="val"></param>
    [PunRPC]
    public void Rset_BOOL_RPC(bool val)
    {
        isReset = val;
    }
    /// <summary>
    ///  Reset pvp UI dat, turns and start drawing new  cards if poker game is not over
    /// </summary>
    /// <param name="isGameOver">True- In case of poker game is over</param>
    /// <param name="IsMaster">True- In case of master client</param>
    public void ResetData(bool isGameOver = false, bool IsMaster = false)
    {

        photonView.RPC("ResetPVPUIData", RpcTarget.All, isGameOver);
        PhotonNetwork.SendAllOutgoingCommands();

        photonView.RPC("RPC_ResetTurn", RpcTarget.All);
        PhotonNetwork.SendAllOutgoingCommands();
        if (PhotonNetwork.IsMasterClient)
        {
            DemoManager.instance.SecondTimeShuffleCall();
        }
        if (!isGameOver)
            SpellManager.instance.DrawCard();
        AddMana();

    }
    /// <summary>
    /// Trigger Generate cards from Masterclient function
    /// </summary>
    /// <param name="isGameOver">True- In case of poker game is over</param>
    public void GenerateCardFromMaster(bool isGameOver)
    {
        if (!isGameOver)
        {
            DemoManager.instance.Generate3CardsStack();
        }
    }
    /// <summary>
    /// Reset pvp UI text object data in network player device
    /// </summary>
    [PunRPC]
    public void ResetPVPUIData(bool isGameOver = false)
    {
        if (isReset)
        {
            isGameOver = isReset;
            P1StaBar.value = 10;
            P1StaTxt.text = "10/10";
            P2StaBar.value = 10;
            P2StaTxt.text = "10/10";
            P1ChessStaminaTxt.text = P1StaTxt.ToString();
            P2ChessStaminaTxt.text = P2StaTxt.ToString();
            P1RageBar.value = 0;
            P1RageTxt.text = "Rage 0";
            P2RageBar.value = 0;
            P2RageTxt.text = "Rage 0";
            UpdateHMTxt();
            PVPManager.manager.myLocalBatAmount = 0;
            UpdateHMTxt();
            photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, false);
            PhotonNetwork.SendAllOutgoingCommands();
            //ResetAttackLocation();
        }
        player1BetAmt = 0;
        player2BetAmt = 0;
        player1Bet.text = "0";
        player2Bet.text = "0";
        AttackFor = 0;
        isResultScreenOn = false;
        //Reset Dack
        List<Card_SO> temp = new List<Card_SO>(deckFullList.Length);
        temp = deckFullList.ToList<Card_SO>();
        DemoManager.instance.deck.Clear();
        DemoManager.instance.deck = temp.GetRange(0, temp.Count);
        DemoManager.instance.listofCards = DemoManager.instance.deck;
        updatePlayerAction("");
        temp.Clear();
        isDefenceLocationSelected = false;
        isAttackLocationSelected = false;
        //IsAttacker = !isLocalPVPFirstTurn;
        playerAttackLocation = AttackLocation.None;
        playerDefenceLocation = AttackLocation.None;
        opponentDefendLocation = AttackLocation.None;
        opponentAttackLocation = AttackLocation.None;
        PlayerChoiceOnce = -1;
        opponentCardSprite.Clear();
        OpponentBestIndex = 0;
        opponetHandCardValues.Clear();
        opponetHandCardColors.Clear();
        P1RemainingHandHealth = (int)P1HealthBar.value;
        P2RemainingHandHealth = (int)P2HealthBar.value;
        P1StartHealth = P1RemainingHandHealth;
        P2StartHealth = P2RemainingHandHealth;
        //Debug.LogError("*** COUNT "+DemoManager.instance.deck.Count);
        PokerButtonManager.instance.bet_attack.interactable = true;
        PokerButtonManager.instance.Reraise_CouterAttack.interactable = true;
        PokerButtonManager.instance.bet_attack.interactable = true;
        //IsPetTurn = false;
        SpellManager.PetAlreadyAttacked = false;
        //
        RemoveOpponentCards();
        RemovePlayerCards();
        PlayerCards.SetActive(true);
        Game.Get().BetAmount = 0;
        myLocalBatAmount = 0;
        rangeCounter = 0;
        OpponentRangCounter = 0;
        manager.BetTextObj.text = "";
        manager.resultText.text = "";
        Game.Get().localBetAmount = 0;
        isLocationChoose = false;
        //AttackChoices.GetComponentInChildren<AttackSlider>()._slider.value = 0;
        attackSlider._slider.value = 0;
        P2LastAttackValue = 0;
        MyLastAttackAmount = 0;
        LastAtkAmt = 0;
        isAllIn = false;
        isFromInbetween = false;
        isNormalBat = false;

        //Removed Old Board Cards
        foreach (Card item in DemoManager.instance.board_cards)
        {
            //Debug.LogError("BOARD CARDS REMOVED");
            Destroy(item.gameObject);
        }
        //
        DemoManager.instance.board_cards.Clear();

        foreach (Transform child in DemoManager.instance.placeholderHand.transform)
        {
            if (child.gameObject.GetComponent<Card>())
            {
                //Debug.LogError("PLACE HOLDER HAND CARD REMOVED");
                Destroy(child.gameObject);
            }
        }


        DemoManager.instance.ResetnumCards(isGameOver);
    }

    //Not -can delete and check after deletion
    public IEnumerator GenerateCardWithDelayFunction(bool isGameOver, float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            Debug.Log("CARD GENERATION CALL 6");
            DemoManager.instance.Generate3CardsStack();
        }
    }
    /// <summary>
    /// Remove poker cards from player's hand
    /// </summary>
    private void RemovePlayerCards()
    {
        foreach (Transform item in PlayerCards.transform)
        {
            if (item.CompareTag("dontRemove"))
            {
                foreach (Transform itemC in item.transform)
                {
                    Destroy(itemC.gameObject);
                }
            }
        }
    }
    /// <summary>
    /// Remove poker cards from opponent' hand
    /// </summary>
    private void RemoveOpponentCards()
    {
        foreach (Transform item in OpponetPlayerCards.transform)
        {
            if (item.CompareTag("dontRemove"))
            {
                foreach (Transform itemC in item.transform)
                {
                    Destroy(itemC.gameObject);
                }
            }
        }
    }


    /// <summary>
    /// RPC to set other player strength of poker hand
    /// </summary>
    /// <param name="strength">strength value of poker hand</param>
    /// <param name="result">result text</param>
    /// <param name="highCardVal">highest card value in hand</param>
    /// <param name="secondHighCardValue">second highest card value in hand</param>
    /// <param name="myHighCardList">player's high cards values list</param>
    [PunRPC]
    public void RPC_SetOthersPlayerStrength(int strength, string result, int highCardVal, int secondHighCardValue, int[] myHighCardList)
    {
        Game.Get().PokerHandResults[1] = result;
        Game.Get().PlayerStrengths[1] = strength;
        Game.Get().OpponentHighCardValue = highCardVal;

        Game.Get().OpponentSecondHighCardValue = secondHighCardValue;
        if (myHighCardList != null)
        {
            Game.Get().OpponentHighCardList.Clear();
            Game.Get().OpponentHighCardList = myHighCardList.ToList();
        }
    }
    //bool used for player's choice in poker game
    int tempChoiceNo = -1;
    /// <summary>
    /// Set choice variable
    /// </summary>
    /// <param name="c"></param>
    public void selectChoice(int c)
    {
        tempChoiceNo = c;
        ConfirmChoice();
    }
    /// <summary>
    /// Confirm player's choice for poker turn
    /// </summary>
    public void ConfirmChoice()
    {
        int c = -1;
        if (tempChoiceNo != -1)
            c = tempChoiceNo;

        if ((!isAttackLocationSelected || !isDefenceLocationSelected) && Game.Get().turn > 0)
        {
            // Debug.LogError("Going here dumbass");

            //    if(!isAttackLocationSelected)
            //    {
            isAttackLocationSelected = true;
            UpdateAttackLocation(GetAttackLocation(c));
            //}

            /* if*///(!isDefenceLocationSelected);
                   //{
            isDefenceLocationSelected = true;
            UpdateDefenceLocation(GetAttackLocation(c));
            //}

            LocationChoices.SetActive(false);
            PVPManager.Get().AttackChoices.SetActive(true);
            DemoManager.instance._pokerButtons.SetActive(false);
            if (P1RageBar.value > 50)
                SpecialAttackButton.SetActive(true);
        }
        else
        {
            if (!isAttackLocationSelected)
            {
                isAttackLocationSelected = true;
                UpdateAttackLocation(GetAttackLocation(c));
            }
            if (!isDefenceLocationSelected)
            {
                isDefenceLocationSelected = true;
                UpdateDefenceLocation(GetAttackLocation(c));
            }

            if (!isCheck)
            {
                LocationChoices.SetActive(false);
                PlayerChoiceOnce = c;
                photonView.RPC("RPC_SetOpponentAttackChoice", RpcTarget.Others, c);
                PhotonNetwork.SendAllOutgoingCommands();
                choice = c;
                if (isAttackViaSpeedPoints)
                {

                    photonView.RPC("UpdateBatAmount", RpcTarget.All, (int)SpeedAttackSlider.instance._slider.value);
                    UpdateBatAmountLocal((int)SpeedAttackSlider.instance._slider.value);
                    photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, (int)SpeedAttackSlider.instance._slider.value);
                    PhotonNetwork.SendAllOutgoingCommands();
                    //  UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                else
                {

                    photonView.RPC("UpdateBatAmount", RpcTarget.All, (int)(AttackSlider.instance._slider.value * 2));
                    UpdateBatAmountLocal((int)(AttackSlider.instance._slider.value * 2));
                    photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, (int)(AttackSlider.instance._slider.value * 2));
                    //UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                //EndTurn(c);
            }
            else
            {
                LocationChoices.SetActive(false);
            }
        }
        choiceConfPopup.SetActive(false);
        AttackChoices.SetActive(false);
        OnClickEndTurn();
    }
    /// <summary>
    /// Reject choice and close confirmation popup
    /// </summary>
    public void RejectChoice()
    {
        choiceConfPopup.SetActive(false);
        tempChoiceNo = -1;
    }
    /// <summary>
    /// Update values for reraise /counter attack in poker
    /// </summary>
    public void UpdateDataForReraise()
    {
        if (isAttackViaSpeedPoints)
        {
            photonView.RPC("UpdateBatAmount", RpcTarget.All, Mathf.RoundToInt(SpeedAttackSlider.instance._slider.value));
            photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, Mathf.RoundToInt(SpeedAttackSlider.instance._slider.value));
            //UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
            UpdateBatAmountLocal(Mathf.RoundToInt(SpeedAttackSlider.instance._slider.value));
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            photonView.RPC("UpdateBatAmount", RpcTarget.All, (int)(AttackSlider.instance._slider.value * 2));
            photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, (int)(AttackSlider.instance._slider.value * 2));
            //UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
            UpdateBatAmountLocal((int)(AttackSlider.instance._slider.value * 2));
            PhotonNetwork.SendAllOutgoingCommands();
        }

    }
    /// <summary>
    /// Update range counter
    /// </summary>
    public void UpdateRangeCounter()
    {
        if (lastAttackType == SliderAttack.nun)
        {
            lastAttackType = AttackSlider.instance._sliderAttack;
            rangeCounter = 1;
        }
        else
        {
            if (lastAttackType == AttackSlider.instance._sliderAttack)
            {
                rangeCounter += 1;
            }
        }
    }
    /// <summary>
    /// Update bet text
    /// </summary>
    /// <param name="c">bet amount</param>
    public void UpdateBatText(int c)
    {
        photonView.RPC("UpdateBatAmount", RpcTarget.All, c);
        UpdateBatAmountLocal(c);
        photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, c);
        PhotonNetwork.SendAllOutgoingCommands();

    }
    /// <summary>
    /// Update bet amount for fold
    /// </summary>
    /// <param name="c"></param>
    public void UpdateBatTextFold(int c)
    {
        photonView.RPC("UpdateBatAmount", RpcTarget.All, c);
        UpdateBatAmountLocal(0);
        photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, c);
        PhotonNetwork.SendAllOutgoingCommands();

    }
    /// <summary>
    /// Update player's remaining health
    /// </summary>
    /// <param name="c">deduction amount</param>
    public void UpdateRemainingHandHealth(int c)
    {
        P1RemainingHandHealth -= c;
        if (P1RemainingHandHealth < 0) P1RemainingHandHealth = 0;
        P1HealthBar.value = P1RemainingHandHealth;
        Debug.Log("P1 HEALTH BAR " + P1HealthBar.value + " C " + c);
        photonView.RPC("SetOpponentRemainingHandHealth_RPC", RpcTarget.Others, c);
        PhotonNetwork.SendAllOutgoingCommands();

    }
    /// <summary>
    /// RPC- Upate remaining health in opponent
    /// </summary>
    /// <param name="h">deduction amount</param>
    [PunRPC]
    public void SetOpponentRemainingHandHealth_RPC(int h)
    {
        P2RemainingHandHealth -= h;
        P2HealthBar.value = P2RemainingHandHealth;

    }

    public GameObject Proj; //Spell projectile ojbect prefab used in poker game
    /// <summary>
    /// RPC-  spell simple attack
    /// </summary>
    /// <param name="targetId">Target card id</param>
    /// <param name="dmg">damage value</param>
    /// <param name="isplayer">True if target is player</param>
    /// <param name="dealdmg">True if projectile will deal damange</param>
    [PunRPC]
    public void SimpleAttackRPC(int targetId, int dmg, bool isplayer, bool dealdmg)
    {
        GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? p1Image.gameObject : SpellManager.instance.playerBattleCards.Find(x => x.id == targetId).gameObject;
        proj.damage = dmg;
        proj.istargetPlayer = isplayer;
        proj.DealDamage = dealdmg;
        proj.lifetime = 2f;
    }
    /// <summary>
    /// Cast spell coroutine
    /// </summary>
    /// <param name="c">card's damage capacity</param>
    /// <returns></returns>
    public IEnumerator DistributeSpellAttack(int c)
    {
        yield return new WaitWhile(() => SpellManager.petAttackStarted);

        int temp = c;
        Debug.Log(temp + " total attack");
        foreach (var item in SpellManager.instance.opponentBattleCards)
        {
            yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            if (temp > item.Hp)
            {
                temp -= item.Hp;
                GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = item.Hp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("SimpleAttackRPC", RpcTarget.Others, item.card.cardId, item.Hp, false, true);
                PhotonNetwork.SendAllOutgoingCommands();
                yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            }
            else if (temp < item.Hp)
            {

                GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = temp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("SimpleAttackRPC", RpcTarget.Others, item.card.cardId, temp, false, true);
                PhotonNetwork.SendAllOutgoingCommands();
                temp = 0;
                yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            }

            if (temp == 0) break;


        }

        if (temp > 0)
        {
            GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
            Projectile proj = o.GetComponent<Projectile>();
            proj.target = p2Image.gameObject;
            proj.damage = temp;
            proj.istargetPlayer = true;
            proj.DealDamage = true;
            proj.lifetime = 2f;
            photonView.RPC("SimpleAttackRPC", RpcTarget.Others, -1, temp, true, false);
            PhotonNetwork.SendAllOutgoingCommands();
        }

        canContinue = true;
    }
    /// <summary>
    /// Cast porjectile for spell attack
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public IEnumerator DistributeAttack(int c)
    {
        yield return new WaitWhile(() => SpellManager.petAttackStarted);

        int temp = AttackFor + c;
        Debug.Log(temp + " total attack");
        foreach (var item in SpellManager.instance.playerBattleCards)
        {
            yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            if (temp > item.Hp)
            {
                temp -= item.Hp;
                GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = item.Hp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("DistributeAttackRPC", RpcTarget.Others, item.card.cardId, item.Hp);
                PhotonNetwork.SendAllOutgoingCommands();
                yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            }
            else if (temp < item.Hp)
            {

                GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = temp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("DistributeAttackRPC", RpcTarget.Others, item.card.cardId, temp);
                PhotonNetwork.SendAllOutgoingCommands();
                temp = 0;
                yield return new WaitWhile(() => SpellManager.IsPetAttacking);
            }

            if (temp == 0) break;


        }
        Debug.LogError("star = " + P1StartHealth + " - " + temp);
        RemainingAtk = P1StartHealth - temp;
        canContinue = true;
    }
    /// <summary>
    /// RPC- spell attack on network 
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="attack"></param>
    [PunRPC]
    public void DistributeAttackRPC(int cardId, int attack)
    {
        foreach (var item in SpellManager.instance.opponentBattleCards)
        {
            if (item.card.cardId == cardId)
            {
                GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = attack;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
            }
        }
    }

    public int RemainingAtk;  //Variable used for holding temporary health value
    public bool canContinue;  //Used in spell cast functionality

    /// <summary>
    /// Used in case of effect of attack location to increase/decrease health
    /// </summary>
    /// <param name="c">effect value</param>
    public IEnumerator UpdateRemainingHandHealthPlus(int c)
    {

        canContinue = false;
        StartCoroutine(DistributeAttack(c));
        yield return new WaitUntil(() => canContinue);
        Debug.LogError("rem attack" + RemainingAtk);
        P1RemainingHandHealth = RemainingAtk;
        if (P1RemainingHandHealth > P1StartHealth) P1RemainingHandHealth = P1StartHealth;

        if (P1RemainingHandHealth < 0) P1RemainingHandHealth = 0;

        P1HealthBar.value = P1RemainingHandHealth;
        photonView.RPC("SetOpponentRemainingHandHealth_RPCPlus", RpcTarget.Others, P1RemainingHandHealth);
        PhotonNetwork.SendAllOutgoingCommands();
        //Update Values in loser

        float TempVal = P1HealthBar.value;
        //Debug.LogError(TempVal+" - "+(c)+" - "+P1RemainingHandHealth);
        P1HealthBar.value = playerDefenceLocation == opponentAttackLocation ? P1RemainingHandHealth + (int)(AttackFor) : TempVal;

        // P1HealthBar.value -= isLocationMatch ? (Game.Get().BatAmount / 2) : Game.Get().BatAmount; //50% reduce damage
        // P1HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount; //100% nutralize damage
        float myAttackRisk = P2StartHealth - P2HealthBar.value;
        P2HealthBar.value = P2StartHealth <= 0 ? AttackFor : P2StartHealth;
        P1StartHealth = (int)P1HealthBar.value;
        //P2StaBar.value -= (int)Game.Get().BetAmount * 0.1f;
        P2StaBar.value -= AttackFor * 0.1f;
        P2StaTxt.text = (P2StaBar.value).ToString();
        //p2Speed += (int)Game.Get().BetAmount * 0.1f;
        p2Speed += (myAttackRisk * 0.1f) - p2SpeedSlowedBy;
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString("F2");
        UpdateHMTxt();
        StartCoroutine(CheckWinNew(1f));
    }
    /// <summary>
    /// RPC- for applying location effect on health
    /// </summary>
    /// <param name="h"></param>
    [PunRPC]
    public void SetOpponentRemainingHandHealth_RPCPlus(int h)
    {
        P2RemainingHandHealth = h;
        if (P2RemainingHandHealth > P2StartHealth) P2RemainingHandHealth = P2StartHealth;
        P2HealthBar.value = P2RemainingHandHealth;
        //Update Data in Winner
        //Debug.LogError("loc match : "+(playerAttackLocation == opponentDefendLocation).ToString()+" deduct : "+(P2RemainingHandHealth - (int)(0.5f * (float)h)).ToString());
        float TempVal = P2HealthBar.value;
        P2HealthBar.value = playerAttackLocation == opponentDefendLocation ? P2RemainingHandHealth + (int)(AttackFor) : TempVal;
        float myAttackRisk = P1StartHealth - P1HealthBar.value;

        P1HealthBar.value = P1StartHealth <= 0 ? AttackFor : P1StartHealth;
        P2StartHealth = (int)P2HealthBar.value;
        // P2HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount;
        //P1StaBar.value -= Game.Get().BetAmount * 0.1f;
        P1StaBar.value -= AttackFor * 0.1f;
        P1StaTxt.text = (P1StaBar.value).ToString();
        P1ChessStaminaTxt.text = P1StaTxt.text;
        //p1Speed += (int)Game.Get().BetAmount * 0.1f;
        //Debug.LogError("Speed Calc To be increased : " + (int)(myAttackRisk * 0.1f) + "Actually increased :" + p1SpeedSlowBy);
        p1Speed += (float)((myAttackRisk * 0.1f));
        P1SpeedTxt.text = "";
        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString("F2");
        UpdateHMTxt();
        StartCoroutine(CheckWinNew(1f));
    }
    /// <summary>
    /// Show extra damage message
    /// </summary>
    /// <param name="c"></param>
    public void ShowExtraDamageMessage(int c)
    {
        LeanTween.scale(extraDamageMessageP1, Vector3.one, .3f);
        photonView.RPC("ShowExtraDamageMessage_RPC", RpcTarget.Others, c);
        PhotonNetwork.SendAllOutgoingCommands();
        UpdateHMTxt();
        Invoke("ResetDamageMessage", 3f);
    }

    /// <summary>
    /// RPC for extra damage message
    /// </summary>    
    [PunRPC]
    public void ShowExtraDamageMessage_RPC(int h)
    {
        LeanTween.scale(extraDamageMessageP2, Vector3.one, .3f);
        Invoke("ResetDamageMessage", 3f);
        UpdateHMTxt();
    }
    /// <summary>
    /// Reset scale of damage message object
    /// </summary>
    public void ResetDamageMessage()
    {
        LeanTween.scale(extraDamageMessageP2, Vector3.zero, .3f);
        LeanTween.scale(extraDamageMessageP1, Vector3.zero, .3f);
    }
    /// <summary>
    /// Update bet amount
    /// </summary>
    /// <param name="c"></param>
    [PunRPC]
    public void UpdateBatAmount(int c)
    {
        //Debug.LogError("**BET VALUE UPDATED");
        if (c == -1)
        {

            BetTextObj.gameObject.SetActive(true);
            BetTextObj.text = "Brace Call";
        }
        else if (c == -2)
        {

            BetTextObj.gameObject.SetActive(true);
            //BatTextObj.text = "Check";
        }
        else
        {
            BetTextObj.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Update last bet amount
    /// </summary>
    /// <param name="c"></param>
    [PunRPC]
    public void UpdateLastBatAmount(int c)
    {
        //Debug.LogError("**BET VALUE UPDATED");
        if (c == -1)
        {

            BetTextObj.gameObject.SetActive(true);
            BetTextObj.text = "Brace Call";
        }
        else if (c == -2)
        {

            BetTextObj.gameObject.SetActive(true);
            //BatTextObj.text = "Check";
        }
        else
        {
            BetTextObj.gameObject.SetActive(true);
            P2LastAttackValue = c;
        }
    }
    public int MyLastBatAmount = -1;//Player's last bet amount

    /// <summary>
    /// Update bet amount in local player
    /// </summary>
    /// <param name="c"></param>
    public void UpdateBatAmountLocal(int c)
    {
        //Debug.LogError("VAlue local " + c);
        MyLastBatAmount = c;
        BetTextObj.gameObject.SetActive(true);
        myLocalBatAmount += c;
        if (myLocalBatAmount < 0)
        {
            myLocalBatAmount = 0;
        }
        Game.Get().localBetAmount = myLocalBatAmount;
        //Info:: This is working ::BetTextObj.text = "BET Value : " +c;
        //  BetTextObj.text = "BET Value : " + myLocalBatAmount;

    }
    /// <summary>
    /// Update rage points in opponent
    /// </summary>
    /// <param name="c">Rage counter</param>
    [PunRPC]
    public void UpdateOpponentRangePoints(int c)
    {
        OpponentRangCounter = c;

        if (c >= 3)
        {
            P2RageBar.value += 10;
            UpdateHMTxt();
            OpponentRangCounter = 0;
        }   //rangeCounter = 0;

    }
    /// <summary>
    /// Update rage points in player
    /// </summary>
    /// <param name="c">Rage counter</param>
    public void UpdateRangePoints(int c)
    {

        //if(PhotonNetwork.LocalPlayer.IsLocal)
        //{
        if (rangeCounter >= 3)
        {
            P1RageBar.value += 10;
            P1RageTxt.text = "Rage " + ((int)P1RageBar.value).ToString();
            rangeCounter = 0;
            lastAttackType = SliderAttack.nun;
        }
    }
    //booleans used in poker game to check different conditions 
    public bool isNormalBat = false, isAllIn = false, isFromInbetween = false;
    public bool IsAnyAllIn = false;
    public bool isfold = false;
    public bool isCheck = false;
    public bool isReraiseAfterOnce = false;
    public bool isLocationChoose = false;

    bool endTurn = false;  //Ture in case of turn is ended
    public bool endChessTurn = false;  //True in case of chess turn end

    public bool LastActionUpdated; //bool to check if last action is updated or not
    /// <summary>
    /// Trigger turn end logic in poker
    /// </summary>
    public void OnClickEndTurn()
    {
        if (!LastActionUpdated)
        {
            if (Game.Get().turn <= 1 || (Game.Get().lastAction == PlayerAction.counterAttack || Game.Get().lastAction == PlayerAction.attack))
            {
                PokerButtonManager.instance.Fold();
                isfold = true;
            }
            else
            {
                PokerButtonManager.instance.Check();
            }
        }

        endTurn = true;
        endTurnTimeStarted = false;
        EndTurnTimer = 30;
        StopCoroutine(UpdateEndTurnTimer());
        LocationChoices.SetActive(false);
        if (Game.Get().lastAction == PlayerAction.attack || Game.Get().lastAction == PlayerAction.counterAttack)
        {

            if (Game.Get().lastAction == PlayerAction.attack && Game.Get().turn >= 2)
            {
                UpdateDataForReraise();
                //Debug.LogError("END TURN CALLED ");
            }
            UpdateRangeCounter();
            UpdateRangePoint(rangeCounter);
            if (isAttackViaSpeedPoints)
            {
                p1AttackFor.text = (Game.Get().lastAction == PlayerAction.counterAttack) ? "Counter attack for " + (int)speedAttackSlider.value
                    : "Attack For " + (int)speedAttackSlider.value;
                photonView.RPC("UpdateAttackForText", RpcTarget.Others, (int)speedAttackSlider.value, Game.Get().lastAction == PlayerAction.counterAttack, false);
                PhotonNetwork.SendAllOutgoingCommands();
                p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);
            }
            else
            {
                Debug.LogError("ELSE PART***");
                p1AttackFor.text = (Game.Get().lastAction == PlayerAction.counterAttack) ? "Counter attack for " + MyLastAttackAmount
                    : "Attack For " + MyLastAttackAmount;
                UpdateBetForPlayer(MyLastAttackAmount);
                photonView.RPC("UpdateAttackForText", RpcTarget.Others, MyLastAttackAmount, Game.Get().lastAction == PlayerAction.counterAttack, false);
                PhotonNetwork.SendAllOutgoingCommands();
                p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);
            }

            //  if(!Game.Get().IsDefender)
            //  {
            //      photonView.RPC("UpdatePotAmountForText",RpcTarget.All,(int)attackSlider._slider.value);
            if (!isNormalBat || !isCheck)
            {
                photonView.RPC("UpdatePotAmountForText", RpcTarget.All, P2LastAttackValue);
                PhotonNetwork.SendAllOutgoingCommands();
            }

            //  }
            Invoke("ResetText", 3f);
        }
        if (!isNormalBat && !isCheck && !isfold && !isReraiseAfterOnce)
        {
            EndTurn(choice);
        }
        else
        {
            if (isfold)
            {
                //Debug.LogError("FOLD ");
                isfold = false;

                PokerButtonManager.instance.FoldAction();
            }
            else if (isNormalBat)
            {
                //Debug.LogError("Going here");
                EndTurnNormalBat();
                isNormalBat = false;
                //isCheck = false;
                // PokerButtonManager.instance.Check();

            }
            else if (isCheck)
            {

                EndTurn(0);

                //Debug.LogError("**END TURN CALLED from NORMAL BET");
            }
            else if (isReraiseAfterOnce)
            {
                isReraiseAfterOnce = false;
                // UpdateRangeCounter();
                UpdateDataForReraise();
                EndTurn(PlayerChoiceOnce);
                //Debug.LogError("**END TURN CALLED from RERAISE");
            }
        }
        choice = -1;

        EndTurnBtn.gameObject.SetActive(false);
        if (isAttackViaSpeedPoints)
        {
            float maxVal = Mathf.Min(PVPManager.manager.P1RemainingHandHealth, PVPManager.manager.P2RemainingHandHealth);
            float perSpeedPoint = maxVal * 0.1f;
            float usedPoints = speedAttackSlider.value / perSpeedPoint;
            Debug.Log("SPEED POINTs " + p1Speed + " --Used Points  " + usedPoints);

            p1Speed -= usedPoints;
            if (p1Speed < 0) p1Speed = 0;
            P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString("F2");
            photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);
            PhotonNetwork.SendAllOutgoingCommands();

            isAttackViaSpeedPoints = false;
        }

        if (Game.Get().turn <= 1)
        {
            if (isAttackViaSpeedPoints)
            {

                photonView.RPC("UpdateBatAmount", RpcTarget.All, (int)SpeedAttackSlider.instance._slider.value);
                UpdateBatAmountLocal((int)SpeedAttackSlider.instance._slider.value);
                photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, (int)SpeedAttackSlider.instance._slider.value);
                //  UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else
            {

                photonView.RPC("UpdateBatAmount", RpcTarget.All, Mathf.RoundToInt((AttackSlider.instance._slider.value * 2)));
                UpdateBatAmountLocal(Mathf.RoundToInt((AttackSlider.instance._slider.value * 2)));
                photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, Mathf.RoundToInt((AttackSlider.instance._slider.value * 2)));
                //UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }

        //speedAttackButton.SetActive(false);
        SpecialAttackButton.SetActive(false);
        //if(Game.Get().lastAction != PlayerAction.brace)

        if (Game.Get().turn < 8)
        {
            photonView.RPC("SwitchPVPTurn", RpcTarget.All);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        //p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one,0.3f);

        LastActionUpdated = false;
        SpellManager.PetAlreadyAttacked = false;
    }

    /// <summary>
    /// RPC- Update speed points in network device
    /// </summary>
    /// <param name="points">speed points</param>
    [PunRPC]
    public void UpdateSpeedPoints(float points)
    {
        p2Speed = points;
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString("F2");
    }
    /// <summary>
    /// End turn and call update turn rpc for netx turn
    /// </summary>
    /// <param name="c">Attack type</param>
    public void EndTurn(int c)
    {
        #region old code
        //return;
        //Debug.LogError("***PVP STATE " + state);
        if (state == PVPStates.Attack || 1 == 1)
        {
            AttackType type = AttackData.GetAttackTypeFrmInt(c);
            playerChoice.attack = type;

            Debug.Log("local : " + isLocalPVPTurn + " master : " + PhotonNetwork.LocalPlayer.IsMasterClient + " 1 - " + p1Speed + " 2 - " + p2Speed);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                photonView.RPC("RPC_UpdateTurn", RpcTarget.All);
                photonView.RPC("RPC_otherplayerTurnPoker", RpcTarget.All, PhotonNetwork.LocalPlayer);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurn();
            }
            else if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                photonView.RPC("RPC_UpdateTurn", RpcTarget.All);
                photonView.RPC("RPC_otherplayerTurnPoker", RpcTarget.All, PhotonNetwork.LocalPlayer);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurn();
            }
            if (type == AttackType.Defend)
            {
                state = PVPStates.DefendLoc;
                AttackChoices.SetActive(false);
                //LocationChoices.SetActive(true);
                skipTurn.gameObject.SetActive(false);
                ModeTxT.text = "Choose Defend Location";
                foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                {
                    item.gameObject.SetActive(true);
                    item.button.interactable = true;
                }
            }
        }
        else if (state == PVPStates.ExtraAttack)
        {
            AttackType type = AttackData.GetAttackTypeFrmInt(c);
            playerChoice.ExtraAttack.Add(type);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                p1Speed -= GameData.Get().GetAttack(type).speed;
                UpdateSpeed();
                if (p1Speed >= lowestSpeedNeeded)
                {
                    state = PVPStates.ExtraAttack;
                    skipTurn.gameObject.SetActive(true);
                    AttackChoices.SetActive(true);
                    PVPManager.Get().speedAttackChoices.SetActive(false);
                    Debug.Log("AttackChoices true");
                    //LocationChoices.SetActive(false);
                    ModeTxT.text = "Choose Extra Attack Type";
                    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                    {
                        item.button.interactable = item.data.speed <= p1Speed;
                        if (item.data.type != AttackType.None)
                            item.ComboTxt.text = item.data.type.ToString();
                        if (item.data.type == AttackType.Defend)
                        {
                            item.gameObject.SetActive(false);
                            Debug.Log($"<color=yellow> defend button false here ..{item.name} </color>");
                        }
                    }
                    //SetModePanelOptions("ExtraAttack");
                    Debug.Log("EXTRA DONE");

                }
                else
                {
                    state = PVPStates.DefendLoc;
                    skipTurn.gameObject.SetActive(false);
                    AttackChoices.SetActive(false);
                    //LocationChoices.SetActive(true);
                    ModeTxT.text = "Choose Defend Location";
                    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                    {
                        item.gameObject.SetActive(true);
                        item.button.interactable = true;
                    }
                    //SetModePanelOptions("DefendLoc");
                }
            }
            else if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                p2Speed -= GameData.Get().GetAttack(type).speed;
                UpdateSpeed();
                if (p2Speed >= lowestSpeedNeeded)
                {
                    state = PVPStates.ExtraAttack;
                    skipTurn.gameObject.SetActive(true);
                    AttackChoices.SetActive(true);
                    PVPManager.Get().speedAttackChoices.SetActive(false);
                    Debug.Log("AttackChoices true");
                    //LocationChoices.SetActive(false);
                    ModeTxT.text = "Choose Extra Attack Type";
                    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                    {
                        item.button.interactable = item.data.speed <= p2Speed;
                        if (item.data.type != AttackType.None)
                            item.ComboTxt.text = item.data.type.ToString();
                        if (item.data.type == AttackType.Defend)
                        {
                            item.gameObject.SetActive(false);
                            Debug.Log($"<color=yellow> defend button false here .. {item.name}</color>");
                        }
                    }
                    //SetModePanelOptions("ExtraAttack");
                }
                else
                {
                    state = PVPStates.DefendLoc;
                    skipTurn.gameObject.SetActive(false);
                    AttackChoices.SetActive(false);
                    //LocationChoices.SetActive(true);
                    ModeTxT.text = "Choose Defend Location";
                    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                    {
                        item.gameObject.SetActive(true);
                        item.button.interactable = true;
                    }
                }
            }
        }
        else if (state == PVPStates.DefendLoc)
        {
            AttackLocation type = AttackData.GetLocationFrmInt(c);
            playerChoice.defendLoc = type;
            // ModePanel.SetActive(false);
            waitPanel.SetActive(true);
            AttackChoices.SetActive(false);
            //LocationChoices.SetActive(true);
            int[] extraChoices = new int[playerChoice.ExtraAttack != null ? playerChoice.ExtraAttack.Count : 0];
            for (int i = 0; i < extraChoices.Length; i++)
            {
                extraChoices[i] = (int)playerChoice.ExtraAttack[i];
            }

            object[] data = new object[] { p1Speed, p2Speed };

            Debug.Log($" <color=yellow> playerChoice.attackLoc {playerChoice.attackLoc} playerChoice.attack {playerChoice.attack} playerChoice.defendLoc {playerChoice.defendLoc} </color>");

            int counterAttackRandomForBothPlayers = UnityEngine.Random.Range(1, 10);
            int criticalhits = UnityEngine.Random.Range(1, 100);
        }
        #endregion
    }
    /// <summary>
    /// Stop poker turn timer
    /// </summary>
    public void StopTimer()
    {
        starttimer = false;
        StopCoroutine("UpdateEndTurnTimer");
        EndTurnBtn.gameObject.SetActive(false);
    }
    /// <summary>
    /// Trigger RPC to set IsAllIn in all player devices
    /// </summary>
    /// <param name="b">boolean value</param>
    public void SyncAllIn(bool b)
    {
        photonView.RPC("SyncAllInRPC", RpcTarget.All, b);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- Set "IsAnyAllInn" value same across all devices
    /// </summary>
    /// <param name="b">boolean value</param>
    [PunRPC]
    public void SyncAllInRPC(bool b)
    {
        PVPManager.manager.IsAnyAllIn = b;
    }
    /// <summary>
    /// End turn with noraml bet type
    /// </summary>
    public void EndTurnNormalBat()
    {
        if (isAllIn)
        {
            //Debug.LogError("is all in");
            isAllIn = false;
            PVPManager.manager.IsAnyAllIn = false;
            PVPManager.manager.SyncAllIn(false);
            // photonView.RPC("DisplayOpponentCard_RPC",RpcTarget.All); 
            photonView.RPC("SetALLButtonsOff_RPC", RpcTarget.All);
            PhotonNetwork.SendAllOutgoingCommands();
            if (!isFromInbetween)
            {
                // Debug.LogError("Not from between");
                //UpdateBatAmountLocal(P1RemainingHandHealth);
                // p1AttackFor.text = "Attack For " + (int)attackSlider._slider.value;
                p1AttackFor.text = "All In";
                p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);
                photonView.RPC("UpdateAttackForText", RpcTarget.Others, MyLastAttackAmount, false, true);
                //  if(!Game.Get().IsDefender)
                //   {
                // photonView.RPC("UpdatePotAmountForText",RpcTarget.All,(int)attackSlider._slider.value);

                int AllInAmount = P2StartHealth - P2RemainingHandHealth;
                photonView.RPC("UpdatePotAmountForAllInText", RpcTarget.All, AllInAmount);
                //photonView.RPC("UpdatePotAmountForAllInText", RpcTarget.All, P2StartHealth);
                PhotonNetwork.SendAllOutgoingCommands();

                //   }
                Invoke("ResetText", 3f);
            }
            else
            {
                photonView.RPC("UpdatePotAmountForText", RpcTarget.All, P2LastAttackValue);
                PhotonNetwork.SendAllOutgoingCommands();
                isFromInbetween = false;
            }
            StartCoroutine(UpdateTurn());
        }
        else
        {
            //  int player1weaknessfactor = p1Char.weakAgainst == p2Char.type ? (int)(0.2f * player1Dmg) : 0;
            // if(!Game.Get().IsDefender)
            // {
            // photonView.RPC("UpdatePotAmountForText",RpcTarget.All,(int)attackSlider._slider.value);
            photonView.RPC("UpdatePotAmountForText", RpcTarget.All, P2LastAttackValue);
            //   }
            photonView.RPC("RPC_UpdateTurn", RpcTarget.All);
            photonView.RPC("RPC_otherplayerTurnPoker", RpcTarget.All, PhotonNetwork.LocalPlayer);
            Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
            Game.Get().NextTurn();
        }



    }
    /// <summary>
    /// RPC -Display opponent hand card
    /// </summary>
    [PunRPC]
    public void DisplayOpponentCard_RPC()
    {
        for (int i = 0; i < OpponentPlayerCardPositions.Count; i++)
        {
            for (int j = 0; j < OpponentPlayerCardPositions[i].childCount; j++)
            {
                //OpponentPlayerCardPositions[i].GetChild(j).gameObject.SetActive(true);
                OpponentPlayerCardPositions[i].GetChild(j).GetChild(1).gameObject.GetComponent<Image>().sprite = opponentCardSprite[i];
            }
        }
    }
    /// <summary>
    /// Update rage points
    /// </summary>
    /// <param name="ragePoints">rage points value</param>
    public void UpdateRangePoint(int ragePoints)
    {
        if (rangeCounter >= 3)
        {
            P1RageBar.value += RagePointReward;
            P1RageTxt.text = "Rage " + P1RageBar.value.ToString();
            photonView.RPC("UpdateRagePoints_RPC", RpcTarget.Others, RagePointReward);
            PhotonNetwork.SendAllOutgoingCommands();
            rangeCounter = 0;
        }

    }
    /// <summary>
    /// RPC- Update rage points
    /// </summary>
    [PunRPC]
    public void UpdateRagePoints_RPC(int ragePoints)
    {
        P2RageBar.value += ragePoints;
        UpdateHMTxt();
    }
    /// <summary>
    /// Deactivate all poker game buttons
    /// </summary>
    [PunRPC]
    public void SetALLButtonsOff_RPC()
    {
        DemoManager.instance._pokerButtons.gameObject.SetActive(false);
    }
    /// <summary>
    /// RPC- set autor turn off/on 
    /// </summary>
    /// <param name="isOn">value</param>
    [PunRPC]
    public void SetAutoTurnOnOff_RPC(bool isOn)
    {
        isAutoTurn = isOn;
    }
    public bool isAutoTurn = false; //Used to update poker turns
    /// <summary>
    /// Turn update coroutie for poker game
    /// </summary>
    public IEnumerator UpdateTurn()
    {
        // Debug.LogError("Updating turn");
        if (isAutoTurn == false)
        {
            // Debug.LogError("setting auto turn");
            photonView.RPC("SetAutoTurnOnOff_RPC", RpcTarget.All, true);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        yield return new WaitForSeconds(0.9f);
        if (Game.Get().turn < 8)
        {
            photonView.RPC("RPC_UpdateTurn", RpcTarget.All);
            PhotonNetwork.SendAllOutgoingCommands();
            StartCoroutine(UpdateTurn());
        }
        else
        {

            StopCoroutine(UpdateTurn());
            photonView.RPC("DisplayOpponentCard_RPC", RpcTarget.All);
            photonView.RPC("SetAutoTurnOnOff_RPC", RpcTarget.All, false);
            PhotonNetwork.SendAllOutgoingCommands();
        }

    }

    /// <summary>
    ///  RPC -Sync player's location choice in all device
    /// </summary>
    /// <param name="attackLoc">attack location index</param>
    /// <param name="attack">attack type</param>
    [PunRPC]
    public void SyncPlayerChoice(int attackLoc, int attack) //,  int defendLoc, int[] extraChoices,object[] data,int counterAttackRandomForBothPlayers,int criticalhits)
    {
        //p1Speed = (float)data[0];
        //p2Speed = (float)data[1];
        PlayerChoice playerChoice;
        playerChoice.attackLoc = AttackData.GetLocationFrmInt(attackLoc);
        playerChoice.attack = AttackData.GetAttackTypeFrmInt(attack);
        //playerChoice.defendLoc = AttackData.GetLocationFrmInt(defendLoc);
        playerChoice.ExtraAttack = new List<AttackType>();
        //foreach (var item in extraChoices)
        //{
        //    playerChoice.ExtraAttack.Add(AttackData.GetAttackTypeFrmInt(item));
        //}

        if (isLocalPVPFirstTurn)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (isLocalPVPTurn)
                {
                    player1Choice.attackLoc = playerChoice.attackLoc;
                    player1Choice.attack = playerChoice.attack;
                    //player1Choice.defendLoc = playerChoice.defendLoc;
                    player1Choice.ExtraAttack = playerChoice.ExtraAttack;
                    //photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
                }
                else
                {
                    player2Choice.attackLoc = playerChoice.attackLoc;
                    player2Choice.attack = playerChoice.attack;
                    //player2Choice.defendLoc = playerChoice.defendLoc;
                    player2Choice.ExtraAttack = playerChoice.ExtraAttack;
                    state = PVPStates.Resolve;
                    Debug.Log("STATE = " + state.ToString());

                }
            }
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (isLocalPVPTurn)
                {
                    player2Choice.attackLoc = playerChoice.attackLoc;
                    player2Choice.attack = playerChoice.attack;
                    //player2Choice.defendLoc = playerChoice.defendLoc;
                    player2Choice.ExtraAttack = playerChoice.ExtraAttack;
                    state = PVPStates.Resolve;
                    Debug.Log("STATE = " + state.ToString());
                    //photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
                    //photonView.RPC("Resolve",RpcTarget.AllBuffered, counterAttackRandomForBothPlayers, criticalhits);
                }
                else
                {
                    player1Choice.attackLoc = playerChoice.attackLoc;
                    player1Choice.attack = playerChoice.attack;
                    // player1Choice.defendLoc = playerChoice.defendLoc;
                    player1Choice.ExtraAttack = playerChoice.ExtraAttack;

                    Debug.Log(" == set mode panel here == ");
                    //SetModePanel();
                }
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (isLocalPVPTurn)
                {
                    player1Choice.attackLoc = playerChoice.attackLoc;
                    player1Choice.attack = playerChoice.attack;
                    //player1Choice.defendLoc = playerChoice.defendLoc;
                    player1Choice.ExtraAttack = playerChoice.ExtraAttack;
                    state = PVPStates.Resolve;
                    Debug.Log("STATE = " + state.ToString());
                    //photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
                    // photonView.RPC("Resolve",RpcTarget.AllBuffered, counterAttackRandomForBothPlayers, criticalhits);

                }
                else
                {
                    player2Choice.attackLoc = playerChoice.attackLoc;
                    player2Choice.attack = playerChoice.attack;
                    //player2Choice.defendLoc = playerChoice.defendLoc;
                    player2Choice.ExtraAttack = playerChoice.ExtraAttack;
                    Debug.Log(" == set mode panel here == ");
                    //SetModePanel();

                }
            }
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (isLocalPVPTurn)
                {
                    player2Choice.attackLoc = playerChoice.attackLoc;
                    player2Choice.attack = playerChoice.attack;
                    // player2Choice.defendLoc = playerChoice.defendLoc;
                    player2Choice.ExtraAttack = playerChoice.ExtraAttack;
                    //photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
                }
                else
                {
                    player1Choice.attackLoc = playerChoice.attackLoc;
                    player1Choice.attack = playerChoice.attack;
                    // player1Choice.defendLoc = playerChoice.defendLoc;
                    player1Choice.ExtraAttack = playerChoice.ExtraAttack;
                    state = PVPStates.Resolve;
                    Debug.Log("STATE = " + state.ToString());

                }
            }
        }
    }
    /// <summary>
    /// Start hand turn swtich - if start hand turn is true then player is Attacker
    /// </summary>
    /// <param name="delay"></param>
    void SwitchStartHandTurn(float delay)
    {

        StartHandTurn = !StartHandTurn;
        // Debug.LogError("start turn changed : "+StartHandTurn);
        photonView.RPC("SwitchStartHandTurnRPC", RpcTarget.Others, delay);
        PhotonNetwork.SendAllOutgoingCommands();
        Invoke("SetModePanel", delay);
    }
    /// <summary>
    /// RPC - start hand turn update
    /// </summary>
    /// <param name="delay"></param>
    [PunRPC]
    public void SwitchStartHandTurnRPC(float delay)
    {
        StartHandTurn = !StartHandTurn;
        // Debug.LogError("start turn changed : "+StartHandTurn);
        Invoke("SetModePanel", delay);
    }
    /// <summary>
    /// Show player's attack options case of it's player's turn
    /// </summary>
    public void SetModePanel()
    {
        IsAttacker = StartHandTurn;

        //Debug.Log("FALSE");
        PVPManager.manager.BetTextObj.text = "";
        PVPManager.manager.resultText.text = "";
        //ModePanel.SetActive(true);
        skipTurn.gameObject.SetActive(false);
        waitPanel.SetActive(false);
        state = PVPStates.AttackLoc;
        //LocationChoices.SetActive(true);
        //ModeTxT.text = "Choose Attack Location";

        Debug.Log("=========================================== set mode panel " + Game.Get()._currnetTurnPlayer);
        SpellManager.PetAlreadyAttacked = false;

        if (!StartHandTurn)
        {

            waitPanel.SetActive(true);
            //  Debug.LogError("=========================================== set mode panel false ===========================================");

            DemoManager.instance._pokerButtons.SetActive(false);
            PokerButtonManager.instance.bet_attack.gameObject.SetActive(true);
            PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(true);
            PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(true);
            PokerButtonManager.instance.call_Engauge.gameObject.SetActive(true);
            PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(true);
            IsPetTurn = false;
        }
        else
        {

            //  Debug.LogError("=========================================== set mode panel true ===========================================");
            StartTimer();
            EndTurnBtn.gameObject.SetActive(true);
            DemoManager.instance._pokerButtons.SetActive(true);
            if (P1RageBar.value > 50)
                SpecialAttackButton.SetActive(true);
            PokerButtonManager.instance.bet_attack.gameObject.SetActive(true);
            PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
            PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(true);
            PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
            PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);
            PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(false);
            IsPetTurn = true;


        }

        foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
        {
            //Debug.Log("FALSE "+item.name);
            item.gameObject.SetActive(true);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (item.data.StaCost > (int)P1StaBar.value)
                {
                    item.button.interactable = false;
                    Debug.Log("FALSE");
                }
                else
                {
                    item.button.interactable = true;
                }
            }
            else
            {
                if (item.data.StaCost > (int)P2StaBar.value)
                {
                    item.button.interactable = false;
                    Debug.Log("FALSE");
                }
                else
                {
                    item.button.interactable = true;
                }
            }

        }
    }

    /// <summary>
    /// Check win method to hande win/lose event in poker game
    /// </summary>
    /// <param name="delay">delay - waiting time before exceuting this function</param>
    /// <returns></returns>
    //[PunRPC]
    public IEnumerator CheckWinNew(float delay = 10f)
    {
        // P1StaBar.value += StaGainedPerMatch;
        // P2StaBar.value += StaGainedPerMatch;
        yield return new WaitForSeconds(delay); //3f


        bool PVPOver = false;
        bool player1Win = false;
        ChoiceDetails.SetActive(false);


        if (P1HealthBar.value <= 0)
        {
            Debug.LogError("1 loss");
            player2.SetActive(false);
            player1.SetActive(false);
            //winTxt.text = PhotonNetwork.PlayerList[1].NickName + " Wins !";
            //winIm.sprite = p2Char.ChracterSp;
            //winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = false;
            if (PVPOver)
            {

                yield return new WaitForSeconds(1.5f);
                myObj.pData.speed = p1Speed;
                opponentObj.pData.speed = p2Speed;
                //Debug.LogError("Saving HP : 1 - "+P1HealthBar.value+" 2 - "+P2HealthBar.value);
                myObj.pData.health = (int)P1HealthBar.value;
                opponentObj.pData.health = (int)P2HealthBar.value;
                Game.Get().MyStamina = (int)P1StaVal;
                Game.Get().OppoStamina = (int)P2StaVal;
                //myObj.pData.stamina = P1StaVal;
                //opponentObj.pData.stamina = P2StaVal;
                photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
                PhotonNetwork.SendAllOutgoingCommands();
                // Invoke("ResetData",2f);
                yield return new WaitForSeconds(.5f);

                //DemoManager.instance.ResetnumCards();
                ResetData(true);
                //Invoke("SetModePanel",3f);
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    if (isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient)
                    { Game.Get().HandleWin(player1Win, isattackerMaster, p1Pos, p2Pos); }
                    else
                    { Game.Get().HandleWin(!player1Win, isattackerMaster, p1Pos, p2Pos); }

                    //Debug.LogError("P1 lose");
                }
            }


        }
        else if (P2HealthBar.value <= 0)
        {

            Debug.LogError("2 loss");
            player2.SetActive(false);
            player1.SetActive(false);
            //winTxt.text = PhotonNetwork.PlayerList[0].NickName + " Wins !";
            //winIm.sprite = p1Char.ChracterSp;
            //winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = true;
            //if(isattackerMaster)
            //
            if (PVPOver)
            {
                yield return new WaitForSeconds(1.5f);
                myObj.pData.speed = p1Speed;
                opponentObj.pData.speed = p2Speed;
                //Debug.LogError("Saving HP : 1 - "+P1HealthBar.value+" 2 - "+P2HealthBar.value);
                myObj.pData.health = (int)P1HealthBar.value;
                opponentObj.pData.health = (int)P2HealthBar.value;
                Game.Get().MyStamina = (int)P1StaVal;
                Game.Get().OppoStamina = (int)P2StaVal;
                //myObj.pData.stamina = P1StaVal;
                //opponentObj.pData.stamina = P2StaVal;
                photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
                PhotonNetwork.SendAllOutgoingCommands();
                // Invoke("ResetData",2f);
                yield return new WaitForSeconds(.5f);

                //DemoManager.instance.ResetnumCards();
                ResetData(true);
                //Invoke("SetModePanel",3f);
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    if (isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient)
                    { Game.Get().HandleWin(player1Win, isattackerMaster, p1Pos, p2Pos); }
                    else
                    { Game.Get().HandleWin(!player1Win, isattackerMaster, p1Pos, p2Pos); }

                    //Debug.LogError("P1 Win");
                }
            }
            //
        }
        else
        {
            Debug.LogError("Both player has health loss");
            //DemoManager.instance.ResetnumCards();
            ResetData();

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                SwitchStartHandTurn(0f);

            yield return new WaitForSeconds(1f);
        }
    }

    public bool isCheckWithoutReset;  //Used for waiting/hodling execution while  poker turn  events are executing
    public bool PVPOver = false;      //True- In case if poker game is over
    /// <summary>
    /// Win/Lose condition handle without resetting data
    /// </summary>
    /// <param name="delay">Delay before starting execution of this method</param>
    /// <returns></returns>
    public IEnumerator CheckWinNewWithoutReset(float delay = 10f)
    {

        // P1StaBar.value += StaGainedPerMatch;
        // P2StaBar.value += StaGainedPerMatch;
        yield return new WaitForSeconds(delay); //3f
                                                // Debug.LogError("Is All in " + IsAnyAllIn);


        bool player1Win = false;
        ChoiceDetails.SetActive(false);

        // Debug.LogError(P1HealthBar.value + " - " + MyLastAttackAmount);
        //  Debug.LogError(P2HealthBar.value + " - " + P2LastAttackValue);
        if (P1HealthBar.value <= 0 && MyLastAttackAmount <= 0)
        {
            StopTimer();
            Debug.LogError("PetAttackWin If");
            player2.SetActive(false);
            player1.SetActive(false);
            //winTxt.text = PhotonNetwork.PlayerList[1].NickName + " Wins !";
            //winIm.sprite = p2Char.ChracterSp;
            //winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = false;
            P2RemainingHandHealth = P2StartHealth;
            P2HealthBar.value = P2RemainingHandHealth;
            if (PVPOver)
            {

                yield return new WaitForSeconds(1.5f);
                myObj.pData.speed = p1Speed;
                opponentObj.pData.speed = p2Speed;
                //Debug.LogError("Saving HP : 1 - "+P1HealthBar.value+" 2 - "+P2HealthBar.value);
                myObj.pData.health = (int)P1HealthBar.value;
                opponentObj.pData.health = (int)P2HealthBar.value;
                Game.Get().MyStamina = (int)P1StaVal;
                Game.Get().OppoStamina = (int)P2StaVal;
                //myObj.pData.stamina = P1StaVal;
                //opponentObj.pData.stamina = P2StaVal;
                photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
                PhotonNetwork.SendAllOutgoingCommands();
                // Invoke("ResetData",2f);
                yield return new WaitForSeconds(.5f);

                //DemoManager.instance.ResetnumCards();
                ResetData(true);
                //Invoke("SetModePanel",3f);
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    if (isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient)
                    { Game.Get().HandleWin(player1Win, isattackerMaster, p1Pos, p2Pos); }
                    else
                    { Game.Get().HandleWin(!player1Win, isattackerMaster, p1Pos, p2Pos); }

                    //  Debug.LogError("P1 lose");
                }
            }


        }
        else if (P2HealthBar.value <= 0 && P2LastAttackValue <= 0)
        {
            Debug.LogError("PetAttackWin Else");
            StopTimer();
            player2.SetActive(false);
            player1.SetActive(false);
            //winTxt.text = PhotonNetwork.PlayerList[0].NickName + " Wins !";
            //winIm.sprite = p1Char.ChracterSp;
            //winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = true;
            //if(isattackerMaster)
            //
            P1RemainingHandHealth = P1StartHealth;
            P1HealthBar.value = P1RemainingHandHealth;
            if (PVPOver)
            {
                yield return new WaitForSeconds(1.5f);
                myObj.pData.speed = p1Speed;
                opponentObj.pData.speed = p2Speed;
                //Debug.LogError("Saving HP : 1 - "+P1HealthBar.value+" 2 - "+P2HealthBar.value);
                myObj.pData.health = (int)P1HealthBar.value;
                opponentObj.pData.health = (int)P2HealthBar.value;
                Game.Get().MyStamina = (int)P1StaVal;
                Game.Get().OppoStamina = (int)P2StaVal;
                //myObj.pData.stamina = P1StaVal;
                //opponentObj.pData.stamina = P2StaVal;
                photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
                PhotonNetwork.SendAllOutgoingCommands();
                // Invoke("ResetData",2f);
                yield return new WaitForSeconds(.5f);

                //DemoManager.instance.ResetnumCards();
                ResetData(true);
                //Invoke("SetModePanel",3f);
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    if (isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient)
                    { Game.Get().HandleWin(player1Win, isattackerMaster, p1Pos, p2Pos); }
                    else
                    { Game.Get().HandleWin(!player1Win, isattackerMaster, p1Pos, p2Pos); }

                    //Debug.LogError("P1 Win");
                }

            }
            //
        }
        //Debug.LogError("pet checked");

        isCheckWithoutReset = false;

    }


    public Chessman myObj = null, opponentObj = null;  //Player and Oppoent cheess piece object
    CharacterData myChar = null, opponentChar = null;  //Player and Oppoent character data

    /// <summary>
    /// Get script instance
    /// </summary>
    /// <returns></returns>
    public static PVPManager Get()
    {
        return manager;
    }
    /// <summary>
    /// Set values for poeker game and update UI with respect to values for poker game
    /// </summary>
    /// <param name="posP1">player position in chessgame/></param>
    /// <param name="posP2">opponent position in chessgame</param>
    /// <param name="isReverse">Ture- for Black chess player type</param>
    [PunRPC]
    public void SetDataRPC(Vector2 posP1, Vector2 posP2, bool isReverse)
    {
        updatePlayerAction("");
        myFoldAmount = 2;
        UpdateBetForPlayer(0);
        playerChoice.ExtraAttack = new List<AttackType>();
        winTxt.gameObject.SetActive(false);
        player1.SetActive(true);
        player2.SetActive(true);


        p1Pos = posP1;
        p2Pos = posP2;

        p1Obj = Game.Get().GetPosition((int)posP1.x, (int)posP1.y).GetComponent<Chessman>();
        p2Obj = Game.Get().GetPosition((int)posP2.x, (int)posP2.y).GetComponent<Chessman>();

        if (isReverse && Game.Get().IsDefender)
        {
            myPiece = p1Obj.type;
            opponentpiece = p2Obj.type;
            myObj = p1Obj;
            opponentObj = p2Obj;
        }
        else if (isReverse && !Game.Get().IsDefender)
        {
            myPiece = p2Obj.type;
            opponentpiece = p1Obj.type;
            myObj = p2Obj;
            opponentObj = p1Obj;
        }
        else if (!isReverse && Game.Get().IsDefender)
        {
            myPiece = p2Obj.type;
            opponentpiece = p1Obj.type;
            myObj = p2Obj;
            opponentObj = p1Obj;
        }
        else if (!isReverse && !Game.Get().IsDefender)
        {
            myPiece = p1Obj.type;
            opponentpiece = p2Obj.type;
            myObj = p1Obj;
            opponentObj = p2Obj;
        }
        IsPetTurn = Game.Get().isMyTurn(myObj.player);
        isLocalPVPFirstTurn = Game.Get().isMyTurn(myObj.player);
        isLocalPVPTurn = !Game.Get().isMyTurn(myObj.player);

        StartHandTurn = !isLocalPVPTurn;
        // Debug.LogError("this is start turn : "+StartHandTurn);
        IsAttacker = StartHandTurn;
        SetChessSpriteForPVP();

        p1Char = myObj.character;
        p2Char = opponentObj.character;
        P1HealthBar.maxValue = p1Char.health;
        P2HealthBar.maxValue = p2Char.health;


        if (!myObj.AlreadyPlayedPvP)
        {
            P1RemainingHandHealth = p1Char.health;
            P1HealthBar.value = p1Char.health;
            P1StartHealth = p1Char.health;
            myObj.AlreadyPlayedPvP = true;

            P1StaVal = 10;
        }
        else
        {
            P1RemainingHandHealth = myObj.pData.health;
            P1HealthBar.value = myObj.pData.health;
            P1StartHealth = myObj.pData.health;

        }
        P1StaVal = Game.Get().MyStamina;

        if (!opponentObj.AlreadyPlayedPvP)
        {
            P2RemainingHandHealth = p2Char.health;
            P2HealthBar.value = p2Char.health;
            P2StartHealth = p2Char.health;
            opponentObj.AlreadyPlayedPvP = true;
            P2StaVal = 10;
        }
        else
        {
            P2RemainingHandHealth = opponentObj.pData.health;
            P2HealthBar.value = opponentObj.pData.health;
            P2StartHealth = opponentObj.pData.health;


        }
        P2StaVal = Game.Get().OppoStamina;


        Debug.LogError(P1StaVal + " ______________________________");
        //Debug.LogError("Set hp : 1 - " + P1HealthBar.value + " , 2 - " + P2HealthBar.value);


        P1StaBar.maxValue = 10;
        P2StaBar.maxValue = 10;
        P1StaBar.value = P1StaVal;
        P2StaBar.value = P2StaVal;

        MaxManaBarVal = -1;
        MyManabarVal = MaxManaBarVal;
        OppoManabarVal = MaxManaBarVal;
        MyManaBar.maxValue = MaxManaBarVal;
        OppoManaBar.maxValue = MaxManaBarVal;
        UpdateManaTxt();


        P1RageBar.maxValue = 100;
        P2RageBar.maxValue = 100;
        P1RageBar.value = 0;
        P2RageBar.value = 0;

        //Debug.LogError("Got Speed : " + p1Speed + " - " + p2Speed);
        p1Speed = 0; // myObj.pData.speed
        p2Speed = 0; //opponentObj.pData.speed


        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString("F2");
        photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);
        PhotonNetwork.SendAllOutgoingCommands();

        //Debug.LogError("Loading data myObj : " + myObj.left);
        isAttackedHigh = 0;
        isAttackedLow = 0;
        isAttackedOnRightSide = 0;
        isAttackedOnLeftSide = 0;
        isAttackedInMiddle = 0;

        isAttackedHighOpponent = 0;
        isAttackedLowOpponent = 0;
        isAttackedOnRightSideOpponent = 0;
        isAttackedOnLeftSideOpponent = 0;
        isAttackedInMiddle = 0;

        myExtraDamagePercentage = 0;//isAttackedHighOpponent * 0.05f
        my_opponentAttackMadeWeakerPerntage = 0;//Mathf.Max(isAttackedOnLeftSideOpponent, isAttackedOnRightSideOpponent) * 0.05f
        my_OpponetSpeedMakeSlowerPercentage = 0;//isAttackedLowOpponent * 0.05f
        my_opponentStaminaLessRecovertPerncetage = 0;//isAttackedInMiddleOpponent * 0.1f



        myExtraDamagePercentageTxt.text = (myExtraDamagePercentage * 100).ToString() + " %"; my_opponentAttackMadeWeakerPerntageTxt.text = (my_OpponetSpeedMakeSlowerPercentage * 100).ToString() + " %"; my_opponentStaminaLessRecovertPerncetageTxt.text = (my_opponentStaminaLessRecovertPerncetage * 100).ToString() + " %"; myOpponentAttackSpeedSlowTxt.text = (my_opponentAttackMadeWeakerPerntage * 100).ToString() + " %";//
                                                                                                                                                                                                                                                                                                                                                                                                                                   //photonView.RPC("UpdateOpponentEffects_RPC",RpcTarget.Others,myExtraDamagePercentage,my_opponentAttackMadeWeakerPerntage,my_opponentStaminaLessRecovertPerncetage,my_OpponetSpeedMakeSlowerPercentage,my_OpponetSpeedMakeSlowerPercentage > 0);

        // UpdateSpeed();

        myChar = myObj.character;
        opponentChar = opponentObj.character;

        if (myObj.playerType == PlayerType.Black)
        {
            p1Image.sprite = myChar.ChracterOppSp;
            p2Image.sprite = opponentChar.ChracterSp;
        }
        else
        {
            p1Image.sprite = myChar.ChracterSp;
            p2Image.sprite = opponentChar.ChracterOppSp;
        }

        if (isLocalPVPTurn)
        {
            Debug.Log(" == set mode panel here == ");
            //Game.Get().loadingScreen.SetActive(true);
            StartCoroutine(Game.Get().SetLoadingScreenOnOff(false, 1f));

            //SpellManager.instance.RemoveOldSpellData();

        }
        else
        {
            // ModePanel.SetActive(false);
            // Game.Get().loadingScreen.SetActive(true);
            StartCoroutine(Game.Get().SetLoadingScreenOnOff(false, 1f));
            //SpellManager.instance.RemoveOldSpellData();
            //  SpellManager.instance.GenerateCardsForPlayer();
            waitPanel.SetActive(true);
        }
        SetModePanel();

        UpdateHMTxt();

        ShowWeaknessFactor();

        AddMana();

        SpellManager.instance.spellCardsDeck = new List<SpellCard>();
        if (myPiece != PieceType.Pawn)
        {
            foreach (var item in myObj.cards)
            {
                SpellManager.instance.spellCardsDeck.Add(item);
            }
            //   Debug.LogError(SpellManager.instance.spellCardsDeck.Count + " cards added");

            SpellManager.instance.ResetData();
        }
    }
    /// <summary>
    /// Spawn pets 
    /// </summary>
    public IEnumerator SpawnPets()
    {

        for (int i = 0; i < startNumCards; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpellManager.instance.DrawCard();
        }
    }
    /// <summary>
    /// Show weakeness  object
    /// </summary>
    public void ShowWeaknessFactor()
    {
        p1Weakness.SetActive(false);
        p2Weakness.SetActive(false);
        CharacterType myCharWeakAgainst = CharacterType.Earth, opponent = CharacterType.Water;
        CharacterType OppCharWeakAgainst = CharacterType.Earth, Me = CharacterType.Water;

        if (Game.Get().IsDefender)
        {
            myCharWeakAgainst = p2Char.weakAgainst;
            opponent = p1Char.type;
            OppCharWeakAgainst = p1Char.weakAgainst;
            Me = p2Char.type;
        }
        else
        {
            myCharWeakAgainst = p1Char.weakAgainst;
            opponent = p2Char.type;
            OppCharWeakAgainst = p2Char.weakAgainst;
            Me = p1Char.type;
        }

        if (myCharWeakAgainst == opponent)
        {
            p1Weakness.SetActive(true);
            p1Weakness.GetComponent<Image>().color = Color.red;
            p1Weakness.GetComponentInChildren<TextMeshProUGUI>().text = "-20";

            p2Weakness.SetActive(true);
            p2Weakness.GetComponent<Image>().color = Color.green;
            p2Weakness.GetComponentInChildren<TextMeshProUGUI>().text = "+20";
        }

        if (OppCharWeakAgainst == Me)
        {
            p1Weakness.SetActive(true);
            p1Weakness.GetComponent<Image>().color = Color.green;
            p1Weakness.GetComponentInChildren<TextMeshProUGUI>().text = "+20";

            p2Weakness.SetActive(true);
            p2Weakness.GetComponent<Image>().color = Color.red;
            p2Weakness.GetComponentInChildren<TextMeshProUGUI>().text = "-20";
        }
    }
    /// <summary>
    ///  Triggers Projectile damage to oppoent player
    /// </summary>
    /// <param name="c">damage amount</param>
    public void DealDamageToOpponent(int c)
    {

        photonView.RPC("DealDamage", RpcTarget.Others, c);
        PhotonNetwork.SendAllOutgoingCommands();
        P2RemainingHandHealth -= c;
        P2StartHealth -= c;
        ShowDealtDamage(c);
        if (P2RemainingHandHealth > P1StartHealth) P2RemainingHandHealth = P2StartHealth;
        if (P2RemainingHandHealth < 0) P2RemainingHandHealth = 0;
        P2HealthBar.value = P2RemainingHandHealth;
        //photonView.RPC("SetOpponentRemainingHandHealth_RPCPlus",RpcTarget.Others,c);
        PhotonNetwork.SendAllOutgoingCommands();
        //Update Values in loser
        //P2StartHealth = (int)P2HealthBar.value;
        UpdateHMTxt();
        if (PVPManager.Get().P2RemainingHandHealth <= 0)
        {
            PVPManager.manager.isAllIn = true;
            PVPManager.manager.IsAnyAllIn = true;
            PVPManager.manager.SyncAllIn(true);
            PVPManager.manager.isFromInbetween = true;
            PVPManager.manager.isNormalBat = true;
            EndTurnBtn.gameObject.SetActive(true);
            LocationChoices.SetActive(false);
            AttackChoices.SetActive(false);
            //// DemoManager.instance._pokerButtons.SetActive(false);
            //// StartTimer();
        }

    }
    /// <summary>
    /// Show damage dealt by player
    /// </summary>
    public void ShowDealtDamage(int c, bool isLocal = true)
    {
        GameObject o = Instantiate(DmgPref, isLocal ? p2Image.transform : p1Image.transform);
        o.GetComponent<DamageIndicator>().DmgText.text = "-" + c;
        Vector3 startPos = Vector3.zero;
        Vector3 midPos1 = new Vector3(0.5f, 30f / 4f, 0f);
        Vector3 midPos2 = new Vector3(0.5f, (30f / 4f) * 3f, 0f);
        Vector3 endPos = new Vector3(0f, 30f, 0f);
        o.LeanMoveLocal(new LTBezierPath(new Vector3[] { startPos, midPos1, midPos2, endPos }), 0.5f).setOnComplete(() =>
    {
        Destroy(o);
    });

    }
    /// <summary>
    /// RPC- Update healt for dealing damage in network devices
    /// </summary>
    /// <param name="c"></param>
    [PunRPC]
    public void DealDamage(int c)
    {
        P1RemainingHandHealth -= c;
        P1StartHealth -= c;
        ShowDealtDamage(c, false);
        if (P1RemainingHandHealth > P1StartHealth) P1RemainingHandHealth = P1StartHealth;
        if (P1RemainingHandHealth < 0) P1RemainingHandHealth = 0;
        P1HealthBar.value = P1RemainingHandHealth;
        //photonView.RPC("SetOpponentRemainingHandHealth_RPCPlus",RpcTarget.Others,c);
        PhotonNetwork.SendAllOutgoingCommands();
        //Update Values in loser
        //P1StartHealth = (int)P1HealthBar.value;
        UpdateHMTxt();
        //StartCoroutine(CheckWinNewWithoutReset(0.1f));
    }
    /// <summary>
    /// Update speed text
    /// </summary>
    public void UpdateSpeed()
    {
        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString("F2");
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString("F2");
    }

    /// <summary>
    /// RPC- Toggle local player turn boolean 
    /// </summary>
    [PunRPC]
    public void SwitchPVPTurn()
    {
        isLocalPVPTurn = !isLocalPVPTurn;
        //IsPetTurn = isLocalPVPTurn;
        //SpellManager.PetAlreadyAttacked = false;
    }
    /// <summary>
    /// RPC- Set local player turn boolean value
    /// </summary>
    /// <param name="b">value</param>
    [PunRPC]
    public void SetPVPTurn(bool b)
    {
        isLocalPVPTurn = b;
    }
    /// <summary>
    /// Restart poker after fold
    /// </summary>
    public void RestartAfterFold()
    {
        photonView.RPC("ResetPVPUIData", RpcTarget.All, false);
        Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
        photonView.RPC("RPC_ResetTurn", RpcTarget.All);

        Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
        if (PhotonNetwork.IsMasterClient)
        {
            DemoManager.instance.SecondTimeShuffleCall();
        }

        SpellManager.instance.DrawCard();

        AddMana();

        //Game.Get().NextTurn();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            SwitchStartHandTurn(3f);

        //Invoke("SetModePanel", 3f);
    }

    /// <summary>
    /// Generate oppoent player cards from dack
    /// </summary>
    public void GenerateOpponentPlayerCards()
    {
        //Debug.LogError(deckFullList.Length);

        for (int j = 0; j < opponentCardValue.Length; j++)
        {
            for (int i = 0; i < deckFullList.Length; i++)
            {
                if ((int)deckFullList[i].cardValue == opponentCardValue[j] && (int)deckFullList[i].cardColor == opponentCardColor[j])
                {
                    //Debug.LogError("MATCH");
                    opponentCardSprite.Add(deckFullList[i].cardSprite);
                    var cardObj = DemoManager.instance.InstantiateCardOpponent(deckFullList[i], OpponentPlayerCardPositions[j].transform.position, OpponentPlayerCardPositions[j].transform, i);

                    //set this card to player poisition
                    cardObj.GetComponent<RectTransform>().parent = OpponentPlayerCardPositions[j].transform;
                    cardObj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30 * j, 0, 0);
                    cardObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                    cardObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                    cardObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);

                    cardObj.GetComponent<RectTransform>().localRotation = Quaternion.EulerRotation(0, 0, 0);
                    // cardObj.gameObject.SetActive(false);

                }
            }
        }

    }
    /// <summary>
    /// Update attack location
    /// </summary>
    /// <param name="location">location type</param>
    public void UpdateAttackLocation(AttackLocation location)
    {
        playerAttackLocation = location;
        photonView.RPC("UpdateOpponentAttackLocation_RPC", RpcTarget.Others, location);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- Update location
    /// </summary>   
    [PunRPC]
    public void UpdateOpponentAttackLocation_RPC(AttackLocation location)
    {
        opponentAttackLocation = location;
    }
    /// <summary>
    /// Update defence location
    /// </summary>
    /// <param name="location">location type</param>
    public void UpdateDefenceLocation(AttackLocation location)
    {
        playerDefenceLocation = location;
        photonView.RPC("UpdateOpponentDefenceLocation_RPC", RpcTarget.Others, location);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC- Defence location update on network 
    /// </summary>
    [PunRPC]
    public void UpdateOpponentDefenceLocation_RPC(AttackLocation location)
    {
        opponentDefendLocation = location;
    }
    /// <summary>
    /// Get attack location type from int value
    /// </summary>
    /// <param name="val">Interge location index</param>
    /// <returns>Attack type</returns>
    public AttackLocation GetAttackLocation(int val)
    {
        AttackLocation location = AttackLocation.None;
        switch (val)
        {
            case -1:
                location = AttackLocation.None;
                break;
            case 0:
                location = AttackLocation.High;
                break;
            case 1:
                location = AttackLocation.Low;
                break;
            case 2:
                location = AttackLocation.Left;
                break;
            case 3:
                location = AttackLocation.Right;
                break;
            case 4:
                location = AttackLocation.Middle;
                break;
            default:
                break;
        }
        return location;
    }
    /// <summary>
    /// Trigges reset data in Master client thorug RPC
    /// </summary>
    public void ReloadScene()
    {
        photonView.RPC("ReloadScene_RPC", RpcTarget.All);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    /// <summary>
    /// RPC reset data
    /// </summary>
    [PunRPC]
    public void ReloadScene_RPC()
    {
        photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
        PhotonNetwork.SendAllOutgoingCommands();
        if (PhotonNetwork.IsMasterClient)
            ResetData();
    }
    public Image myPieceImg, oppPiece;  //Player and Opponent chess piece image reference in Poker game screen
    /// <summary>
    /// Sets piece sprites in poker game for player and oppoent
    /// </summary>
    public void SetChessSpriteForPVP()
    {

        string player = "";
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient) //if(PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer) 
        {
            player = "white";

        }
        else
        {
            player = "black";
        }


        Debug.Log("METHOD CALLED ***** " + player);
        PlayerType mytype = PlayerType.Black;//= PlayerType.Black = player ? "black" : "white";
        PlayerType oppType = PlayerType.White;
        if (player == "black")
        {
            mytype = PlayerType.Black;
            oppType = PlayerType.White;
        }
        else if (player == "white")
        {
            mytype = PlayerType.White;
            oppType = PlayerType.Black;
        }
        Debug.Log(mytype + " ------ " + oppType + " ==MY PIECE " + myPiece + " -OPP PIECE " + opponentpiece);
        PlayerType playerType = mytype;

        switch (myPiece)
        {
            case PieceType.Queen: myPieceImg.sprite = playerType == PlayerType.Black ? black_queen : white_queen; break;
            case PieceType.Knight: myPieceImg.sprite = playerType == PlayerType.Black ? black_knight : white_knight; break;
            case PieceType.Bishop: myPieceImg.sprite = playerType == PlayerType.Black ? black_bishop : white_bishop; break;
            case PieceType.King: myPieceImg.sprite = playerType == PlayerType.Black ? black_king : white_king; break;
            case PieceType.Rook: myPieceImg.sprite = playerType == PlayerType.Black ? black_rook : white_rook; break;
            case PieceType.Pawn: myPieceImg.sprite = playerType == PlayerType.Black ? black_pawn : white_pawn; break;
        }

        switch (opponentpiece)
        {

            case PieceType.Queen: oppPiece.sprite = oppType == PlayerType.Black ? black_queen : white_queen; break;
            case PieceType.Knight: oppPiece.sprite = oppType == PlayerType.Black ? black_knight : white_knight; break;
            case PieceType.Bishop: oppPiece.sprite = oppType == PlayerType.Black ? black_bishop : white_bishop; break;
            case PieceType.King: oppPiece.sprite = oppType == PlayerType.Black ? black_king : white_king; break;
            case PieceType.Rook: oppPiece.sprite = oppType == PlayerType.Black ? black_rook : white_rook; break;
            case PieceType.Pawn: oppPiece.sprite = oppType == PlayerType.Black ? black_pawn : white_pawn; break;
        }

    }
    /// <summary>
    /// Shows popup incase player's stamina is low - Due to low stamina player can not attack wit respect to it's healt points in poker
    /// </summary>
    public void ShowLowStaminaPopup()
    {
        LeanTween.scale(P1StaminPopup, Vector3.zero, 0f);
        P1StaminPopup.SetActive(true);
        LeanTween.scale(P1StaminPopup, Vector3.one, 0.5f);

        Invoke(nameof(HideLowStamina), 2f);
    }
    /// <summary>
    /// Hide low stamina popup
    /// </summary>
    public void HideLowStamina()
    {
        P1StaminPopup.SetActive(false);
        LeanTween.scale(P1StaminPopup, Vector3.zero, 0f);
    }
}
