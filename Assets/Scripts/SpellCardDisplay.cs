using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Represents the display and interaction logic for spell cards in the game.
/// This class manages the UI components associated with a spell card,
/// updates its display based on game state, and handles user interactions.
/// </summary>
public class SpellCardDisplay : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The SpellCard associated with this display.
    /// </summary>
    public SpellCard card;

    /// <summary>
    /// UI components for displaying the card's attributes.
    /// </summary>
    [SerializeField]
    private TMPro.TextMeshProUGUI manaTxt;         // Displays the mana cost of the card.
    private TMPro.TextMeshProUGUI attackTxt;       // Displays the attack value of the card.
    private TMPro.TextMeshProUGUI healthTxt;       // Displays the health value of the card.
    private TMPro.TextMeshProUGUI cardNameTxt;     // Displays the name of the card.
    private TMPro.TextMeshProUGUI DescriptionTxt;  // Displays the description of the card.
    private TMPro.TextMeshProUGUI SpeedTxt;        // Displays the speed of the card.

    /// <summary>
    /// Images used in the card display.
    /// </summary>
    public Image Bg;                             // Background image of the card.
    public Image cardImage;                      // Image representing the card itself.
    public Image BackSide;                       // Image for the back side of the card.

    /// <summary>
    /// Outline component for visual effects.
    /// </summary>
    public Outline MainBGOutline;                // Outline for highlighting the card.

    /// <summary>
    /// Position of the card on the UI.
    /// </summary>
    public SpellCardPosition cardPosition;       // Enum indicating the card's position.

    /// <summary>
    /// Photon view component for network synchronization.
    /// </summary>
    public PhotonView photonView;

    /// <summary>
    /// Index of the card in the display or deck.
    /// </summary>
    public int index;

    /// <summary>
    /// The canvas containing this card's UI elements.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Initial sorting order of the canvas.
    /// </summary>
    public int startSortOrder;

    /// <summary>
    /// Indicates if the card is in preview mode.
    /// </summary>
    public bool IsPreview = false;

    /// <summary>
    /// Reference to the background game object for the card.
    /// </summary>
    public GameObject cbg;                      // Background GameObject.

    /// <summary>
    /// Reference to the root game object for transformations.
    /// </summary>
    public GameObject RootObj;                  // Root object for scaling and positioning.

    /// <summary>
    /// Initializes the card display and updates the card data when the script starts.
    /// </summary>
    void Start()
    {
        UpdateCardData();                      // Update card information on start.
        photonView = GetComponent<PhotonView>(); // Get the PhotonView component for networking.
        startSortOrder = canvas.sortingOrder;  // Store the initial sorting order of the canvas.
    }

    /// <summary>
    /// Sets the visibility of card attribute UI elements.
    /// </summary>
    /// <param name="isShow">Determines whether to show or hide the elements.</param>
    public void set(bool isShow)
    {
        // Activate or deactivate the parent objects of the text components based on isShow parameter.
        manaTxt.transform.parent.gameObject.SetActive(isShow);
        attackTxt.transform.parent.gameObject.SetActive(isShow);
        healthTxt.transform.parent.gameObject.SetActive(isShow);
        SpeedTxt.transform.parent.gameObject.SetActive(isShow);
        cardNameTxt.gameObject.SetActive(isShow);
        cardImage.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// Updates the card display with the current attributes of the card.
    /// </summary>
    public void UpdateCardData()
    {
        // Update the text fields with the corresponding values from the card.
        UpdateText(manaTxt, card.Manacost.ToString());
        UpdateText(cardNameTxt, card.cardName.ToString());
        UpdateText(attackTxt, card.Attack.ToString());
        UpdateText(healthTxt, card.Health.ToString());
        UpdateText(DescriptionTxt, card.discription.ToString());
        UpdateText(SpeedTxt, card.speed.ToString());

        // Determine which sprite to display based on the player type.
        if (PVPManager.manager != null)
        {
            if (PVPManager.manager.myObj.playerType == PlayerType.Black)
                UpdateImage(cardImage, card.OppocardSprite); // Update image if player is Black.
            UpdateImage(cardImage, card.MycardSprite);       // Update image to player's card sprite.
        }
        else
        {
            UpdateImage(cardImage, card.MycardSprite);       // Update image to player's card sprite.
        }
    }

    /// <summary>
    /// Updates the text of a TextMeshProUGUI component.
    /// </summary>
    /// <param name="txt">The TextMeshProUGUI component to update.</param>
    /// <param name="val">The new value to set.</param>
    void UpdateText(TMPro.TextMeshProUGUI txt, string val)
    {
        txt.text = val; // Set the text to the new value.
    }

    /// <summary>
    /// Updates the sprite of an Image component.
    /// </summary>
    /// <param name="img">The Image component to update.</param>
    /// <param name="val">The new sprite to set.</param>
    void UpdateImage(Image img, Sprite val)
    {
        img.sprite = val; // Set the image sprite to the new value.
    }

    /// <summary>
    /// Handles mouse hover events when the mouse enters the card area.
    /// Activates card preview effects.
    /// </summary>
    private void OnMouseEnter()
    {
        // Exit if the card is in preview mode or it's not the player's turn.
        if (IsPreview || !PVPManager.Get().IsPetTurn || cardPosition == SpellCardPosition.petHomeOppoent)
            return;

        MainBGOutline.enabled = true;               // Enable outline effect on mouse enter.
        cardReseted = false;                        // Reset card state.
        LeanTween.scale(this.gameObject, Vector3.one * 1.5f, 0.25f); // Scale the card up for emphasis.
        SpellManager.instance.MouseOverOpponentCard(card.cardId, true); // Notify the manager about the mouse over event.
        canvas.sortingOrder = 1000;                 // Bring the card to the front of the canvas.
    }

    /// <summary>
    /// Handles mouse hover events when the mouse exits the card area.
    /// Deactivates card preview effects.
    /// </summary>
    private void OnMouseExit()
    {
        // Exit if the card is in preview mode or it's not the player's turn.
        if (IsPreview || !PVPManager.Get().IsPetTurn)
            return;

        MainBGOutline.enabled = false;             // Disable outline effect on mouse exit.
        LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f); // Scale the card down.
        SpellManager.instance.MouseOverOpponentCard(card.cardId, false); // Notify the manager about the mouse exit event.
        canvas.sortingOrder = startSortOrder;      // Reset the canvas sorting order.
    }
    public void SummonCard()
    {
        // This method is called to summon a card based on its type (Spell or Pet).
        // If the card is in preview mode, it adds the card to the player's deck.
        // If the card is a Spell, it casts the spell.
        // If the card is a Pet, it changes the card's position to the center.
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
                ChangeCardPostionToCenter();
            }
        }
    }

    public void AddCardToDeck()
    {
        // This method adds the current card to the player's deck if the pet slot is open and the deck size is less than the maximum limit (33 cards).
        // It scales down the card's root object to zero over 0.3 seconds, then adds the card to the deck and destroys the card game object.
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
        // This method casts the spell represented by the card.
        // It first checks if it is the pet's turn and if the card's mana cost is within the player's mana bar value.
        // If valid, it deducts the mana cost from the player's mana bar and initiates the spell casting coroutine.
        // The card object is destroyed after a short delay.
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
        // This method changes the position of the card to the center of the battle area for Pets.
        // It checks if it is the pet's turn and whether the mana cost is sufficient.
        // If valid, it instantiates a battle card and adds it to the player's battle cards list.
        // The card is then moved to the battle area with a scaling and position animation.
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
        // This method changes the position of an opponent's card to the center of the battle area.
        // It creates a new battle card instance for the opponent and updates its position and ID.
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
        // This method sets the parent of the card to the battle card object if it's the current turn of the local player.
        if (Game.Get()._currnetTurnPlayer != Photon.Pun.PhotonNetwork.LocalPlayer) return;

        this.gameObject.transform.SetParent(SpellManager.instance.spellCardBattleObj);
    }

    public void ChangeParent(GameObject obj, Transform newParent = null)
    {
        // This method changes the parent of a specified game object to a new parent.
        // If no new parent is provided, it defaults to the spell battle card prefab's transform.
        obj.transform.SetParent(newParent ?? SpellManager.instance.spellBettleCardPrefeb.transform);
        obj.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }

    public void ChangeParentHome()
    {
        // This method resets the animation flag and does not change any properties.
        animating = false;
        return;
    }

    public void ShowCaseCard()
    {
        // This method enables the outline of the main background and scales up the card to showcase it.
        MainBGOutline.enabled = true;
        LeanTween.scale(this.gameObject, Vector3.one * 2f, 0.25f);
    }

    public void ResizeShowCaseCard()
    {
        // This method disables the outline and scales the card back to its original size.
        MainBGOutline.enabled = false;
        LeanTween.scale(this.gameObject, Vector3.one, 0.25f);
    }

    public bool cardReseted; // Indicates whether the card has been reset

    private void Update()
    {
        // This method is called every frame to check if it is the player's turn to control pets.
        // If it is not the player's turn, it calls the ResetCard method to reset the card.
        if (PVPManager.manager != null)
        {
            if (!PVPManager.manager.IsPetTurn)
            {
                ResetCard();
            }
        }
    }

    bool animating = false; // Flag to track if the card is currently animating

    public void ResetCard()
    {
        // This method resets the card's state if it is the local player's turn and if an animation is not already in progress.
        // It disables the background outline, scales the card down, and resets the sorting order of the canvas.
        // The method also sets the cardReseted flag to true to indicate that the card has been reset.
        if (animating || !PVPManager.manager.isLocalPVPTurn) return;

        animating = true; // Set animating flag to true
        MainBGOutline.enabled = false; // Disable the main background outline
        if (gameObject)
        {
            // Scale the card down to 0.7 over 0.25 seconds and call ChangeParentHome upon completion
            LeanTween.scale(this.gameObject, Vector3.one * 0.7f, 0.25f).setOnComplete(ChangeParentHome);
        }
        canvas.sortingOrder = startSortOrder; // Reset the canvas sorting order to its original value
        cardReseted = true; // Indicate that the card has been reset
    }
}