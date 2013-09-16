// Steve Yeager
// 8.7.2013

using UnityEngine;

/// <summary>
/// Tile texture over object.
/// </summary>
[ExecuteInEditMode]
public class Checker : MonoBehaviour
{
  void OnEnable()
  {
    renderer.material.mainTextureScale = new Vector2(transform.lossyScale.x/2, transform.lossyScale.y/2);
  }
}