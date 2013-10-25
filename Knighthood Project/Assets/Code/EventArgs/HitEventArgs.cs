// Steve Yeager
// 8.18.2013

using System;

/// <summary>
/// Event args for an object being hit.
/// </summary>
public class HitEventArgs : EventArgs
{
  /// <summary>Hit info to be passed.</summary>
  public readonly HitInfo hitInfo;
  /// <summary>If the attackValue killed the object.</summary>
  public readonly bool dead;


  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="hitInfo">Hit info to be passed.</param>
  /// <param name="dead">If the attackValue killed the object.</param>
  public HitEventArgs(HitInfo hitInfo, bool dead)
  {
    this.hitInfo = hitInfo;
    this.dead = dead;
  }
}