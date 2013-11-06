// Steve Yeager
// 8.18.2013

using System;

/// <summary>
/// Event args for an object being hit.
/// </summary>
public class HitEventArgs : EventArgs
{
    /// <summary>Hit info to be passed.</summary>
    public readonly HitInfo hitInfo;
    /// <summary>How much health remaining.</summary>
    public readonly int health;
    /// <summary>How much damage done.</summary>
    public readonly int damage;


    public HitEventArgs(HitInfo hitInfo, int health, int damage)
    {
        this.hitInfo = hitInfo;
        this.health = health;
        this.damage = damage;
    }
}