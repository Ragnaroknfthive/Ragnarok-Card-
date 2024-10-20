////////////////////////////////////////////////////////
///LocationOBject.cs
///
///This script is used to highlight the location object when the mouse is over it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationObject : MonoBehaviour
{
    public Color HighlightColor;//Highlight Color
    private Image part;//Image component
    public int id;

    public int damaged_times = 0;//Number of times damaged
    public float damaged_amt = 0.1f;//Amount of damage
    public static List<LocationObject> objects = new List<LocationObject>();//List of location objects
    public TextMeshProUGUI DispText;//Display Text
    public string LocationName;//Location Name


    void Awake()
    {
        if (objects == null) objects = new List<LocationObject>();//If the list is null, create a new list
        if (!objects.Contains(this)) objects.Add(this);//If the list does not contain this object, add it to the list
    }

    void OnDestroy()
    {
        objects.Remove(this);//Remove the object from the list
    }

    public static LocationObject GetLocation(int i)//This method is used to get the location object by his id
    {
        objects = PVPManager.manager.locationObjects;//Set the objects list to the location objects list
        //Debug.LogError("object set");//Log the object set

        foreach (var item in objects)//Loop through the objects
        {
            if (item.id == i) return item;//If the object id is equal to the id, return the object
        }
        return null;
    }
    private void Start()
    {
        part = GetComponent<Image>();//Get the image component
        part.color = Color.white - (Color.white * (damaged_amt * damaged_times));//Set the color
        DispText.text = "";//Set the display text to empty
    }

    void OnEnable()//When the object is enabled
    {
        if (!objects.Contains(this)) objects.Add(this);//If the list does not contain this object, add it to the list
        part = GetComponent<Image>();//Get the image component
        part.color = Color.white - (Color.white * (damaged_amt * damaged_times));//Set the color
        DispText.text = "";//Set the display text to empty
    }


    public void OnPointerEnter()//When the pointer enters the object
    {
        part.color = HighlightColor;//Set the color to the highlight color
        DispText.text = LocationName;//Set the display text to the location name
    }

    public void OnPointerExit()//When the pointer exits the object
    {
        part.color = Color.white - (Color.white * (damaged_amt * damaged_times));//Set the color
    }

    public void OnPointerDown()//When the pointer is down
    {
        PVPManager.manager.selectChoice(id);//Select the choice
    }
}
