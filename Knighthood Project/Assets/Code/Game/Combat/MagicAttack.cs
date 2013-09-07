// Steve Yeager
// 9.5.2013

using UnityEngine;

/// <summary>
/// Attack that requires magic from the character.
/// </summary>
public class MagicAttack : Attack
{
    public GameObject AttackPrefab;
    public int magicRequired;
    private ObjectRecycler AttackPrefabPool;
    private GameObject currentAttack;
    public bool parent = true;
    public Vector3 shootVector;
    public bool oneHit;


    #region MonoBehaviour Overrides

    private void Start()
    {
        AttackPrefabPool = parent ? new ObjectRecycler(AttackPrefab, myTransform) : new ObjectRecycler(AttackPrefab);
    }

    #endregion



    public override bool Activate()
    {
        if (character.GetComponent<Magic>().CastMagic(magicRequired))
        {
            Attack();
            return true;
        }
        else
        {
            return false;
        }
    }


    private void Attack()
    {
        currentAttack = AttackPrefabPool.nextFree;
        currentAttack.transform.position = myTransform.position + new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.rotation = myTransform.rotation;
        currentAttack.transform.Align();

        if (shootVector != Vector3.zero)
        {
            currentAttack.GetComponent<Hitbox>().Initialize(character, hitInfo, hitboxTime, hitNumber, myTransform.TransformDirection(shootVector), oneHit);
        }
        else
        {
            currentAttack.GetComponent<Hitbox>().Initialize(character, hitInfo, hitboxTime, hitNumber, oneHit);
        }

        InvokeMethod("EndAttack", attackTime + cooldown);
    }

}