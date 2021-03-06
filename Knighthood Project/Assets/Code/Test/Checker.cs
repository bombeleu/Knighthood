﻿// Steve Yeager
// 8.7.2013

using UnityEngine;

/// <summary>
/// Tile texture over object.
/// </summary>
[ExecuteInEditMode]
public class Checker : MonoBehaviour
{
    public Material CheckerMaterial;

    void OnEnable()
    {
        renderer.material = new Material(CheckerMaterial);
        renderer.sharedMaterial.mainTextureScale = new Vector2(transform.lossyScale.x / 2, transform.lossyScale.y / 2);

        if (gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            if (tag == "Translucent")
            {
                renderer.sharedMaterial.color = Color.gray;
            }
            else
            {
                renderer.sharedMaterial.color = Color.white;                
            }
        }
    }
}