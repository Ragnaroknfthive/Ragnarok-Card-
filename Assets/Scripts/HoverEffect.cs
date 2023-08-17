using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverEffect : MonoBehaviour
{
    [SerializeField]
    Image hoverImage;
    
    public void ShowHoverEffect()
    {
       hoverImage.sprite= MenuUI.Get().deckCardHoverSprite;
        hoverImage.enabled = true;
    }
    public void HideHoveEffect()
    {
        hoverImage.enabled = false;

        hoverImage.sprite =null;
    }
    public void ShowHoverEffectDeck()
    {
        hoverImage.sprite = MenuUI.Get().deckHoverSprite;
        hoverImage.enabled = true;
    }
    public void HideHoveEffectDeck()
    {
        hoverImage.enabled = false;

        hoverImage.sprite = null;
    }
}
