////////////////////////////////////////////////////////
///UI_panel.cs
///
///This script is used to open and close UI panels.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UI_panel : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;//The canvas group component

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();//Get the canvas group component
    }

    public void Close(bool instant = false)//Here we are closing the panel, we can choose to do it instantly or not
    {
        if (instant)
        {
            group.alpha = 0f;//Set the alpha to 0
            gameObject.SetActive(false);//Set the gameobject to inactive
        }
        else
        {
            LeanTween.alphaCanvas(group, 0f, 0f);//LeanTween the alpha to 0
            Invoke(nameof(ClosePanel), 0f);//Invoke the ClosePanel method after 0 seconds
        }

        group.interactable = group.blocksRaycasts = false;//Disable the interactable and blocksRaycasts
    }
    public void ClosePanel()//Method to close the panel
    {
        gameObject.SetActive(false);
    }
    public void OpenPanel()//Method to open the panel
    {
        gameObject.SetActive(true);
    }
    public void Open(bool instant = false)//Here we are opening the panel, we can choose to do it instantly or not
    {
        if (instant)
        {
            group.alpha = 1f;//Set the alpha to 1
            gameObject.SetActive(true);//Set the gameobject to active
        }
        else
        {
            gameObject.SetActive(true);//Set the gameobject to active
            LeanTween.alphaCanvas(group, 1f, 0);//LeanTween the alpha to 1
        }

        group.interactable = group.blocksRaycasts = true;//Enable the interactable and blocksRaycasts
    }
}
