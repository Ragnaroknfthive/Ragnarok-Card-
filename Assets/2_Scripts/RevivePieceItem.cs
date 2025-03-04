////////////////////////////////////////////////////////////////////
///RevivePieceItem.cs
///
///This script is used to change the pawn to a new piece when the pawn reaches the end of the board.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivePieceItem : MonoBehaviour
{

    public int id;
    public void Select()
    {
        Game.Get().ChangePawnToNewPiece(id);//Change the pawn to a new piece
    }
}
