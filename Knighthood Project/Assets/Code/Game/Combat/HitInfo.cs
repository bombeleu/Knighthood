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
    #region Public Fields

    /// <summary>Base damage before stats are factored in.</summary>
    public int baseDamage;
    public int damage { get; private set; }
    public enum Effects { None = -1, Fire = 0, Lightning = 1, Acid = 2, Earth = 3, Ice = 4 }
    public Effects effect;
    public Vector3 knockBack;

    #endregion

    #region Const Fields

    private const float ATTACKMODIFIER = 0.5f;

    #endregion


    #region Public Methods

    /// <summary>
    /// Multiply base damage by correct attack stat.
    /// </summary>
    /// <param name="statManager">Attacker's myStats.</param>
    public void FactorAttackStats(StatManager statManager)
    {
        if (effect == Effects.None)
        {
            damage = Mathf.CeilToInt(baseDamage * statManager.physicalAttack*ATTACKMODIFIER);
        }
        else
        {
            damage = Mathf.CeilToInt(baseDamage * statManager.magicAttack*ATTACKMODIFIER);
        }
    }


    /// <summary>
    /// Multiple base damage by correct defense stat. Needs to have FactorAttackStats called first.
    /// </summary>
    /// <param name="statManager">Reciever's myStats.</param>
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

    #endregion
}