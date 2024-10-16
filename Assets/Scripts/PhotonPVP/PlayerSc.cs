////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PlayerSc.cs
//FileType: C# Source file
//Description : This is a c# script reference used for Player UI (Not used now)
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSc : MonoBehaviour
{

    public Image CharIm; //character sprite
    public Image ProfIm; //Profile image sprite

    public CharacterData Char;      //character scriptable

    public PlayerChoice choice;     //player's location choice

    public List<PlayerChoice> extraChoices; 

}
