// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;

/// <summary>
/// Info to pass from an attack.
/// </summary>
[Serializable]
public class HitInfo
{
    /// <summary>Base damage before stats are factored in.</summary>
    public int baseDamage;
    public int damage { get; private set; }
    public enum Effects { None, Fire, Lightning, Acid, Earth, Ice }
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
            damage = baseDamage * statManager.physicalAttack;
        }
        else
        {
            damage = baseDamage * statManager.magicAttack;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="statManager"></param>
    public void FactorDefendStats(StatManager statManager)
    {
        if (effect == Effects.None)
        {
            damage = Mathf.CeilToInt(damage/statManager.physicalDefense);
        }
        else
        {
            damage = Mathf.CeilToInt(damage/statManager.magicDefense);
        }
    }

}