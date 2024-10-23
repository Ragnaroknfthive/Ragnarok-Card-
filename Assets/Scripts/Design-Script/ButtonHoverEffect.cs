////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: ButtonScrollbarController.cs
//FileType: C# Source file
//Description : This script is used for activating hover effect on a object when pointer is hover over button object
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject unionGameObject;                  //Effect object

    /// <summary>
    /// Show hover effect on object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowUnionGameObject(true);
    }
    /// <summary>
    /// Hide hover effect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ShowUnionGameObject(false);
    }
    /// <summary>
    /// Show/Hide effect object
    /// </summary>
    /// <param name="show">True : indicates show and flase indicates hide</param>
    private void ShowUnionGameObject(bool show)
    {
        unionGameObject.SetActive(show);
    }
}
