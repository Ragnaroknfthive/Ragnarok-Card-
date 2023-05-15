using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

/*Bet = Attack
Reraise = Counter Attack
Call = Engauge
Fold = Brace + 5% stamina
Check = Defend = + 5% stamina
*/

public class PokerButtonManager : MonoBehaviour
{
    public static PokerButtonManager instance;
    private PhotonView _pv;

    public Button bet_attack, Reraise_CouterAttack, call_Engauge, fold_Brace_5_Stamina, check_Defend_5_Stamin, allIn_btn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    public void Fold()
    {
        //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;

        Debug.Log("fold brace button click lose this player health and change turn");
        DemoManager.instance._pokerButtons.SetActive(false);
        PVPManager.Get().AttackChoices.SetActive(false);
        // PVPManager.Get().waitPanel.SetActive(true);
        // Game.Get().NextTurn();
        PVPManager.manager.UpdateBatTextFold(-1);

        //// PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
        //// PVPManager.manager.StartTimer();

        PVPManager.manager.isfold = true;
        
        Game.Get().UpdateLastAction(PlayerAction.brace);
        
    }

    public void Bet()
    {
        //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;
        //   Debug.Log("bet is attack open attack slider screen");
        DemoManager.instance._pokerButtons.SetActive(false);
        if (false) //Game.Get().turn < 1
        {
            //  Debug.LogError("Es ist not okay........................");
            PVPManager.Get().AttackChoices.SetActive(true);
            PVPManager.Get().speedAttackChoices.SetActive(false);


            // PVPManager.manager.UpdateRemainingHandHealth(PVPManager.manager.P2LastAttackValue);
            // PVPManager.manager.UpdateBatText(0);
            // PVPManager.manager.UpdateLocationChoices();
            // PVPManager.manager.isNormalBat = true;

            Game.Get().UpdateLastAction(PlayerAction.attack);
        }
        else
        {
            //if(PVPManager.manager.P1HealthBar.value >= Game.Get().BetAmount)
            //{
            //    if(PVPManager.manager.P1HealthBar.value == Game.Get().BetAmount)
            //    {
            //        PVPManager.manager.isAllIn = true;
            //    }
            //    PVPManager.manager.UpdateRemainingHandHealth(Game.Get().BetAmount);
            //    PVPManager.manager.UpdateBatText(0);
            //  //  PVPManager.manager.UpdateBatText(Game.Get().BetAmount);
            //    PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
            //    PVPManager.manager.isNormalBat = true;
            //    Game.Get().UpdateLastAction(PlayerAction.engage);
            //}
            //else
            //{
            //    Debug.Log("***Ex:Not Enought Health");
            //}

            //==LocalBet

            if (PVPManager.manager.P1HealthBar.value >= PVPManager.manager.P2LastAttackValue)
            {
                if (PVPManager.manager.P1HealthBar.value == PVPManager.manager.P2LastAttackValue || PVPManager.manager.P2RemainingHandHealth <= 0)
                {
                    PVPManager.manager.isAllIn = true;
                    PVPManager.manager.IsAnyAllIn = true;
                    PVPManager.manager.SyncAllIn(true);
                    PVPManager.manager.isFromInbetween = true;
                }

                //PVPManager.manager.DeductStamina(MathF.Round(PVPManager.manager.P2LastAttackValue / 10f,1));
                // Debug.LogError(Game.Get().lastAction);
                if (Game.Get().lastAction == PlayerAction.counterAttack)
                {
                    PVPManager.manager.UpdateBetForPlayer((PVPManager.manager.LastAtkAmt - PVPManager.manager.MyLastAttackAmount));
                    PVPManager.manager.UpdateRemainingHandHealth(PVPManager.manager.LastAtkAmt - PVPManager.manager.MyLastAttackAmount);
                }
                else
                {
                    PVPManager.manager.UpdateBetForPlayer((PVPManager.manager.P2LastAttackValue));
                    PVPManager.manager.UpdateRemainingHandHealth(PVPManager.manager.P2LastAttackValue);
                }

                //PVPManager.manager.UpdateBatText(0);
                //   PVPManager.manager.UpdateBatText(PVPManager.manager.P2LastAttackValue);  2-6 to avoid doubel value addition in Bet
                PVPManager.manager.UpdateBatText(0);
                // if(!PVPManager.Get().isLocalPVPFirstTurn && !PVPManager.Get().isDefenceLocationSelected){
                //     //PVPManager.Get().LocationChoices.SetActive(true);
                //     //PVPManager.Get().LocationChoiceHeading.SetActive(true);
                // }else{

                // }
                // PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
                // PVPManager.manager.StartTimer();
                PVPManager.manager.UpdateLocationChoices();

                PVPManager.manager.isNormalBat = true;
                Game.Get().UpdateLastAction(PlayerAction.engage);
            }
            else
            {
                //Debug.LogError("***Ex:Not Enought Health" + PVPManager.manager.P1HealthBar.value + " - " + PVPManager.manager.P2LastAttackValue);
            }
        }
            if((PVPManager.manager.isAttackLocationSelected && PVPManager.manager.IsAttacker) || (PVPManager.manager.isDefenceLocationSelected && !PVPManager.manager.IsAttacker))
                PVPManager.manager.OnClickEndTurn();
        // PVPManager.Get().AttackChoices.SetActive(true);
    }


    public void Attack()
    { //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;
        Debug.Log("bet is attack open attack slider screen");

        AttackSlider.instance.setAction(PlayerAction.attack);


        DemoManager.instance._pokerButtons.SetActive(false);

        PVPManager.Get().AttackChoices.SetActive(true);

        // if(PVPManager.Get().p1Speed > 0)
        //     PVPManager.Get().ShowSpeedAttackSlider();
        // else
        //     PVPManager.Get().speedAttackChoices.SetActive(false);
        //}
        //else
        //{
        //    if(PVPManager.manager.P1HealthBar.value >= Game.Get().BatAmount)
        //    {
        //        PVPManager.manager.UpdateBatText(Game.Get().BatAmount);
        //        PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
        //        PVPManager.manager.isNormalBat = true;
        //        Game.Get().UpdateLastAction(PlayerAction.engage);
        //    }
        //    else
        //    {
        //        Debug.Log("***Ex:Not Enought Health");
        //    }
        //}


        // PVPManager.Get().AttackChoices.SetActive(true);
    }
    public void Check()
    { //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;
        Debug.Log("bet is attack open attack slider screen");
        //Debug.LogError("Its checking");
        DemoManager.instance._pokerButtons.SetActive(false);
        if (Game.Get().turn <= 1)
        {
            PVPManager.Get().AttackChoices.SetActive(true);
            PVPManager.Get().speedAttackChoices.SetActive(false);
        }
        else
        {
            // if(!PVPManager.manager.isDefenceLocationSelected)
            // {
            //     PVPManager.Get().LocationChoices.SetActive(true);

            // }
            // else
            // {
            //     PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
            //     PVPManager.manager.StartTimer();
            // }

            PVPManager.manager.UpdateLocationChoices();

        }
        

        Game.Get().UpdateLastAction(PlayerAction.defend);
        PVPManager.manager.isCheck = true;
        
        PVPManager.manager.OnClickEndTurn();
        // PVPManager.Get().AttackChoices.SetActive(true);
    }
    public void Reraise()
    {
        //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;
        AttackSlider.instance.setAction(PlayerAction.counterAttack);
        Debug.Log("Reraise is attack open attack slider screen");
        DemoManager.instance._pokerButtons.SetActive(false);
        if (Game.Get().turn <= 1)
        {
            PVPManager.Get().AttackChoices.SetActive(true);
            PVPManager.Get().speedAttackChoices.SetActive(false);
            PVPManager.manager.isReraiseAfterOnce = true;
            PVPManager.manager.isNormalBat = false;
        }
        else
        {
            PVPManager.manager.isReraiseAfterOnce = true;
            PVPManager.Get().AttackChoices.SetActive(true);
            PVPManager.Get().speedAttackChoices.SetActive(false);
            //  PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
            PVPManager.manager.isNormalBat = false;
        }
        Game.Get().UpdateLastAction(PlayerAction.counterAttack);
    }

    [PunRPC]
    private void RPC_foldbuttonEffect(Player _player, int _localBetAmount)
    {
        bool isOver = false;

        PVPManager.manager.BetTextObj.text = "Brace Call";
        PVPManager.manager.StopTimer();
        //Debug.LogError("Attacks " + PVPManager.manager.P2LastAttackValue + " - " + PVPManager.manager.MyLastAttackAmount);
        if (PhotonNetwork.LocalPlayer.NickName == _player.NickName)
        {
            if(PVPManager.manager.P2StartHealth <= 0 && PVPManager.manager.P2LastAttackValue > 0)
                PVPManager.manager.P2StartHealth += PVPManager.manager.P2LastAttackValue;
            PVPManager.manager.P2HealthBar.value = PVPManager.manager.P2StartHealth; ;// PVPManager.manager.P2LastAttackValue;
            PVPManager.manager.P2RemainingHandHealth = PVPManager.manager.P2StartHealth;//PVPManager.manager.P2LastAttackValue;
                                                                                        //  Debug.LogError(PVPManager.manager.P2RemainingHandHealth + " PLAYER HEALTH ");
                                                                                        //if(Game.Get().turn < 2)
                                                                                        //{
                                                                                        //  int loseHP =(int)( Game.Get().BetAmount * 0.1f);
                                                                                        //  int maxlose = (int)(PVPManager.manager.P1HealthBar.maxValue * .5f);
            int finalLoseHealth = PVPManager.Get().myFoldAmount;  //lose 2 health and increase 1 stamina for fold
                                      //if(loseHP <= maxlose)
                                      //{
                                      //    finalLose = loseHP;
                                      //}
                                      //else
                                      //{
                                      //    finalLose = maxlose;
                                      //}
            float foldCost = 1f;

            PVPManager.manager.P1StaBar.value += foldCost;
            PVPManager.manager.P2StaVal += (PVPManager.manager.P2LastAttackValue / 10);
            PVPManager.manager.P2StaBar.value = PVPManager.manager.P2StaVal;

            // PVPManager.manager.P1HealthBar.value -= finalLose;
            //  if(PVPManager.manager.P1HealthBar.)
            // if (Game.Get().turn >= 2)
            // {
            //     finalLoseHealth = 0;
            //     //  finalLoseHealth = _localBetAmount+2;
            // }
            Debug.Log("LOCAL PLAYER BAT AMOUNT " + _localBetAmount + " Final Lose " + finalLoseHealth);
            PVPManager.manager.P1HealthBar.value -= finalLoseHealth;
            if (PVPManager.manager.P1HealthBar.value < 0)
            {
                PVPManager.manager.P1HealthBar.value = 0;
            }
            PVPManager.manager.P1StartHealth = (int)PVPManager.manager.P1HealthBar.value;
            PVPManager.manager.UpdateHMTxt();

            //}
            //else
            //{
            //    PVPManager.manager.P1HealthBar.value -= Game.Get().BetAmount;
            //    PVPManager.manager.UpdateHMTxt();
            //}
            if (PVPManager.manager.P1HealthBar.value == 0)
            {
                isOver = true;
            }
        }
        else
        {
            //if(Game.Get().turn < 2)
            //{
            //  int loseHP = (int)(Game.Get().BetAmount * 0.1f);
            //  int maxlose = (int)(PVPManager.manager.P2HealthBar.maxValue * .5f);
            int finalLose = PVPManager.Get().myFoldAmount;
            // if (Game.Get().turn >= 2)
            // {
            //     finalLose = 0;
            //     // finalLose = _localBetAmount + 2;
            // }
            Debug.Log("NON LOCAL PLAYER BAT AMOUNT " + _localBetAmount + " Final Lose " + finalLose);
            //if(loseHP <= maxlose)
            //{
            //    finalLose = loseHP;
            //}
            //else
            //{
            //    finalLose = maxlose;
            //}
            if(PVPManager.manager.P1StartHealth <= 0 && PVPManager.manager.MyLastAttackAmount > 0)
                PVPManager.manager.P1StartHealth += PVPManager.manager.MyLastAttackAmount;
            PVPManager.manager.P1RemainingHandHealth = PVPManager.manager.P1StartHealth;
            PVPManager.manager.P1HealthBar.value = PVPManager.manager.P1StartHealth;
            PVPManager.manager.P2HealthBar.value -= finalLose;
            if (PVPManager.manager.P2HealthBar.value < 0)
            {
                PVPManager.manager.P2HealthBar.value = 0;
            }
            PVPManager.manager.P2StartHealth = (int)PVPManager.manager.P2HealthBar.value;
            float foldCost = 1f;

            PVPManager.manager.P2StaBar.value += foldCost;
            PVPManager.manager.P1StaVal += (PVPManager.manager.P2LastAttackValue / 10);
            PVPManager.manager.P1StaBar.value = PVPManager.manager.P2StaVal;

            PVPManager.manager.UpdateHMTxt();
            //}
            //else
            //{
            //    PVPManager.manager.P1HealthBar.value -= Game.Get().BetAmount;
            //    PVPManager.manager.UpdateHMTxt();
            //}
            if (PVPManager.manager.P2HealthBar.value == 0)
            {
                isOver = true;
            }
            PVPManager.Get().myFoldAmount ++;
        }
        if (isOver)
        {
            PVPManager.manager.ResumeChessGame();
        }
        else
        {
            Invoke("Restart", 3f);
        }
        //you and your slowest pet both lose 1 health.       
    }
    public void Restart()
    {
        PVPManager.manager.RestartAfterFold();
    }
    public void FoldAction()
    {

        _pv.RPC("RPC_foldbuttonEffect", RpcTarget.All, PhotonNetwork.LocalPlayer, Game.Get().localBetAmount);
    }
    public void AllIn()
    { //Do not allow click if result is declared
        if (PVPManager.manager.isResultScreenOn) return;
        Debug.Log("bet is attack open attack slider screen");
        DemoManager.instance._pokerButtons.SetActive(false);


        PVPManager.manager.isAllIn = true;
        PVPManager.manager.IsAnyAllIn = true;
        PVPManager.manager.SyncAllIn(true);
        // PVPManager.manager.UpdateBatText((int)PVPManager.manager.P1HealthBar.value); //2-6 to avoid double value
        PVPManager.manager.UpdateBatText(0);
        PVPManager.manager.UpdateRemainingHandHealth((int)PVPManager.manager.P1HealthBar.value);
        //  PVPManager.manager.UpdateBatText(Game.Get().BetAmount);
        ////         PVPManager.manager.EndTurnBtn.gameObject.SetActive(true);
        //// PVPManager.manager.StartTimer();
        PVPManager.manager.isNormalBat = true;
        Game.Get().UpdateLastAction(PlayerAction.engage);

        PVPManager.manager.OnClickEndTurn();


        // PVPManager.Get().AttackChoices.SetActive(true);
    }
    public void SetAllButtonsOff()
    {
        Reraise_CouterAttack.gameObject.SetActive(false);

        bet_attack.gameObject.SetActive(false);

        allIn_btn.gameObject.SetActive(false);
        call_Engauge.gameObject.SetActive(false);
        fold_Brace_5_Stamina.gameObject.SetActive(false);
        check_Defend_5_Stamin.gameObject.SetActive(false);
    }
}
