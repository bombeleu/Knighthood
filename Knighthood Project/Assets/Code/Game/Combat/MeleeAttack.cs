// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Attacks that only use the basic hitbox prefab.
/// </summary>
public class MeleeAttack : Attack
{
    public Vector2 size;
    public GameObject currentHitbox;


    public override bool Activate()
    {
        Attack();
        return true;
    } // end Activate


    /// <summary>
    /// Create hitbox.
    /// </summary>
    private void Attack()
    {
        currentHitbox = GameResources.Instance.Hitbox_Pool.nextFree;
        currentHitbox.transform.parent = myTransform;
        currentHitbox.transform.localScale = new Vector3(1f, size.y, size.x);
        currentHitbox.transform.localPosition = new Vector3(0f, offset.y, offset.x);
        currentHitbox.transform.rotation = myTransform.rotation;

        currentHitbox.GetComponent<Hitbox>().Initialize(character, hitInfo, hitboxTime);

        InvokeMethod("EndAttack", attackTime + cooldown);
    } // end Attack
	
} // end MeleeAttack class