// Steve Yeager
// 8.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Hitbox for melee attacks that don't require any art.
/// </summary>
public class MeleeHitbox : Hitbox
{
  public Vector3 size;


  public void Initialize(Character sender, Vector3 size, HitInfo hitInfo, float time)
  {
    myTransform.localScale = size;
    this.hitInfo = hitInfo;
    this.time = time;
    base.Initialize(sender);
  } // end Initialize

} // end MeleeHitbox class