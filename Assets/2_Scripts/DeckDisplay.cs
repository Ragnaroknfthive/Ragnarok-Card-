using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    public TextMeshProUGUI cdName;
    public TextMeshProUGUI cdHealth;
    public TextMeshProUGUI cdAttack;
    public TextMeshProUGUI cdMana;
    public TextMeshProUGUI cdSpeed;
    public SpellCard data;
    public GameObject rootOb;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(SpellCard card){
        data = card;
        cdName.text = card.cardName;
        cdHealth.text = card.Health.ToString();
        cdAttack.text = card.Attack.ToString();
        cdMana.text = card.Manacost.ToString();
        cdSpeed.text = card.speed.ToString();
    }

    public void RemoveCard(){
        rootOb.LeanScale(Vector3.zero,0.3f).setOnComplete(()=>{
            DeckManager.instance.RemoveCard(data);
        });

    }

}
