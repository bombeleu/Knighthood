// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Info to pass from an attack.
/// </summary>
public class HitInfo
{
  /// <summary>How much damage being applied.</summary>
  public readonly int damage;


  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="damage">How much damage being applied.</param>
  public HitInfo(int damage)
  {
    this.damage = damage;
  } // end HitInfo

} // end HitInfo class