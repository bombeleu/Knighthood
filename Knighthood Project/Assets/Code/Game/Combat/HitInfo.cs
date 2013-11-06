// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Info to pass from an attackValue.
/// </summary>
[Serializable]
public class HitInfo
{
    #region Public Fields

    public int damage;
    public enum Effects { None = -1, Fire = 0, Lightning = 1, Acid = 2, Earth = 3, Ice = 4 }
    public Effects effect = Effects.None;
    public Vector3 knockBack;

    #endregion

    #region Private Fields

    private float attackStat;

    #endregion

    #region Const Fields

    private const float ATTACKMODIFIER = 0.5f;
    private const float MODIFIER = 20f;

    #endregion


    #region Public Methods

    /// <summary>
    /// Factor attack strength into damage.
    /// </summary>
    /// <param name="attackStat">Attack strength. Usually AttackPhysical or AttackMagic.</param>
    /// <returns>New HitInfo to give to Hitbox.</returns>
    public HitInfo Attack(float attackStat)
    {
        return new HitInfo()
        {
            //damage = Mathf.CeilToInt(damage * (int)attackStat * ATTACKMODIFIER),
            damage = this.damage,
            effect = this.effect,
            knockBack = this.knockBack,
            attackStat = attackStat,
        };
    }


    /// <summary>
    /// Factor average attack strength of multiple attackers into damage.
    /// </summary>
    /// <param name="attackStat">Attack strengths of all attackers. Usually AttackPhysical or AttackMagic.</param>
    /// <returns>New HitInfo to give to Hitbox.</returns>
    public HitInfo Attack(IEnumerable<float> attackStats)
    {
        return new HitInfo()
        {
            //damage = Mathf.CeilToInt(damage * (int)attackStats.Average() * ATTACKMODIFIER),
            damage = this.damage,
            effect = this.effect,
            knockBack = this.knockBack,
            attackStat = attackStats.Average(),
        };
    }


    /// <summary>
    /// Factor defense strength into damage. Usually DefenseStoutness.
    /// </summary>
    public void Defend(float defendStat, Vector3 defender, Vector3 attacker)
    {
        //damage = Mathf.CeilToInt(damage / defendStat);
        var delta = attackStat - defendStat;
        var mult = 1 + Mathf.Abs(delta / MODIFIER);
        if (attackStat > defendStat)
        {
            damage = Mathf.CeilToInt(damage * mult);
        }
        else
        {
            damage = Mathf.CeilToInt(damage / mult);
        }

        knockBack = new Vector3(Mathf.Sign(attacker.x - defender.x) * knockBack.x, knockBack.y, 0f);
    }

    #endregion
}