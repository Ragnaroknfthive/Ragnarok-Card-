using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class BattleCardDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public SpellCard card;
    [SerializeField]
    private TMPro.TextMeshProUGUI staminaTxt, attackTxt, healthTxt, cardNameTxt, DescriptionTxt, SpeedTxt;
    public int Hp;
    public Image Bg, cardImage;
    public Outline MainBGOutline;
    public SpellCardPosition cardPosition;
    public PhotonView photonView;

    public GameObject DmgPref;

    public int id;

    public bool IsAttackedThisRound;

    public bool IsDead;

    // public void OnInstantiate(object[] data){

    //     card = (SpellCard)data[0];
    //     cardPosition = SpellCardPosition.perBattleOpponent;
    //     if(!PhotonNetwork.LocalPlayer.IsMasterClient)
    //         LeanTween.rotate(gameObject,new Vector3(0,0,-180),0);

    // }

    // Start is called before the first frame update

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        photonView = GetComponent<PhotonView>();

    }

    void Start()
    {
        UpdateCardData();
    }

    public void UpdateCardData()
    {
        Hp = card.Health;
        UpdateText(staminaTxt, card.stamina.ToString());
        UpdateText(cardNameTxt, card.cardName.ToString());
        UpdateText(attackTxt, card.Attack.ToString());
        UpdateText(healthTxt, card.Health.ToString());
        UpdateText(DescriptionTxt, card.discription.ToString());
        UpdateText(SpeedTxt, card.speed.ToString());

        if (cardPosition == SpellCardPosition.petHomePlayer || cardPosition == SpellCardPosition.petBattlePlayer)
        {
            if (PVPManager.manager.myObj.playerType == PlayerType.Black)
                UpdateImage(cardImage, card.OppocardSprite);
            else
                UpdateImage(cardImage, card.MycardSprite);
        }
        else if (cardPosition == SpellCardPosition.perBattleOpponent || cardPosition == SpellCardPosition.petHomeOppoent)
        {
            if (PVPManager.manager.opponentObj.playerType == PlayerType.Black)
                UpdateImage(cardImage, card.OppocardSprite);
            else
                UpdateImage(cardImage, card.MycardSprite);
        }

    }
    void UpdateText(TMPro.TextMeshProUGUI txt, string val)
    {
        txt.text = val;
    }
    void UpdateImage(Image img, Sprite val)
    {
        img.sprite = val;
    }
    private void OnMouseEnter()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.perBattleOpponent || PVPManager.manager.isResultScreenOn) return;
        //if(!SpellManager.instance.isShowCasing)
        //{
        //SpellManager.instance.isShowCasing = true;
        // LeanTween.move(gameObject,SpellManager.instance.showCaseRef,0);
        MainBGOutline.enabled = true;
        // LeanTween.scale(this.gameObject,Vector3.one * 1.5f,0.25f);//.setOnComplete(ChangeParentShowCase);
        //}
    }
    private void OnMouseExit()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.perBattleOpponent || PVPManager.manager.isResultScreenOn) return;
        // ChangeParentHome();
        MainBGOutline.enabled = false;
        // LeanTween.scale(this.gameObject,Vector3.one,0.25f);//.setOnComplete(ChangeParentHome);

    }

    public void ResetAttack()
    {
        IsAttackedThisRound = false;
    }

    public void Attack(int i, bool isplayer = false)
    {
        if(isplayer && PVPManager.Get().P2RemainingHandHealth <= 0){
            return;
        }
        IsAttackedThisRound = true;
        SpellManager.IsPetAttacking = true;
        //if(PVPManager.manager.isResultScreenOn) return;
        //Debug.LogError("ATTACK ");

        GameObject o = Instantiate(card.SpellProjectilePref, transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? (PVPManager.Get().p2Image.gameObject) : SpellManager.instance.opponentBattleCards.Find(x => x.card.cardId == i).gameObject;
        proj.damage = card.Attack;
        proj.istargetPlayer = isplayer;
        proj.DealDamage = true;
        proj.lifetime = 2f;
        // if(ob.GetComponent<PVPManager>() != null && Game.Get()._currnetTurnPlayer == PhotonNetwork.LocalPlayer){
        //     Debug.LogError("Attacking : "+ob.name);
        //     ob.GetComponent<PVPManager>().DealDamageToOpponent(card.Attack);
        // }

        // if(ob.GetComponent<BattleCardDisplay>() != null)
        // {
        //     Debug.LogError("Attacking : "+ob.GetComponent<BattleCardDisplay>().card.cardId);
        //     ob.GetComponent<BattleCardDisplay>().DealDamage(card.Attack);
        // }
        //SpellManager.IsPetAttacking = false;
        SpellManager.instance.ExecuteAttack(i, isplayer, card.cardId, id);
        PVPManager.manager.isCheckWithoutReset = true;
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));
        // photonView.RPC("PetAttackRPC",RpcTarget.Others,i,isplayer);
        // PhotonNetwork.SendAllOutgoingCommands();
        // ChangeCardPostionToCenter();
        // this is comment
        
    }

    [PunRPC]
    public void AttackRPC(int i, bool isplayer)
    {
        //SpellManager.IsPetAttacking = true;
        GameObject o = Instantiate(card.SpellProjectilePref, transform.position, Quaternion.identity);
        Projectile proj = o.GetComponent<Projectile>();
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : SpellManager.instance.opponentBattleCards[i].gameObject;
        proj.damage = card.Attack;
        proj.lifetime = 2f;
        SpellManager.IsPetAttacking = false;
        PVPManager.manager.isCheckWithoutReset = true;
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));
    }



    public void DealDamage(int c)
    {

        Hp -= c;
        Hp = Mathf.Clamp(Hp, 0, card.Health);
        ShowDamage(c);
        UpdateText(healthTxt, Hp.ToString());
        if (Hp <= 0)
            Kill();
    }

    public void ShowDamage(int c)
    {
        GameObject o = Instantiate(DmgPref, transform);
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

    void OnDestroy()
    {
        SpellManager.instance.playerBattleCards.Remove(this);
        SpellManager.instance.DestroyOb(card.cardId);
    }



    public void Kill()
    {
        IsDead = true;
        PVPManager.manager.myObj.cards.Remove(card);
        Destroy(gameObject, 0.7f);
    }

    public void ChangeCardPostionToCenter()
    {

        //LeanTween.move(this.gameObject,SpellManager.instance.spellCardBattleObj,.3f).setOnComplete(ChangeParent);
    }
    public void ChangeParent()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(SpellManager.instance.spellCardBattleObj);
    }
    public void ChangeParentShowCase()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(SpellManager.instance.showCaseParent);
    }
    public void ChangeParentHome()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(SpellManager.instance.spellCardsPlayer);
        if (SpellManager.instance.isShowCasing)
        {
            SpellManager.instance.isShowCasing = false;
        }
    }
    public void ChangeParent(Transform parent_)
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(parent_);
    }
    public void OnDrag(PointerEventData eventData)
    {

        this.transform.GetComponent<RectTransform>().position = new Vector3(eventData.position.x, eventData.position.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

}
