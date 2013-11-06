// Steve Yeager
// 9.7.2013

using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attack that can be created using public variables in the inspector.
/// </summary>
public class GenericAttack : Attack
{
    #region Private Fields

    private Transform myTransform;
    private Character myCharacter;
    private Magic myMagic;

    #endregion

    #region GUI Fields

    public bool showTooltips;

    #endregion

    #region Base Fields

    public Texture attackAnimation;
    public GameObject Attack_Prefab;
    private ObjectRecycler Attack_Pool;
    private GameObject currentAttack;
    public HitInfo hitInfo;
    public float hitboxTime;
    public Vector2 offset;
    public bool parented;
    public int hitNumber = 1;
    public bool oneShot;

    #endregion

    #region Melee Fields

    public bool melee;
    public Vector2 meleeSize;

    #endregion

    #region Magic Fields

    public bool magic;
    public int magicRequired;

    #endregion

    #region Projectile Fields

    public bool projectile;
    [Obsolete("Maybe use angle? Or don't need at all?")]
    public Vector2 projectileVector;
    public float projectileSpeed;

    #endregion

    #region Movement Fields

    public bool move;
    public Vector3 endVector;
    public float moveTime;

    #endregion


    #region MonoBehaviour Overrides

    void Awake()
    {
        // get references
        myCharacter = GetComponent<Character>();
        myTransform = transform;
        myMagic = GetComponent<Magic>();

        // setup
        if (!melee)
        {
            Attack_Pool = new ObjectRecycler(Attack_Prefab, (projectile ? null : myTransform));
        }
    }

    #endregion

    #region Attack Overrides

    public override bool CanActivate()
    {
        if (magic)
        {
            return canActivate && myMagic.EnoughMagic(magicRequired);
        }

        return canActivate;
    }


    public override Texture Activate()
    {
        if (!canActivate) return null;

        if (magic && !myMagic.CastMagic(magicRequired))
        {
            return null;
        }

        attackJob = new Job(Attack());

        return attackAnimation;
    }


    public override void Cancel()
    {
        attackJob.Kill();

        if (currentAttack != null && (parented || melee))
        {
            currentAttack.GetComponent<Hitbox>().End();
        }

        manager.EndAttack(true);

        InvokeAction(() => canActivate = true, cooldown);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Run through all attacking stages.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        canActivate = false;

        myHealth.attackArmor = attackArmor;

        yield return WaitForTime(windUp);

        if (melee)
        {
            MeleeAttack();
        }
        else if (projectile)
        {
            ProjectileAttack();
        }
        else
        {
            StandardAttack();
        }

        yield return WaitForTime(attackTime + windDown);
        manager.EndAttack(false);

        myHealth.attackArmor = CharacterHealth.AttackArmor.None;

        yield return WaitForTime(cooldown);
        canActivate = true;
    }


    private void MeleeAttack()
    {
        currentAttack = GameResources.Instance.Hitbox_Pool.nextFree;
        currentAttack.transform.parent = myTransform;
        currentAttack.transform.localRotation = Quaternion.identity;
        currentAttack.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.localScale = new Vector3(1f, meleeSize.y, meleeSize.x);
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, FactorAttack(), hitboxTime, hitNumber);
    }


    private void ProjectileAttack()
    {
        currentAttack = Attack_Pool.nextFree;
        currentAttack.transform.localPosition = myTransform.position + myTransform.TransformDirection(0f, offset.y, offset.x);
        currentAttack.transform.rotation = myTransform.rotation;
        currentAttack.transform.Align();
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, FactorAttack(), hitboxTime, hitNumber, myTransform.forward * projectileSpeed, oneShot);
    }


    private void StandardAttack()
    {
        currentAttack = Attack_Pool.nextFree;
        currentAttack.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.localRotation = Quaternion.identity;
        currentAttack.transform.Align();
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, FactorAttack(), hitboxTime, hitNumber, oneShot);
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HitInfo FactorAttack()
    {
        if (magic)
        {
            return hitInfo.Attack(myCharacter.myStats.attackMagic.value);
        }
        else
        {
            return hitInfo.Attack(myCharacter.myStats.attackPhysical.value);
        }
    }

    #endregion
}