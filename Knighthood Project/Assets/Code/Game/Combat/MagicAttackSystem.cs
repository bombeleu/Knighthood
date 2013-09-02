// Steve Yeager
// 8.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class MagicAttackSystem : AttackSystem
{
    private Magic magic;
    public int magicRequired;



    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();
        magic = GetSafeComponent<Magic>();
    } // end Start

    #endregion


    public override bool Initiate()
    {
        if (magic.CastMagic(magicRequired) && !attacking)
        {
            Attack();
            return true;
        }
        else
        {
            return false;
        }
    } // end Activate


    public override void Attack()
    {

    } // end Attack

} // end MagicAttackSystem class