using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameData : MonoBehaviour
{

    private static GameData data;

    public List<CharacterData> characters;
    public List<AttackData> attacks;

    public List<SpellCard> spells;

    private void Awake()
    {
        data = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        characters = Resources.LoadAll<CharacterData>("Data/Character").ToList();
        attacks = Resources.LoadAll<AttackData>("Data/Attacks").ToList();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CharacterData GetCharacter(string id){
        foreach (var item in characters)
        {
            if(item.id == id)
                return item;
        }
        return null;
    }

    public SpellCard GetSpell(int id){
        foreach (var item in spells)
        {
            if(item.cardId == id)
                return item;
        }
        return null;
    }

    public AttackData GetAttack(AttackType type){
        foreach (var item in attacks)
        {
            if(item.type == type)
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

    public static GameData Get(){
        return data;
    }
}
