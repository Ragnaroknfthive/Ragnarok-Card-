////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: AttackData.cs
//FileType: C# Source file
//Description : This is a scritpable object to hold attack data in poker
////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

/// <summary>
/// Attack location type in poker game withrespect to body part
/// </summary>
[System.Serializable]
public enum AttackLocation{
    None = -1,
    High = 0,
    Low = 1,
    Left = 2,
    Right = 3,
    Middle = 4
}
/// <summary>
/// Slider Attack type
/// </summary>
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
    public int damage;          //Damage by this attack
    public float speed;         //Speed of attack
    public int StaCost;         //Stamina cost for this attack
    //public AttackLocation location;
    public AttackType type;     //Attack type

    /// <summary>
    /// Get attack type from interger value
    /// </summary>
    /// <param name="i">attack value</param>
    /// <returns>Attack type</returns>
    public static AttackType GetAttackTypeFrmInt(int i){
        return (AttackType) i;
    }
    /// <summary>
    /// Get attack location type from interger value
    /// </summary>
    /// <param name="i">attack location value</param>
    /// <returns>Attack location type</returns>
    public static AttackLocation GetLocationFrmInt(int i){
        return (AttackLocation) i;
    }

}
