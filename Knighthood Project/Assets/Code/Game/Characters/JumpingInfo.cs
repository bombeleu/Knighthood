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
  /// <summary>How fast the character moves upward.</summary>
  public float jumpSpeed;
  /// <summary>How long the character can jump.</summary>
  public float climbTime;

} // end JumpingInfo class