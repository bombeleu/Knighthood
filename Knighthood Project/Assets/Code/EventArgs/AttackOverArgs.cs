// Steve Yeager
// 10.17.2013

using System;

public class AttackOverArgs : EventArgs
{
    public readonly bool cancelled;


    public AttackOverArgs(bool cancelled)
    {
        this.cancelled = cancelled;
    }
}