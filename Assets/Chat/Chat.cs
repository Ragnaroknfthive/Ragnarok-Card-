using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class Chat : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject Message;
    [SerializeField] GameObject content;
    [SerializeField] GameObject scrollView;

    public void SendMessage()
    {
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, (PhotonNetwork.NickName + " : " + inputField.text));
        inputField.text = "";
    }
    public void ShowHideChat()
    {
        if (scrollView.activeSelf)
        {
            scrollView.SetActive(false);
        }
        else
        {
            scrollView.SetActive(true);
        }
    }

    [PunRPC]
    public void GetMessage(string ReceiveMessage)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            GameObject newMessage = Instantiate(Message, Vector3.zero, Quaternion.identity, content.transform);
            newMessage.transform.Rotate(Vector3.forward, -180);
            newMessage.GetComponent<Message>().MyMessage.text = ReceiveMessage;
        }
        else
        {
            GameObject newMessage = Instantiate(Message, Vector3.zero, Quaternion.identity, content.transform);
            newMessage.GetComponent<Message>().MyMessage.text = ReceiveMessage;
        }
    }
}
