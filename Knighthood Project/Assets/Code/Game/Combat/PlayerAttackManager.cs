// Steve Yeager
// 9.2.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Manages player attacks.
/// </summary>
public class PlayerAttackManager : BaseMono
{
    public Attack[] attacks = new Attack[7];
    public enum AttacksTypes
    {
        None = -1,
        Left = 0,
        Up = 1,
        Right = 2,
        MagicLeft = 3,
        MagicUp = 4,
        MagicRight = 5,
        MagicDown = 6,
    }



    /// <summary>
    /// Activate the correct Attack.
    /// </summary>
    /// <param name="attack">Attack to activate.</param>
    /// <returns>True, if Attack is not null and Attack successfully activates.</returns>
    public bool Activate(AttacksTypes attack)
    {
        Log(attack);
        if (attacks[(int)attack] != null && attacks[(int)attack].Activate())
        {
            return true;
        }
        else
        {
            return false;
        }
    } // end Activate
	
} // end PlayerAttackManager class