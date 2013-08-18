// Steve Yeager
// 8.7.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Tile texture over object.
/// </summary>
public class Checker : MonoBehaviour
{
  void Update()
  {
    renderer.material.mainTextureScale = new Vector2(transform.lossyScale.x/2, transform.lossyScale.y/2);
  } // end Awake

} // end Checker class