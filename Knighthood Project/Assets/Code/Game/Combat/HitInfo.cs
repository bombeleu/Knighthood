// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Info to pass from an attack.
/// </summary>
[Serializable]
public class HitInfo
{
    /// <summary>Base damage before stats are factored in.</summary>
    public int damage;
    public enum Effects { None, Fire, Acid, Earth, Ice }
    public Effects effect;
    public Vector3 knockBack;



    /// <summary>
    /// 
    /// </summary>
    /// <param name="statManager"></param>
    public void FactorAttackStats(StatManager statManager)
    {
        if (effect == Effects.None)
        {
            damage *= statManager.physicalAttack;
        }
        else
        {
            damage *= statManager.magicAttack;
        }
    } // end FactorAttackStats


    /// <summary>
    /// 
    /// </summary>
    /// <param name="statManager"></param>
    public void FactorDefendStats(StatManager statManager)
    {
        if (effect == Effects.None)
        {
            damage /= statManager.physicalAttack;
        }
        else
        {
            damage /= statManager.magicAttack;
        }
    } // end FactorDefendStats

} // end HitInfo class