////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: SpellCard.cs
//FileType: C# Source file
//Description : This is a scritpable object used to define spell card
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spell card type
/// </summary>
public enum CardType { None, Pet, Spell, Buff }

public class SpellCard :ScriptableObject
{
    public int cardId = 0;                          //Card id to identify card
    public string cardName;                         //Name of spell card
    public CardType cardType;                       //Type of card
    public string discription;                      //Card detailed description
    public int Attack;                              //Card's attack capacity
    public int Health;                              //Card's health
    public int stamina;                             //Stamina of card
    public int speed;                               //Speed value of card
    public int Manacost = 3;                        //Mana cost for this card

    public Sprite MycardSprite;                     //Card sprite for player
    public Sprite OppocardSprite;                   //Card sprite for opponent side
    public GameObject SpellProjectilePref;          //Projectile prefab for this card

}

