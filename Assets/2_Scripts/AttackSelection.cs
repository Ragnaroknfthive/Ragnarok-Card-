///////////////////////////////////////////////////////
/// AttackSelection.cs
/// 
/// This script is responsible for displaying the attack selection in the battle scene.
/// It also handles the attack's damage, speed, stamina cost, and type.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackSelection : MonoBehaviour
{

    public AttackData data;//Attack data
    public Button button;//Button to select the attack

    public Text infoTxt;//Text to display the attack info

    public Text ComboTxt;//Text to display the attack combo

    public static List<AttackSelection> attackSelections = new List<AttackSelection>();//List of all attack selections
    private void Awake()
    {
        if (!attackSelections.Contains(this)) attackSelections.Add(this);//Add this attack selection to the list
    }
    void Start()
    {
        if (infoTxt != null && data.type != AttackType.None) UpdateInfo();//Update the attack info text
    }

    void UpdateInfo()
    {
        if (data.type == AttackType.Defend)
            infoTxt.text = "Damage : " + data.damage + "\n Speed : " + data.speed + "\n Sta cost : +" + (-data.StaCost);//If the attack is a defend type, display the info with a + sign before the stamina cost
        else
            infoTxt.text = "Damage : " + data.damage + "\n Speed : " + data.speed + "\n Sta cost : " + data.StaCost;//If the attack is not a defend type, display the info with a - sign before the stamina cost
    }

    public void UpdateComboTxt(int i)
    {//Update the combo text
        switch (i)//Switch case to check the combo number
        {
            case 0:
                ComboTxt.text = data.type.ToString();//Display the attack type
                break;
            case 1:
                ComboTxt.text = data.type.ToString() + " 2";//Display the attack type with 2
                break;
            case 2:
                ComboTxt.text = data.type.ToString() + " 3";//Display the attack type with 3
                break;
        }
    }
}
