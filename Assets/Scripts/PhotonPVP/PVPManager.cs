using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;
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

public struct PlayerChoice
{
    public AttackLocation attackLoc;
    public AttackType attack;
    public List<AttackType> ExtraAttack;
    public AttackLocation defendLoc;

}

public class PVPManager : MonoBehaviour
{

    public PhotonView photonView;
    public static PVPManager manager;
    public GameObject player1, player2;
    public Image p1Image, p2Image;
    public Image p1Outline, p2Outline, chessTurnIndicator;

    public Text[] p1Name, p2Name;

    public CharacterData p1Char, p2Char;

    public GameObject ModePanel;
    public GameObject waitPanel;
    public Text ModeTxT;

    public PVPStates state;

    public bool isLocalPVPTurn;
    public bool isLocalPVPFirstTurn = false;

    public PlayerChoice player1Choice, player2Choice, playerChoice;
    public List<PlayerChoice> player1ExtraChoices, player2ExtraChoices;
    public List<int> opponetHandCardValues;
    public List<int> opponetHandCardColors;
    public int StaGainedPerMatch = 10;



    public GameObject ChoiceDetails;
    public Text[] player1ChoiceTxt, player2ChoiceTxt;

    public Slider P1HealthBar, P2HealthBar;
    public Slider P1StaBar, P2StaBar;

    public float P1StaVal, P2StaVal;

    public Slider P1RageBar, P2RageBar;
    public Text P1SpeedTxt, P2SpeedTxt;

    public float p1Speed, p2Speed;

    public int P1HeavyComboIndex, P2HeavyComboIndex;
    public int P1SpeedComboIndex, P2SpeedComboIndex;
    public int P1RemainingHandHealth, P2RemainingHandHealth, P2LastAttackValue = 0;
    public int p1bar { get { return P1RemainingHandHealth; } set { P1RemainingHandHealth = value; } }
    public Text winTxt;
    public Image winIm;

    public Vector2 p1Pos, p2Pos;
    public Chessman p1Obj, p2Obj;
    private float lowestSpeedNeeded = 1.5f;



    bool isattackerMaster;

    public Button skipTurn;
    public GameObject LocationChoices, AttackChoices, LocationChoiceHeading;

    public Text P1StaTxt, P2StaTxt, P1HealthTxt, P2HealthTxt, P1RageTxt, P2RageTxt;
    public Text MyManaBarTxt, OppoManaBarTxt;

    public Text P1ExtraDamageAnimationText, P2ExtraDamageAnimationText;

    public List<GameObject> _playersNameInPvP = new List<GameObject>();
    public float BatPoints = 0;
    public float p1SliderAttack, p2SliderAttack;
    public Button EndTurnBtn;
    public Text BetTextObj, EndTurnTimerText, ChessTurnTimerText;
    int choice = -1;
    public GameObject PlayerCards, BoardCards, OpponetPlayerCards;
    public int rangeCounter = 0;
    public SliderAttack lastAttackType = SliderAttack.nun;
    public int PlayerChoiceOnce = -1, OpponentChoiceOne = -1, OpponentRangCounter = 0;
    public int[] opponentCardColor = new int[2] { -1, -1 };
    public int[] opponentCardValue = new int[2] { -1, -1 };
    public List<Sprite> opponentCardSprite = new List<Sprite>();
    public Card_SO[] deckFullList;
    public List<RectTransform> OpponentPlayerCardPositions;
    public GameObject BoardCardParent;

    public AttackSlider attackSlider;
    public int RagePointReward = 20;
    public AttackLocation opponentAttackLocation, opponentDefendLocation, playerAttackLocation, playerDefenceLocation;
    public bool isAttackLocationSelected = false, isDefenceLocationSelected = false;
    public int MyLastAttackAmount = 0;
    public int P1StartHealth = 100, P2StartHealth = 100;
    public int myLocalBatAmount = 0;
    public Text resultText;
    public Text LocationChoiceText;
    public GameObject choiceConfPopup;
    public int EndTurnTimer = 5, ChessTurnTimer = 30;
    public int OriginalTimerVal = 30, OriginalChessTimerValue = 30;
    public GameObject moveChoiceConfirmation;
    public GameObject TimerObject;
    public Text p1AttackFor, p2AttackFor;
    public int AttackFor = 0;
    public int OpponentBestIndex = 0;
    public GameObject extraDamageMessageP1, extraDamageMessageP2;
    public bool isAttackViaSpeedPoints = false;
    public float p1DamageCapacityReducedby = 0, p2DamageCapacityReducedby = 0, p1StaminRecoveryReducedBy = 0, p2StaminaRecovertReducedby = 0, p1DamageIncreasedby = 0, p2DamageIncreasedby = 0, p1SpeedSlowBy = 0, opponentAttackWeakPercentageVal = 0, p2SpeedSlowedBy;
    public float myExtraDamagePercentage, my_OpponetSpeedMakeSlowerPercentage, my_opponentAttackMadeWeakerPerntage, my_opponentStaminaLessRecovertPerncetage;
    public Text myExtraDamagePercentageTxt, my_opponentAttackMadeWeakerPerntageTxt, my_opponentStaminaLessRecovertPerncetageTxt, myOpponentAttackSpeedSlowTxt;
    public Text p2ExtraDamagePercentageTxt, p2_opponentAttackMadeWeakerPerntageTxt, p2_opponentStaminaLessRecovertPerncetageTxt, p2OpponentAttackSpeedSlowTxt;
    public GameObject p1SpeedSlowObj, p2SpeedSlowObj;

    public GameObject p1Weakness, p2Weakness;

    public int myFoldAmount = 4;
    public bool StartHandTurn;
    private void Awake()
    {
        manager = this;
    }

    public MovePlate selectedMove;

    public PieceType myPiece, opponentpiece, tempPiece, opponentPieceAttack, tempPieceOpp;

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;
    public GameObject speedAttackChoices, speedAttackButton;
    public Slider speedAttackSlider;
    public int isAttackedOnLeftSide = 0, isAttackedOnRightSide = 0, isAttackedInMiddle = 0, isAttackedHigh = 0, isAttackedLow = 0;
    public int isAttackedOnLeftSideOpponent = 0, isAttackedOnRightSideOpponent = 0, isAttackedInMiddleOpponent = 0, isAttackedHighOpponent = 0, isAttackedLowOpponent = 0;
    public TextMeshProUGUI debug;

    public Slider MyManaBar, OppoManaBar;

    public int MaxManaBarVal;
    public int MyManabarVal;
    public int OppoManabarVal;
    public GameObject DmgPref;
    public int startNumCards = 3;
    public GameObject SpecialAttackButton;

    public bool IsAttacker;

    public bool IsPetTurn = false;

    public TextMeshProUGUI player1Bet, player2Bet;
    public float player1BetAmt, player2BetAmt;

    public Image P1Shield, P2Shield;
    
    public TextMeshProUGUI P1LastAction, P2LastAction;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        //P1RemainingHandHealth =(int) P1HealthBar.maxValue;
        //P2RemainingHandHealth = (int)P2HealthBar.maxValue;

        int a = 2;
        a *= 2;
        Debug.Log("a is " + a);
        Debug.Log("<color=yellow> pvp manager start  </color>");
        //UpdateHMTxt();


    }

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
    bool timerStarted = false;
    public IEnumerator UpdateChessTurnTimer()
    {
        timerStarted = true;
        //Debug.LogError("TICK");
        ChessTurnTimerText.text =

        ChessTurnTimer.ToString();

        //while(EndTurnTimer > 0) 
        //{



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
        // if(ChessTurnTimer < 0)
        // {
        //     ChessTurnTimer = 0;
        // }
        // ChessTurnTimerText.text = ChessTurnTimer.ToString();
        // //}
        // if(ChessTurnTimer <= 0)
        // {
        //     if(!endChessTurn)
        //     {
        //         if(Game.Get().ChessCanvas.activeSelf) 
        //         {
        //             Game.Get().NextTurn();
        //         }
        //         StopCoroutine(UpdateChessTurnTimer());
        //         ChessTurnTimer = 30;
        //         timerStarted = false;
        //     }
        // }
        // else
        // {
        //     if(!endChessTurn)
        //     {
        //         StartCoroutine(UpdateChessTurnTimer());
        //     }
        // }


    }

    public float timer;
    bool starttimer;

    public float ChessTimer;
    bool startChesstimer;

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
    public IEnumerator UpdateEndTurnTimer()
    {

        //Debug.LogError("TICK");
        // EndTurnTimerText.text = EndTurnTimer.ToString();

        // //while(EndTurnTimer > 0) 
        // //{
        //     yield return new WaitForSeconds(1);
        //     EndTurnTimer -= 1;
        //     if(EndTurnTimer < 0)
        //     {
        //         EndTurnTimer = 0;
        //     }
        // EndTurnTimerText.text = EndTurnTimer.ToString();
        // //}
        // if(EndTurnTimer <= 0)
        // {
        //     if(!endTurn)
        //     {
        //         OnClickEndTurn();
        //         StopCoroutine(UpdateEndTurnTimer());
        //         EndTurnTimer = 30;
        //     }
        // }
        // else 
        // {
        //     if(!endTurn)
        //     { StartCoroutine(UpdateEndTurnTimer()); }
        // }

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
                    // if(Game.Get().ChessCanvas.activeSelf) 
                    // {
                    //     Game.Get().NextTurn();
                    // }
                    //StopCoroutine(UpdateChessTurnTimer());

                    EndTurnTimer = 30;
                    endTurnTimeStarted = false;
                    yield break;
                }
            }
        }



    }

    public void StartTimer()
    {
        if (endTurnTimeStarted)
            return;
        endTurn = false;
        EndTurnTimer = OriginalTimerVal;
        endTurnTimeStarted = true;
        timer = 30f;
        starttimer = true;
        // StopCoroutine(UpdateEndTurnTimer());
        // //Debug.LogError("Timer STARTED FROM HERE");
        // StartCoroutine(UpdateEndTurnTimer());

    }

    public void ChessMoveConfirmed()
    {
        if (selectedMove != null)
        {

            Debug.Log("SELECTED MOVE TYPE " + selectedMove.GetReference().type);
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
    private void SetLastPieceInfo(PieceType type, PieceType oppPiece)
    {
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            PVPManager.manager.myPiece = type;
            PVPManager.manager.opponentpiece = oppPiece;
            photonView.RPC("SetPieceType", RpcTarget.Others, type, oppPiece);
        }
    }
    public void ShowSpeedAttackSlider()
    {
        //AttackChoices.SetActive(false);
        //speedAttackChoices.SetActive(true);
    }
    public void SetOpponentAttackPieceInfo(PieceType type)
    {

        // opponentpiece = type;
        if (PhotonNetwork.LocalPlayer != Game.Get()._currnetTurnPlayer)
        {
            //  PVPManager.manager.myPiece = type;
            //  Debug.Log("OPP PIECe TYPE " + type + " This Player is Turn Player-" + (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer));


            //  photonView.RPC("SetOpponentAttackPiecePieceType",RpcTarget.Others,type);
        }
        if (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
            Debug.Log("OPP PIECe TYPE " + type + " This Player is Turn Player-" + (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer));


            PVPManager.manager.tempPieceOpp = type;

            //  photonView.RPC("SetOpponentAttackPiecePieceType",RpcTarget.Others,type);
        }
    }
    [PunRPC]
    public void SetPieceType(PieceType type, PieceType myType)
    {
        Debug.Log("MY TYPE " + myPiece);

        Debug.Log("OPP TYPE " + type);
        myPiece = myType;
        manager.opponentpiece = type;
    }
    [PunRPC]
    public void SetOpponentAttackPiecePieceType(PieceType type)
    {
        manager.myPiece = type;
        Debug.Log("OPP PIECe TYPE " + type + " This Player is Turn Player-" + (PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer));
    }
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



    }
    public void ResetSpeed()
    {
        LeanTween.scale(p1SpeedSlowObj.gameObject, Vector3.zero, 0.3f);
        LeanTween.scale(p2SpeedSlowObj.gameObject, Vector3.zero, 0.3f);
    }
    [PunRPC]
    public void SetOpponentAttackWeakPercentage_RPC(float val)
    {
        opponentAttackWeakPercentageVal = val;
    }
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

        //Debug.LogError("myObj updated : " + myObj.high);
    }
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
    public void ChessMoveRejected()
    {
        selectedMove = null;
        moveChoiceConfirmation.gameObject.SetActive(false);
    }

    bool endTurnTimeStarted = false;


    public void UpdateHMTxt()
    {

        P1HealthTxt.text = P1HealthBar.value + " / " + P1HealthBar.maxValue;
        P2HealthTxt.text = P2HealthBar.value + " / " + P2HealthBar.maxValue;
        P1StaTxt.text = "Stamina " + MathF.Round(P1StaVal, 2) + " / " + P1StaBar.maxValue;
        P2StaTxt.text = "Stamina " + MathF.Round(P2StaVal, 2) + " / " + P2StaBar.maxValue;
        P1RageTxt.text = "Rage " + P1RageBar.value.ToString();//+//" / "+P1RageBar.maxValue;
        P2RageTxt.text = "Rage " + P2RageBar.value.ToString();//+" / "+P2RageBar.maxValue;
        P1SpeedTxt.text = p1Speed.ToString();
        P2SpeedTxt.text = p2Speed.ToString();
        P1StaBar.value = P1StaVal;
        P2StaBar.value = P2StaVal;
        
        //Debug.LogError("P1 STAMIN " + P1StaBar.value + " Time "+ DateTime.Now);
        //Debug.LogError("P2 STAMIN " + P2StaBar.value);
    }

    public void UpdateBetForPlayer(float s){
        player1BetAmt = s;
        player1Bet.text = player1BetAmt.ToString();
        photonView.RPC("UpdateBetForPlayerRPC",RpcTarget.Others,s);
    }

    [PunRPC]
    public void UpdateBetForPlayerRPC(float s){
        player2BetAmt = s;
        player2Bet.text = player2BetAmt.ToString();
    }

     public void updatePlayerAction(string s){
        P1LastAction.text = s;
        photonView.RPC("updatePLayerActionRPC",RpcTarget.Others,s);
    }

    [PunRPC]
    public void updatePLayerActionRPC(string s){
        P2LastAction.text = s;
    }

    public void UpdateManaTxt()
    {

        MyManaBar.value = MyManabarVal;
        OppoManaBar.value = OppoManabarVal;
        MyManaBarTxt.text = MyManabarVal + " / " + MaxManaBarVal;
        OppoManaBarTxt.text = OppoManabarVal + " / " + MaxManaBarVal;
    }

    public void SetData(Vector2 posP1, Vector2 posP2, bool localplayerTurn, bool isReverse)
    {

        Debug.Log("<color=yellow> ==== === = set data = === ==== </color>");

        //TODO pvp start 
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
            // if(PhotonNetwork.LocalPlayer.IsMasterClient){
            //     isLocalPVPFirstTurn = true;

            // }else{
            //     isLocalPVPFirstTurn = false;


            // }

        }
        else
        {
            Game.Get().IsDefender = true;
            isLocalPVPFirstTurn = false;
            // if(PhotonNetwork.LocalPlayer.IsMasterClient){
            //     isLocalPVPFirstTurn = false;              
            // }
            // else{
            //     isLocalPVPFirstTurn = true;

            // }

            // ModePanel.SetActive(false);
            // waitPanel.SetActive(true);
        }

        //IsPetTurn = isLocalPVPFirstTurn;

        isattackerMaster = isLocalPVPFirstTurn && PhotonNetwork.LocalPlayer.IsMasterClient;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            isattackerMaster = !isLocalPVPFirstTurn && !PhotonNetwork.LocalPlayer.IsMasterClient;

        //Info:Set laoding screen to avoid lag view and Reset Spell Cards
        Game.Get().loadingScreen.SetActive(true);
        SpellManager.instance.RemoveOldSpellData();
        SpellManager.instance.spawned_ids = new List<int>();

        //
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RPC("SetDataRPC", RpcTarget.AllBuffered, posP1, posP2, isReverse);







        //Debug.Log("<color=yellow> ==== === = set data end = === ==== </color>");

    }
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
    [PunRPC]
    public void UpdatePotAmountForText(int sliderVal)
    {
        AttackFor += sliderVal;
        BetTextObj.text = "TOTAL ATTACK : " + AttackFor;
    }
    [PunRPC]
    public void UpdatePotAmountForAllInText(int sliderVal)
    {
        AttackFor = sliderVal;
        BetTextObj.text = "TOTAL ATTACK : " + AttackFor;
    }
    public void ResetText()
    {
        p2AttackFor.text = "";
        p1AttackFor.text = "";

        p1AttackFor.gameObject.transform.parent.localScale = Vector3.zero;
        p2AttackFor.gameObject.transform.parent.localScale = Vector3.zero;
    }

    public int LastAtkAmt;
    [PunRPC]
    public void UpdateLastAtkAmt(int f)
    {
        LastAtkAmt = f;
    }
    public void sliderAttackbuttonClick(int sliderAttack, float StaminaConsumed, PlayerAction action)
    {
        MyLastAttackAmount = sliderAttack;
        Game.Get().UpdateLastAction(action);
        photonView.RPC("UpdateLastAtkAmt", RpcTarget.All, sliderAttack);
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
            // if(!isAttackLocationSelected && isLocalPVPFirstTurn)
            // {
            //     LocationChoiceHeading.GetComponent<Text>().text = "Choose Attack Location";
            //     LocationChoiceHeading.SetActive(true);
            //     LocationChoices.SetActive(true);
            //     isLocationChoose = true;
            // }
            // else if(!isDefenceLocationSelected && !isLocalPVPFirstTurn){
            //     LocationChoiceHeading.GetComponent<Text>().text = "Choose Defence Location";
            //     LocationChoiceHeading.SetActive(true);
            //     LocationChoices.SetActive(true);
            //     isLocationChoose = true;
            // }
            // else
            // {
            //     EndTurnBtn.gameObject.SetActive(true);
            //     StartTimer();
            // }
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
            // if(!isAttackLocationSelected && isLocalPVPFirstTurn)
            // {
            //     LocationChoiceHeading.GetComponent<Text>().text = "Choose Attack Location";
            //     LocationChoiceHeading.SetActive(true);
            //     LocationChoices.SetActive(true);
            //     isLocationChoose = true;
            // }
            // else if(!isDefenceLocationSelected && !isLocalPVPFirstTurn){
            //     LocationChoiceHeading.GetComponent<Text>().text = "Choose Defence Location";
            //     LocationChoiceHeading.SetActive(true);
            //     LocationChoices.SetActive(true);
            //     isLocationChoose = true;
            // }
            // else
            // {
            //     EndTurnBtn.gameObject.SetActive(true);
            //     StartTimer();
            // }
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
            //// EndTurnBtn.gameObject.SetActive(true);
            //// StartTimer();
            OnClickEndTurn();
        }


        DeductStamina(StaminaConsumed);

        // if(!isDefenceLocationSelected && !isLocalPVPFirstTurn){
        //     LocationChoiceHeading.GetComponent<Text>().text = "Choose Defence Location";
        //     LocationChoiceHeading.gameObject.SetActive(true);
        //     LocationChoices.SetActive(true);
        // }


    }

    public void LaunchSpecialAttack()
    {
        StartCoroutine(SpellManager.instance.CastSpell(p1Char.SpecialAttack));
        SpecialAttackButton.SetActive(false);
        DeductRage(p1Char.SpecialAttackCost);
    }

    public void DeductRage(int v)
    {
        P1RageBar.value -= v;
        UpdateHMTxt();
        photonView.RPC("DeductRageRPC", RpcTarget.Others, v);
    }

    [PunRPC]
    public void DeductRageRPC(int i)
    {
        P2RageBar.value -= i;
        UpdateHMTxt();
    }

    public void DeductStamina(float i)
    {
        P1StaVal -= i;
        UpdateHMTxt();
        photonView.RPC("DeductStaminaRPC", RpcTarget.Others, i);
    }

    [PunRPC]
    public void DeductStaminaRPC(float i)
    {
        P2StaVal -= i;
        UpdateHMTxt();
    }

    public void DeductSpeed(float i)
    {
        p1Speed -= i;
        p1Speed = Mathf.Clamp(p1Speed, 0, p1Speed);

        UpdateHMTxt();
        photonView.RPC("DeductSpeedRPC", RpcTarget.Others, i);
    }

    [PunRPC]
    public void DeductSpeedRPC(float i)
    {
        p2Speed -= i;
        UpdateHMTxt();
    }

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

                if (checkCost < 0) checkCost = 0;

                P1StaBar.value += checkCost;
                isCheck = false;
                UpdateHMTxt();
                photonView.RPC("UpdateOpponentStamina", RpcTarget.Others, checkCost);
            }
            //AttackChoices.SetActive(false);
            //LocationChoices.SetActive(false);
            //ModePanel.SetActive(true);
            //waitPanel.SetActive(false);
            //DemoManager.instance._pokerButtons.SetActive(true); Debug.LogError("ATTACKE RPC False from here");
            //PokerButtonManager.instance.bet_attack.gameObject.SetActive(false);
            //PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(false);

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

            // else
            // {
            //    if(P1HealthBar.value<(2*Game.Get().BatAmount))
            //{
            //    PokerButtonManager.instance.Reraise_CouterAttack.interactable = false;
            //}
            //else
            //{
            //    PokerButtonManager.instance.Reraise_CouterAttack.interactable = true;
            //}


            PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(true);
            //PokerButtonManager.instance.Reraise_CouterAttack.interactable = Game.Get().BetAmount * 2 <= P1HealthBar.value ? true : false;

            //   PokerButtonManager.instance.Reraise_CouterAttack.interactable = Game.Get().BetAmount * 2 <= P1RemainingHandHealth ? true : false;



            //Debug.LogError("**GAME Bet AMOUNT" + Game.Get().BetAmount);
            //Debug.LogError("**P1 TEMP HEALTH" + P1RemainingHandHealth);
            //PokerButtonManager.instance.bet_attack.interactable = P2LastAttackValue <= P1HealthBar.value ? true : false;

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
                // if((Game.Get().turn == 3 || Game.Get().turn == 5 || Game.Get().turn == 7)  && Game.Get().lastAction == PlayerAction.engage)
                // {
                //     //Debug.LogError("***LAST ATTACK TYPE 4" + Game.Get().lastAction);
                //     PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                //     PokerButtonManager.instance.call_Engauge.gameObject.SetActive(false);

                //     PokerButtonManager.instance.bet_attack.gameObject.SetActive(true);
                //     PokerButtonManager.instance.check_Defend_5_Stamin.gameObject.SetActive(true);

                //     PokerButtonManager.instance.fold_Brace_5_Stamina.gameObject.SetActive(false);
                // }


            }
            //if(PokerButtonManager.instance.Reraise_CouterAttack.interactable==false && PokerButtonManager.instance.Reraise_CouterAttack.gameObject.activeInHierarchy)
            //{
            //    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
            //    PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
            //    PokerButtonManager.instance.allIn_btn.interactable = true;
            //}
            //else
            //{
            //    PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
            //    PokerButtonManager.instance.allIn_btn.interactable = false;
            //}


            //All In
            if (P2LastAttackValue > P1RemainingHandHealth) // if(Game.Get().BetAmount * 2 > P1RemainingHandHealth)
            {
                PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                if (!PokerButtonManager.instance.bet_attack.gameObject.activeSelf)
                {
                    if (P2LastAttackValue > P1RemainingHandHealth)
                    {
                        //Debug.LogError("ALL IN TRUE FROM HERE " + P2LastAttackValue + " Remaining Health " + P1RemainingHandHealth);
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
                    }
                    else
                    {
                        //Debug.LogError("ALL IN SET FALSE FROM HERE ");
                        PokerButtonManager.instance.allIn_btn.gameObject.SetActive(false);
                    }
                }

            }
            else
            {
                if (P2RemainingHandHealth <= 0)
                {
                    PokerButtonManager.instance.Reraise_CouterAttack.gameObject.SetActive(false);
                    PokerButtonManager.instance.allIn_btn.gameObject.SetActive(true);
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
            DemoManager.instance._pokerButtons.SetActive(true);
            if (P1RageBar.value > 50)
                SpecialAttackButton.SetActive(true);
            //Debug.LogError("**DEFENDER NO_" + Game.Get().turn);
            if (Game.Get().lastAction != PlayerAction.attack && Game.Get().lastAction != PlayerAction.counterAttack && !isAttackLocationSelected)// || !isDefenceLocationSelected
            {
                // LocationChoices.SetActive(true);
                // LocationChoiceHeading.SetActive(true);
                // LocationChoiceHeading.GetComponent<Text>().text = "Choose Attack Location";
                // isLocationChoose = true;
                // DemoManager.instance._pokerButtons.SetActive(false);
            }
            else
            {

            }
            //if(PhotonNetwork.LocalPlayer.IsMasterClient)
            //{
            //    P1HealthBar.value -= Game.Get().HealthDemage;
            //}
            //else
            //{
            //    P2HealthBar.value -= Game.Get().HealthDemage;
            //}
            UpdateHMTxt();

            IsPetTurn = true;
            SpellManager.PetAlreadyAttacked = false;
            if (!SpellManager.PetAlreadyAttacked)
            {
                SpellManager.instance.PetAttack();
                SpellManager.PetAlreadyAttacked = true;
            }


        }
        //
        //if(Game.Get().turn == 4)
        //{
        //    DemoManager.instance.Generate3CardsStack();

        //    for(int i = 0 ; i < DemoManager.instance.board_cards.Count ; i++)
        //    {
        //        DemoManager.instance.board_cards[i].gameObject.SetActive(true);

        //    }
        //}
        //if(Game.Get().turn == 6)
        //{
        //    DemoManager.instance.Generate3CardsStack();

        //    for(int i = 0 ; i < DemoManager.instance.board_cards.Count ; i++)
        //    {
        //        DemoManager.instance.board_cards[i].gameObject.SetActive(true);

        //    }
        //}
        //if(Game.Get().turn == 8)
        //{
        //    DemoManager.instance.CompareHand(1);

        //}
        //
        if (_player != PhotonNetwork.LocalPlayer)
        {
            //isLocalPVPTurn = true;
            SpellManager.PetAlreadyAttacked = false;
            //Debug.LogError("ATTACKE COLOR RPC" + Game.Get().LastAttackerColor);
            //if(Game.Get().LastAttackerColor != Game.Get().GetCurrentPlayer())
            //{ DemoManager.instance._pokerButtons.SetActive(true); }
            //else
            //{
            //    DemoManager.instance._pokerButtons.SetActive(false);
            //    AttackChoices.SetActive(false);
            //    waitPanel.SetActive(true);
            //}
        }
        else
        {

            // Debug.Log("you are not turn player please wait");
        }
    }
    [PunRPC]
    public void UpdateOpponentStamina(float val)
    {
        // float checkCost = 1f;
        P2StaBar.value += val;
        UpdateHMTxt();
        // isCheck = false;
    }
    [PunRPC]
    public void RPC_SetLastAttackerColor(string _Last_color)
    {
        Game.Get().LastAttackerColor = _Last_color;
    }
    [PunRPC]
    public void RPC_SetOpponentAttackChoice(int c)
    {

        OpponentChoiceOne = c;
    }

    [PunRPC]


    int LastGameTurn = -1;
    List<int> ListOfCounterAttacks = new List<int>();
    [PunRPC]
    public void RPC_UpdateTurn()
    {

        //&&( Game.Get().lastAction!= PlayerAction.defend && Game.Get().lastAction!=PlayerAction.engage && Game.Get().lastAction != PlayerAction.brace)
        Debug.Log("LAST ATTACK TYPE " + Game.Get().lastAction + PVPManager.manager.isattackerMaster);
        //debug.text = "LAST ATTACK TYPE " + Game.Get().lastAction + " - "+ LastGameTurn;
        //if(PhotonNetwork.LocalPlayer.NickName== Game.Get()._currnetTurnPlayer.NickName  && Game.Get().localBetAmount>=P2RemainingHandHealth && Game.Get().turn==7 && P1RemainingHandHealth>0 && MyLastBatAmount>P2RemainingHandHealth && !isAutoTurn ) 
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

        // if(Game.Get().lastAction == PlayerAction.counterAttack && Game.Get().turn % 2 == 0 && !ListOfCounterAttacks.Contains(Game.Get().turn)){
        //     ListOfCounterAttacks.Add(Game.Get().turn);
        //     Game.Get().turn -= 1;
        //     return;
        // }
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

            //for(int i = 0 ; i < DemoManager.instance.board_cards.Count ; i++)
            //{
            //    DemoManager.instance.board_cards[i].gameObject.SetActive(true);

            //}
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
            //SpellManager.instance.DrawCard();
            //for(int i = 0 ; i < DemoManager.instance.board_cards.Count ; i++)
            //{
            //    DemoManager.instance.board_cards[i].gameObject.SetActive(true);

            //}

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

            //int S=    DemoManager.instance.CompareHandWithStrength(1);
            //     //Debug.LogError("*** PLAYER STRENGTH " +S+"===PLAYER NAME :"+ (PhotonNetwork.LocalPlayer.IsMasterClient?_playersNameInPvP[0].GetComponentInChildren<Text>().text.ToString(): _playersNameInPvP[1].GetComponentInChildren<Text>().text.ToString()));


            //        int indexMax
            //= !PlayerStrengths.Any() ? -1 :
            //PlayerStrengths
            //.Select((value,index) => new { Value = value,Index = index })
            //.Aggregate((a,b) => (a.Value > b.Value) ? a : b)
            //.Index;
            // PlayerStrengths.Max

        }


    }

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

    public void AddStamina()
    {
        float val = 1f - (p1StaminRecoveryReducedBy);
        P1StaVal += val;
        //Debug.LogError("Stamina recovery : " + val + " - " + p1StaminRecoveryReducedBy);
        P1StaVal = Mathf.Clamp(P1StaVal, 0, P1StaBar.maxValue);
        UpdateHMTxt();
        photonView.RPC("AddStaminaRPC", RpcTarget.Others, val);
    }

    [PunRPC]
    public void AddStaminaRPC(float i)
    {
        P2StaVal += i;
        P2StaVal = Mathf.Clamp(P2StaVal, 0, P2StaBar.maxValue);
        UpdateHMTxt();
    }



    public void DeductMana(int i)
    {
        MyManabarVal -= i;
        UpdateManaTxt();
        photonView.RPC("UpdateManaRPC", RpcTarget.Others, i);
    }

    [PunRPC]
    public void UpdateManaRPC(int i)
    {
        OppoManabarVal -= i;
        UpdateManaTxt();
    }


    [PunRPC]
    public void RPC_ResetTurn()
    {
        Game.Get().turn = 0;
        ListOfCounterAttacks = new List<int>();
        //
    }

    public void DisplayResult()
    {
        for (int s = 0; s < Game.Get().PlayerStrengths.Count; s++)
        {
            if (s == 0)
            {
                //Debug.LogError("***MY STRENGTH " + Game.Get().PlayerStrengths[0]);

            }
            if (s == 1)
            {
                //Debug.LogError("***OPPONENT STRENGTH " + Game.Get().PlayerStrengths[1]);
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
            if(StartHandTurn){
                DeductSpeed(p1Speed);
                DeductStamina((P1StaVal * 0.75f));
            }
            resultText.text += "\n Damage Dealt = 0";
            StartCoroutine(ShowShieldEff());
        }else{
            if(!StartHandTurn){
                LocationObject.GetLocation(OpponentChoiceOne).damaged_amt++;
            }
        }
        StartCoroutine("ResultRPCLocal");
        //if(PhotonNetwork.IsMasterClient)
        // photonView.RPC("ResultRPC",RpcTarget.Others);
        //Invoke("ResetData",2f);

        //Invoke("SetModePanel",3f);
    }

    IEnumerator ShowShieldEff(){
        
        if(StartHandTurn){
            GameObject o = Instantiate(Proj,p1Image.transform.position,Quaternion.identity);
            o.GetComponent<Projectile>().DealDamage = false;
            o.GetComponent<Projectile>().target = P2Shield.gameObject;
            LeanTween.color(P2Shield.GetComponent<RectTransform>(),Color.white,0.3f);
            yield return new WaitForSeconds(1.3f);
            LeanTween.color(P2Shield.GetComponent<RectTransform>(),new Color(0f,0f,0f,0f),0.3f);
        }else{
            GameObject o = Instantiate(Proj,p2Image.transform.position,Quaternion.identity);
            o.GetComponent<Projectile>().DealDamage = false;
            o.GetComponent<Projectile>().target = P1Shield.gameObject;            
            LeanTween.color(P1Shield.GetComponent<RectTransform>(),Color.white,0.3f);
            yield return new WaitForSeconds(1.3f);
            LeanTween.color(P1Shield.GetComponent<RectTransform>(),new Color(0f,0f,0f,0f),0.3f);
        }
    }



    //[PunRPC]
    //public void ResultRPC()
    //{
    //    bool isLocationMatch = false;

    //    if(Game.Get().PlayerStrengths[0] > Game.Get().PlayerStrengths[1])
    //    {
    //        if(Game.Get().lastAction == PlayerAction.defend && isAttackLocationSelected)
    //        {
    //            if(opponentDefendLocation == playerAttackLocation)
    //            {
    //                isLocationMatch = true;
    //            }
    //            else
    //            {
    //                isLocationMatch = false;
    //            }
    //        }
    //        //Debug.LogError("***OPPONENT HEALTH DEC");
    //        BetTextObj.text = "YOU WIN";
    //        float myAttackRis = P1StartHealth - P1HealthBar.value;
    //        P1HealthBar.value = P1StartHealth;
    //        P2StartHealth = (int)P2HealthBar.value;
    //        // P2HealthBar.value -= isLocationMatch? 0:Game.Get().BetAmount;

    //        //P1StaBar.value -= Game.Get().BetAmount * 0.1f;
    //        P1StaBar.value -= myAttackRis * 0.1f;
    //        P1StaTxt.text = (P1StaBar.value).ToString();
    //        //p1Speed += (int)Game.Get().BetAmount * 0.1f;
    //        p1Speed += (int)myAttackRis * 0.1f;
    //        P1SpeedTxt.text = "";
    //        P1SpeedTxt.text =((int)p1Speed).ToString();
    //        UpdateHMTxt();
    //    }
    //    else if(Game.Get().PlayerStrengths[0] < Game.Get().PlayerStrengths[1])
    //    {
    //        if(Game.Get().lastAction == PlayerAction.defend && isDefenceLocationSelected)
    //        {
    //            if(playerDefenceLocation == opponentAttackLocation)
    //            {
    //                isLocationMatch = true;
    //            }
    //            else
    //            {
    //                isLocationMatch = false;
    //            }
    //        }
    //        //Debug.LogError("***MY HEALTH DEC");
    //        BetTextObj.text = "YOU LOSE";
    //        float myAttackRisk = P2StartHealth - P2HealthBar.value;
    //        P2HealthBar.value = P2StartHealth;
    //        P1StartHealth = (int)P1HealthBar.value;
    //        //P1HealthBar.value -= isLocationMatch ? 0: Game.Get().BetAmount;
    //        //P2StaBar.value -= Game.Get().BetAmount * 0.1f;
    //        P2StaBar.value -= myAttackRisk * 0.1f;
    //        P2StaTxt.text = (P2StaBar.value).ToString();
    //        //p2Speed += (int)Game.Get().BetAmount * 0.1f; 
    //        p2Speed += (int)myAttackRisk * 0.1f;
    //        P2SpeedTxt.text =  ((int)p2Speed).ToString();
    //        UpdateHMTxt();
    //        // Game.Get().NextTurn();
    //    }
    //    else if(Game.Get().PlayerStrengths[0] == Game.Get().PlayerStrengths[1])
    //    {
    //        Debug.LogError("MY HIGH CARD " + Game.Get().MyHighCardValue);
    //        Debug.LogError("OPPONANT HIGH CARD " + Game.Get().OpponentHighCardValue);

    //        if(Game.Get().MyHighCardValue > Game.Get().OpponentHighCardValue)
    //        {
    //            if(Game.Get().lastAction == PlayerAction.defend && isAttackLocationSelected)
    //            {
    //                if(opponentDefendLocation == playerAttackLocation)
    //                {
    //                    isLocationMatch = true;
    //                }
    //                else
    //                {
    //                    isLocationMatch = false;
    //                }
    //            }
    //            BetTextObj.text = "YOU WIN";
    //            float myAttackRisk = P1StartHealth - P1HealthBar.value;
    //            P1HealthBar.value = P1StartHealth;
    //            P2StartHealth = (int)P2HealthBar.value;
    //            // P2HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount;
    //          //  P1StaBar.value -= Game.Get().BetAmount * 0.1f;
    //            P1StaBar.value -= myAttackRisk * 0.1f;
    //            P1StaTxt.text = (P1StaBar.value).ToString();
    //            //p1Speed += (int)Game.Get().BetAmount * 0.1f;
    //            p1Speed += (int)myAttackRisk * 0.1f;
    //            P1SpeedTxt.text = "";
    //            P1SpeedTxt.text =  ((int)p1Speed).ToString();
    //            UpdateHMTxt();
    //        }
    //        else if(Game.Get().MyHighCardValue < Game.Get().OpponentHighCardValue)
    //        {
    //            if(Game.Get().lastAction == PlayerAction.defend && isDefenceLocationSelected)
    //            {
    //                if(playerDefenceLocation == opponentAttackLocation)
    //                {
    //                    isLocationMatch = true;
    //                }
    //                else
    //                {
    //                    isLocationMatch = false;
    //                }
    //            }
    //            BetTextObj.text = "YOU LOSE";
    //            float myAttackRisk = P2StartHealth - P2HealthBar.value;
    //            P2HealthBar.value = P2StartHealth;
    //            P1StartHealth = (int)P1HealthBar.value;
    //            // P1HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount;
    //            // P2StaBar.value -= Game.Get().BetAmount * 0.1f;
    //            P2StaBar.value -= myAttackRisk * 0.1f;
    //            P2StaTxt.text = (P2StaBar.value).ToString();
    //            //p2Speed += (int)Game.Get().BetAmount * 0.1f;
    //            p2Speed += (int)myAttackRisk * 0.1f;
    //            P2SpeedTxt.text =  ((int)p2Speed).ToString();
    //            UpdateHMTxt();
    //        }
    //        else if(Game.Get().MyHighCardValue == Game.Get().OpponentHighCardValue)
    //        {
    //            P1HealthBar.value = P1StartHealth;
    //            P2HealthBar.value = P2StartHealth;
    //            BetTextObj.text = "TIE";
    //            UpdateHMTxt();
    //        }
    //    }
    //   // StartCoroutine(CheckWinNew());
    //}

    public IEnumerator ResultRPCLocal()
    {
        yield return new WaitWhile(()=>SpellManager.IsPetAttacking);
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
            //Debug.LogError("MY HIGH CARD " + mCard);
            //Debug.LogError("OPPONANT HIGH CARD " + oCard);

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
                            isPlayer1Win = TieMethod();
                            isTie = true;
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
                            isPlayer1Win = TieMethod();
                            isTie = true;
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
                            isPlayer1Win = TieMethod();
                            isTie = true;
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
                            isPlayer1Win = TieMethod();
                            isTie = true;
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
                        isPlayer1Win = TieMethod();
                        isTie = true;

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
                            isPlayer1Win = TieMethod();
                            isTie = true;
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
        if(isTie){
            StartCoroutine(CheckWinNew(3f));
        }
        //Debug.LogError("PLAYER WIN :" + isPlayer1Win);
        StopTimer();
        //StartCoroutine(CheckWinNew());
    }

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

        //Check for weakness factor
        // ChekcForWeaknessFactor();
        // ReaduceAttackAmount();
        // ManangeAttackLocationEffects();
        Invoke("CheckForEffects", 1.5f);
        Debug.Log("LOCATION MATCH " + isLocationMatch);
        DemoManager.instance.HighLightWinnerHand(false);
        // float TempVal = P1HealthBar.value;
        //// P1HealthBar.value = isLocationMatch ? P1StartHealth : TempVal;
        // //P1HealthBar.value -= isLocationMatch ? (Game.Get().BatAmount / 2) : Game.Get().BatAmount; //50% reduce damage
        // // P1HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount; //100% nutralize damage
        // float myAttackRisk = P2StartHealth - P2HealthBar.value;
        // P2HealthBar.value = P2StartHealth;
        // P1StartHealth = (int)P1HealthBar.value;
        // //P2StaBar.value -= (int)Game.Get().BetAmount * 0.1f;
        // P2StaBar.value -= myAttackRisk * 0.1f;
        // P2StaTxt.text = (P2StaBar.value).ToString();
        // //p2Speed += (int)Game.Get().BetAmount * 0.1f;
        // p2Speed += (int)myAttackRisk * 0.1f;
        // P2SpeedTxt.text = MathF.Round(p2Speed,2).ToString();
        // UpdateHMTxt();
    }

    public void CheckForEffects()
    {
        ManangeAttackLocationEffects();
        ChekcForWeaknessFactor();
    }

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


        //float TempVal = P2HealthBar.value;
        //P2HealthBar.value = isLocationMatch ? P2StartHealth : TempVal;
        //float myAttackRisk = P1StartHealth - P1HealthBar.value;
        //P1HealthBar.value = P1StartHealth;
        //P2StartHealth = (int)P2HealthBar.value;
        //// P2HealthBar.value -= isLocationMatch ? 0 : Game.Get().BetAmount;
        ////P1StaBar.value -= Game.Get().BetAmount * 0.1f;
        //P1StaBar.value -= myAttackRisk * 0.1f;
        //P1StaTxt.text = (P1StaBar.value).ToString();
        ////p1Speed += (int)Game.Get().BetAmount * 0.1f;
        //p1Speed += (int)myAttackRisk * 0.1f;
        //P1SpeedTxt.text = "";
        //P1SpeedTxt.text = MathF.Round(p1Speed,2).ToString();
        //UpdateHMTxt();
        return isPlayer1Win;
    }

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
    //private void ReaduceAttackAmount()
    //{
    //    int player1weaknessfactor =  (int)(opponentAttackWeakPercentageVal * AttackFor);
    //    UpdateRemainingHandHealthPlus(player1weaknessfactor);

    //        Debug.Log("Less DAMAGE %___" + opponentAttackWeakPercentageVal);

    //}
    public int criticalHitPercentage = 0;

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
                P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString();
                photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);

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

    private void ManangeAttackLocationEffects()
    {
        if (playerDefenceLocation == opponentAttackLocation)
        {
            return;
        }

        photonView.RPC("SetOpponentAttackedSide_RPC", RpcTarget.Others, (int)opponentAttackLocation);
        PhotonNetwork.SendAllOutgoingCommands();
    }



    public void ResumeChessGame()
    {
        StartCoroutine(CheckWinNew(1f));
    }
    public bool isResultScreenOn = false;
    public bool isReset = false;
    [PunRPC]
    public void Rset_BOOL_RPC(bool val)
    {
        isReset = val;
    }
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
        if(!isGameOver)
        SpellManager.instance.DrawCard();
        AddMana();

    }
    public void GenerateCardFromMaster(bool isGameOver)
    {
        if (!isGameOver)
        {
            Debug.Log("CARD GENERATION CALL 5");
            DemoManager.instance.Generate3CardsStack();
        }
    }
    [PunRPC]
    public void ResetPVPUIData(bool isGameOver = false)
    {
        if (isReset)
        {
            isGameOver = isReset;
            //Debug.LogError("**DATA RESET");
            // P1HealthBar.maxValue = 100;
            // P2HealthBar.maxValue = 100;
            // P1HealthBar.value = 100;
            // P2HealthBar.value = 100;
            // P1StartHealth =(int) P1HealthBar.value;
            // P2StartHealth = (int)P2HealthBar.value;

            P1StaBar.value = 10;
            P1StaTxt.text = "10/10";
            P2StaBar.value = 10;
            P2StaTxt.text = "10/10";
            //p1Speed = 0;
            //P1SpeedTxt.text = MathF.Round(p1Speed,2).ToString();
            //p2Speed = 0;
            //P2SpeedTxt.text = MathF.Round(p2Speed,2).ToString();
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

        //  //Debug.LogError("***CHILD COUNT " + DemoManager.instance.placeholderHand.transform.childCount);
        //TODO pvp start 
        //if (PhotonNetwork.IsMasterClient && !isGameOver)
        //{
        //    //Debug.LogError("***Three Cards Generated from here");
        //    DemoManager.instance.Generate3CardsStack();
        //}


    }
    bool alreadyGenerated = false;
    public void ResetAllreadyGenerated()
    {
        alreadyGenerated = false;
    }

    public IEnumerator GenerateCardWithDelayFunction(bool isGameOver, float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient && !isGameOver)
        {
            Debug.Log("CARD GENERATION CALL 6");
            DemoManager.instance.Generate3CardsStack();
        }
    }
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

    [PunRPC]
    public void RPC_SetOthersAsDefenders(float damageVal, bool isDefender = false)
    {
        Game.Get().IsDefender = isDefender;
        Game.Get().HealthDemage = damageVal;
    }
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
    int tempChoiceNo = -1;
    public void selectChoice(int c)
    {
        tempChoiceNo = c;
        ConfirmChoice();
        
        //choiceConfPopup.SetActive(true);

        //if((!isAttackLocationSelected || !isDefenceLocationSelected) && Game.Get().turn>0)
        //{
        ////    if(!isAttackLocationSelected)
        ////    {
        //        isAttackLocationSelected = true;
        //        UpdateAttackLocation(GetAttackLocation(c));
        //    //}

        //   /* if*///(!isDefenceLocationSelected);
        //    //{
        //        isDefenceLocationSelected = true;
        //        UpdateDefenceLocation(GetAttackLocation(c));
        //    //}

        //    LocationChoices.SetActive(false);
        //    DemoManager.instance._pokerButtons.SetActive(true);
        //}
        //else
        //{
        //    if(!isAttackLocationSelected)
        //    {
        //        isAttackLocationSelected = true;
        //        UpdateAttackLocation(GetAttackLocation(c));
        //    }
        //    if(!isDefenceLocationSelected)
        //    {
        //        isDefenceLocationSelected = true;
        //        UpdateDefenceLocation(GetAttackLocation(c));
        //    }

        //    if(!isCheck)
        //    {
        //        //if(!isAttackLocationSelected)
        //        //{
        //        //    isAttackLocationSelected = true;
        //        //    UpdateAttackLocation(GetAttackLocation(c));
        //        //}

        //        LocationChoices.SetActive(false);
        //        PlayerChoiceOnce = c;
        //        photonView.RPC("RPC_SetOpponentAttackChoice",RpcTarget.Others,c);
        //        Debug.Log($"<color=yellow>c is {c} state is {state} {AttackData.GetLocationFrmInt(c)} and poker attack {AttackSlider.instance._sliderAttack.ToString()}</color>");
        //        Debug.LogError("Slider val " + AttackSlider.instance._slider.value);

        //        EndTurnBtn.gameObject.SetActive(true);
        //        choice = c;

        //        //UpdateRangeCounter();
        //        //  photonView.RPC("UpdateOpponentRangePoints",RpcTarget.Others,rangeCounter);
        //        //photonView.RPC("UpdateRangePoints",RpcTarget.All,rangeCounter);
        //        // UpdateRangePoints(rangeCounter);
        //        photonView.RPC("UpdateBatAmount",RpcTarget.All,(int)AttackSlider.instance._slider.value);
        //        UpdateBatAmountLocal((int)AttackSlider.instance._slider.value);
        //        photonView.RPC("UpdateLastBatAmount",RpcTarget.Others,(int)AttackSlider.instance._slider.value);
        //        UpdateRemainingHandHealth((int)AttackSlider.instance._slider.value);
        //        PhotonNetwork.SendAllOutgoingCommands();
        //        //EndTurn(c);
        //    }
        //    else
        //    {
        //        //if(!isDefenceLocationSelected)
        //        //{
        //        //    isDefenceLocationSelected = true;
        //        //    UpdateDefenceLocation(GetAttackLocation(c));
        //        //}
        //        LocationChoices.SetActive(false);
        //        //PlayerChoiceOnce = c;
        //        //photonView.RPC("RPC_SetOpponentAttackChoice",RpcTarget.Others,c);
        //        //Debug.Log($"<color=yellow>c is {c} state is {state} {AttackData.GetLocationFrmInt(c)} and poker attack {AttackSlider.instance._sliderAttack.ToString()}</color>");
        //        //Debug.LogError("Slider val " + AttackSlider.instance._slider.value);

        //        EndTurnBtn.gameObject.SetActive(true);
        //        //choice = c;

        //        //UpdateRangeCounter();
        //        //  photonView.RPC("UpdateOpponentRangePoints",RpcTarget.Others,rangeCounter);
        //        //photonView.RPC("UpdateRangePoints",RpcTarget.All,rangeCounter);
        //        // UpdateRangePoints(rangeCounter);
        //        // photonView.RPC("UpdateBatAmount",RpcTarget.All,(int)AttackSlider.instance._slider.value);
        //        PhotonNetwork.SendAllOutgoingCommands();
        //        //EndTurn(c);
        //    }
        //}

    }
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
                // if (isattackerMaster)
                // {
                //     if (PhotonNetwork.LocalPlayer.IsMasterClient)
                //         SetOpponentAttackedSide_Local(c);
                // }
                // else
                // {
                //     if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                //         SetOpponentAttackedSide_Local(c);
                // }
            }
            if (!isDefenceLocationSelected)
            {
                isDefenceLocationSelected = true;
                UpdateDefenceLocation(GetAttackLocation(c));
            }

            if (!isCheck)
            {
                //if(!isAttackLocationSelected)
                //{
                //    isAttackLocationSelected = true;
                //    UpdateAttackLocation(GetAttackLocation(c));
                //}

                LocationChoices.SetActive(false);
                PlayerChoiceOnce = c;
                photonView.RPC("RPC_SetOpponentAttackChoice", RpcTarget.Others, c);
                //Debug.Log($"<color=yellow>c is {c} state is {state} {AttackData.GetLocationFrmInt(c)} and poker attack {AttackSlider.instance._sliderAttack.ToString()}</color>");
                //Debug.LogError("Slider val " + AttackSlider.instance._slider.value);

                //// EndTurnBtn.gameObject.SetActive(true);
                //// StartTimer();
                choice = c;

                //UpdateRangeCounter();
                //  photonView.RPC("UpdateOpponentRangePoints",RpcTarget.Others,rangeCounter);
                //photonView.RPC("UpdateRangePoints",RpcTarget.All,rangeCounter);
                // UpdateRangePoints(rangeCounter);
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
                //if(!isDefenceLocationSelected)
                //{
                //    isDefenceLocationSelected = true;
                //    UpdateDefenceLocation(GetAttackLocation(c));
                //}
                LocationChoices.SetActive(false);
                //PlayerChoiceOnce = c;
                //photonView.RPC("RPC_SetOpponentAttackChoice",RpcTarget.Others,c);
                //Debug.Log($"<color=yellow>c is {c} state is {state} {AttackData.GetLocationFrmInt(c)} and poker attack {AttackSlider.instance._sliderAttack.ToString()}</color>");
                //Debug.LogError("Slider val " + AttackSlider.instance._slider.value);

                //// EndTurnBtn.gameObject.SetActive(true);
                //// StartTimer();
                //choice = c;

                //UpdateRangeCounter();
                //  photonView.RPC("UpdateOpponentRangePoints",RpcTarget.Others,rangeCounter);
                //photonView.RPC("UpdateRangePoints",RpcTarget.All,rangeCounter);
                // UpdateRangePoints(rangeCounter);
                // photonView.RPC("UpdateBatAmount",RpcTarget.All,(int)AttackSlider.instance._slider.value);
                PhotonNetwork.SendAllOutgoingCommands();
                //EndTurn(c);
            }
        }
        choiceConfPopup.SetActive(false);


        AttackChoices.SetActive(false);
        OnClickEndTurn();
        //// StartTimer();
        //// EndTurnBtn.gameObject.SetActive(true);
        // if(!isLocalPVPFirstTurn){

        // }
    }
    public void RejectChoice()
    {
        choiceConfPopup.SetActive(false);
        tempChoiceNo = -1;
    }

    public void UpdateDataForReraise()
    {
        //photonView.RPC("UpdateOpponentRangePoints",RpcTarget.Others,rangeCounter);
        // UpdateRangePoints(rangeCounter);
        //photonView.RPC("UpdateRangePoints",RpcTarget.All,rangeCounter);

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
            UpdateBatAmountLocal((int)(AttackSlider.instance._slider.value * 2 ));
            PhotonNetwork.SendAllOutgoingCommands();
        }

    }

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
    public void UpdateBatText(int c)
    {
        photonView.RPC("UpdateBatAmount", RpcTarget.All, c);
        UpdateBatAmountLocal(c);
        photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, c);

    }
    public void UpdateBatTextFold(int c)
    {
        photonView.RPC("UpdateBatAmount", RpcTarget.All, c);
        UpdateBatAmountLocal(0);
        photonView.RPC("UpdateLastBatAmount", RpcTarget.Others, c);

    }
    public void UpdateRemainingHandHealth(int c)
    {
        P1RemainingHandHealth -= c;
        if (P1RemainingHandHealth < 0) P1RemainingHandHealth = 0;
        P1HealthBar.value = P1RemainingHandHealth;
        Debug.Log("P1 HEALTH BAR " + P1HealthBar.value + " C " + c);
        photonView.RPC("SetOpponentRemainingHandHealth_RPC", RpcTarget.Others, c);

    }
    [PunRPC]
    public void SetOpponentRemainingHandHealth_RPC(int h)
    {
        P2RemainingHandHealth -= h;
        P2HealthBar.value = P2RemainingHandHealth;

    }

    public GameObject Proj;

    [PunRPC]
    public void SimpleAttackRPC(int targetId, int dmg, bool isplayer,bool dealdmg){
        GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? p1Image.gameObject : SpellManager.instance.playerBattleCards.Find(x => x.id == targetId).gameObject;
        proj.damage = dmg;
        proj.istargetPlayer = isplayer;
        proj.DealDamage = dealdmg;
        proj.lifetime = 2f;
    }
    
    public IEnumerator DistributeSpellAttack(int c){
        yield return new WaitWhile(()=> SpellManager.petAttackStarted);
        
        int temp = c;
        Debug.Log(temp+" total attack");
        foreach (var item in SpellManager.instance.opponentBattleCards)
        {
            yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
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
                photonView.RPC("SimpleAttackRPC",RpcTarget.Others,item.card.cardId,item.Hp,false,true);
                yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
            }
            else if(temp < item.Hp){

                GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = temp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("SimpleAttackRPC",RpcTarget.Others,item.card.cardId,temp,false,true);
                
                temp = 0;
                yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
            }

            if(temp == 0) break;
            
            
        }

        if(temp > 0){
            GameObject o = Instantiate(Proj, p1Image.transform.position, Quaternion.identity);
            Projectile proj = o.GetComponent<Projectile>();
            proj.target = p2Image.gameObject;
            proj.damage = temp;
            proj.istargetPlayer = true;
            proj.DealDamage = true;
            proj.lifetime = 2f;
            photonView.RPC("SimpleAttackRPC",RpcTarget.Others,-1,temp,true,false);
        }
        
        canContinue = true;
    }

    public IEnumerator DistributeAttack(int c)
    {
        yield return new WaitWhile(()=> SpellManager.petAttackStarted);
        
        int temp = AttackFor + c;
        Debug.Log(temp+" total attack");
        foreach (var item in SpellManager.instance.playerBattleCards)
        {
            yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
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
                photonView.RPC("DistributeAttackRPC", RpcTarget.Others, item.card.cardId,item.Hp);
                yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
            }
            else if(temp < item.Hp){

                GameObject o = Instantiate(Proj, p2Image.transform.position, Quaternion.identity);
                Projectile proj = o.GetComponent<Projectile>();
                proj.target = item.gameObject;
                proj.damage = temp;
                proj.istargetPlayer = false;
                proj.DealDamage = true;
                proj.lifetime = 2f;
                photonView.RPC("DistributeAttackRPC", RpcTarget.Others, item.card.cardId,temp);
                
                temp = 0;
                yield return new WaitWhile(()=> SpellManager.IsPetAttacking);
            }

            if(temp == 0) break;
            
            
        }
        Debug.LogError("star = "+P1StartHealth+" - "+ temp);
        RemainingAtk = P1StartHealth - temp;
        canContinue = true;
    }

    // IEnumerator DistributeCOR(){

    // }

    [PunRPC]
    public void DistributeAttackRPC(int cardId, int attack)
    {
        foreach (var item in SpellManager.instance.opponentBattleCards)
        {
            if(item.card.cardId == cardId){
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

    public int RemainingAtk;
    public bool canContinue;



    public IEnumerator UpdateRemainingHandHealthPlus(int c)
    {
        
        canContinue = false;
        StartCoroutine(DistributeAttack(c));
        yield return new WaitUntil(()=> canContinue);
        Debug.LogError("rem attack"+RemainingAtk);
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
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString();
        UpdateHMTxt();
        StartCoroutine(CheckWinNew(1f));
    }

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
        //p1Speed += (int)Game.Get().BetAmount * 0.1f;
        //Debug.LogError("Speed Calc To be increased : " + (int)(myAttackRisk * 0.1f) + "Actually increased :" + p1SpeedSlowBy);
        p1Speed += (float)((myAttackRisk * 0.1f));
        P1SpeedTxt.text = "";
        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString();
        UpdateHMTxt();
        StartCoroutine(CheckWinNew(1f));
    }

    public void ShowExtraDamageMessage(int c)
    {
        LeanTween.scale(extraDamageMessageP1, Vector3.one, .3f);
        photonView.RPC("ShowExtraDamageMessage_RPC", RpcTarget.Others, c);
        UpdateHMTxt();
        Invoke("ResetDamageMessage", 3f);
    }
    [PunRPC]
    public void ShowExtraDamageMessage_RPC(int h)
    {
        LeanTween.scale(extraDamageMessageP2, Vector3.one, .3f);
        Invoke("ResetDamageMessage", 3f);
        UpdateHMTxt();
    }
    public void ResetDamageMessage()
    {
        LeanTween.scale(extraDamageMessageP2, Vector3.zero, .3f);
        LeanTween.scale(extraDamageMessageP1, Vector3.zero, .3f);
    }
    //
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

            //Game.Get().BetAmount += c;
            //if(Game.Get().BetAmount > Mathf.Min(P1RemainingHandHealth,P2RemainingHandHealth))
            //{
            //    Game.Get().BetAmount = 100;
            //}
            //BetTextObj.text = "BET Value : " + Game.Get().BetAmount.ToString();
        }
    }
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
            //Debug.LogError("Value set here " + c);
            BetTextObj.gameObject.SetActive(true);
            P2LastAttackValue = c;
            //Game.Get().BetAmount += c;
            //if(Game.Get().BetAmount > Mathf.Min(P1RemainingHandHealth,P2RemainingHandHealth))
            //{
            //    Game.Get().BetAmount = 100;
            //}
            //Info: This is Working line
            // BetTextObj.text = "BET Value : " + P2LastAttackValue;

            // BetTextObj.text = "BET Value : " + myLocalBatAmount;
        }
    }
    public int MyLastBatAmount = -1;
    public void UpdateBatAmountLocal(int c)
    {
        //Debug.LogError("VAlue local " + c);
        MyLastBatAmount = c;
        BetTextObj.gameObject.SetActive(true);
        //Game.Get().BetAmount += c;
        //if(Game.Get().BetAmount > Mathf.Min(P1RemainingHandHealth,P2RemainingHandHealth))
        //{
        //    Game.Get().BetAmount = 100;
        //}
        myLocalBatAmount += c;
        if (myLocalBatAmount < 0)
        {
            myLocalBatAmount = 0;
        }
        Game.Get().localBetAmount = myLocalBatAmount;
        //Info:: This is working ::BetTextObj.text = "BET Value : " +c;
        //  BetTextObj.text = "BET Value : " + myLocalBatAmount;

    }
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
        //}
        //else
        //{
        //    if(c >= 3)
        //    {
        //        P2RageBar.value += 10;
        //        P2RageTxt.text = ((int)P2RageBar.value).ToString();
        //        //rangeCounter = 0;
        //    }           
        //}
    }
    public bool isNormalBat = false, isAllIn = false, isFromInbetween = false;
    public bool IsAnyAllIn = false;
    public bool isfold = false;
    public bool isCheck = false;
    public bool isReraiseAfterOnce = false;
    public bool isLocationChoose = false;
    // public void StartTimer() 
    // {
    //     if(endTurnTimeStarted)
    //         return;
    //     endTurn = false;
    //     EndTurnTimer = OriginalTimerVal;
    //     StopCoroutine(UpdateEndTurnTimer());
    //     //Debug.LogError("Timer STARTED FROM HERE");
    //     StartCoroutine(UpdateEndTurnTimer());
    // }
    bool endTurn = false;
    public bool endChessTurn = false;

    public bool LastActionUpdated;
    public void OnClickEndTurn()
    {
        if (!LastActionUpdated)
        {
            if (Game.Get().turn <= 1 || (Game.Get().lastAction == PlayerAction.counterAttack || Game.Get().lastAction == PlayerAction.attack))
            {
                PokerButtonManager.instance.Fold();
                isfold = true;
            }
            // else if (Game.Get().lastAction == PlayerAction.counterAttack || (Game.Get().lastAction == PlayerAction.attack && Game.Get().turn % 2 == 0 && Game.Get().turn > 1))
            // {
            //     Debug.LogError("Es ist okay....");
            //     PokerButtonManager.instance.Bet();
            // }
            else
            {
                PokerButtonManager.instance.Check();
            }
            // AttackChoices.SetActive(false);
            // LocationChoices.SetActive(false);
            // attackSlider.gameObject.SetActive(false);
            // choiceConfPopup.SetActive(false);
        }

        //Debug.LogError(P1RemainingHandHealth + " - " + P2RemainingHandHealth);

        // if (P1RemainingHandHealth <= 0 || P2RemainingHandHealth <= 0)
        // {
        //     PVPManager.manager.isAllIn = true;
        //     PVPManager.manager.IsAnyAllIn = true;
        //     PVPManager.manager.SyncAllIn(true);
        //     PVPManager.manager.isNormalBat = true;
        //     PVPManager.manager.isFromInbetween = true;
        //     PVPManager.manager.isAutoTurn = false;
        // }

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
                p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);
            }
            else
            {
                p1AttackFor.text = (Game.Get().lastAction == PlayerAction.counterAttack) ? "Counter attack for " + MyLastAttackAmount
                    : "Attack For " + MyLastAttackAmount;
                UpdateBetForPlayer(MyLastAttackAmount);
                photonView.RPC("UpdateAttackForText", RpcTarget.Others, MyLastAttackAmount, Game.Get().lastAction == PlayerAction.counterAttack, false);
                p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one, 0.3f);
            }

            //if(!Game.Get().IsDefender)
            //  {
            //photonView.RPC("UpdatePotAmountForText",RpcTarget.All,(int)attackSlider._slider.value);
            photonView.RPC("UpdatePotAmountForText", RpcTarget.All, P2LastAttackValue);
            //  }
            Invoke("ResetText", 3f);
        }
        if (!isNormalBat && !isCheck && !isfold && !isReraiseAfterOnce)
        {
            //if(PVPManager.manager.P1HealthBar.value == Game.Get().BetAmount)
            //{
            //    PVPManager.manager.isAllIn = true;
            //}
            EndTurn(choice);

            //Debug.LogError("**END TURN CALLED from IF CONDITION");
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
        //if(Game.Get().lastAction == PlayerAction.attack || Game.Get().lastAction == PlayerAction.counterAttack)
        //{
        //    if(Game.Get().lastAction == PlayerAction.attack && Game.Get().turn>=2)
        //    {
        //        UpdateDataForReraise();
        //        Debug.LogError("END TURN CALLED ");
        //    }
        //    UpdateRangeCounter();
        //    UpdateRangePoint(rangeCounter);
        //}
        EndTurnBtn.gameObject.SetActive(false);
        if (isAttackViaSpeedPoints)
        {
            float maxVal = Mathf.Min(PVPManager.manager.P1RemainingHandHealth, PVPManager.manager.P2RemainingHandHealth);
            float perSpeedPoint = maxVal * 0.1f;
            float usedPoints = speedAttackSlider.value / perSpeedPoint;
            Debug.Log("SPEED POINTs " + p1Speed + " --Used Points  " + usedPoints);

            p1Speed -= usedPoints;
            if (p1Speed < 0) p1Speed = 0;
            P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString();
            photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);

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
            photonView.RPC("SwitchPVPTurn", RpcTarget.All);
        //p1AttackFor.gameObject.transform.parent.GetComponent<RectTransform>().LeanScale(Vector3.one,0.3f);

        LastActionUpdated = false;
        SpellManager.PetAlreadyAttacked = false;
    }


    [PunRPC]
    public void UpdateSpeedPoints(float points)
    {
        p2Speed = points;
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString();
    }
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
                // photonView.RPC("RPC_SetLastAttackerColor",RpcTarget.All,Game.Get().GetCurrentPlayer());
                // Game.Get().IsDefender = false;
                //   photonView.RPC("RPC_SetOthersAsDefenders",RpcTarget.Others,AttackSlider.instance._slider.value,true);
                //  Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
                photonView.RPC("RPC_otherplayerTurnPoker", RpcTarget.All, PhotonNetwork.LocalPlayer);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
                Game.Get().NextTurn();


                //Game.Get().NextTurn();
                //AttackLocation type = AttackData.GetLocationFrmInt(c);
                //playerChoice.defendLoc = type;
                //ModePanel.SetActive(false);
                //waitPanel.SetActive(true);
                //AttackChoices.SetActive(false);
                //LocationChoices.SetActive(true);
                //int[] extraChoices = new int[playerChoice.ExtraAttack != null ? playerChoice.ExtraAttack.Count : 0];
                //for (int i = 0; i < extraChoices.Length; i++)
                //{
                //    extraChoices[i] = (int)playerChoice.ExtraAttack[i];
                //}

                //object[] data = new object[] { p1Speed, p2Speed };

                //Debug.Log($" <color=yellow> playerChoice.attackLoc {playerChoice.attackLoc} playerChoice.attack {playerChoice.attack} playerChoice.defendLoc {playerChoice.defendLoc} </color>");

                //int counterAttackRandomForBothPlayers = UnityEngine.Random.Range(1, 10);
                //int criticalhits = UnityEngine.Random.Range(1, 100);

                //photonView.RPC("SyncPlayerChoice", RpcTarget.All, (int)playerChoice.attackLoc, (int)playerChoice.attack);
                //Game.Get().NextTurn();
                //Debug.Log("In Master sp = "+p1Speed +" l needed : "+lowestSpeedNeeded);
                //if (p1Speed >= lowestSpeedNeeded)
                //{
                //    state = PVPStates.ExtraAttack;
                //    skipTurn.gameObject.SetActive(true);
                //    AttackChoices.SetActive(true);
                //    Debug.Log("AttackChoices true");
                //    LocationChoices.SetActive(false);
                //    ModeTxT.text = "Choose Extra Attack Type";
                //    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                //    {
                //        item.button.ctable = item.data.speed <= p1Speed;
                //        if (item.data.type != AttackType.None)
                //            item.ComboTxt.text = item.data.type.ToString();
                //        if (item.data.type == AttackType.Defend)
                //        {
                //            item.gameObject.SetActive(false);

                //            Debug.Log($"<color=yellow> defend button false here .. {item.name}</color>");
                //        }
                //    }
                //    //SetModePanelOptions("ExtraAttack");
                //    Debug.Log("EXTRA DONE");
                //}
                //else
                //{
                //state = PVPStates.DefendLoc;
                //AttackChoices.SetActive(false);
                //LocationChoices.SetActive(true);
                //skipTurn.gameObject.SetActive(false);
                //ModeTxT.text = "Choose Defend Location";
                //foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                //{
                //    item.gameObject.SetActive(true);
                //    item.button.interactable = true;
                //}
                //SetModePanelOptions("DefendLoc");
                //}
            }
            else if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                photonView.RPC("RPC_UpdateTurn", RpcTarget.All);
                // photonView.RPC("RPC_SetLastAttackerColor",RpcTarget.All,Game.Get().GetCurrentPlayer());
                //  Game.Get().IsDefender = false;
                // photonView.RPC("RPC_SetOthersAsDefenders",RpcTarget.Others,AttackSlider.instance._slider.value,true);

                //Game.Get().NextTurn();

                photonView.RPC("RPC_otherplayerTurnPoker", RpcTarget.All, PhotonNetwork.LocalPlayer);
                Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();

                Game.Get().NextTurn();
                //Game.Get().NextTurn();
                //ModePanel.SetActive(false);
                //waitPanel.SetActive(true);
                //AttackChoices.SetActive(false);
                //LocationChoices.SetActive(true);
                //int[] extraChoices = new int[playerChoice.ExtraAttack != null ? playerChoice.ExtraAttack.Count : 0];
                //for (int i = 0; i < extraChoices.Length; i++)
                //{
                //    extraChoices[i] = (int)playerChoice.ExtraAttack[i];
                //}

                //object[] data = new object[] { p1Speed, p2Speed };

                //Debug.Log($" <color=yellow> playerChoice.attackLoc {playerChoice.attackLoc} playerChoice.attack {playerChoice.attack} playerChoice.defendLoc {playerChoice.defendLoc} </color>");

                //int counterAttackRandomForBothPlayers = UnityEngine.Random.Range(1, 10);
                //int criticalhits = UnityEngine.Random.Range(1, 100);

                //photonView.RPC("SyncPlayerChoice", RpcTarget.All, (int)playerChoice.attackLoc, (int)playerChoice.attack);
                //Game.Get().NextTurn();
                //if (p2Speed >= lowestSpeedNeeded)
                //{
                //    state = PVPStates.ExtraAttack;
                //    skipTurn.gameObject.SetActive(true);
                //    AttackChoices.SetActive(true);
                //    Debug.Log("AttackChoices true");
                //    LocationChoices.SetActive(false);
                //    ModeTxT.text = "Choose Extra Attack Type";
                //    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                //    {
                //        item.button.interactable = item.data.speed <= p2Speed;
                //        if (item.data.type != AttackType.None)
                //            item.ComboTxt.text = item.data.type.ToString();
                //        if (item.data.type == AttackType.Defend)
                //        {
                //            item.gameObject.SetActive(false);
                //            Debug.Log($"<color=yellow> defend button false here .. {item.name}</color>");
                //        }
                //    }
                //    //SetModePanelOptions("ExtraAttack");
                //}
                //else
                //{
                //    state = PVPStates.DefendLoc;
                //    AttackChoices.SetActive(false);
                //    LocationChoices.SetActive(true);
                //    skipTurn.gameObject.SetActive(false);
                //    ModeTxT.text = "Choose Defend Location";
                //    foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                //    {
                //        item.gameObject.SetActive(true);
                //        item.button.interactable = true;
                //    }
                //    //SetModePanelOptions("DefendLoc");
                //}
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
                //SetModePanelOptions("DefendLoc");
            }
            //Debug.Log(state);
            //state = PVPStates.Defend;

            //ModeTxT.text = state.ToString();

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
                    //SetModePanelOptions("DefendLoc");
                }
            }

            //state = PVPStates.Defend;
            //ModeTxT.text = state.ToString();
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
            //photonView.RPC("SyncPlayerChoice",RpcTarget.All,(int)playerChoice.attackLoc,(int)playerChoice.attack,(int)playerChoice.defendLoc,extraChoices,data,counterAttackRandomForBothPlayers,criticalhits);
        }
        #endregion
    }

    public void StopTimer()
    {
        starttimer = false;
        StopCoroutine("UpdateEndTurnTimer");
        EndTurnBtn.gameObject.SetActive(false);
    }

    public void SyncAllIn(bool b)
    {
        photonView.RPC("SyncAllInRPC", RpcTarget.All, b);
    }

    [PunRPC]
    public void SyncAllInRPC(bool b)
    {
        PVPManager.manager.IsAnyAllIn = b;
    }

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

                photonView.RPC("UpdatePotAmountForAllInText", RpcTarget.All, P2StartHealth);
                //   }
                Invoke("ResetText", 3f);
            }
            else
            {
                photonView.RPC("UpdatePotAmountForText", RpcTarget.All, P2LastAttackValue);
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
    public void UpdateRangePoint(int ragePoints)
    {
        if (rangeCounter >= 3)
        {
            P1RageBar.value += RagePointReward;
            P1RageTxt.text = "Rage " + P1RageBar.value.ToString();
            photonView.RPC("UpdateRagePoints_RPC", RpcTarget.Others, RagePointReward);
            rangeCounter = 0;
        }

    }
    [PunRPC]
    public void UpdateRagePoints_RPC(int ragePoints)
    {
        P2RageBar.value += ragePoints;
        UpdateHMTxt();
    }
    [PunRPC]
    public void SetALLButtonsOff_RPC()
    {
        DemoManager.instance._pokerButtons.gameObject.SetActive(false);
    }
    [PunRPC]
    public void SetAutoTurnOnOff_RPC(bool isOn)
    {
        isAutoTurn = isOn;
    }
    public bool isAutoTurn = false;
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
            Photon.Pun.PhotonNetwork.SendAllOutgoingCommands();
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
    public void SetModePanelOptions(string s)
    {
        Debug.Log($"<color=yellow>  {s}  </color>");

        switch (s)
        {
            case "AttackLoc":
                Debug.Log("=========================================== Set Mode Panel Options  ");

                ModePanel.SetActive(true);
                skipTurn.gameObject.SetActive(false);
                waitPanel.SetActive(false);
                state = PVPStates.AttackLoc;
                //LocationChoices.SetActive(true);
                ModeTxT.text = "Choose Attack Location";
                break;
            case "Attack":
                state = PVPStates.Attack;
                //LocationChoices.SetActive(false);
                AttackChoices.SetActive(true);

                PVPManager.Get().speedAttackChoices.SetActive(false);
                Debug.Log("AttackChoices true");
                ModeTxT.text = "Choose Attack Type";
                foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                {
                    item.button.interactable = true;
                    item.gameObject.SetActive(true);
                    Debug.Log($"<color=yellow>  {item.name}  </color>");
                }
                break;
            case "ExtraAttack":
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
                    if (item.data.type == AttackType.Defend)
                    {
                        item.gameObject.SetActive(false);
                        Debug.Log($"<color=yellow> defend button false here .. {item.name}</color>");
                    }
                }
                break;
            case "DefendLoc":
                state = PVPStates.DefendLoc;
                skipTurn.gameObject.SetActive(false);
                AttackChoices.SetActive(false);
                ///LocationChoices.SetActive(true);
                ModeTxT.text = "Choose Defend Location";
                foreach (var item in GameObject.FindObjectsOfType<AttackSelection>())
                {
                    item.button.interactable = true;
                }
                break;
        }
    }

    #region commeted code
    // [PunRPC]
    // public void SyncPlayerData(int attack, int defend,int[] extraChoices){
    //     PlayerChoice playerChoice;
    //     playerChoice.attackLoc = AttackData.GetLocationFrmInt(attack);
    //     playerChoice.defendLoc = AttackData.GetLocationFrmInt(defend);
    //     playerChoice.ExtraAttack = new List<AttackType>();
    //     foreach (var item in extraChoices)
    //     {
    //         playerChoice.ExtraAttack.Add(AttackData.GetAttackTypeFrmInt(item));
    //     }

    //     if(isLocalPVPTurn && isLocalPVPFirstTurn){
    //         player1Choice.attackLoc = playerChoice.attackLoc;
    //         player1Choice.defendLoc = playerChoice.defendLoc;
    //         player1Choice.ExtraAttack = playerChoice.ExtraAttack;
    //     }else if(!isLocalPVPTurn && !isLocalPVPFirstTurn){
    //         player2Choice.attackLoc = playerChoice.attackLoc;
    //         player2Choice.defendLoc = playerChoice.defendLoc;
    //         player2Choice.ExtraAttack = playerChoice.ExtraAttack;
    //     }else if(!isLocalPVPTurn && isLocalPVPFirstTurn){
    //         player2Choice.attackLoc = playerChoice.attackLoc;
    //         player2Choice.defendLoc = playerChoice.defendLoc;
    //         player2Choice.ExtraAttack = playerChoice.ExtraAttack;
    //     }else if(isLocalPVPTurn && !isLocalPVPFirstTurn){
    //         player1Choice.attackLoc = playerChoice.attackLoc;
    //         player1Choice.defendLoc = playerChoice.defendLoc;
    //         player1Choice.ExtraAttack = playerChoice.ExtraAttack;
    //     }



    //     if(!isLocalPVPTurn && !isLocalPVPFirstTurn){
    //         // playerOtherChoice.attack = playerChoice.attack;
    //         // playerOtherChoice.defend = playerChoice.defend;
    //         photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
    //         SetModePanel();

    //     }

    //     if(!isLocalPVPTurn && isLocalPVPFirstTurn){
    //         // playerOtherChoice.attack = playerChoice.attack;
    //         // playerOtherChoice.defend = playerChoice.defend;
    //         state = PVPStates.Resolve;
    //         photonView.RPC("SwitchPVPTurn",RpcTarget.AllBuffered);
    //         photonView.RPC("Resolve",RpcTarget.AllBuffered);
    //     }


    // }
    #endregion

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

    void SwitchStartHandTurn(float delay){
        
        StartHandTurn = !StartHandTurn;
       // Debug.LogError("start turn changed : "+StartHandTurn);
        photonView.RPC("SwitchStartHandTurnRPC",RpcTarget.Others,delay);
        Invoke("SetModePanel",delay);
    }
    [PunRPC]
    public void SwitchStartHandTurnRPC(float delay){
        StartHandTurn = !StartHandTurn;
       // Debug.LogError("start turn changed : "+StartHandTurn);
        Invoke("SetModePanel",delay);
    }

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

        //TODO solve here enable the defend button 
        //for (int i = 0; i < AttackChoices.transform.childCount; i++)
        //{
        //    AttackChoices.transform.GetChild(i).gameObject.SetActive(true);
        //    //Debug.Log($"<color=yellow> defend button true here ..  {AttackChoices.transform.GetChild(i).gameObject.name} </color> ");
        //}


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

    public void SkipExtra()
    {
        state = PVPStates.DefendLoc;
        ModeTxT.text = "Choose Defend Location";
        skipTurn.gameObject.SetActive(false);
        AttackChoices.SetActive(false);
        //LocationChoices.SetActive(true);
    }



    [PunRPC]
    public void ShowplayerChoices()
    {
        waitPanel.SetActive(false);
        player1ChoiceTxt[0].text = player1Choice.attackLoc.ToString();
        player1ChoiceTxt[1].text = player1Choice.defendLoc.ToString();
        player2ChoiceTxt[0].text = player2Choice.attackLoc.ToString();
        player2ChoiceTxt[1].text = player2Choice.defendLoc.ToString();
        ChoiceDetails.SetActive(true);

    }
    #region oldCode
    // [PunRPC]
    // public void Resolve(int counterAttackRandomForBothPlayers,int criticalhits)
    // {

    //     Debug.Log("========================= <color=yellow> resolve start </color>===================================");

    //     //Debug.Log($"<color=yellow> {name} health {p1Obj.character.health} </color>  high {p1Obj.high}" +
    //     //$"low {p1Obj.low} left {p1Obj.left} right {p1Obj.right} medle {p1Obj.medle}");


    //     waitPanel.SetActive(false);
    //     player1ChoiceTxt[0].text = player1Choice.attackLoc.ToString();
    //     player1ChoiceTxt[1].text = player1Choice.defendLoc.ToString();
    //     player2ChoiceTxt[0].text = player2Choice.attackLoc.ToString();
    //     player2ChoiceTxt[1].text = player2Choice.defendLoc.ToString();
    //     ChoiceDetails.SetActive(true);

    //     float player1Dmg = 0;
    //     float player2Dmg = 0;

    //     float extra1Dmg = 0;
    //     float extra2Dmg = 0;

    //     foreach (var item in player1Choice.ExtraAttack)
    //     {
    //         extra1Dmg += GameData.Get().GetAttack(item).damage;
    //     }
    //     foreach (var item in player2Choice.ExtraAttack)
    //     {
    //         extra2Dmg += GameData.Get().GetAttack(item).damage;
    //     }


    //     //////////////////////  same position of p1/p2 high and p1/p2 def     **** counter attack ****  ///////////////////////////////////////  
    //     foreach (AttackLocation item in Enum.GetValues(typeof(AttackLocation)))
    //     {
    //         if (item==player1Choice.attackLoc && item== player2Choice.defendLoc)
    //         {
    //             //Debug.Log("<color=yellow> Both player same position attack and defend </color>");

    //             if (counterAttackRandomForBothPlayers < 5)
    //             {
    //                 extra1Dmg = 20;
    //                 Debug.Log($"<color=yellow> player 1 is success heavy attack  counterAttackRandomForBothPlayers is {counterAttackRandomForBothPlayers}</color>");

    //                 StartCoroutine(P1TextAnimationExtraDamage(extra1Dmg + "-", Color.green, "Counter attack"));

    //             }

    //         }
    //         else if (item == player1Choice.defendLoc && item == player2Choice.attackLoc)
    //         {
    //             Debug.Log("<color=yellow> Both player same position attack and defend </color>");


    //             if (counterAttackRandomForBothPlayers < 5)
    //             {
    //                 extra2Dmg = 20;
    //                 Debug.Log($"<color=yellow> player 1 is success heavy attack  counterAttackRandomForBothPlayers is {counterAttackRandomForBothPlayers}</color>");
    //                 StartCoroutine(P2TextAnimationExtraDamage(extra2Dmg + "-", Color.green, "Counter attack"));
    //             }
    //         }            
    //     }



    //     //Debug.Log("Choices attack 1 : " + player1Choice.attack + " 2 : " + player2Choice.attack);
    //     //Debug.Log("Choices attack loc 1 : " + player1Choice.attackLoc + " 2 : " + player2Choice.attackLoc);
    //     //Debug.Log("Damage PrevExtras 1 : " + GameData.Get().GetAttack(player1Choice.attack).damage + " 2 : " + GameData.Get().GetAttack(player2Choice.attack).damage);

    //     if (GameData.Get().GetAttack(player1Choice.attack).damage ==null)
    //     {
    //         Debug.LogError("is null");
    //     }
    //     else
    //     {
    //         //Debug.Log("is not null GameData.Get().GetAttack(player1Choice.attack).damage");
    //     }
    //     player1Dmg = GameData.Get().GetAttack(player1Choice.attack).damage + extra1Dmg;
    //     player2Dmg = GameData.Get().GetAttack(player2Choice.attack).damage + extra2Dmg;

    //     //Debug.Log("Damage PrevFactors 1 : "+player1Dmg+" 2 : "+player2Dmg);

    //     ///////////////////////////////////////////////// *** high low left right middle *** //////////////////////////////////////////////////////////////

    //     if (player1Choice.attackLoc == AttackLocation.High || player2Choice.attackLoc==AttackLocation.High)
    //     {
    //         if (p1Obj.high<=0)
    //         {
    //             //Debug.Log($"player 1 damage increse {player1Dmg} {((player1Dmg * 5) / 100)}");
    //             player1Dmg += ((player1Dmg * 5) / 100);
    //             player2Dmg += ((player2Dmg * 5) / 100);
    //             //Debug.Log($"player 1 damage increse after {player1Dmg} player 2 {player2Dmg}");
    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * 5) / 100)+"+",Color.black,"High"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * 5) / 100)+"+", Color.black, "High"));
    //         }
    //         else
    //         {
    //             Debug.LogError($"player 1 damage increse {player1Dmg} {((player1Dmg * p1Obj.high) / 100)}");
    //             player1Dmg += ((player1Dmg * p1Obj.high) / 100);
    //             player2Dmg += ((player2Dmg * p2Obj.high) / 100);
    //             Debug.LogError($"player 1 damage increse after {player1Dmg} player 2 {player2Dmg}");
    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * p1Obj.high) / 100) + "+", Color.black, "High"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * p2Obj.high) / 100) + "+", Color.black, "High"));
    //         }

    //     }

    //     if (player1Choice.attackLoc == AttackLocation.Low || player2Choice.attackLoc==AttackLocation.Low)
    //     {
    //         if (player1Choice.attackLoc == AttackLocation.Low)
    //         {
    //             if (p2Obj.low<=0)
    //             {
    //                 p2Speed -= ((p2Speed * 5) / 100);
    //                 StartCoroutine(P2TextAnimationExtraDamage(((p2Speed * 5) / 100) + "-", Color.black, "Low"));
    //             }
    //             else
    //             {
    //                 p2Speed -= ((p2Speed * p2Obj.low) / 100);
    //                 StartCoroutine(P2TextAnimationExtraDamage(((p2Speed * p2Obj.low) / 100) + "-", Color.black, "Low"));
    //             }
    //         }

    //         if (player2Choice.attackLoc == AttackLocation.Low)
    //         {
    //             if (p1Obj.low <= 0)
    //             {
    //                 p1Speed -= ((p1Speed * 5) / 100);
    //                 StartCoroutine(P1TextAnimationExtraDamage((((p1Speed * 5) / 100) + "-"), Color.black, "Low"));
    //             }
    //             else
    //             {
    //                 p1Speed -= ((p1Speed * p1Obj.low) / 100);
    //                 StartCoroutine(P1TextAnimationExtraDamage((((p1Speed * p1Obj.low) / 100) + "-"), Color.black, "Low"));
    //             }
    //         }            
    //     }

    //     if (player1Choice.attackLoc == AttackLocation.Left || player2Choice.attackLoc==AttackLocation.Left)
    //     {
    //         if (p1Obj.left<=0)
    //         {
    //             player1Dmg -= ((player1Dmg * 5) / 100);
    //             player2Dmg -= ((player2Dmg * 5) / 100);

    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * 5) / 100) + "-", Color.black, "Left"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * 5) / 100) + "-", Color.black, "Left"));
    //         }
    //         else
    //         {
    //             player1Dmg -= ((player1Dmg * p1Obj.left) / 100);
    //             player2Dmg -= ((player2Dmg * p2Obj.left) / 100);

    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * p1Obj.left) / 100) + "-", Color.black, "Left"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * p2Obj.left) / 100) + "-", Color.black, "Left"));
    //         }
    //     }

    //     if (player1Choice.attackLoc == AttackLocation.Right || player2Choice.attackLoc==AttackLocation.Right)
    //     {
    //         if (p1Obj.right <= 0)
    //         {
    //             player1Dmg -= ((player1Dmg * 5) / 100);
    //             player2Dmg -= ((player2Dmg * 5) / 100);

    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * 5) / 100) + "-", Color.black, "Right"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * 5) / 100) + "-", Color.black, "Right"));
    //         }
    //         else
    //         {
    //             player1Dmg -= ((player1Dmg * p1Obj.right) / 100);
    //             player2Dmg -= ((player2Dmg * p2Obj.right) / 100);

    //             StartCoroutine(P1TextAnimationExtraDamage(((player1Dmg * p1Obj.right) / 100) + "-", Color.black, "Right"));
    //             StartCoroutine(P2TextAnimationExtraDamage(((player2Dmg * p2Obj.right) / 100) + "-", Color.black, "Right"));
    //         }
    //     }

    //     if (player1Choice.attackLoc == AttackLocation.Middle || player2Choice.attackLoc==AttackLocation.Middle)
    //     {
    //         if (player1Choice.attackLoc == AttackLocation.Middle)
    //         {
    //             if (p1Obj.medle<=0)
    //             {
    //                 P2StaBar.value -= ((P2StaBar.value * 5) / 100);

    //                 StartCoroutine(P2TextAnimationExtraDamage(((P2StaBar.value * 5) / 100) + "-", Color.black, "Middle"));
    //             }
    //             else
    //             {
    //                 P2StaBar.value -= ((P2StaBar.value * p1Obj.medle) / 100);
    //                 StartCoroutine(P2TextAnimationExtraDamage(((P2StaBar.value * p1Obj.medle) / 100) + "-", Color.black, "Middle"));
    //             }
    //         }

    //         if (player2Choice.attackLoc == AttackLocation.Middle)
    //         {
    //             if (p2Obj.medle<=0)
    //             {
    //                 P1StaBar.value -= ((P2StaBar.value * 5) / 100);

    //                 StartCoroutine(P1TextAnimationExtraDamage(((P1StaBar.value * 5) / 100) + "-", Color.black, "Middle"));
    //             }
    //             else
    //             {
    //                 P1StaBar.value -= ((P2StaBar.value * p2Obj.medle) / 100);
    //                 StartCoroutine(P1TextAnimationExtraDamage(((P2StaBar.value * p2Obj.medle) / 100) + "-", Color.black, "Middle"));
    //             }
    //         }
    //     }


    //     int player1weaknessfactor = p1Char.weakAgainst == p2Char.type ? (int) (0.2f * player1Dmg) : 0;
    //     int player1guessfactor = player1Choice.attackLoc == player2Choice.defendLoc ? (int) (0.2f * player1Dmg) : 0;
    //     int player1defend = player2Choice.attack == AttackType.Defend ? (int) (0.5f * player1Dmg) : 0;
    //     //Debug.Log("MidVals 1 : weakness - "+player1weaknessfactor+" guess - "+player1guessfactor+" def - "+player1defend);
    //     player1Dmg -= (player1weaknessfactor + player1guessfactor + player1defend);

    //     int player2weaknessfactor = p2Char.weakAgainst == p1Char.type ? (int) (0.2f * player2Dmg) : 0;
    //     int player2guessfactor = player2Choice.attackLoc == player1Choice.defendLoc ? (int) (0.2 * player2Dmg) : 0;
    //     int player2defend = player1Choice.attack == AttackType.Defend ? (int) (0.5f * player2Dmg) : 0;
    //     //Debug.Log("MidVals 2 : weakness - "+player2weaknessfactor+" guess - "+player2guessfactor+" def - "+player2defend);
    //     player2Dmg -= (player2weaknessfactor + player2guessfactor + player2defend);

    //     //Debug.Log("Damage 1 : "+player1Dmg+" 2 : "+player2Dmg);

    //     //////////////////////////////////// ***** critical hits ***** /////////////////////////////////////////////////        
    //     if (criticalhits <= 10)
    //     {
    //         player2Dmg *= 2;
    //         player1Dmg *= 2;

    //         StartCoroutine(P1TextAnimationExtraDamage((player2Dmg *= 2) + "+", Color.red, "Critical"));
    //         StartCoroutine(P2TextAnimationExtraDamage((player1Dmg *= 2) + "+", Color.red, "Critical"));

    //         Debug.Log("<color=yellow> IS CRITICAL HITS </color>");
    //     }


    //     P1HealthBar.value -= player2Dmg;
    //     P2HealthBar.value -= player1Dmg;

    //     P1StaBar.value -= GameData.Get().GetAttack(player1Choice.attack).StaCost;
    //     P2StaBar.value -= GameData.Get().GetAttack(player2Choice.attack).StaCost;

    //     if(player1Choice.attack == AttackType.Heavy){
    //         if(P1HeavyComboIndex == 1){
    //             P1HealthBar.value += 5;
    //         }else if (P1HeavyComboIndex == 2){
    //             P1RageBar.value += 20;
    //         }
    //     }else if(player1Choice.attack == AttackType.Speed){
    //         if(P1SpeedComboIndex == 1){
    //             P1StaBar.value += 10;
    //         }else if (P1SpeedComboIndex == 2){
    //             P1RageBar.value += 10;
    //         }
    //     }

    //     if(player2Choice.attack == AttackType.Heavy){
    //         if(P2HeavyComboIndex == 1){
    //             P2HealthBar.value += 5;
    //         }else if (P2HeavyComboIndex == 2){
    //             P2RageBar.value += 20;
    //         }
    //     }else if(player2Choice.attack == AttackType.Speed){
    //         if(P2SpeedComboIndex == 1){
    //             P2StaBar.value += 10;
    //         }else if (P2SpeedComboIndex == 2){
    //             P2RageBar.value += 10;
    //         }
    //     }


    //     float p1s=0, p2s=0;
    //     p1s = GameData.Get().GetAttack(player1Choice.attack).speed;
    //     p2s = GameData.Get().GetAttack(player2Choice.attack).speed;
    //     if(p1s > p2s){
    //         p1Speed += 0;
    //         p2Speed += Mathf.Abs(p1s - p2s);
    //     }else if(p2s > p1s){
    //         p1Speed += Mathf.Abs(p1s - p2s);
    //         p2Speed += 0;   
    //     }else{
    //         p1Speed += 0;
    //         p2Speed += 0;
    //     }

    //     //p1Speed -= GameData.Get().GetAttack(player1Choice.ExtraAttack).speed;
    //     //p2Speed -= GameData.Get().GetAttack(player2Choice.ExtraAttack).speed;

    //     UpdateHMTxt();
    //     UpdateSpeed();
    //     UpdateComboIndex();

    //     player1Choice.attackLoc = AttackLocation.None;
    //     player1Choice.defendLoc = AttackLocation.None;
    //     player2Choice.attackLoc = AttackLocation.None;
    //     player2Choice.defendLoc = AttackLocation.None;
    //     player1Choice.attack = AttackType.None;
    //     player2Choice.attack = AttackType.None;
    //     playerChoice.ExtraAttack = new List<AttackType>();


    //     if(PhotonNetwork.LocalPlayer.IsMasterClient)
    //         photonView.RPC("CheckWin",RpcTarget.All);

    //     Debug.Log("========================= <color=yellow> resolve end </color>===================================");
    // }

    #endregion
    public IEnumerator P1TextAnimationExtraDamage(string s, Color color, string attackName)
    {

        P1ExtraDamageAnimationText.gameObject.SetActive(true);
        P1ExtraDamageAnimationText.text = attackName + "  " + s;
        P2ExtraDamageAnimationText.color = color;
        P1ExtraDamageAnimationText.GetComponent<Animator>().Play("MoveCharacter");

        yield return new WaitForSecondsRealtime(3f);
        P1ExtraDamageAnimationText.text = "";
        P2ExtraDamageAnimationText.color = Color.black;
        P1ExtraDamageAnimationText.gameObject.SetActive(false);

    }
    public IEnumerator P2TextAnimationExtraDamage(string s, Color color, string attackName)
    {

        P2ExtraDamageAnimationText.gameObject.SetActive(true);
        P2ExtraDamageAnimationText.text = attackName + "  " + s;
        P2ExtraDamageAnimationText.color = color;
        P2ExtraDamageAnimationText.GetComponent<Animator>().Play("MoveCharacter");

        yield return new WaitForSecondsRealtime(3f);
        P2ExtraDamageAnimationText.text = "";
        P2ExtraDamageAnimationText.color = Color.black;
        P2ExtraDamageAnimationText.gameObject.SetActive(false);
    }


    public void UpdateComboIndex()
    {
        //Debug.Log("GETTING CALLED");
        if (player1Choice.attack == AttackType.Heavy)
        {
            P1SpeedComboIndex = 0;
            P1HeavyComboIndex++;
            P1HeavyComboIndex %= 3;
        }
        else if (player1Choice.attack == AttackType.Speed)
        {
            P1SpeedComboIndex++;
            P1HeavyComboIndex = 0;
            P1SpeedComboIndex %= 3;
        }
        else if (player1Choice.attack == AttackType.Defend)
        {
            P1HeavyComboIndex = 0;
            P1SpeedComboIndex = 0;
        }
        if (player2Choice.attack == AttackType.Heavy)
        {
            P2SpeedComboIndex = 0;
            P2HeavyComboIndex++;
            P2HeavyComboIndex %= 3;
        }
        else if (player2Choice.attack == AttackType.Speed)
        {
            P2SpeedComboIndex++;
            P2HeavyComboIndex = 0;
            P2SpeedComboIndex %= 3;
        }
        else if (player2Choice.attack == AttackType.Defend)
        {
            P1HeavyComboIndex = 0;
            P1SpeedComboIndex = 0;
        }

        foreach (var item in AttackSelection.attackSelections)
        {
            //Debug.Log("ATTACKS FOUND");
            if (item.data.type == AttackType.Heavy)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    item.UpdateComboTxt(P1HeavyComboIndex);
                }
                else if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    item.UpdateComboTxt(P2HeavyComboIndex);
                }
            }
            else if (item.data.type == AttackType.Speed)
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    item.UpdateComboTxt(P1SpeedComboIndex);
                }
                else if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    item.UpdateComboTxt(P2SpeedComboIndex);
                }
            }
        }
    }

    [PunRPC]
    public IEnumerator CheckWin()
    {
        P1StaBar.value += StaGainedPerMatch;
        P2StaBar.value += StaGainedPerMatch;
        yield return new WaitForSeconds(2f);
        bool PVPOver = false;
        bool player1Win = false;
        ChoiceDetails.SetActive(false);


        if (P1HealthBar.value <= 0)
        {
            player2.SetActive(false);
            player1.SetActive(false);
            winTxt.text = PhotonNetwork.PlayerList[1].NickName + " Wins !";
            winIm.sprite = p2Char.ChracterOppSp;
            winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = false;
            //if(isattackerMaster)
            //p2Obj.pData.health = (int)P2HealthBar.value;
            //p2Obj.pData.stamina = P2StaBar.value;
            Game.Get().OppoStamina = (int)P2StaBar.value;
            // else
            //     Game.Get().GetPosition((int)p1Pos.x, (int)p1Pos.y).GetComponent<Chessman>().characterHealth = (int)P2HealthBar.value;

            //TODO health bar display code here
            Debug.Log($"<color=yellow>   Health Update Here  player1 {p1Obj.pData.health}  player2 {p2Obj.pData.health}  healthbar2 {P2HealthBar.value} </color>");
            p2Obj.UpdateHealth(p2Obj.pData.health);
            if (p2Obj.GetComponent<IHealthBar>() != null)
            {
                Debug.Log("is not null");
                p2Obj.GetComponent<IHealthBar>().UpdateHealth(p2Obj.pData.health);
            }
            else
            {
                Debug.Log("is null IHealthBar");
            }

            if (p2Obj.GetComponent<ISaveHighLowLeftRightMedle>() != null)
            {
                p2Obj.high += 5;
                p2Obj.low += 5;
                p2Obj.left += 5;
                p2Obj.right += 5;
                p2Obj.medle += 5;

                Debug.Log($" is not null high {p2Obj.high} low {p2Obj.low} " +
                    $"left {p2Obj.left} right {p2Obj.right} medle {p2Obj.medle}  ");

                p2Obj.GetComponent<ISaveHighLowLeftRightMedle>().SaveHighLowLeftRightMedle(p2Obj.high, p2Obj.low, p2Obj.left
                    , p2Obj.right, p2Obj.medle);
            }
            else
            {
                Debug.Log("is null ISaveHighLowLeftRightMedle");
            }

        }
        else if (P2HealthBar.value <= 0)
        {
            player2.SetActive(false);
            player1.SetActive(false);
            winTxt.text = PhotonNetwork.PlayerList[0].NickName + " Wins !";
            winIm.sprite = p1Char.ChracterSp;
            winTxt.gameObject.SetActive(true);
            PVPOver = true;
            player1Win = true;
            //if(isattackerMaster)
            //p1Obj.pData.health = (int)P1HealthBar.value;
            //p1Obj.pData.stamina = P1StaBar.value;
            Game.Get().MyStamina = (int)P1StaBar.value;
            // else
            //     Game.Get().GetPosition((int)p2Pos.x, (int)p2Pos.y).GetComponent<Chessman>().characterHealth = (int)P1HealthBar.value;


            //TODO health bar display code here
            Debug.Log($"<color=yellow>   Health Update Here  player1 {p1Obj.pData.health}  player2 {p2Obj.pData.health}  healthbar2 {P2HealthBar.value} </color>");
            p1Obj.UpdateHealth(p1Obj.pData.health);
            if (p1Obj.GetComponent<IHealthBar>() != null)
            {
                Debug.Log("is not null");

                p1Obj.GetComponent<IHealthBar>().UpdateHealth(p1Obj.pData.health);
            }
            else
            {
                Debug.Log("is null");
            }
            if (p1Obj.GetComponent<ISaveHighLowLeftRightMedle>() != null)
            {
                p1Obj.high += 5;
                p1Obj.low += 5;
                p1Obj.left += 5;
                p1Obj.right += 5;
                p1Obj.medle += 5;

                Debug.Log($" is not null high {p1Obj.high} low {p1Obj.low} left {p1Obj.left} right {p1Obj.right} medle {p1Obj.medle}  ");
                p1Obj.GetComponent<ISaveHighLowLeftRightMedle>().SaveHighLowLeftRightMedle(p1Obj.high, p1Obj.low, p1Obj.left
                    , p1Obj.right, p1Obj.medle);
            }
            else
            {
                Debug.Log("is null ISaveHighLowLeftRightMedle");
            }
        }
        else
        {

            if (isLocalPVPTurn)
            {
                Debug.Log(" == set mode panel here == ");
                //SetModePanel();

            }
            else
            {
                //ModePanel.SetActive(false);
                waitPanel.SetActive(true);
            }
        }

        if (PVPOver)
        {
            yield return new WaitForSeconds(2f);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                // if(isLocalPVPFirstTurn)
                //     Game.Get().HandleWin(player1Win,isattackerMaster,p1Pos,p2Pos);
                // else
                //     Game.Get().HandleWin(!player1Win,isattackerMaster,p1Pos,p2Pos);
            }

        }

    }

    

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

            //DemoManager.instance.ResetnumCards();
            ResetData();
            //SetModePanel();
            //if(!isPlayer1Winner && Game.Get()._currnetTurnPlayer == PhotonNetwork.LocalPlayer)
            //{
            //    Game.Get().NextTurn();
            //}
            if(PhotonNetwork.LocalPlayer.IsMasterClient)
                SwitchStartHandTurn(0f);
            yield return new WaitForSeconds(1f);
            // if (Game.Get()._currnetTurnPlayer == PhotonNetwork.LocalPlayer)//&& isPlayer1Winner)
            // {
            //     //Debug.LogError("**Set Mode Panel from here 1");
            //     SetModePanel();
            //     EndTurnBtn.gameObject.SetActive(true);
            //     StartTimer();
            // }
            // else //if(Game.Get()._currnetTurnPlayer == PhotonNetwork.LocalPlayer && !isPlayer1Winner)
            // {
            //     // ModePanel.SetActive(false);
            //     //Debug.LogError("**Set Wait Panel from here 2");
            //     waitPanel.SetActive(true);
            // }
            
            //Invoke("SetModePanel",0.5f);
            //Debug.LogError("Else part");

        }
    }

    public bool isCheckWithoutReset;
    public bool PVPOver = false;
    
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


    public Chessman myObj = null, opponentObj = null;
    CharacterData myChar = null, opponentChar = null;


    public static PVPManager Get()
    {
        return manager;
    }

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
        //foreach (var item in p1Name)
        //{
        //    item.text = PhotonNetwork.PlayerList[0].NickName;    
        //}
        //foreach (var item in p2Name)
        //{
        //    item.text = PhotonNetwork.PlayerList[1].NickName;    
        //}

        p1Pos = posP1;
        p2Pos = posP2;

        p1Obj = Game.Get().GetPosition((int)posP1.x, (int)posP1.y).GetComponent<Chessman>();
        p2Obj = Game.Get().GetPosition((int)posP2.x, (int)posP2.y).GetComponent<Chessman>();

        //Debug.LogError("CALLED SETTING DATA RPC BOTH CLIENTS +++++++++++++++++++++++++++++++++");
        //if(!Game.Get().IsDefender) 
        //{
        //    myPiece = p2Obj.type;
        //    opponentpiece = p1Obj.type;
        //}
        //else 
        //{
        //    opponentpiece = p2Obj.type;
        //    myPiece = p1Obj.type;
        //}

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


        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString();
        photonView.RPC("UpdateSpeedPoints", RpcTarget.Others, p1Speed);


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

        foreach (var item in myObj.cards)
        {
            SpellManager.instance.spellCardsDeck.Add(item);
        }
        //   Debug.LogError(SpellManager.instance.spellCardsDeck.Count + " cards added");


        SpellManager.instance.ResetData();
        

        // StartTimer();
        // EndTurnBtn.gameObject.SetActive(true);

        //Debug.Log($"<color=yellow> {name} health {p1Char.health} </color>  high {p1Obj.high}" +
        //$"low {p1Obj.low} left {p1Obj.left} right {p1Obj.right} medle {p1Obj.medle}");
    }

    public IEnumerator SpawnPets(){
        
        for (int i = 0; i < startNumCards; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpellManager.instance.DrawCard();
        }
    }



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

    public void UpdateSpeed()
    {
        P1SpeedTxt.text = MathF.Round(p1Speed, 2).ToString();
        P2SpeedTxt.text = MathF.Round(p2Speed, 2).ToString();
    }

    [PunRPC]
    public void SwitchPVPTurn()
    {
        isLocalPVPTurn = !isLocalPVPTurn;
        //IsPetTurn = isLocalPVPTurn;
        //SpellManager.PetAlreadyAttacked = false;
    }

    [PunRPC]
    public void SetPVPTurn(bool b)
    {
        isLocalPVPTurn = b;
    }
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
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            SwitchStartHandTurn(3f);

        //Invoke("SetModePanel", 3f);
    }

    public void UpdateHealthTextP1()
    {
        P1HealthTxt.text = ((int)P1HealthBar.value + "/" + P1HealthBar.maxValue).ToString();
    }
    public void UpdateStaminaTextP1()
    {
        //Debug.LogError("*1Stamina Changed From Here " + P1StaBar.value);
        // P1StaTxt.text = P1StaBar.value + "/" + P1StaBar.maxValue;
    }
    public void UpdateRageTextP1()
    {
        P1RageTxt.text = "Rage" + ((int)P1RageBar.value).ToString();
    }
    public void UpdateHealthTextP2()
    {
        P2HealthTxt.text = ((int)P2HealthBar.value + "/" + P2HealthBar.maxValue);
    }
    public void UpdateStaminaTextP2()
    {

        //Debug.LogError("*1Stamina 2 Changed From Here " + P2StaBar.value);
        // P2StaTxt.text =   P2StaBar.value + "/" + P2StaBar.maxValue;
    }
    public void UpdateRageTextP2()
    {
        P2RageTxt.text = "Rage" + ((int)P2RageBar.value).ToString();//+ "/" + P1HealthBar.maxValue;
        UpdateHMTxt();
    }
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
    public void UpdateAttackLocation(AttackLocation location)
    {
        playerAttackLocation = location;
        photonView.RPC("UpdateOpponentAttackLocation_RPC", RpcTarget.Others, location);
    }
    [PunRPC]
    public void UpdateOpponentAttackLocation_RPC(AttackLocation location)
    {
        opponentAttackLocation = location;
    }
    public void UpdateDefenceLocation(AttackLocation location)
    {
        playerDefenceLocation = location;
        photonView.RPC("UpdateOpponentDefenceLocation_RPC", RpcTarget.Others, location);
    }
    [PunRPC]
    public void UpdateOpponentDefenceLocation_RPC(AttackLocation location)
    {
        opponentDefendLocation = location;
    }
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
    public void ReloadScene()
    {
        photonView.RPC("ReloadScene_RPC", RpcTarget.All);
    }
    [PunRPC]
    public void ReloadScene_RPC()
    {
        photonView.RPC("Rset_BOOL_RPC", RpcTarget.All, true);
        PhotonNetwork.SendAllOutgoingCommands();
        if (PhotonNetwork.IsMasterClient)
            ResetData();
    }
    public Image myPieceImg, oppPiece;

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
}
