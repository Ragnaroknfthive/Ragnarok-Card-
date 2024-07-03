using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationObject : MonoBehaviour
{
    public Color HighlightColor;
    private Image part; 
    public int id;

    public int damaged_times = 0;
    public float damaged_amt = 0.1f;
    public static List<LocationObject> objects = new List<LocationObject>();
    public TextMeshProUGUI DispText;
    public string LocationName;
    public GameObject PartHitDetails;
    public TextMeshProUGUI partHitTimeText,paneltiesForBeingHitText;
    
    void Awake()
    {
        if(objects == null)
            objects = new List<LocationObject>();
        if(!objects.Contains(this)){
            objects.Add(this);
        }
    }    
    
    void OnDestroy()
    {
        objects.Remove(this);
    }

    public static LocationObject GetLocation(int i){
     objects = PVPManager.manager.locationObjects;
            Debug.LogError("object set");

        foreach (var item in objects)
        {
            if(item.id == i)
                return item;
        }
        return null;
    }
    private void Start()
    {
        part = GetComponent<Image>();
        part.color = Color.white - (Color.white * (damaged_amt *  GetHitTimes()));
        DispText.text = "";
    }
    
    void OnEnable()
    {
        if(!objects.Contains(this)){
            objects.Add(this);
        }
        part = GetComponent<Image>();
        part.color = Color.white - (Color.white * (damaged_amt *  GetHitTimes()));
        DispText.text = "";
    }


    public void OnPointerEnter(){
        part.color = HighlightColor;
        DispText.text = LocationName;

        partHitTimeText.text ="Hit : "+ GetHitTimes().ToString();
       
        paneltiesForBeingHitText.text = "Damage : " + damaged_amt.ToString();
        PartHitDetails.SetActive(true);
        Debug.LogError("Name of Location " + LocationName);
    }
    public int GetHitTimes() 
    {
        int attackTimes = 0;
        switch(LocationName)
        {
            case "High":
                attackTimes =  PVPManager.manager.isAttackedHigh;
                break;
            case "Left":
                attackTimes = PVPManager.manager.isAttackedOnLeftSide;
                break;
            case "Low":
                attackTimes = PVPManager.manager.isAttackedLow;
                break;
            case "Middle":
                attackTimes = PVPManager.manager.isAttackedInMiddle;
                break;
            case "Right":
                attackTimes = PVPManager.manager.isAttackedOnRightSide;
                break;
            default:
                break;
        }
        return attackTimes;
    }

    public void OnPointerExit(){
        part.color = Color.white - (Color.white * (damaged_amt *  GetHitTimes()));
        PartHitDetails.SetActive(false);
    }

    public void OnPointerDown(){
        PVPManager.manager.selectChoice(id);
    }
}
