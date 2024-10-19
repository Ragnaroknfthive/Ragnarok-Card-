using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AttackLocation{
    None = -1,
    High = 0,
    Low = 1,
    Left = 2,
    Right = 3,
    Middle = 4
}

[System.Serializable]
public enum AttackType{
    None = 0,
    Heavy = 1,
    Speed = 2,
    Defend = 3
}

[CreateAssetMenu(fileName = "AttackData", order = 0,menuName ="Data/AttackData")]
public class AttackData : ScriptableObject
{
    public int damage;
    public float speed;
    public int StaCost;
    //public AttackLocation location;
    public AttackType type;


    public static AttackType GetAttackTypeFrmInt(int i){
        return (AttackType) i;
    }

    public static AttackLocation GetLocationFrmInt(int i){
        return (AttackLocation) i;
    }

}
