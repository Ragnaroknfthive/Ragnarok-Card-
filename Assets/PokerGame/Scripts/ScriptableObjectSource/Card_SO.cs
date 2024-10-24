////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Card_SO.cs
//FileType: C# Scriptable object file
//Description : This is a scirptable object file for poker game card
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

/// <summary>
/// Value of poker card
/// </summary>
public enum CardValue { two, three, four, five, six, seven, eight, nine, ten, jack, queen, king, ace,none };
/// <summary>
/// Cart type - > Hears , diamonds, spades ,clubs 
/// </summary>
public enum CardColor { hearts, diamonds, spades, clubs }

/// <summary>
/// Card scriptable object
/// </summary>
[Serializable]
public class Card_SO : ScriptableObject
{
    
    public Sprite cardSprite;               //Card sprite
    public CardValue cardValue;             //Value of card
    public CardColor cardColor;             //Type of card
}
