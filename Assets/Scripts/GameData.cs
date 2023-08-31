using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameData : MonoBehaviour
{

    private static GameData data;

    public List<CharacterData> characters;
    public List<AttackData> attacks;

    public List<SpellCard> Pets;
    public List<SpellCard> Spells;

    public List<Sprite> DummyProfile = new List<Sprite>();
    public static string playerProfileUrl;
    public static bool hasProfileImage = false;
    public static Texture2D playerProfileTexture, opponentProfileTexture;
    public static Sprite playerSprite,opponentSprite;
    public static int dummyProfileIndex=-1;
    public const string hasProfileConst = "hasProfileConst", profileUrl = "profileUrl", dummyProfileImageIndex = "dummyProfileIndex";
    private void Awake()
    {
        if (data == null)
        {
            data = this;
            DontDestroyOnLoad(data.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        playerSprite = DummyProfile[0];
        opponentSprite = DummyProfile[0];
        characters = Resources.LoadAll<CharacterData>("Data/Character").ToList();
        //Debug.LogError(characters.Count);
        attacks = Resources.LoadAll<AttackData>("Data/Attacks").ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public CharacterData GetCharacter(string id)
    {
        foreach (var item in characters)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }

    public SpellCard GetPet(int id)
    {
        foreach (var item in Pets)
        {
            if (item.cardId == id)
                return item;
        }
        return null;
    }

    public SpellCard GetSpell(int id)
    {
        foreach (var item in Spells)
        {
            if (item.cardId == id)
                return item;
        }
        return null;
    }

    public AttackData GetAttack(AttackType type)
    {
        foreach (var item in attacks)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }

    // public AttackData GetAttack(AttackLocation type){
    //     foreach (var item in attacks)
    //     {
    //         if(item.location == type)
    //             return item;
    //     }
    //     return null;
    // }

    public static GameData Get()
    {
        return data;
    }
    public static Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture,new Rect(0.0f,0.0f,texture.width,texture.height),new Vector2(0.5f,0.5f),100.0f);
    }
}
