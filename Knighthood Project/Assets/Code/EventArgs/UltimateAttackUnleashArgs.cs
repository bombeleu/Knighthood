// Steve Yeager
// 10.20.2013

using System;
using UnityEngine;

public class UltimateAttackUnleashArgs : EventArgs
{
    public readonly Vector3 startPosition;
    public readonly Quaternion startRotation;
    public readonly int attack;


    public UltimateAttackUnleashArgs(Vector3 startPosition, Quaternion startRotation, int attack)
    {
        this.startPosition = startPosition;
        this.startRotation = startRotation;
        this.attack = attack;
    }
}