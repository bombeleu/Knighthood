// Steve Yeager
// 8.21.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Holds all attacks for a player.
/// </summary>
[Obsolete("Use PlayerAttackManager instead", true)]
public class PlayerAttacks : BaseMono
{
    public enum Attacks
    {
        LightNormal = 0, LightSide = 1, LightUp = 2, LightDown = 3,
        HeavyNormal = 4, HeavySide = 5, HeavyUp = 6, HeavyDown = 7,
        RangedNormal = 8, RangedSide = 9, RangedUp = 10, RangedDown = 11,
        MagicLeft = 12, MagicUp = 13, MagicRight = 14, MagicDown = 15,
        None
    }
    public Attack[] attackSystems = new Attack[16];


    /// <summary>
    /// Activate the correct Attack.
    /// </summary>
    /// <param name="attack">Attack to activate.</param>
    /// <returns>True, if Attack is not null and Attack successfully activates.</returns>
    public bool Activate(Attacks attack)
    {
        Log(attack);
        if (attackSystems[(int)attack] != null && attackSystems[(int)attack].Activate())
        {
            return true;
        }
        else
        {
            return false;
        }
    } // end Activate

} // end PlayerAttacks class