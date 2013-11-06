// Steve Yeager
// 8.25.2013

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Health for characters.
/// </summary>
public class CharacterHealth : Health
{
    #region Reference Fields

    private StatManager statManager;
    private Transform myTransform;

    #endregion

    #region Public Fields

    public enum AttackArmor { None, Weak, Strong }
    public AttackArmor attackArmor;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        statManager = GetSafeComponent<Character>().myStats;
        myTransform = transform;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set current and max health through the StatManager.
    /// </summary>
    /// <param name="statManager"></param>
    public void Initialize(StatManager stats)
    {
        currentHealth = maxHealth;
        this.statManager = stats;
    }

    #endregion

    #region Health Overrides

    public override void RecieveHit(List<object> senders, int hitID, HitInfo hitInfo)
    {
        // no hit if same hitID
        if (hitID == lastHitID) return;
        lastHitID = hitID;

        int damage = 0;
        // no damage applied if invincible or have Strong Attack Armor
        if (!invincible && attackArmor != AttackArmor.Strong)
        {
            hitInfo.Defend(statManager.defenseStoutness.value, myTransform.position, ((MonoBehaviour)senders[0]).transform.position);
            damage = hitInfo.damage;

            // status effect
            if (hitInfo.effect != HitInfo.Effects.None)
            {
                damage = Mathf.CeilToInt(damage * statusEffectivenesses[(int)hitInfo.effect]);
                if (damage > 0)
                {
                    Log(hitInfo.effect + ":" + (int)hitInfo.effect, Debugger.LogTypes.Combat);
                    StopAllCoroutines();
                    StartCoroutine(statusMethods[(int)hitInfo.effect], damage);
                }
            }

            ChangeHealth(-damage);
            CreateIndicator(damage);
        }

        //if (HitEvent != null)
        //{
        //    HitEvent(senders, new HitEventArgs(hitInfo, currentHealth, damage));
        //}

        OnGroupHitEvent(senders, new HitEventArgs(hitInfo, currentHealth, damage));
    }


    //[Obsolete("Making one RecieveHit")]
    //public override void RecieveHit(object[] senders, int hitID, HitInfo hitInfo)
    //{
    //    // no hit if same hitID
    //    if (hitID == lastHitID) return;
    //    lastHitID = hitID;

    //    int damage = 0;
    //    // no damage applied if invincible or have Strong Attack Armor
    //    if (!invincible && attackArmor != AttackArmor.Strong)
    //    {
    //        hitInfo = hitInfo.LocalizeDamage(statManager);
    //        damage = hitInfo.damage;

    //        // status effect
    //        if (hitInfo.effect != HitInfo.Effects.None)
    //        {
    //            damage = Mathf.CeilToInt(damage * statusEffectivenesses[(int)hitInfo.effect]);
    //            if (damage > 0)
    //            {
    //                Log(hitInfo.effect + ":" + (int)hitInfo.effect, Debugger.LogTypes.Combat);
    //                StopAllCoroutines();
    //                StartCoroutine(statusMethods[(int)hitInfo.effect], damage);
    //            }
    //        }

    //        ChangeHealth(-damage);
    //        CreateIndicator(damage);
    //    }

    //    OnGroupHitEvent(senders, new HitEventArgs(hitInfo, currentHealth, damage));
    //}

    #endregion
}