using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType{
    Fire,
    Water,
    Wind,
    Earth,
    Ether
}

public enum CharacterStatus{
    Human,
    God
}

[CreateAssetMenu(fileName = "CharacterData", order = 0,menuName ="Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string id;
    public int level;

    public int health;
    public int strength;
    public int stamina;
    public int agility;
    public int dexterity;
    public int defence;
    public Sprite ChracterSp;
    public Sprite ChracterOppSp;
    public CharacterType type;
    public CharacterStatus status;

    public CharacterType weakAgainst;
    public Color tileColor;    

    public SpellCard SpecialAttack;
    public int SpecialAttackCost;
}
