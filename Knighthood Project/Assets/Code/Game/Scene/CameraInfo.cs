// Steve Yeager
// 8.19.2013

using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CameraInfo
{
  /// <summary>Screen percentage when crossed moves the camera.</summary>
  public float boundaryDynamic = 0.25f;
  /// <summary>Screen percentage when crossed stops the player.</summary>
  public float boundaryStatic = 0.05f;
  [HideInInspector]
  /// <summary>If the camera remains still.</summary>
  public bool locked;
  /// <summary>How close the camera can be to the players. Camera orthagonal size.</summary>
  public float minDistance = 11f;
  /// <summary>How far the camera can be from the players. Camera orthagonal size.</summary>
  public float maxDistance = 15f;


  public float baseSpeed = 2f;


  public float boundaryOut;
  public float boundaryIn;



  public float zoomSpeed = 10f;
  public float distanceBuffer = 0.1f;

} // end CameraInfo class