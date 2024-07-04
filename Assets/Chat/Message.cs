using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Message : MonoBehaviour
{
    public TextMeshProUGUI MyMessage;

    public void Start()
    {
        GetComponent<RectTransform>().SetAsFirstSibling();
    }
}
