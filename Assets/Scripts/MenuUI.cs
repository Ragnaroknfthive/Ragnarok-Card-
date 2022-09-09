using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MenuUI : MonoBehaviour
{

    private static MenuUI mui;

    public GameObject ErrorDisp;

    private PhotonView photonView;
    
    public Text MsgTxt;
    public Button PlayBtn;

    private void Awake()
    {
        mui = this;
        PlayBtn.GetComponentInChildren<Text>().text = "Connecting...";
        PlayBtn.interactable = false;
    }
    public static MenuUI Get(){
        return mui;
    }
    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMsg(string msg,bool Cancellable = false, bool AutoDistruct = false){
        MsgTxt.text = msg;
        ErrorDisp.SetActive(true);
       
        if(ErrorDisp.GetComponentInChildren<Button>()!=null)
        ErrorDisp.GetComponentInChildren<Button>().gameObject.SetActive(Cancellable);
        if(AutoDistruct){
            StartCoroutine(Hide());
        }
    }

    IEnumerator Hide(){
        yield return new WaitForSeconds(3f);
        
        ErrorDisp.gameObject.SetActive(false);
       
    }

    public void UpdatePlayButtonText() 
    {
        PlayBtn.GetComponentInChildren<Text>().text = "PLAY";
        PlayBtn.interactable = true;
    }
}
