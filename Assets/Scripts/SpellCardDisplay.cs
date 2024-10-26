////////////////////////////////////////////////
///SpellCardDisplay.cs
///
///This script is used to display the card data on the card prefab,
///it also handles the drag and drop functionality of the card.

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
    public PhotonView phtnView;

    public int index;

    public Canvas canvas;

    public int startSortOrder;
    public bool IsPreview = false;
    public GameObject cbg;

    public GameObject RootObj;

    void Start()
    {
        UpdateCardData();//Update the card data on the card prefab
        phtnView = GetComponent<PhotonView>();//Get the photon view component
        startSortOrder = canvas.sortingOrder;//Get the starting sorting order of the card
    }

    public void set(bool isShow)//Set the card data on the card prefab
    {
        manaTxt.transform.parent.gameObject.SetActive(isShow);//Set the mana text on the card prefab
        attackTxt.transform.parent.gameObject.SetActive(isShow);//Set the attack text on the card prefab
        healthTxt.transform.parent.gameObject.SetActive(isShow);//Set the health text on the card prefab
        SpeedTxt.transform.parent.gameObject.SetActive(isShow);//Set the speed text on the card prefab
        cardNameTxt.gameObject.SetActive(isShow);//Set the card name text on the card prefab
        cardImage.gameObject.SetActive(isShow);//Set the card image on the card prefab
    }

    public void UpdateCardData()//Update the card data on the card prefab
    {
        UpdateText(manaTxt, card.Manacost.ToString());//Update the mana text on the card prefab
        UpdateText(cardNameTxt, card.cardName.ToString());//Update the card name text on the card prefab
        UpdateText(attackTxt, card.Attack.ToString());//Update the attack text on the card prefab
        UpdateText(healthTxt, card.Health.ToString());//Update the health text on the card prefab
        UpdateText(DescriptionTxt, card.discription.ToString());//Update the description text on the card prefab
        UpdateText(SpeedTxt, card.speed.ToString());//Update the speed text on the card prefab
        if (PVPManager.manager != null)//Check if the PVP manager is not null
        {
            if (PVPManager.manager.myObj.playerType == PlayerType.Black)//Check if the player type is black
                UpdateImage(cardImage, card.OppocardSprite);//Update the card image on the card prefab
            else
                UpdateImage(cardImage, card.MycardSprite);//Update the card image on the card prefab
        }
        else
        {
            UpdateImage(cardImage, card.MycardSprite);//Update the card image on the card prefab
        }

    }
    void UpdateText(TMPro.TextMeshProUGUI txt, string val)
    {
        txt.text = val;//Update the text on the card prefab
    }
    void UpdateImage(Image img, Sprite val)
    {
        img.sprite = val;//Update the image on the card prefab
    }
    private void OnMouseEnter()//When the mouse enters the card
    {
        if (IsPreview)//Check if the card is in preview mode
            return;
        if (!PVPManager.Get().IsPetTurn)//Check if it is not the pet turn
            return;
        if (cardPosition == SpellCardPosition.petHomeOppoent)//Check if the card position is the pet home opponent
            return;
        MainBGOutline.enabled = true;//Enable the main background outline
        cardReseted = false;//Set the card reseted to false
        LeanTween.scale(this.gameObject, Vector3.one * 1.5f, 0.25f);//Scale the card to 1.5
        SpellManager.instance.MouseOverOpponentCard(card.cardId, true);//Call the mouse over opponent card function
        canvas.sortingOrder = 1000;//Set the sorting order of the canvas to 1000
    }
    private void OnMouseExit()
    {
        if (IsPreview)//Check if the card is in preview mode
            return;
        if (!PVPManager.Get().IsPetTurn)//Check if it is not the pet turn
            return;
        MainBGOutline.enabled = false;//Disable the main background outline
        LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f);//Scale the card to 0.7
        SpellManager.instance.MouseOverOpponentCard(card.cardId, false);//Call the mouse over opponent card function
        canvas.sortingOrder = startSortOrder;//Set the sorting order of the canvas to the starting sorting order
    }
    public void SummonCard()//Summon the card
    {
        if (IsPreview)//Check if the card is in preview mode
        {
            AddCardToDeck();//Add the card to the deck
        }
        else
        {
            if (card.cardType == CardType.Spell)
            {
                CastSpell();//Cast the spell
            }
            else if (card.cardType == CardType.Pet)//Check if the card type is pet
            {
                ChangeCardPostionToCenter();//Change the card position to the center
            }
        }


    }
    public void AddCardToDeck()//Add the card to the deck
    {
        if (DeckManager.instance.isPetOpen)
        {
            if (DeckManager.instance.playerDeck.Count == 33)//Check if the player deck count is 33
                return;
        }

        RootObj.LeanScale(Vector3.zero, 0.3f).setOnComplete(() =>//Scale the root object to zero
        {
            DeckManager.instance.AddCard(card);//Add the card to the deck
            Destroy(gameObject);//Destroy the game object
        });
    }

    void CastSpell()//Cast the spell
    {
        if (!PVPManager.Get().IsPetTurn) return;//Check if it is not the pet turn

        if (card.Manacost > PVPManager.Get().MyManabarVal)//Check if the mana cost is greater than the player mana bar value
            return;

        if (PVPManager.manager.P2RemainingHandHealth <= 0)//Check if the player 2 remaining hand health is less than or equal to 0
            return;

        PVPManager.Get().DeductMana(card.Manacost);//Deduct the mana cost
        StartCoroutine(SpellManager.instance.CastSpell(index));//Start the cast spell coroutine

        Destroy(this.gameObject, 0.1f);//Destroy the game object
    }



    public void ChangeCardPostionToCenter()//Change the card position to the center
    {
        if (!PVPManager.Get().IsPetTurn) return;//Check if it is not the pet turn

        if (card.Manacost > PVPManager.Get().MyManabarVal) return;//Check if the mana cost is greater than the player mana bar value

        GameObject tempObj = SpellManager.instance.InstantiateSpellBattleCard(GameData.Get().GetPet(card.cardId), Bg.transform.position, this.gameObject.transform, 0);//Instantiate the spell battle card
        tempObj.GetComponent<BattleCardDisplay>().cardPosition = SpellCardPosition.petBattlePlayer;//Set the card position to pet battle player
        tempObj.GetComponent<BattleCardDisplay>().id = SpellManager.instance.playerBattleCards.Count + 1;//Set the id
        if (PhotonNetwork.IsMasterClient == false)//Check if the player is not the master client
            LeanTween.rotate(tempObj, new Vector3(0, 0, -180), 0);//Rotate the card
        SpellManager.instance.playerBattleCards.Add(tempObj.GetComponent<BattleCardDisplay>());//Add the card to the player battle cards
        LeanTween.move(tempObj, SpellManager.instance.spellCardBattleObj, .3f).setOnComplete(() => { ChangeParent(tempObj, SpellManager.instance.spellCardBattleObj); SpellManager.instance.PetAttack(); });//Move the card to the spell card battle object
        SpellManager.instance.MoveOpponentCardToBattleArea(card.cardId, tempObj.GetComponent<BattleCardDisplay>().id);//Move the opponent card to the battle area
        PVPManager.Get().DeductMana(card.Manacost);//Deduct the mana cost
    }



    public void ChangeOpponentCardPostionToCenter(int battleId)//Change the opponent card position to the center
    {
        GameObject tempObj = SpellManager.instance.InstantiateSpellBattleCard(GameData.Get().GetPet(card.cardId), Bg.transform.position, this.gameObject.transform, 0);//Instantiate the spell battle card
        tempObj.GetComponent<BattleCardDisplay>().cardPosition = SpellCardPosition.perBattleOpponent;//Set the card position to pet battle player
        tempObj.GetComponent<BattleCardDisplay>().id = battleId + 1;//Set the id

        if (PhotonNetwork.IsMasterClient == false)//Check if the player is not the master client
            LeanTween.rotate(tempObj, new Vector3(0, 0, -180), 0);//Rotate the card
        SpellManager.instance.opponentBattleCards.Add(tempObj.GetComponent<BattleCardDisplay>());//Add the card to the player battle cards
        LeanTween.move(tempObj, SpellManager.instance.opponentSpellBattleObject, .3f).setOnComplete(() => ChangeParent(tempObj, SpellManager.instance.opponentSpellBattleObject));//Move the card to the spell card battle object
    }
    public void ChangeParent()//Change the parent of the card
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player

        this.gameObject.transform.SetParent(SpellManager.instance.spellCardBattleObj);//Set the parent of the game object to the spell card battle object
    }

    public void ChangeParent(GameObject obj, Transform newParent = null)//Change the parent of the card
    {
        if (newParent == null)//Check if the new parent is null
        {
            newParent = SpellManager.instance.spellBettleCardPrefeb.transform;//Set the new parent to the spell battle card prefab
        }
        obj.transform.SetParent(newParent);//Set the parent of the object to the new parent
        obj.transform.localScale = Vector3.one;//Set the scale of the object to one
        Destroy(this.gameObject);//Destroy the game object
    }
    public void ChangeParentShowCase()//Change the parent of the card to the showcase
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player

        this.gameObject.transform.SetParent(SpellManager.instance.showCaseParent);//Set the parent of the game object to the showcase parent
    }
    public void ChangeParentHome()//Change the parent of the card to the home
    {
        animating = false;//Set the animating to false
        return;//Not used

        //Not used
        //if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        //this.gameObject.transform.SetParent(SpellManager.instance.spellCardsPlayer);
        //if (SpellManager.instance.isShowCasing)
        //{
        //SpellManager.instance.isShowCasing = false;
        //}
    }
    public void ChangeParent(Transform parent_)//Change the parent of the card
    {
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;//Check if the current turn player is not the local player

        this.gameObject.transform.SetParent(parent_);
    }
    public void OnDrag(PointerEventData eventData)//Empty function
    {
    }

    public void OnBeginDrag(PointerEventData eventData)//Empty function
    {
    }

    public void OnEndDrag(PointerEventData eventData)//Empty function
    {

    }
    public void ShowCaseCard()//Showcase the card
    {
        MainBGOutline.enabled = true;//Enable the main background outline
        LeanTween.scale(this.gameObject, Vector3.one * 2f, 0.25f);//Scale the card to 2
    }
    public void ResizeShowCaseCard()
    {
        MainBGOutline.enabled = false;//Disable the main background outline
        LeanTween.scale(this.gameObject, Vector3.one, 0.25f);//Scale the card to 1
    }

    public bool cardReseted;//Check if the card is reseted
    private void Update()
    {
        if (PVPManager.manager != null)//Check if the PVP manager is not null
        {
            if (!PVPManager.manager.IsPetTurn)//Check if it is not the pet turn
            {
                ResetCard();//Reset the card
            }
        }

    }
    bool animating = false;//Check if the card is animating
    public void ResetCard()//Reset the card
    {
        if (animating || !PVPManager.manager.isLocalPVPTurn) return;//Check if the card is animating or it is not the local PVP turn

        animating = true;//Set the animating to true
        MainBGOutline.enabled = false;//Disable the main background outline
        if (gameObject)//Check if the game object is not null
        {
            LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f).setOnComplete(ChangeParentHome);//Scale the card to 0.7
        }
        canvas.sortingOrder = startSortOrder;//Set the sorting order of the canvas to the starting sorting order
        cardReseted = true;//Set the card reseted to true
    }
}
