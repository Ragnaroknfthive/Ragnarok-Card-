using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class SpellManager : MonoBehaviourPunCallbacks
{
    public Transform spellCardsPlayer, spellCardsOpponent,spellCardBattleObj,opponentSpellBattleObject;
    public GameObject spellCardPrefab,spellBettleCardPrefeb;
    private GameObject tempSpellCardObj;
    public List<SpellCard> spellCardsDeck;
    public List<SpellCardDisplay> opponentCards=new List<SpellCardDisplay>();
    public List<SpellCardDisplay> playerCards = new List<SpellCardDisplay>();
    public List<BattleCardDisplay> opponentBattleCards = new List<BattleCardDisplay>();
    public List<BattleCardDisplay> playerBattleCards = new List<BattleCardDisplay>();

    public Transform showCaseParent,showCaseRef;
    
    public static SpellManager instance;
    public PhotonView photonView;

    public static bool IsPetAttacking;
    public static bool PetAlreadyAttacked;
    public Button PetAttk;
    public int Mana = 0;

    public GameObject MyMainDeck, OppoMainDeck;

    public List<int> spawned_ids = new List<int>();
    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        spellCardsDeck = new List<SpellCard>();
        string[] deckStr = PhotonNetwork.LocalPlayer.CustomProperties["PlayerDeck"].ToString().Split('_');
        foreach (var item in deckStr)
        {
            spellCardsDeck.Add(GameData.Get().GetSpell(System.Convert.ToInt32(item)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartPetAttack(){

        playerBattleCards = playerBattleCards.OrderBy((item)=>item.card.speed).ToList();
        opponentBattleCards = opponentBattleCards.OrderBy((item)=>item.card.speed).ToList();
        //Debug.LogError(playerBattleCards.Count + " - "+ opponentBattleCards.Count);
    
        if(opponentBattleCards.Count > 0){
            int id = 0;
            int max_len = Mathf.Max(playerBattleCards.Count, opponentBattleCards.Count);
            //Debug.LogError(max_len);

            for (int i = max_len - 1; i >= 0 ; i--)
            {   
                BattleCardDisplay item = null;
                BattleCardDisplay oppo = null;
                if(i < playerBattleCards.Count)
                    item = playerBattleCards[i];
                if(i < opponentBattleCards.Count)
                    oppo = opponentBattleCards[i];

                Debug.LogError(i.ToString()+" - "+(oppo != null).ToString() + " - "+ (item != null).ToString());
                if(item != null){
                    if(oppo != null){
                        item.Attack(oppo.id);
                        yield return new WaitWhile(()=>IsPetAttacking);
                        yield return new WaitForSeconds(0.5f);
                        // if(item.card.speed > oppo.card.speed){
                        //     item.Attack(oppo.gameObject);
                        //     yield return new WaitWhile(()=>IsPetAttacking);
                        //     yield return new WaitForSeconds(0.5f);
                        // }
                        // else{
                        //     oppo.Attack(item.gameObject);
                        //     yield return new WaitWhile(()=>IsPetAttacking);
                        //     yield return new WaitForSeconds(0.5f);
                        // }
                    }else{
                        item.Attack(-1,true);
                        yield return new WaitWhile(()=>IsPetAttacking);
                        yield return new WaitForSeconds(0.5f);
                    }
                    // }else{
                    //     if(i != 0){
                    //         if(opponentBattleCards[i-1] != null){
                    //             item.Attack(opponentBattleCards[i-1].gameObject);
                    //             yield return new WaitWhile(()=>IsPetAttacking);
                    //             yield return new WaitForSeconds(0.5f);
                    //         }else{
                    //             item.Attack(PVPManager.Get().gameObject);
                    //             yield return new WaitWhile(()=>IsPetAttacking);
                    //             yield return new WaitForSeconds(0.5f);
                    //         }
                    //     }
                    // }
                }

                // if(oppo != null){
                //     if(item != null){
                //         if(oppo.card.speed > item.card.speed){
                //             oppo.Attack(item.gameObject);
                //             yield return new WaitWhile(()=>IsPetAttacking);
                //             yield return new WaitForSeconds(0.5f);
                //         }else{
                //             item.Attack(oppo.gameObject);
                //             yield return new WaitWhile(()=>IsPetAttacking);
                //             yield return new WaitForSeconds(0.5f);
                //         }
                //     }else{
                //         if(i!=0){
                //             if(playerBattleCards[i-1] != null){
                //                 oppo.Attack(playerBattleCards[i-1].gameObject);
                //                 yield return new WaitWhile(()=>IsPetAttacking);
                //                 yield return new WaitForSeconds(0.5f);
                //             }else{
                //                 oppo.Attack(PVPManager.Get().gameObject);
                //                 yield return new WaitWhile(()=>IsPetAttacking);
                //                 yield return new WaitForSeconds(0.5f);
                //             }
                //         }
                        
                //     }
                // }

            }
        }else if(playerBattleCards.Count > 0){
            for (int i = playerBattleCards.Count - 1; i >= 0 ; i--)
            {   
                BattleCardDisplay item = null;
                
                if(i < playerBattleCards.Count)
                    item = playerBattleCards[i];

                if(item != null){
                    item.Attack(-1,true);
                    yield return new WaitWhile(()=>IsPetAttacking);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        
    }

    public void PetAttack(){
        if(PVPManager.Get().isLocalPVPTurn && !PetAlreadyAttacked){
            StartCoroutine(StartPetAttack());
            PetAlreadyAttacked = true;
        }
    }

    

    

    public void ExecuteAttack(int i,bool IsPlayer,int cardId,int attacker){
        photonView.RPC("ExecuteAttackRPC",RpcTarget.Others,i,IsPlayer,cardId,attacker);
    }

    [PunRPC]
    public void ExecuteAttackRPC(int i, bool isplayer,int cardId,int attacker){
        SpellCard card = spellCardsDeck.Find(x => x.cardId == cardId);
        SpellManager.IsPetAttacking = true;
        GameObject o = Instantiate(card.SpellProjectilePref,opponentBattleCards.Find(x => x.id == attacker).gameObject.transform.position,Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : playerBattleCards.Find(x => x.id == i).gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = isplayer;
        proj.DealDamage = !isplayer;
        proj.lifetime = 2f;
        SpellManager.IsPetAttacking = false;
    }

    public void DestroyOb(int i){
        photonView.RPC("OnDestroyRPC",RpcTarget.Others,i);
    }

    [PunRPC]
    public void OnDestroyRPC(int cardId){
        BattleCardDisplay battleCard = opponentBattleCards.Find(x=>x.card.cardId == cardId);
        SpellManager.instance.opponentBattleCards.Remove(battleCard);
    }


    

    public int offset_card = 50;//50
    public GameObject InstantiateSpellCard(SpellCard sO,Vector3 pos,Transform parent)
    {
        tempSpellCardObj = Instantiate(spellCardPrefab,pos,Quaternion.identity,parent);
        tempSpellCardObj.GetComponent<SpellCardDisplay>().card = sO;
        
        return tempSpellCardObj;
    }

    public GameObject InstantiateSpellBattleCard(SpellCard sO,Vector3 pos,Transform parent,int num_card)
    {
        tempSpellCardObj = Instantiate(spellBettleCardPrefeb,pos,Quaternion.identity,parent);
        tempSpellCardObj.GetComponent<BattleCardDisplay>().card = sO;
        tempSpellCardObj.transform.localScale = Vector3.one;

        return tempSpellCardObj;
    }
    public void RemoveOldSpellData() 
    {
        
        for(int i = 0 ; i < playerBattleCards.Count ; i++)
        {
            Destroy(playerBattleCards[i].gameObject);
            // playerBattleCards.RemoveAt(i);
           
        }
        for(int i = 0 ; i < playerCards.Count ; i++)
        {
            Destroy(playerCards[i].gameObject);
            // playerCards.RemoveAt(i);
           
        }
        for(int i = 0 ; i < opponentCards.Count ; i++)
        {
            Destroy(opponentCards[i].gameObject);
          //  opponentCards.RemoveAt(i);
            
        }
        for(int i = 0 ; i < opponentBattleCards.Count ; i++)
        {
            Destroy(opponentBattleCards[i].gameObject);
           // opponentBattleCards.RemoveAt(i);
            
        }
        playerBattleCards.Clear();
        playerCards.Clear();
        opponentBattleCards.Clear();
        opponentCards.Clear();
    }

    public void DrawCard(){
        Debug.LogError("cards holding now : "+spellCardsPlayer.transform.childCount);
        if(spellCardsPlayer.childCount == 9)
            return;
        int i = Random.Range(0,spellCardsDeck.Count);
        while(spawned_ids.Contains(i)){
            i = Random.Range(0,spellCardsDeck.Count);
        }
        spawned_ids.Add(i);
        GenerateCardsForPlayer(i);
    }


    public void GenerateCardsForPlayer(int i) 
    {
        
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i],Vector3.one,MyMainDeck.transform);
        //obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomePlayer;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(true);
        obj.transform.localScale = Vector3.one * 0.7f;
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;
        //obj.transform.Rotate(Vector3.forward * 270);
        // obj.transform.eulerAngles = new Vector3(0,0,0);
        if(PhotonNetwork.IsMasterClient==false)
            LeanTween.rotate(obj,new Vector3(0,0,-180),0);
        playerCards.Add(obj.GetComponent<SpellCardDisplay>());
        
        LeanTween.move(obj,spellCardsPlayer.position,0.3f).setOnComplete(()=>{
            obj.transform.SetParent(spellCardsPlayer);
            obj.transform.position = Vector3.zero;
        });
        
        //Debug.Log("RPC CALLED ");
        photonView.RPC("GenerateCardsForOppnent",RpcTarget.Others,i);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void CastSpell(int i){
        SpellCard card = spellCardsDeck[i];
        GameObject o = Instantiate(card.SpellProjectilePref,PVPManager.Get().p1Image.gameObject.transform.position,Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = PVPManager.Get().p2Image.gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = true;
        proj.DealDamage = true;
        proj.lifetime = 2f;
        photonView.RPC("CastSpellRPC",RpcTarget.Others,i);
    }

    [PunRPC]
    public void CastSpellRPC(int i){
        SpellCard card = spellCardsDeck[i];
        GameObject o = Instantiate(card.SpellProjectilePref,PVPManager.Get().p2Image.gameObject.transform.position,Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = PVPManager.Get().p1Image.gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = true;
        proj.DealDamage = false;
        proj.lifetime = 2f;
        Destroy(opponentCards.Find((item)=>item.index == i).gameObject);
    }


    

    public void MoveOpponentCardToBattleArea(int cardId, int battleId) 
    {
        photonView.RPC("ChangeOpponentCardPostionToCenter_RPC",RpcTarget.Others,cardId,battleId);
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
        GameObject obj = InstantiateSpellCard(spellCardsDeck[i],Vector3.one,OppoMainDeck.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpellCardDisplay>().cardPosition = SpellCardPosition.petHomeOppoent;
        obj.GetComponent<SpellCardDisplay>().index = i;
        obj.GetComponent<SpellCardDisplay>().set(false);
        spawned_ids.Add(i);
        obj.GetComponent<SpellCardDisplay>().canvas.sortingOrder = spawned_ids.Count + 1;
        //obj.transform.Rotate(Vector3.forward * 90);
        if(PhotonNetwork.IsMasterClient)
        { LeanTween.rotate(obj,new Vector3(0,0,180),0); }
        else { LeanTween.rotate(obj,new Vector3(0,0,180),0); }
        
        obj.GetComponent<SpellCardDisplay>().BackSide.gameObject.SetActive(true);
        obj.transform.localScale = Vector3.one * 0.7f;
        opponentCards.Add(obj.GetComponent<SpellCardDisplay>());

        LeanTween.move(obj,spellCardsOpponent.position,0.3f).setOnComplete(()=>{
            obj.transform.SetParent(spellCardsOpponent);
            obj.transform.position = Vector3.zero;
        });
        Debug.LogError("CALLED RPC");
        
    }

    public void MouseOverOpponentCard(int cardId,bool isEnter=true)
    {

        //photonView.RPC("MouseEnter_RPC",RpcTarget.Others,cardId,isEnter);
        //PhotonNetwork.SendAllOutgoingCommands();
    }

    [Photon.Pun.PunRPC]
    public void MouseEnter_RPC(int cardId,bool isEnter)
    {
        // if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;
        if(isEnter)
        { SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ShowCaseCard(); }
        else 
        {
            SpellManager.instance.opponentCards.Find(x => x.card.cardId == cardId).ResizeShowCaseCard();
        }

    }

    public bool isShowCasing = false;
}
