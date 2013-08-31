// Steve Yeager
// 8.24.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Holds info for creating a hitbox.
/// </summary>
[Serializable]
public class AttackInfo
{
  public Vector3 offset;
  public float time;
  public HitInfo hitInfo;

} // end AttackInfo class