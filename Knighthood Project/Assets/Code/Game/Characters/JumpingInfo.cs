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
  public float jumpSpeed;
  public float climbTime;
  public float leapTime;
  public float floatTime;
} // end JumpingInfo class