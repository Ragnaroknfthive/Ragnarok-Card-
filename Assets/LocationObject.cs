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
        part.color = Color.white - (Color.white * (damaged_amt *  damaged_times));
        DispText.text = "";
    }
    
    void OnEnable()
    {
        if(!objects.Contains(this)){
            objects.Add(this);
        }
        part = GetComponent<Image>();
        part.color = Color.white - (Color.white * (damaged_amt *  damaged_times));
        DispText.text = "";
    }


    public void OnPointerEnter(){
        part.color = HighlightColor;
        DispText.text = LocationName;
    }

    public void OnPointerExit(){
        part.color = Color.white - (Color.white * (damaged_amt *  damaged_times));
    }

    public void OnPointerDown(){
        PVPManager.manager.selectChoice(id);
    }
}
