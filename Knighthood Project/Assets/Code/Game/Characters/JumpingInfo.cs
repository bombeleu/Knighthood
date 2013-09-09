// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class to hold all info needed for the myCharacter jumping state.
/// </summary>
[Serializable]
public class JumpingInfo
{
  /// <summary>How fast the myCharacter moves upward.</summary>
  public float jumpSpeed;
  /// <summary>How long the myCharacter can jump.</summary>
  public float climbTime;

} // end JumpingInfo class