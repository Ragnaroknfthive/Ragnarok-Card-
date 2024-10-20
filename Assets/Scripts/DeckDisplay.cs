////////////////////////////////////////////////////////
///DeckDisplay.cs
///
///This script is used to display the cards in the deck.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    public TextMeshProUGUI cdName;//Card Name
    public TextMeshProUGUI cdHealth;//Card Health
    public TextMeshProUGUI cdAttack;//Card Attack
    public TextMeshProUGUI cdMana;//Card Mana
    public TextMeshProUGUI cdSpeed;//Card Speed
    public SpellCard data;//Spell Data
    public GameObject rootOb;//Root Object

    public void Set(SpellCard card)//Here we set the card data
    {
        data = card;//Set the card data
        cdName.text = card.cardName;//Set the card name
        cdHealth.text = card.Health.ToString();//Set the card health
        cdAttack.text = card.Attack.ToString();//Set the card attack
        cdMana.text = card.Manacost.ToString();//Set the card mana cost
        cdSpeed.text = card.speed.ToString();//Set the card speed
    }

    public void RemoveCard()
    {
        rootOb.LeanScale(Vector3.zero, 0.3f).setOnComplete(() =>//Remove the card from the deck
        {
            DeckManager.instance.RemoveCard(data);//Remove the card from the deck
        });

    }

}
