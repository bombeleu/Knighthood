// Steve Yeager
// 9.1.2013

using UnityEngine;

/// <summary>
/// Base class for all attacks.
/// </summary>
public abstract class Attack : BaseMono
{
    /// <summary>
    /// Start the attack if possible.
    /// </summary>
    /// <returns>True, if the attack was successfully started.</returns>
    public abstract Texture Activate();

}