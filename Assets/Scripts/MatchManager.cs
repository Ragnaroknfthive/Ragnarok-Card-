using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField]
    PhotonCallback photonCallback;
    // Start is called before the first frame update
    void Start()
    {
       photonCallback= FindObjectOfType<PhotonCallback>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuickMatch() 
    {
        photonCallback.QuickMatch();
    }
}
