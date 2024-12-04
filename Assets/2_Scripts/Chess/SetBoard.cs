using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetBoard : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject invBoard;
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            board.SetActive(true);
            invBoard.SetActive(false);
        }
        else
        {
            board.SetActive(false);
            invBoard.SetActive(true);
        }
    }
}
