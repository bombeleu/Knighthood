// Steve Yeager
// 8.19.2013

using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CameraInfo
{
  /// <summary></summary>
  public float boundaryDynamic;
  /// <summary></summary>
  public float boundaryStatic;
  [HideInInspector]
  /// <summary></summary>
  public bool locked;

} // end CameraInfo class