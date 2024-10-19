using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum CardValue { two, three, four, five, six, seven, eight, nine, ten, jack, queen, king, ace,none };
public enum CardColor { hearts, diamonds, spades, clubs }

[Serializable]
public class Card_SO : ScriptableObject
{
    
    public Sprite cardSprite;   
    public CardValue cardValue;
    public CardColor cardColor;


}
