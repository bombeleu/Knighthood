// Steve Yeager
// 

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
[Serializable]
public class AbilitySystem
{
    public GameObject Attack_Prefab;
	
} // end AbilitySystem class


[Serializable]
public class MagicAbilitySystem : AbilitySystem
{
    public float magic;
}