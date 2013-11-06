// Steve Yeager
// 10.28.2013

using System;

/// <summary>
/// 
/// </summary>
public class HealthEventArgs : EventArgs
{
    public readonly int health;
    public readonly int change;


    public HealthEventArgs(int health, int change)
    {
        this.health = health;
        this.change = change;
    }
}