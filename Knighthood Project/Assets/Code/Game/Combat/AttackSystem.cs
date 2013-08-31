// Steve Yeager
// 8.21.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all attacks.
/// </summary>
public abstract class AttackSystem : BaseMono
{
  protected Transform myTransform;
  protected Character character;
  protected bool attacking = false;


  protected virtual void Awake()
  {
    // get references
    myTransform = transform;
    character = GetSafeComponent<Character>();
  } // end Awake

  public abstract bool Initiate();

  public abstract void Attack();

} // end AttackSystem class