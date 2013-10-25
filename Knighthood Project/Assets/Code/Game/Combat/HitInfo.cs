// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;

/// <summary>
/// Info to pass from an attackValue.
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
    /// Factor in reciever's StatManager to the damage.
    /// </summary>
    /// <param name="statManager">Reciever.</param>
    /// <returns>New HitInfo with correct damage.</returns>
    public HitInfo LocalizeDamage(StatManager statManager)
    {
        int newDamage = damage;

        if (effect == Effects.None)
        {
            newDamage = Mathf.CeilToInt(damage / statManager.physicalDefense);
        }
        else
        {
            newDamage = Mathf.CeilToInt(damage / statManager.magicDefense);
        }

        return new HitInfo()
        {
            baseDamage = baseDamage,
            damage = newDamage,
            effect = effect,
            knockBack = knockBack
        };
    }


    /// <summary>
    /// Correct knockback horizontal direction.
    /// </summary>
    /// <param name="target">Reciever.</param>
    /// <param name="position">Attacker.</param>
    /// <returns>New HitInfo with correct knockback.</returns>
    public HitInfo TransformKnockBack(Vector3 target, Vector3 position)
    {
        return new HitInfo()
        {
            baseDamage = baseDamage,
            damage = damage,
            effect = effect,
            knockBack = new Vector3(Mathf.Sign(target.x - position.x)*knockBack.x, knockBack.y, 0f)
        };
    }



    /// <summary>
    /// Multiply base damage by correct attackValue stat.
    /// </summary>
    /// <param name="statManager">Attacker's myStats.</param>
    /// <remarks>May need to change. Wind and Earth don't have effects.</remarks>
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
    /// 
    /// </summary>
    /// <param name="magicAttack"></param>
    public void FactorMagicAttack(int magicAttack)
    {
        damage = Mathf.CeilToInt(baseDamage * magicAttack * ATTACKMODIFIER);
    }

    #endregion
}