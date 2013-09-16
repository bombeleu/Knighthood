// Steve Yeager
// 9.6.2013

using UnityEngine;

/// <summary>
/// Extend the Transform class.
/// </summary>
public static class TransformExtension
{
    public static void Align(this Transform transform)
    {
        transform.localScale = new Vector3(Vector3.Dot(transform.forward, Vector3.right) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}