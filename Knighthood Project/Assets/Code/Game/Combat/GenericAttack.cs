// Steve Yeager
// 9.7.2013

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 
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

    public string attackType;
    public string attackName;
    public Texture attackAnimation;

    // attack
    public float windup;
    public float attackTime;
    public float cooldown;

    // hitbox
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

    #region Public Methods

    public override Texture Activate()
    {
        if (magic && !myMagic.CastMagic(magicRequired))
        {
            return null;
        }

        StartCoroutine(Attack());
        return attackAnimation;
    }

    #endregion

    #region Private Methods

    private IEnumerator Attack()
    {
        yield return WaitForTime(windup);

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
        
        yield return WaitForTime(attackTime + cooldown);
        myCharacter.EndAttack();
    }


    private void MeleeAttack()
    {
        currentAttack = GameResources.Instance.Hitbox_Pool.nextFree;
        currentAttack.transform.parent = myTransform;
        currentAttack.transform.localRotation = Quaternion.identity;
        currentAttack.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.localScale = new Vector3(1f, meleeSize.y, meleeSize.x);
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, hitInfo, hitboxTime, hitNumber);
    }


    private void ProjectileAttack()
    {
        currentAttack = (GameObject) Instantiate(Attack_Prefab,
                                                 myTransform.position + myTransform.forward * offset.x + myTransform.up * offset.y,
                                                 myTransform.rotation); 
        currentAttack.transform.Align();
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, hitInfo, hitboxTime, hitNumber, myTransform.forward*projectileSpeed, oneShot);
    }


    private void StandardAttack()
    {
        currentAttack = Attack_Pool.nextFree;
        currentAttack.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.localRotation = Quaternion.identity;
        currentAttack.transform.Align();
        currentAttack.GetComponent<Hitbox>().Initialize(myCharacter, hitInfo, hitboxTime, hitNumber, oneShot);
    }

    #endregion

}