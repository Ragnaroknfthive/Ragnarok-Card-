using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivePieceItem : MonoBehaviour
{

    public int id;
    public void Select(){
        Game.Get().ChangePawnToNewPiece(id);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
