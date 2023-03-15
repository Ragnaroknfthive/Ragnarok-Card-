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
}
