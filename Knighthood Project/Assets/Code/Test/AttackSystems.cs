// Steve Yeager
// 

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class AttackSystems : BaseMono
{
    public AbilitySystem[] attackSystems;

    public AttackSystems()
    {
        attackSystems = new AbilitySystem[]
        {
            new AbilitySystem(),
            new AbilitySystem(),
            new MagicAbilitySystem()
        };
    }

} // end AttackSystems class