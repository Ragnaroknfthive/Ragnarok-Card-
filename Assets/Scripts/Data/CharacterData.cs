////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: CharacterData.cs
//FileType: C# Source file
//Description : This is a scritpable object used to define different characters
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

/// <summary>
/// Type of character
/// </summary>
public enum CharacterType{
    Fire,
    Water,
    Wind,
    Earth,
    Ether
}
/// <summary>
/// Status of character
/// </summary>
public enum CharacterStatus{
    Human,
    God
}

[CreateAssetMenu(fileName = "CharacterData", order = 0,menuName ="Data/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string id;                           //Character id used to identify character
    public int level;                           //Character level

    public int health;                          //Character health
    public int strength;                        //Strength of character
    public int stamina;                         //Character stamina
    public int agility;                        
    public int dexterity;
    public int defence;
    public Sprite ChracterSp;                   //Character sprite
    public Sprite ChracterOppSp;                //Character sprite oppsite type of player-black /white player
    public CharacterType type;                  //Type of character
    public CharacterStatus status;              //Status of charcater

    public CharacterType weakAgainst;           //Type of the character against which this character is weak
    public Color tileColor;                     //Tile color for this character ( chess board tile color)

    public SpellCard SpecialAttack;             //Spell card for this character
    public int SpecialAttackCost;               //Cost of spell attack
}
