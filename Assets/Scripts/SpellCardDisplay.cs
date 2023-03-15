using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class SpellCardDisplay : MonoBehaviourPunCallbacks, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public SpellCard card;
    [SerializeField]
    private TMPro.TextMeshProUGUI manaTxt, attackTxt, healthTxt, cardNameTxt, DescriptionTxt, SpeedTxt;
    public Image Bg, cardImage, BackSide;
    public Outline MainBGOutline;
    public SpellCardPosition cardPosition;
    public PhotonView photonView;

    public int index;

    public Canvas canvas;

    public int startSortOrder;
    public bool IsPreview = false;
    public GameObject cbg;

    public GameObject RootObj;


    // Start is called before the first frame update
    void Start()
    {
        UpdateCardData();
        photonView = GetComponent<PhotonView>();
        startSortOrder = canvas.sortingOrder;
    }

    public void set(bool isShow)
    {
        manaTxt.transform.parent.gameObject.SetActive(isShow);
        attackTxt.transform.parent.gameObject.SetActive(isShow);
        healthTxt.transform.parent.gameObject.SetActive(isShow);
        SpeedTxt.transform.parent.gameObject.SetActive(isShow);
        //DescriptionTxt.gameObject.SetActive(isShow);
        cardNameTxt.gameObject.SetActive(isShow);
        cardImage.gameObject.SetActive(isShow);
        //cbg.gameObject.SetActive(isShow);

    }

    public void UpdateCardData()
    {
        UpdateText(manaTxt, card.Manacost.ToString());
        UpdateText(cardNameTxt, card.cardName.ToString());
        UpdateText(attackTxt, card.Attack.ToString());
        UpdateText(healthTxt, card.Health.ToString());
        UpdateText(DescriptionTxt, card.discription.ToString());
        UpdateText(SpeedTxt, card.speed.ToString());
        if (PVPManager.manager != null)
        {
            if (PVPManager.manager.myObj.playerType == PlayerType.Black)
                UpdateImage(cardImage, card.OppocardSprite);
            else// if(cardPosition == SpellCardPosition.perBattleOpponent || cardPosition == SpellCardPosition.petHomeOppoent)
                UpdateImage(cardImage, card.MycardSprite);
        }
        else
        {
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
        if (IsPreview)
            return;
        if (!PVPManager.Get().IsPetTurn)
            return;
        if (cardPosition == SpellCardPosition.petHomeOppoent)
            return;

        // if((Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.petHomeOppoent || PVPManager.manager.isResultScreenOn)){

        //     return;
        // } 

        //if(!SpellManager.instance.isShowCasing)
        //{
        //SpellManager.instance.isShowCasing = true;
        // LeanTween.move(gameObject,SpellManager.instance.showCaseRef,0);
        MainBGOutline.enabled = true;
        cardReseted = false;
        LeanTween.scale(this.gameObject, Vector3.one * 1.5f, 0.25f);//.setOnComplete(ChangeParentShowCase);
        SpellManager.instance.MouseOverOpponentCard(card.cardId, true);
        canvas.sortingOrder = 1000;
        //}
    }
    private void OnMouseExit()
    {
        if (IsPreview)
            return;
        if (!PVPManager.Get().IsPetTurn)
            return;
        // if((Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.petHomeOppoent || PVPManager.manager.isResultScreenOn)) return;
        // ChangeParentHome();
        MainBGOutline.enabled = false;
        LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f);//.setOnComplete(ChangeParentHome);
        SpellManager.instance.MouseOverOpponentCard(card.cardId, false);
        canvas.sortingOrder = startSortOrder;

    }
    public void SummonCard()
    {
        if (IsPreview)
        {
            AddCardToDeck();
        }
        else
        {
            if (card.cardType == CardType.Spell)
            {
                CastSpell();
            }
            else if (card.cardType == CardType.Pet)
            {
                //if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer || cardPosition == SpellCardPosition.petHomeOppoent || PVPManager.manager.isResultScreenOn) return;
                ChangeCardPostionToCenter();
            }
        }


    }
    public void AddCardToDeck()
    {
        if (DeckManager.instance.isPetOpen)
        {
            if (DeckManager.instance.playerDeck.Count == 33)
                return;
        }

        RootObj.LeanScale(Vector3.zero, 0.3f).setOnComplete(() =>
        {
            DeckManager.instance.AddCard(card);
            Destroy(gameObject);
        });
    }

    void CastSpell()
    {
        //if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;
        if (!PVPManager.Get().IsPetTurn) return;

        if (card.Manacost > PVPManager.Get().MyManabarVal)
            return;

        if (PVPManager.manager.P2RemainingHandHealth <= 0)
            return;

        PVPManager.Get().DeductMana(card.Manacost);
        StartCoroutine(SpellManager.instance.CastSpell(index));

        Destroy(this.gameObject, 0.1f);
    }



    public void ChangeCardPostionToCenter()
    {

        //if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;
        if (!PVPManager.Get().IsPetTurn) return;

        if (card.Manacost > PVPManager.Get().MyManabarVal)
            return;

        GameObject tempObj = SpellManager.instance.InstantiateSpellBattleCard(GameData.Get().GetPet(card.cardId), Bg.transform.position, this.gameObject.transform, 0);
        tempObj.GetComponent<BattleCardDisplay>().cardPosition = SpellCardPosition.petBattlePlayer;
        tempObj.GetComponent<BattleCardDisplay>().id = SpellManager.instance.playerBattleCards.Count + 1;
        if (PhotonNetwork.IsMasterClient == false)
            LeanTween.rotate(tempObj, new Vector3(0, 0, -180), 0);

        SpellManager.instance.playerBattleCards.Add(tempObj.GetComponent<BattleCardDisplay>());
        LeanTween.move(tempObj, SpellManager.instance.spellCardBattleObj, .3f).setOnComplete(() => { ChangeParent(tempObj, SpellManager.instance.spellCardBattleObj); SpellManager.instance.PetAttack(); });
        SpellManager.instance.MoveOpponentCardToBattleArea(card.cardId, tempObj.GetComponent<BattleCardDisplay>().id);

        PVPManager.Get().DeductMana(card.Manacost);


    }



    public void ChangeOpponentCardPostionToCenter(int battleId)
    {
        //PhotonNetwork.Instantiate("SpellBattleCardPrefeb",Bg.transform.position,Quaternion.identity,0,new object[]{card});
        //  if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        GameObject tempObj = SpellManager.instance.InstantiateSpellBattleCard(GameData.Get().GetPet(card.cardId), Bg.transform.position, this.gameObject.transform, 0);
        tempObj.GetComponent<BattleCardDisplay>().cardPosition = SpellCardPosition.perBattleOpponent;
        tempObj.GetComponent<BattleCardDisplay>().id = battleId;

        if (PhotonNetwork.IsMasterClient == false)
            LeanTween.rotate(tempObj, new Vector3(0, 0, -180), 0);
        SpellManager.instance.opponentBattleCards.Add(tempObj.GetComponent<BattleCardDisplay>());
        LeanTween.move(tempObj, SpellManager.instance.opponentSpellBattleObject, .3f).setOnComplete(() => ChangeParent(tempObj, SpellManager.instance.opponentSpellBattleObject));



    }
    public void ChangeParent()
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(SpellManager.instance.spellCardBattleObj);
    }

    public void ChangeParent(GameObject obj, Transform newParent = null)
    {
        // if(Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        if (newParent == null)
        {
            newParent = SpellManager.instance.spellBettleCardPrefeb.transform;
        }
        obj.transform.SetParent(newParent);
        obj.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
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
        //  this.transform.GetComponent<RectTransform>().position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
    public void ShowCaseCard()
    {
        MainBGOutline.enabled = true;
        LeanTween.scale(this.gameObject, Vector3.one * 2f, 0.25f);
    }
    public void ResizeShowCaseCard()
    {
        MainBGOutline.enabled = false;
        LeanTween.scale(this.gameObject, Vector3.one, 0.25f);
    }

    public bool cardReseted;
    private void Update()
    {
        if (PVPManager.manager != null)
        {
            if (!PVPManager.manager.IsPetTurn)
            {
                ResetCard();
            }
        }

    }

    public void ResetCard()
    {
        MainBGOutline.enabled = false;
        LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f);//.setOnComplete(ChangeParentHome);
        //SpellManager.instance.MouseOverOpponentCard(card.cardId, false);
        canvas.sortingOrder = startSortOrder;
        cardReseted = true;
    }
}
