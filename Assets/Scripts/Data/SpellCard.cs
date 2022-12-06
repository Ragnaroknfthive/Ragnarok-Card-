

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { None, Pet, Spell, Buff }

public class SpellCard :ScriptableObject
{
    public int cardId = 0;
    public string cardName;
    public CardType cardType;
    public string discription;
    public int Attack;
    public int Health;
    public int stamina;
    public int speed;
    public int Manacost = 3;

    public Sprite MycardSprite;
    public Sprite OppocardSprite;
    public GameObject SpellProjectilePref;

}

