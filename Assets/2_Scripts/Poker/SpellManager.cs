////////////////////////////////////////////
/// SpellManager.cs
/// 
/// This script is responsible for managing the spell cards of the player and the opponent.
/// It also handles the attack of the pets.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using Unity.Collections;
using System;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class SpellManager : MonoBehaviourPunCallbacks
{
    #region Attributes
    //The following variables are used to manage the spell cards of the player and the opponent.
    public Transform spellCardsPlayer, spellCardsOpponent, spellCardBattleObj, opponentSpellBattleObject;
    public GameObject spellCardPrefab, spellBettleCardPrefeb;//
    private GameObject tempSpellCardObj;
    public List<SpellCard> spellCardsDeck;
    public List<SpellCardDisplay> opponentCards = new List<SpellCardDisplay>();
    public List<SpellCardDisplay> playerCards = new List<SpellCardDisplay>();
    public List<BattleCardDisplay> opponentBattleCards = new List<BattleCardDisplay>();
    public List<BattleCardDisplay> playerBattleCards = new List<BattleCardDisplay>();

    public Transform showCaseParent, showCaseRef;
    public int offset_card = 50;//The offset of the card
    public static SpellManager instance;
    public new PhotonView photonView;
    public bool isShowCasing = false;//Check if the card is showcasing
    public static bool IsPetAttacking;
    public static bool PetAlreadyAttacked;
    public Button PetAttk;
    public int Mana = 0;

    public GameObject MyMainDeck, OppoMainDeck;

    public List<int> spawned_ids = new List<int>();
    public GameObject DestroyCardEff, DestroyCardEffOppo;

    public static bool petAttackStarted;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();//Get the photon view component
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            photonView.RPC("hardCodeRPC", RpcTarget.Others);
        }
    }
    [PunRPC]
    private void hardCodeRPC()
    {
        print("hardCodeRPC");
        Destroy(opponentCards[0].gameObject);
        opponentCards.RemoveAt(0);
    }
    #endregion

    #region Data Reset
    public void ResetData()//Reset the data of the spell cards
    {
        print("Resetting data");
        spawned_ids = new List<int>();//Clear the spawned ids list
        foreach (Transform item in spellCardsPlayer)//Loop through the spell cards of the player
        {
            DestroyImmediate(item.gameObject);//Destroy the spell card game object
        }
        ClearList();
        photonView.RPC("ResetDataRPC", RpcTarget.Others);//Reset the data of the spell cards of the opponent
    }

    [PunRPC]
    public void ResetDataRPC()//Reset the data of the spell cards of the opponent
    {
        print("Resetting data RPC");
        spawned_ids = new List<int>();//Clear the spawned ids list
        foreach (Transform item in spellCardsPlayer)//Loop through the spell cards of the opponent
        {
            DestroyImmediate(item.gameObject);//Destroy the spell card game object
        }
        ClearList();
        StartCoroutine(PVPManager.manager.SpawnPets());//Spawn the pets
    }
    #endregion

    #region Pet Attack
    public IEnumerator StartPetAttack()//Start the attack of the pets
    {
        if (PVPManager.manager.isAllIn || PVPManager.manager.IsAnyAllIn)//Check if all the pets are in the battle
        {
            yield return null;//Return null
        }
        else
        {
            SpellManager.petAttackStarted = true;//Set the pet
            playerBattleCards = playerBattleCards.OrderBy((item) => item.card.speed).ToList();//Order the player battle cards by speed
            opponentBattleCards = opponentBattleCards.OrderBy((item) => item.card.speed).ToList();//Order the opponent battle cards by speed

            if (opponentBattleCards.Count > 0)//If the opponent battle cards count is greater than 0
            {
                int id = 0;//Set the id to 0
                int max_len = Mathf.Max(playerBattleCards.Count, opponentBattleCards.Count);//Get the maximum length of the player battle cards and the opponent battle cards
                foreach (var item in playerBattleCards)//Loop through the player battle cards
                {
                    if (item.IsAttackedThisRound) continue;//If the item is attacked this round, continue
                    if (PVPManager.manager.PVPOver) break;//If the PVP is over, break
                    BattleCardDisplay oppo = null;//Set the opponent to null
                    while (id < opponentBattleCards.Count)//While the id is less than the opponent battle cards count
                    {
                        oppo = opponentBattleCards[id];//Set the opponent to the opponent battle cards at the id
                        if (oppo != null)//If the opponent is not null
                        {
                            if (!oppo.IsDead)//If the opponent is not dead
                                break;//Break
                            else
                            {
                                id++;//Increment the id
                            }

                        }
                        else//If the opponent is null
                        {
                            id++;//Increment the id
                        }
                    }

                    if (oppo != null)//If the opponent is not null, then attack the opponent
                    {
                        item.Attack(oppo.card.cardId);//Attack the opponent
                        yield return new WaitForSeconds(0.5f);//Wait for 0.5 seconds
                        yield return new WaitWhile(() => IsPetAttacking);//Wait while the pet is attacking

                    }
                    else
                    {
                        item.Attack(-1, true);//Do this
                        yield return new WaitForSeconds(0.5f);//Wait for 0.5 seconds
                        yield return new WaitWhile(() => IsPetAttacking);//Wait while the pet is attacking

                    }
                    yield return new WaitForSeconds(0.5f);//Wait for 0.5 seconds
                    yield return new WaitWhile(() => IsPetAttacking);//Wait while the pet is attacking
                    yield return new WaitWhile(() => PVPManager.manager.isCheckWithoutReset);//Wait while the PVP manager is checking without resetting

                }
            }
            else if (playerBattleCards.Count > 0)//If the player battle cards count is greater than 0
            {
                for (int i = playerBattleCards.Count - 1; i >= 0; i--)//Loop through the player battle cards
                {
                    BattleCardDisplay item = null;//Set the item to null
                    if (PVPManager.manager.PVPOver) break;//If the PVP is over, break
                    if (i < playerBattleCards.Count)//If i is less than the player battle cards count
                        item = playerBattleCards[i];//Set the item to the player battle cards at i
                    if (item != null)//If the item is not null
                    {
                        item.Attack(-1, true);//Do this
                        yield return new WaitForSeconds(0.5f);//Wait for 0.5 seconds
                        yield return new WaitWhile(() => IsPetAttacking);//Wait while the pet is attacking

                    }
                    yield return new WaitWhile(() => PVPManager.manager.isCheckWithoutReset);//Wait while the PVP manager is checking without resetting
                }
            }
            SpellManager.petAttackStarted = false;//Set the pet attack started to false
        }

    }
    public void PetAttack()//Make the pet attack
    {
        if (PVPManager.Get().IsPetTurn && !PetAlreadyAttacked)//If it is the pet's turn and the pet has not already attacked
        {
            StartCoroutine(StartPetAttack());//Start the pet attack
        }
    }
    public void ExecuteAttack(int i, bool IsPlayer, int cardId, int attacker)//Function to execute the attack
    {
        print("Executing attack");
        ClearList();
        photonView.RPC("ExecuteAttackRPC", RpcTarget.Others, i, IsPlayer, cardId, attacker);//Execute the attack on the target
    }

    [PunRPC]
    public void ExecuteAttackRPC(int i, bool isplayer, int cardId, int attacker)//Function to execute the attack
    {
        print("Executing attack RPC");
        SpellCard card = GameData.Get().GetPet(cardId);//Get the spell card
        SpellManager.IsPetAttacking = true;//Set the pet attacking to true

        //Instantiate the spell projectile
        GameObject o = Instantiate(card.SpellProjectilePref, opponentBattleCards.Find(x => x.card.cardId == cardId).gameObject.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();//Get the projectile component

        //Set the target, damage, is target player, deal damage, and lifetime of the projectile
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : playerBattleCards.Find(x => x.card.cardId == i).gameObject;
        proj.damage = card.Attack;//Set the damage
        proj.istargetPlayer = isplayer;//Set the is target player
        proj.DealDamage = !isplayer;//Set the deal damage
        proj.lifetime = 2f;//Set the lifetime
        PVPManager.manager.isCheckWithoutReset = true;//Set the PVP manager to check without resetting
        ClearList();
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));//Check the win without resetting
    }
    #endregion

    #region Destroy Object
    public void DestroyOb(int i)//The function to destroy the object
    {
        print("Destroying object");
        ClearList();
        photonView.RPC("OnDestroyRPC", RpcTarget.Others, i);//Destroy the object
    }

    [PunRPC]
    public void OnDestroyRPC(int cardId)//The function to destroy the object
    {
        print("Destroying rpc");
        ClearList();
        BattleCardDisplay battleCard = opponentBattleCards.Find(x => x.card.cardId == cardId);
        SpellManager.instance.opponentBattleCards.Remove(battleCard);
        ClearList();
    }
    public void RemoveOldSpellData()//Function to remove the old spell data
    {
        //All the next scripts work in the same way
        for (int i = 0; i < playerBattleCards.Count; i++)//Loop through the player battle cards
        {
            if (playerBattleCards[i])//If the player battle cards at i is not null
            {
                Destroy(playerBattleCards[i].gameObject);
                print("Destroying player battle cards");
                /*if (playerBattleCards?.Any() ?? false)
                {
                    playerBattleCards.RemoveAll(item => item == null);
                }*/
                ClearList();
            }//Destroy the player battle cards at i
        }
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i])
            {
                Destroy(playerCards[i].gameObject);
                print("Destroying player cards");
                ClearList();
            }
        }
        for (int i = 0; i < opponentCards.Count; i++)
        {
            if (opponentCards[i])
            {
                Destroy(opponentCards[i].gameObject);
                print("Destroying opponent cards");
                ClearList();
            }
        }
        for (int i = 0; i < opponentBattleCards.Count; i++)
        {
            if (opponentBattleCards[i])
            {
                Destroy(opponentBattleCards[i].gameObject);
                print("Destroying opponent battle cards");
                ClearList();
            }
        }
        playerBattleCards.Clear();//Clear the player battle cards
        playerCards.Clear();//Clear the player cards
        opponentBattleCards.Clear();//Clear the opponent battle cards
        opponentCards.Clear();//Clear the opponent cards
    }
    #endregion

    #region Instantiate Spell Card
    public void DrawCard()//Function to draw the card
    {
        if (spawned_ids.Count == spellCardsDeck.Count)//If the spawned ids count is equal to the spell cards deck count
        {
            MyMainDeck.SetActive(false);//Set the main deck of the player to false
            photonView.RPC("SetDeckIm", RpcTarget.Others, false);//Set the deck image of the opponent
            return;
        }
        else
        {
            MyMainDeck.SetActive(true);//Set the main deck of the player to true
            photonView.RPC("SetDeckIm", RpcTarget.Others, true);//Set the deck image of the opponent
        }

        int i = Random.Range(0, spellCardsDeck.Count);//Get a random number between 0 and the spell cards deck count
        while (spawned_ids.Contains(i))
        {
            i = Random.Range(0, spellCardsDeck.Count);//Get a random number between 0 and the spell cards deck count
        }
        spawned_ids.Add(i);//Add the id to the spawned ids list

        if (spellCardsPlayer.childCount == 9)//If the spell cards player child count is 9
        {
            DestroyCard(i);
        }//Destroy the card
        else
        {
            GenerateCardsForPlayer(i);
        }//Generate the cards for the player
    }
    public GameObject InstantiateSpellCard(SpellCard sO, Vector3 pos, Transform parent)//Function to instantiate the spell card
    {
        print("Instantiating spell card");
        //Instantiate the spell card prefab
        tempSpellCardObj = Instantiate(spellCardPrefab, pos, Quaternion.identity, parent);
        //Set the card of the spell card object
        tempSpellCardObj.GetComponent<SpellCardDisplay>().card = sO;
        ClearList();
        return tempSpellCardObj;//Return the spell card object
    }
    //Function to instantiate the spell battle card
    public GameObject InstantiateSpellBattleCard(SpellCard sO, Vector3 pos, Transform parent, int num_card)
    {
        print("Instantiating spell battle card");
        tempSpellCardObj = Instantiate(spellBettleCardPrefeb, pos, Quaternion.identity, parent);//Instantiate the spell battle card prefab
        tempSpellCardObj.GetComponent<BattleCardDisplay>().card = sO;//Set the card of the spell card object
        tempSpellCardObj.transform.localScale = Vector3.one;//Set the scale of the spell card object
        ClearList();
        return tempSpellCardObj;//Return the spell card object
    }
    #endregion

    #region Destroy Card
    public void DestroyCard(int i)//Function to destroy the card
    {
        print("Destroying card");
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);//Instantiate the spell card
        obj.transform.localPosition = Vector3.zero;//Set the local position of the spell card to zero
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;//Set the card position of the spell card
        obj.GetComponent<SpellCardDisplay>().index = i;//Set the index of the spell card
        obj.GetComponent<SpellCardDisplay>().set(true);//Set the spell card
        obj.transform.localScale = Vector3.one * 0.7f;//Set the scale of the spell card
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;//Set the sorting order of the spell card

        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>//Move the spell card
        {
            PVPManager.manager.myObj.cards.Remove(spellCardsDeck[i]);//Remove the spell card from the player's object
            if (PhotonNetwork.IsMasterClient)//If the player is the master client
                Instantiate(DestroyCardEff, spellCardsPlayer.position, Quaternion.identity);//Instantiate the destroy card effect
            else
                Instantiate(DestroyCardEffOppo, spellCardsPlayer.position, Quaternion.identity);//Instantiate the destroy card effect
            Destroy(obj.gameObject, 0.1f);//Destroy the spell card game object
        });
        ClearList();
        photonView.RPC("DestroyCardRPC", RpcTarget.Others, i);//Destroy the card
    }

    [PunRPC]
    public void DestroyCardRPC(int i)//Destroy the card
    {
        ClearList();
        print("Destroying card RPC");
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);//Instantiate the spell card
        obj.transform.localPosition = Vector3.zero;//Set the local position of the spell card to zero
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;//Set the card position of the spell card
        obj.GetComponent<SpellCardDisplay>().index = i;//Set the index of the spell card
        obj.GetComponent<SpellCardDisplay>().set(false);//Set the spell card
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;//Set the sorting order of the spell card
        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true);//Set the back side of the spell card to true
        obj.transform.localScale = Vector3.one * 0.7f;//Set the scale of the spell card
        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>//Move the spell card
        {
            if (PhotonNetwork.IsMasterClient)
                Instantiate(DestroyCardEff, spellCardsOpponent.position, Quaternion.identity);//Instantiate the destroy card effect
            else
                Instantiate(DestroyCardEffOppo, spellCardsOpponent.position, Quaternion.identity);//Instantiate the destroy card effect
            Destroy(obj.gameObject, 0.1f);//Destroy the spell card game object
        });
    }
    #endregion

    #region Generate Cards
    public void GenerateCardsForPlayer(int i)
    {
        print("Generating cards for player");
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);//Instantiate the spell card
        obj.transform.localPosition = Vector3.zero;//Set the local position of the spell card to zero
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;//Set the card position of the spell card
        obj.GetComponent<SpellCardDisplay>().index = i;//Set the index of the spell card
        obj.GetComponent<SpellCardDisplay>().set(true);//Set the spell card
        obj.transform.localScale = Vector3.one * 0.7f;//Set the scale of the spell card
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;//Set the sorting order of the spell card
        if (PhotonNetwork.IsMasterClient == false)//If the player is not the master client
            LeanTween.rotate(obj, new Vector3(0, 0, -180), 0);//Rotate the spell card
        playerCards.Add(obj.GetComponent<SpellCardDisplay>());//Add the spell card to the player cards list
        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>//Move the spell card
        {
            obj.transform.SetParent(spellCardsPlayer);//Set the parent of the spell card to the spell cards player
            obj.transform.position = Vector3.zero;//Set the position of the spell card to zero
        });
        ClearList();
        photonView.RPC("GenerateCardsForOppnent", RpcTarget.Others, spellCardsDeck[i].cardId);//Generate the cards for the opponent
        PhotonNetwork.SendAllOutgoingCommands();//Send all the outgoing commands
    }
    [PunRPC]
    public void GenerateCardsForOppnent(int i)//Function to generate the cards for the opponent
    {
        ClearList();
        print("Generating cards for opponent");
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);//Instantiate the spell card
        obj.transform.localPosition = Vector3.zero;//Set the local position of the spell card to zero
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;//Set the card position of the spell card
        obj.GetComponent<SpellCardDisplay>().index = i;//
        obj.GetComponent<SpellCardDisplay>().set(false);//Set the spell card
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;//Set the sorting order of the spell card
        if (PhotonNetwork.IsMasterClient)//If the player is the master client
        { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }//Rotate the spell card
        else { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }//Rotate the spell card

        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true);//Set the back side of the spell card to true
        obj.transform.localScale = Vector3.one * 0.7f;//Set the scale of the spell card
        opponentCards.Add(obj.GetComponent<SpellCardDisplay>());//Add the spell card to the opponent cards list

        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>//Move the spell card
        {
            obj.transform.SetParent(spellCardsOpponent);//Set the parent of the spell card to the spell cards opponent
            obj.transform.position = Vector3.zero;//Set the position of the spell card to zero
        });
    }
    #endregion

    #region Cast Spell
    public IEnumerator CastSpell(int i)//Function to cast the spell by index
    {
        print("Casting spell_1");
        SpellCard card = spellCardsDeck[i];//Get the spell card
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack));//Distribute the spell attack
        PVPManager.Get().canContinue = false;//Set the can continue to false
        yield return new WaitUntil(() => PVPManager.Get().canContinue);//Wait until the PVP manager can continue
        ClearList();
    }

    /*public IEnumerator CastSpell(SpellCard card)//Function to cast the spell by card
    {
        print("Casting spell_2");
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack));//Distribute the spell attack
        PVPManager.Get().canContinue = false;//Set the can continue to false
        yield return new WaitUntil(() => PVPManager.Get().canContinue);//Wait until the PVP manager can continue
        ClearList();
    }*/
    public void GetIndexSpell(double ind)
    {
        foreach (SpellCardDisplay card in playerCards)
        {
            if(card.indexDouble == ind)
            {
                photonView.RPC("DestroyRPC", RpcTarget.Others, playerCards.IndexOf(card));
            }
        }
    }
    [PunRPC]
    public void DestroyRPC(int var)
    {
        print("Destroy RPC");
        Destroy(opponentCards[var].gameObject);
        opponentCards.RemoveAt(var);
    }

    [PunRPC]
    public void CastSpellWithCard()//Function to cast the spell with the card
    {
        SpellCard card = PVPManager.manager.p2Char.SpecialAttack;//Get the spell card
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity);//Instantiate the spell projectile
        Projectile proj = o.GetComponent<Projectile>();//Get the projectile component
        proj.target = PVPManager.Get().p1Image.gameObject;//Set the target of the projectile
        proj.damage = card.Attack;//Set the damage of the projectile
        proj.istargetPlayer = true;//Set the is target player of the projectile
        proj.DealDamage = false;//Set the deal damage of the projectile
        proj.lifetime = 2f;//Set the lifetime of the projectile
        print("Casting spell");
    }

    [PunRPC]
    public void CastSpellRPC(int i)//Function to cast the spell
    {
        print("Casting spellRPC______________________________________________________________");
        SpellCard card = GameData.Get().GetPet(i);//Get the spell card
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity);//Instantiate the spell projectile
        Projectile proj = o.GetComponent<Projectile>();//Get the projectile component
        proj.target = PVPManager.Get().p1Image.gameObject;//Set the target of the projectile
        proj.damage = card.Attack;//Set the damage of the projectile
        proj.istargetPlayer = true;//Set the is target player of the projectile
        proj.DealDamage = false;//Set the deal damage of the projectile
        proj.lifetime = 2f;//Set the lifetime of the projectile
        
        Destroy(opponentCards.Find((item) => item.index == i).gameObject);//Destroy the opponent cards
        ClearList();
    }

    #endregion

    #region Move Opponent Card To Battle Area
    public void MoveOpponentCardToBattleArea(int cardId, int battleId)//Function to move the opponent card to the battle area
    {
        print("Moving opponent card to battle area");
        ClearList();
        photonView.RPC("ChangeOpponentCardPostionToCenter_RPC", RpcTarget.Others, cardId, battleId);//Change the opponent card position to the center
        PhotonNetwork.SendAllOutgoingCommands();//Send all the outgoing commands
    }

    [Photon.Pun.PunRPC]
    public void ChangeOpponentCardPostionToCenter_RPC(int cardId, int battleId)//Function to change the opponent card position to the center
    {
        print("Changing opponent card position to centerRPC");
        ClearList();
        SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ChangeOpponentCardPostionToCenter(battleId);//Change the opponent card position to the center
    }
    [PunRPC]
    public void SetDeckIm(bool b)//Set the deck image
    {
        ClearList();
        print("Setting deck image");
        OppoMainDeck.SetActive(b);//Set the main deck of the opponent to b
    }
    #endregion

    #region Mouse Over Opponent Card
    public void MouseOverOpponentCard(int cardId, bool isEnter = true)//Empty function
    {

    }

    [Photon.Pun.PunRPC]
    public void MouseEnter_RPC(int cardId, bool isEnter)//Function to mouse enter
    {
        if (isEnter)
        { SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ShowCaseCard(); }//Show the case card
        else
        {
            SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ResizeShowCaseCard();//Resize the show case card
        }
    }
    #endregion
    public void ClearList()//Function to clear the list
    {
        if (opponentCards?.Any() ?? false)
        {
            int index = opponentCards.FindIndex(item => item == null);
            print("Index of null element of the list opponentCards: " + index);
            opponentCards.RemoveAll(item => item == null);
        }

        if (opponentBattleCards?.Any() ?? false)
        {
            int index = opponentBattleCards.FindIndex(item => item == null);
            print("Index of null element of the list opponentBattleCards: " + index);
            opponentBattleCards.RemoveAll(item => item == null);
        }

        if (playerCards?.Any() ?? false)
        {
            int index = playerCards.FindIndex(item => item == null);
            print("Index of null element of the list playerCards: " + index);
            playerCards.RemoveAll(item => item == null);
        }

        if (playerBattleCards?.Any() ?? false)
        {
            int index = playerBattleCards.FindIndex(item => item == null);
            print("Index of null element of the list playerBattleCards: " + index);
            playerBattleCards.RemoveAll(item => item == null);
        }

        if (spellCardsDeck?.Any() ?? false)
        {
            int index = spellCardsDeck.FindIndex(item => item == null);
            print("Index of null element of the list spellCardsDeck: " + index);
            spellCardsDeck.RemoveAll(item => item == null);
        }
    }
}