// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class to hold all info needed for the character jumping state.
/// </summary>
[Serializable]
public class JumpingInfo
{
  /// <summary></summary>
  public float jumpSpeed;
  /// <summary></summary>
  public float climbTime;
  /// <summary></summary>
  public float floatTime;

} // end JumpingInfo class