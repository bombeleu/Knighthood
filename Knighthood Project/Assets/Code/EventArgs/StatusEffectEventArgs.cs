// Steve Yeager
// 10/5/2013

using System;

/// <summary>
/// 
/// </summary>
public class StatusEffectEventArgs : EventArgs
{
    public readonly bool superEffective;
    public readonly float time;

    public StatusEffectEventArgs(bool superEffective, float time)
    {
        this.superEffective = superEffective;
        this.time = time;
    }
}