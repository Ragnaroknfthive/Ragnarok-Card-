////////////////////////////////////////////
/// HoverEffect.cs
/// 
/// This script is responsible for showing and hiding hover effects on UI elements.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverEffect : MonoBehaviour
{
    [SerializeField] Image hoverImage;//Image to show hover effect

    public void ShowHoverEffect()//Show hover effect on card
    {
        //hoverImage.sprite= MenuUI.Get().deckCardHoverSprite;
        hoverImage.enabled = true;//Enable hover effect
    }
    public void HideHoveEffect()//Hide hover effect on card
    {
        hoverImage.enabled = false;

        hoverImage.sprite = null;
    }
    public void ShowHoverEffectDeck()//Show hover effect on deck
    {
        //hoverImage.sprite = MenuUI.Get().deckHoverSprite;
        hoverImage.enabled = true;
    }
    public void HideHoveEffectDeck()//Hide hover effect on deck
    {
        hoverImage.enabled = false;

        hoverImage.sprite = null;
    }
}
