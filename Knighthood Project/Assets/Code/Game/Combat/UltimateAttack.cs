// Steve Yeager
// 10.20.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
public abstract class UltimateAttack : BaseMono
{
    public string attackName;
    public float attackTime;

    public abstract void Activate();
}