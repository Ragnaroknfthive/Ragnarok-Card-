//////////////////////////////////////////////////////
///GameManager.cs
///
///This script is a singleton class that manages the game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instace;//singleton
    public bool isFristMovePawn = true;//check if the first move is made by the pawn

    private void Awake()//Singleton
    {
        if (instace == null)
        {
            instace = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
