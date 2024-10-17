////////////////////////////
///MatchManager.cs
///
///Here we have the MatchManager class, which is responsible for managing the match.
///It has a QuickMatch method that calls the QuickMatch method from the PhotonCallback class.
////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField]
    PhotonCallback photonCallback;//Reference to the PhotonCallback class

    void Start()
    {
        photonCallback = FindObjectOfType<PhotonCallback>();//Find the PhotonCallback object in the scene
    }
    public void QuickMatch()//Method to call the QuickMatch method from the PhotonCallback class
    {
        photonCallback.QuickMatch();//Call the QuickMatch method from the PhotonCallback class
    }
}
