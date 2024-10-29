//////////////////////////////////////////
/// BattleCardDisplay.cs
/// 
/// This script is responsible for displaying the cards in the battle scene.
/// It also handles the card's health, attack, and stamina, as well as the card's position.

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
    #region Attributes
    public SpellCard card;
    [SerializeField] private TMPro.TextMeshProUGUI staminaTxt, attackTxt, healthTxt, cardNameTxt, DescriptionTxt, SpeedTxt;
    public int Hp;
    public Image Bg, cardImage;
    public Outline MainBGOutline;
    public SpellCardPosition cardPosition;
    public PhotonView photonView;
    public GameObject DmgPref;
    public int id;
    public bool IsAttackedThisRound;
    public bool IsDead;
    #endregion

    #region Unity Methods
    void Awake()
    {
        photonView = GetComponent<PhotonView>();//Get the PhotonView component
    }
    void Start()
    {
        UpdateCardData();//Update the card data
    }
    #endregion

    #region Mouse Events
    private void OnMouseEnter()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.perBattleOpponent || PVPManager.manager.isResultScreenOn) return;//Check if the current turn player is not the local player or the card position is per battle opponent or the result screen is on
        MainBGOutline.enabled = true;//Enable the main background outline
    }
    private void OnMouseExit()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.perBattleOpponent || PVPManager.manager.isResultScreenOn) return;//Check if the current turn player is not the local player or the card position is per battle opponent or the result screen is on
        MainBGOutline.enabled = false;//Disable the main background outline
    }

    #endregion

    public void UpdateCardData()
    {
        Hp = card.Health;
        UpdateText(staminaTxt, card.stamina.ToString());//Update the stamina text
        UpdateText(cardNameTxt, card.cardName.ToString());//Update the card name text
        UpdateText(attackTxt, card.Attack.ToString());//Update the attack text
        UpdateText(healthTxt, card.Health.ToString());//Update the health text
        UpdateText(DescriptionTxt, card.discription.ToString());//Update the description text
        UpdateText(SpeedTxt, card.speed.ToString());//Update the speed text
        if (cardPosition == SpellCardPosition.petHomePlayer || cardPosition == SpellCardPosition.petBattlePlayer)//Check if the card position is pet home player or pet battle player
        {
            if (PVPManager.manager.myObj.playerType == PlayerType.Black) UpdateImage(cardImage, card.OppocardSprite);//Update the card image
            else UpdateImage(cardImage, card.MycardSprite);//Update the card image
        }
        else if (cardPosition == SpellCardPosition.perBattleOpponent || cardPosition == SpellCardPosition.petHomeOppoent)//Check if the card position is per battle opponent or pet home opponent
        {
            if (PVPManager.manager.opponentObj.playerType == PlayerType.Black) UpdateImage(cardImage, card.OppocardSprite);//Update the card image
            else UpdateImage(cardImage, card.MycardSprite);//Update the card image
        }
    }
    void UpdateText(TMPro.TextMeshProUGUI txt, string val)
    {
        txt.text = val;//Set the text value
    }
    void UpdateImage(Image img, Sprite val)
    {
        img.sprite = val;//Set the image sprite
    }
    public void ResetAttack()
    {
        IsAttackedThisRound = false;//Set the attack status to false
    }
    public void Attack(int i, bool isplayer = false)
    {
        if (isplayer && PVPManager.Get().P2RemainingHandHealth <= 0) return;//Check if the player 2 remaining hand health is less than or equal to 0
        IsAttackedThisRound = true;//Set the attack status to true
        SpellManager.IsPetAttacking = true;//Set the pet attacking status to true
        GameObject o = Instantiate(card.SpellProjectilePref, transform.position, Quaternion.identity);//Instantiate the spell projectile prefab
        Projectile proj = o.GetComponent<Projectile>();//Get the Projectile component
        proj.target = isplayer ? (PVPManager.Get().p2Image.gameObject) : SpellManager.instance.opponentBattleCards.Find(x => x.card.cardId == i).gameObject;//Set the target
        proj.damage = card.Attack;//Set the damage
        proj.istargetPlayer = isplayer;//Set the target player
        proj.DealDamage = true;//Set the deal damage status to true
        proj.lifetime = 2f;//Set the lifetime
        SpellManager.instance.ExecuteAttack(i, isplayer, card.cardId, id);//Execute the
        PVPManager.manager.isCheckWithoutReset = true;//Set the check without reset status to true
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));//Start the check win coroutine
    }
    [PunRPC]
    public void AttackRPC(int i, bool isplayer)
    {
        GameObject o = Instantiate(card.SpellProjectilePref, transform.position, Quaternion.identity);//Instantiate the spell projectile prefab
        Projectile proj = o.GetComponent<Projectile>();//Get the Projectile component
        proj.target = isplayer ? (PVPManager.Get().p1Image.gameObject) : SpellManager.instance.opponentBattleCards[i].gameObject;//Set the target
        proj.damage = card.Attack;//Set the damage
        proj.lifetime = 2f;//Set the lifetime
        SpellManager.IsPetAttacking = false;//Set the pet attacking status to false
        PVPManager.manager.isCheckWithoutReset = true;//Set the check without reset status to true
        StartCoroutine(PVPManager.manager.CheckWinNewWithoutReset(0.1f));//Start the check win coroutine
    }
    public void DealDamage(int c)
    {
        Hp -= c;//Decrease the health
        Hp = Mathf.Clamp(Hp, 0, card.Health);//Clamp the health
        ShowDamage(c);//Show the damage
        UpdateText(healthTxt, Hp.ToString());//Update the health text
        if (Hp <= 0) Kill();//Check if the health is less than or equal to 0
    }
    public void ShowDamage(int c)
    {
        GameObject o = Instantiate(DmgPref, transform);//Instantiate the damage prefab
        o.GetComponent<DamageIndicator>().DmgText.text = "-" + c;//Set the damage text
        Vector3 startPos = Vector3.zero;//Set the start position
        Vector3 midPos1 = new Vector3(0.5f, 30f / 4f, 0f);//Set the mid position 1
        Vector3 midPos2 = new Vector3(0.5f, (30f / 4f) * 3f, 0f);//Set the mid position 2
        Vector3 endPos = new Vector3(0f, 30f, 0f);//Set the end position
        o.LeanMoveLocal(new LTBezierPath(new Vector3[] { startPos, midPos1, midPos2, endPos }), 0.5f).setOnComplete(() =>//Move the object
        {
            Destroy(o);//Destroy the object
        });
    }
    void OnDestroy()//When the object is destroyed
    {
        SpellManager.instance.playerBattleCards.Remove(this);//Remove the card from the player battle cards list
        SpellManager.instance.DestroyOb(card.cardId);//Destroy the object
    }
    public void Kill()//Kill the card
    {
        IsDead = true;//Set the dead status to true
        PVPManager.manager.myObj.cards.Remove(card);//Remove the card from the player cards list
        Destroy(gameObject, 0.7f);//Destroy the object
    }
    public void ChangeCardPostionToCenter() { }
    public void ChangeParent()//Method to change the parent
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player
        this.gameObject.transform.SetParent(SpellManager.instance.spellCardBattleObj);//Set the parent to the spell card battle object
    }
    public void ChangeParentShowCase()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player
        this.gameObject.transform.SetParent(SpellManager.instance.showCaseParent);//Set the parent to the show case parent
    }
    public void ChangeParentHome()//Method to change the parent
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player
        this.gameObject.transform.SetParent(SpellManager.instance.spellCardsPlayer);//Set the parent to the spell cards player
        if (SpellManager.instance.isShowCasing) SpellManager.instance.isShowCasing = false;//Check if the show casing status is true
    }
    public void ChangeParent(Transform parent_)//Method to change the parent
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player
        this.gameObject.transform.SetParent(parent_);//Set the parent to the parent
    }
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.GetComponent<RectTransform>().position = new Vector3(eventData.position.x, eventData.position.y);//Set the position
    }
    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }

}
