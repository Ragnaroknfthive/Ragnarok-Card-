using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnCrown : MonoBehaviour
{
    public Game game;
    public Chessman pawn;

    private void SetPawnToCrown()
    {
        pawn = game.SetPawnToCrown();
    }

    public void Queen()
    {
        SetPawnToCrown();
        pawn.PawnToQueen();
        game.HidePawnCrownedSelector();
        game.NextTurn();
    }

    public void Rook()
    {
        SetPawnToCrown();
        pawn.PawnToRook();
        game.HidePawnCrownedSelector();
        game.NextTurn();
    }

    public void Knight()
    {
        SetPawnToCrown();
        pawn.PawnToKnight();
        game.HidePawnCrownedSelector();
        game.NextTurn();
    }

    public void Bishop()
    {
        SetPawnToCrown();
        pawn.PawnToBishop();
        game.HidePawnCrownedSelector();
        game.NextTurn();
    }
}
