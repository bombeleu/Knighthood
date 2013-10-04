// Steve Yeager
// 10.3.2013

using System;

/// <summary>
/// Event args for when a player collects money.
/// </summary>
public class MoneyEventAgs : EventArgs
{
    public readonly int worth;


    public MoneyEventAgs(int worth)
    {
        this.worth = worth;
    }
}