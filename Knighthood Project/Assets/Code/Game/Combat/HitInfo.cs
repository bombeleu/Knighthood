// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Info to pass from an attack.
/// </summary>
[Serializable]
public class HitInfo
{
  /// <summary>How much damage being applied.</summary>
  public int damage;
  public enum Effects { None, Fire, Acid, Earth, Ice }
  public Effects effect;
  public Vector3 knockBack;



  public void FactorAttackStats(StatManager statManager)
  {
    if (effect == Effects.None)
    {
      damage *= statManager.physicalAttack;
    }
    else
    {
      damage *= statManager.magicAttack;
    }
  } // end FactorAttackStats


  public void FactorDefendStats(StatManager statManager)
  {
    if (effect == Effects.None)
    {
      damage /= statManager.physicalAttack;
    }
    else
    {
      damage /= statManager.magicAttack;
    }
  } // end FactorDefendStats

} // end HitInfo class