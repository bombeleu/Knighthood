// Steve Yeager
// 8.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class MagicAttackSystem : AttackSystem
{
  private Magic magic;
  public int magicRequired;



  #region MonoBehaviour Overrides

  protected override void Awake()
  {
    base.Awake();
    magic = GetSafeComponent<Magic>();
  } // end Awake

  #endregion


  public override bool Initiate()
  {
    if (magic.CastMagic(magicRequired))
    {

      return true;
    }
    else
    {
      return false;
    }
  } // end Initiate


  public override void Attack()
  {
    
  } // end Attack

} // end MagicAttackSystem class