using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class SpellManager : MonoBehaviourPunCallbacks
{
    public Transform spellCardsPlayer, spellCardsOpponent, spellCardBattleObj, opponentSpellBattleObject;
    public GameObject spellCardPrefab, spellBettleCardPrefeb;
    private GameObject tempSpellCardObj;
    public List<SpellCard> spellCardsDeck;
    public List<SpellCardDisplay> opponentCards = new List<SpellCardDisplay>();
    public List<SpellCardDisplay> playerCards = new List<SpellCardDisplay>();
    public List<BattleCardDisplay> opponentBattleCards = new List<BattleCardDisplay>();
    public List<BattleCardDisplay> playerBattleCards = new List<BattleCardDisplay>();

    public Transform showCaseParent, showCaseRef;

    public static SpellManager instance;
    public PhotonView photonView;

    public static bool IsPetAttacking;
    public static bool PetAlreadyAttacked;
    public Button PetAttk;
    public int Mana = 0;

    public GameObject MyMainDeck, OppoMainDeck;

    public List<int> spawned_ids = new List<int>();
    public GameObject DestroyCardEff, DestroyCardEffOppo;

    public static bool petAttackStarted;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetData()
    {
        spawned_ids = new List<int>();
        foreach (Transform item in spellCardsPlayer)
        {
            DestroyImmediate(item.gameObject);
        }
        //StartCoroutine(PVPManager.manager.SpawnPets());        
        photonView.RPC("ResetDataRPC", RpcTarget.Others);
    }

    [PunRPC]
    public void ResetDataRPC()
    {
        
        spawned_ids = new List<int>();
        foreach (Transform item in spellCardsPlayer)
        {
            DestroyImmediate(item.gameObject);
        }
        StartCoroutine(PVPManager.manager.SpawnPets());
    }

    public IEnumerator StartPetAttack()
    {
        SpellManager.petAttackStarted = true;
        playerBattleCards = playerBattleCards.OrderBy((item) => item.card.speed).ToList();
        opponentBattleCards = opponentBattleCards.OrderBy((item) => item.card.speed).ToList();
        //Debug.LogError(playerBattleCards.Count + " - "+ opponentBattleCards.Count);

        if (opponentBattleCards.Count > 0)
        {
            int id = 0;
            int max_len = Mathf.Max(playerBattleCards.Count, opponentBattleCards.Count);
            //Debug.LogError(max_len);

            foreach (var item in playerBattleCards)
            {
                if (item.IsAttackedThisRound) continue;
                if(PVPManager.manager.PVPOver) break;
                BattleCardDisplay oppo = null;
                while (id < opponentBattleCards.Count)
                {
                    oppo = opponentBattleCards[id];
                    if (oppo != null)
                    {
                        if (!oppo.IsDead)
                            break;
                        else
                            id++;

                    }
                    else
                    {
                        id++;
                    }
                }

                if (oppo != null)
                {
                    item.Attack(oppo.card.cardId);
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitWhile(() => IsPetAttacking);

                }
                else
                {
                    item.Attack(-1, true);
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitWhile(() => IsPetAttacking);

                }
                yield return new WaitForSeconds(0.5f);
                yield return new WaitWhile(() => IsPetAttacking);
                yield return new WaitWhile(()=>PVPManager.manager.isCheckWithoutReset);

            }
            #region  old code
            // for (int i = max_len - 1; i >= 0 ; i--)
            // {   
            //     BattleCardDisplay item = null;
            //     BattleCardDisplay oppo = null;
            //     if(i < playerBattleCards.Count)
            //         item = playerBattleCards[i];
            //     if(i < opponentBattleCards.Count)
            //         oppo = opponentBattleCards[i];

            //     Debug.LogError(i.ToString()+" - "+(oppo != null).ToString() + " - "+ (item != null).ToString());
            //     if(item != null){
            //         if(oppo != null){
            //             item.Attack(oppo.id);
            //             yield return new WaitWhile(()=>IsPetAttacking);
            //             yield return new WaitForSeconds(0.5f);
            //             // if(item.card.speed > oppo.card.speed){
            //             //     item.Attack(oppo.gameObject);
            //             //     yield return new WaitWhile(()=>IsPetAttacking);
            //             //     yield return new WaitForSeconds(0.5f);
            //             // }
            //             // else{
            //             //     oppo.Attack(item.gameObject);
            //             //     yield return new WaitWhile(()=>IsPetAttacking);
            //             //     yield return new WaitForSeconds(0.5f);
            //             // }
            //         }else{
            //             item.Attack(-1,true);
            //             yield return new WaitWhile(()=>IsPetAttacking);
            //             yield return new WaitForSeconds(0.5f);
            //         }
            //         // }else{
            //         //     if(i != 0){
            //         //         if(opponentBattleCards[i-1] != null){
            //         //             item.Attack(opponentBattleCards[i-1].gameObject);
            //         //             yield return new WaitWhile(()=>IsPetAttacking);
            //         //             yield return new WaitForSeconds(0.5f);
            //         //         }else{
            //         //             item.Attack(PVPManager.Get().gameObject);
            //         //             yield return new WaitWhile(()=>IsPetAttacking);
            //         //             yield return new WaitForSeconds(0.5f);
            //         //         }
            //         //     }
            //         // }
            //     }

            //     // if(oppo != null){
            //     //     if(item != null){
            //     //         if(oppo.card.speed > item.card.speed){
            //     //             oppo.Attack(item.gameObject);
            //     //             yield return new WaitWhile(()=>IsPetAttacking);
            //     //             yield return new WaitForSeconds(0.5f);
            //     //         }else{
            //     //             item.Attack(oppo.gameObject);
            //     //             yield return new WaitWhile(()=>IsPetAttacking);
            //     //             yield return new WaitForSeconds(0.5f);
            //     //         }
            //     //     }else{
            //     //         if(i!=0){
            //     //             if(playerBattleCards[i-1] != null){
            //     //                 oppo.Attack(playerBattleCards[i-1].gameObject);
            //     //                 yield return new WaitWhile(()=>IsPetAttacking);
            //     //                 yield return new WaitForSeconds(0.5f);
            //     //             }else{
            //     //                 oppo.Attack(PVPManager.Get().gameObject);
            //     //                 yield return new WaitWhile(()=>IsPetAttacking);
            //     //                 yield return new WaitForSeconds(0.5f);
            //     //             }
            //     //         }

            //     //     }
            //     // }

            // }
            #endregion
        }
        else if (playerBattleCards.Count > 0)
        {
            for (int i = playerBattleCards.Count - 1; i >= 0; i--)
            {
                BattleCardDisplay item = null;
                if(PVPManager.manager.PVPOver) break;
                if (i < playerBattleCards.Count)
                    item = playerBattleCards[i];

                if (item != null)
                {
                    item.Attack(-1, true);
                    yield return new WaitForSeconds(0.5f);
                    yield return new WaitWhile(() => IsPetAttacking);

                }
                yield return new WaitWhile(()=>PVPManager.manager.isCheckWithoutReset);

            }
        }
        SpellManager.petAttackStarted = false;
    }

    public void PetAttack()
    {
        if (PVPManager.Get().IsPetTurn && !PetAlreadyAttacked && Game.Get().turn > 2)
        {
            StartCoroutine(StartPetAttack());

        }
    }





    public void ExecuteAttack(int i, bool IsPlayer, int cardId, int attacker)
    {
        photonView.RPC("ExecuteAttackRPC", RpcTarget.Others, i, IsPlayer, cardId, attacker);
    }

    [PunRPC]
    public void ExecuteAttackRPC(int i, bool isplayer, int cardId, int attacker)
    {
        SpellCard card = GameData.Get().GetPet(cardId);
        SpellManager.IsPetAttacking = true;
        GameObject o = Instantiate(card.SpellProjectilePref, opponentBattleCards.Find(x => x.card.cardId == cardId).gameObject.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : playerBattleCards.Find(x => x.card.cardId == i).gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = isplayer;
        proj.DealDamage = !isplayer;
        proj.lifetime = 2f;
        PVPManager.manager.isCheckWithoutReset = true;
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));
    }

    public void DestroyOb(int i)
    {
        photonView.RPC("OnDestroyRPC", RpcTarget.Others, i);
    }

    [PunRPC]
    public void OnDestroyRPC(int cardId)
    {
        BattleCardDisplay battleCard = opponentBattleCards.Find(x => x.card.cardId == cardId);
        SpellManager.instance.opponentBattleCards.Remove(battleCard);
    }




    public int offset_card = 50;//50
    public GameObject InstantiateSpellCard(SpellCard sO, Vector3 pos, Transform parent)
    {
        tempSpellCardObj = Instantiate(spellCardPrefab, pos, Quaternion.identity, parent);
        tempSpellCardObj.GetComponent<SpellCardDisplay>().card = sO;

        return tempSpellCardObj;
    }

    public GameObject InstantiateSpellBattleCard(SpellCard sO, Vector3 pos, Transform parent, int num_card)
    {
        tempSpellCardObj = Instantiate(spellBettleCardPrefeb, pos, Quaternion.identity, parent);
        tempSpellCardObj.GetComponent<BattleCardDisplay>().card = sO;
        tempSpellCardObj.transform.localScale = Vector3.one;

        return tempSpellCardObj;
    }
    public void RemoveOldSpellData()
    {

        for (int i = 0; i < playerBattleCards.Count; i++)
        {
            if (playerBattleCards[i])
                Destroy(playerBattleCards[i].gameObject);
            // playerBattleCards.RemoveAt(i);

        }
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i])
                Destroy(playerCards[i].gameObject);
            // playerCards.RemoveAt(i);

        }
        for (int i = 0; i < opponentCards.Count; i++)
        {
            if (opponentCards[i])
                Destroy(opponentCards[i].gameObject);
            //  opponentCards.RemoveAt(i);

        }
        for (int i = 0; i < opponentBattleCards.Count; i++)
        {
            if (opponentBattleCards[i])
                Destroy(opponentBattleCards[i].gameObject);
            // opponentBattleCards.RemoveAt(i);

        }
        playerBattleCards.Clear();
        playerCards.Clear();
        opponentBattleCards.Clear();
        opponentCards.Clear();
    }

    public void DrawCard()
    {
        //Debug.LogError("cards holding now : " + spellCardsPlayer.transform.childCount);
        //Debug.LogError(spellCardsDeck.Count+" : " + spawned_ids.Count);
        if (spawned_ids.Count == spellCardsDeck.Count)
        {
            MyMainDeck.SetActive(false);
            photonView.RPC("SetDeckIm", RpcTarget.Others, false);
            return;
        }
        else
        {
            MyMainDeck.SetActive(true);
            photonView.RPC("SetDeckIm", RpcTarget.Others, true);
        }

        int i = Random.Range(0, spellCardsDeck.Count);
        while (spawned_ids.Contains(i))
        {
            i = Random.Range(0, spellCardsDeck.Count);
        }
        spawned_ids.Add(i);

        if (spellCardsPlayer.childCount == 9)
            DestroyCard(i);
        else
            GenerateCardsForPlayer(i);
    }

    [PunRPC]
    public void SetDeckIm(bool b)
    {
        OppoMainDeck.SetActive(b);
    }



    public void DestroyCard(int i)
    {
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(true);
        obj.transform.localScale = Vector3.one * 0.7f;
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;

        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>
        {
            PVPManager.manager.myObj.cards.Remove(spellCardsDeck[i]);
            if (PhotonNetwork.IsMasterClient)
                Instantiate(DestroyCardEff, spellCardsPlayer.position, Quaternion.identity);
            else
                Instantiate(DestroyCardEffOppo, spellCardsPlayer.position, Quaternion.identity);
            Destroy(obj.gameObject,0.1f);

            // obj.transform.SetParent(spellCardsPlayer);
            // obj.transform.position = Vector3.zero;
        });
        photonView.RPC("DestroyCardRPC", RpcTarget.Others, i);
    }

    [PunRPC]
    public void DestroyCardRPC(int i)
    {
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(false);
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;
        //obj.transform.Rotate(Vector3.forward * 90);
        // if (PhotonNetwork.IsMasterClient)
        // { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }
        // else { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }
        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true);
        obj.transform.localScale = Vector3.one * 0.7f;

        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>
        {
            // obj.transform.SetParent(spellCardsOpponent);
            // obj.transform.position = Vector3.zero;
            if (PhotonNetwork.IsMasterClient)
                Instantiate(DestroyCardEff, spellCardsOpponent.position, Quaternion.identity);
            else
                Instantiate(DestroyCardEffOppo, spellCardsOpponent.position, Quaternion.identity);
            Destroy(obj.gameObject,0.1f);
        });
    }


    public void GenerateCardsForPlayer(int i)
    {

        GameObject obj = InstantiateSpellCard(spellCardsDeck[i], Vector3.one, MyMainDeck.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(true);
        obj.transform.localScale = Vector3.one * 0.7f;
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;
        //obj.transform.Rotate(Vector3.forward * 270);
        // obj.transform.eulerAngles = new Vector3(0,0,0);
        if (PhotonNetwork.IsMasterClient == false)
            LeanTween.rotate(obj, new Vector3(0, 0, -180), 0);
        playerCards.Add(obj.GetComponent<SpellCardDisplay>());

        LeanTween.move(obj, spellCardsPlayer.position, 0.3f).setOnComplete(() =>
        {
            obj.transform.SetParent(spellCardsPlayer);
            obj.transform.position = Vector3.zero;
        });

        //Debug.Log("RPC CALLED ");
        photonView.RPC("GenerateCardsForOppnent", RpcTarget.Others, spellCardsDeck[i].cardId);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public IEnumerator CastSpell(int i)
    {
        SpellCard card = spellCardsDeck[i];
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack));
        PVPManager.Get().canContinue = false;
        yield return new WaitUntil(()=> PVPManager.Get().canContinue);
        // if(PVPManager.Get().RemainingAtk > 0){
        //     GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p1Image.gameObject.transform.position, Quaternion.identity);
        //     Projectile proj = o.GetComponent<Projectile>();
        //     proj.target = PVPManager.Get().p2Image.gameObject;
        //     proj.damage = card.Attack;
        //     proj.istargetPlayer = true;
        //     proj.DealDamage = true;
        //     proj.lifetime = 2f;
        //     PVPManager.manager.myObj.cards.Remove(card);
        //     photonView.RPC("CastSpellRPC", RpcTarget.Others, spellCardsDeck[i].cardId);
        // } 
        
    }

    public IEnumerator CastSpell(SpellCard card)
    {
        
        StartCoroutine(PVPManager.Get().DistributeSpellAttack(card.Attack));
        PVPManager.Get().canContinue = false;
        yield return new WaitUntil(()=> PVPManager.Get().canContinue);

    }

    [PunRPC]
    public void CastSpellWithCard()
    {
        SpellCard card = PVPManager.manager.p2Char.SpecialAttack;
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = PVPManager.Get().p1Image.gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = true;
        proj.DealDamage = false;
        proj.lifetime = 2f;
    }

    [PunRPC]
    public void CastSpellRPC(int i)
    {
        SpellCard card = GameData.Get().GetPet(i);
        GameObject o = Instantiate(card.SpellProjectilePref, PVPManager.Get().p2Image.gameObject.transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = PVPManager.Get().p1Image.gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = true;
        proj.DealDamage = false;
        proj.lifetime = 2f;
        Destroy(opponentCards.Find((item) => item.index == i).gameObject);
    }




    public void MoveOpponentCardToBattleArea(int cardId, int battleId)
    {
        photonView.RPC("ChangeOpponentCardPostionToCenter_RPC", RpcTarget.Others, cardId, battleId);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [Photon.Pun.PunRPC]
    public void ChangeOpponentCardPostionToCenter_RPC(int cardId, int battleId)
    {
        // if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ChangeOpponentCardPostionToCenter(battleId);

    }

    [PunRPC]
    public void GenerateCardsForOppnent(int i)
    {
        GameObject obj = InstantiateSpellCard(GameData.Get().GetPet(i), Vector3.one, OppoMainDeck.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(false);
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;
        //obj.transform.Rotate(Vector3.forward * 90);
        if (PhotonNetwork.IsMasterClient)
        { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }
        else { LeanTween.rotate(obj, new Vector3(0, 0, 180), 0); }

        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true);
        obj.transform.localScale = Vector3.one * 0.7f;
        opponentCards.Add(obj.GetComponent<SpellCardDisplay>());

        LeanTween.move(obj, spellCardsOpponent.position, 0.3f).setOnComplete(() =>
        {
            obj.transform.SetParent(spellCardsOpponent);
            obj.transform.position = Vector3.zero;
        });
        // Debug.LogError("CALLED RPC");

    }

    public void MouseOverOpponentCard(int cardId, bool isEnter = true)
    {

        //photonView.RPC("MouseEnter_RPC",RpcTarget.Others,cardId,isEnter);
        //PhotonNetwork.SendAllOutgoingCommands();
    }

    [Photon.Pun.PunRPC]
    public void MouseEnter_RPC(int cardId, bool isEnter)
    {
        // if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;
        if (isEnter)
        { SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ShowCaseCard(); }
        else
        {
            SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ResizeShowCaseCard();
        }

    }

    public bool isShowCasing = false;
}
