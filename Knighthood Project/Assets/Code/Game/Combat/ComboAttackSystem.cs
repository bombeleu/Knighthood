// Steve Yeager
// 8.24.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for attack systems that feature combos.
/// </summary>
public class ComboAttackSystem : AttackSystem
{
  private int currentAttack = 0;
  public float coolDownTime;
  public Vector3[] hitboxSizes;
  public Vector3[] hitboxOffsets;
  public HitInfo[] hitInfos;
  public float[] attackTimes;
  private bool queued = false;
  private GameObject currentHitbox;


  public override bool Initiate()
  {
    if (attacking)
    {
      queued = true;
    }
    else
    {
      Attack();
    }

    return true;
  } // end Initiate


  /// <summary>
  /// Start the attack. Create the hitbox.
  /// </summary>
  public override void Attack()
  {
    StopCoroutine("CoolDown");

    attacking = true;
    currentHitbox = GameResources.Instance.Hitbox_Pool.nextFree;
    currentHitbox.transform.parent = myTransform;
    currentHitbox.transform.rotation = myTransform.rotation;
    currentHitbox.transform.localPosition = hitboxOffsets[currentAttack];
    currentHitbox.GetComponent<MeleeHitbox>().Initialize(character, hitboxSizes[currentAttack], hitInfos[currentAttack], attackTimes[currentAttack]);

    StartCoroutine("CoolDown", attackTimes[currentAttack]);
    
    if (currentAttack == hitInfos.Length - 1)
    {
      currentAttack = 0;
    }
    else
    {
      currentAttack++;
    } 
  } // end Attack


  /// <summary>
  /// Time after an attack to be able to increase the combo.
  /// </summary>
  private IEnumerator CoolDown(float wait)
  {
    yield return new WaitForSeconds(wait);
    if (queued)
    {
      queued = false;
      Attack();
      yield break;
    }

    attacking = false;
    yield return new WaitForSeconds(coolDownTime);
    currentAttack = 0;
  } // end CoolDown

} // end ComboAttackSystem class