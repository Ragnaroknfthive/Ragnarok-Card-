//////////////////////////////////////////////////////////////////
///Game Data.cs
///
///This script is used to store all the data of the game. It is a singleton class.
///It stores all the character data, attack data, pet data and spell data.
///It also stores the profile image of the player and the opponent.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameData : MonoBehaviour
{

    private static GameData data;//singleton instance

    public List<CharacterData> characters;//list of all characters
    public List<AttackData> attacks;//list of all attacks

    public List<SpellCard> Pets;//list of all pets
    public List<SpellCard> Spells;//list of all spells

    public List<Sprite> DummyProfile = new List<Sprite>();
    public static string playerProfileUrl;//url of the player profile image
    public static bool hasProfileImage = false;//flag to check if the player has a profile image
    public static Texture2D playerProfileTexture, opponentProfileTexture;//texture of the player profile image
    public static Sprite playerSprite, opponentSprite;//sprite of the player profile image
    public static int dummyProfileIndex = -1;//index of the dummy profile image
    public const string hasProfileConst = "hasProfileConst", profileUrl = "profileUrl", dummyProfileImageIndex = "dummyProfileIndex";//constants for player profile image
    private void Awake()
    {
        if (data == null)//singleton instance
        {
            data = this;
            DontDestroyOnLoad(data.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    void Start()
    {
        playerSprite = DummyProfile[0];//default player profile image
        opponentSprite = DummyProfile[0];//default opponent profile image
        characters = Resources.LoadAll<CharacterData>("Data/Character").ToList();//load all character data
        attacks = Resources.LoadAll<AttackData>("Data/Attacks").ToList();//load all attack data
    }

    public CharacterData GetCharacter(string id)//get character data by id
    {
        foreach (var item in characters)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }//The way this function works is that it goes through all the characters and checks if the id of the
    //character is equal to the id passed as a parameter. If it is, it returns the character data, the next methods
    //works in a similar way.

    public SpellCard GetPet(int id)//get pet data by id
    {
        foreach (var item in Pets)
        {
            if (item.cardId == id)
                return item;
        }
        return null;
    }//

    public SpellCard GetSpell(int id)//get spell data by id
    {
        foreach (var item in Spells)
        {
            if (item.cardId == id)
                return item;
        }
        return null;
    }

    public AttackData GetAttack(AttackType type)//get attack data by type
    {
        foreach (var item in attacks)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }

    public static GameData Get()//get singleton instance
    {
        return data;
    }
    public static Sprite SpriteFromTexture2D(Texture2D texture)//convert texture to sprite
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);//create sprite from texture
    }
}
