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


    #region MonoBehaviour Overrides

    private void Start()
    {
        AttackPrefabPool = new ObjectRecycler(AttackPrefab, myTransform);
    }

    #endregion



    public override bool Activate()
    {
        if (magicRequired <= character.GetComponent<Magic>().currentMagic)
        {
            character.GetComponent<Magic>().currentMagic -= magicRequired;
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
        currentAttack.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentAttack.transform.rotation = myTransform.rotation;

        currentAttack.GetComponent<Hitbox>().Initialize(character, hitInfo, hitboxTime, hitNumber);

        InvokeMethod("EndAttack", attackTime + cooldown);
    }

}