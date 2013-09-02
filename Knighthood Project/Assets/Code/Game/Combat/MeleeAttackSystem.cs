// Steve Yeager
// 8.31.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for single melee attack systems.
/// </summary>
public class MeleeAttackSystem : AttackSystem
{
    public float windup;
    public float cooldown;
    //public AttackInfo attackInfo;

    public override bool Initiate()
    {
        if (attacking)
        {
            return false;
        }
        else
        {
            StartCoroutine(Windup());
            return true;
        }
    } // end Activate


    public override void Attack()
    {
        // create attack ---
        //InvokeAction(EndAttack, attackInfo.time);
    } // end Attack


    private IEnumerator Windup()
    {
        // animation ---
        yield return WaitForTime(windup);
        Attack();
    } // end Windup
	
} // end MeleeAttackSystem class