using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackSelection : MonoBehaviour
{

    public AttackData data;
    public Button button;

    public Text infoTxt;

    public Text ComboTxt;

    public static List<AttackSelection> attackSelections = new List<AttackSelection>();
    private void Awake()
    {
        if(!attackSelections.Contains(this))
            attackSelections.Add(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("<color=white> Attack Selection start </color>");

        //PVPManager.manager.P1HeavyComboIndex = 0;
        //PVPManager.manager.P1SpeedComboIndex = 0;

        //when match start data.type is 0 to solve combo reset
        //data.type = 0;

        if (infoTxt != null && data.type != AttackType.None){
            UpdateInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateInfo(){
        if(data.type == AttackType.Defend)
            infoTxt.text = "Damage : "+data.damage+"\n Speed : "+data.speed+"\n Sta cost : +"+(-data.StaCost);
        else
            infoTxt.text = "Damage : "+data.damage+"\n Speed : "+data.speed+"\n Sta cost : "+data.StaCost;
    }

    public void UpdateComboTxt(int i){
        ////Debug.Log("Updating for "+data.type.ToString()+" i = "+i);
        switch (i)
        {
            case 0:
                ComboTxt.text = data.type.ToString();
                break;
            case 1:
                ComboTxt.text = data.type.ToString()+" 2";
                break;
            case 2:
                ComboTxt.text = data.type.ToString()+" 3";
                break;
        }
    }
}
