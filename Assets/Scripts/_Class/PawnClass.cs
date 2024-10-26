using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnClass : MonoBehaviour
{
    [SerializeField] private bool isMoved = false;
    public bool IsMoved()
    {
        return isMoved;
    }
    public void SetMoved(bool value)
    {
        isMoved = value;
    }
}
