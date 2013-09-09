// Steve Yeager
// 9.7.2013

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Base attack manager for all characters.
/// </summary>
[ExecuteInEditMode]
public class AttackManager : BaseMono
{
    public GenericAttack[] genericAttacks = new GenericAttack[0];
    public string[] genericAttackTypes = new string[0];
    public string[] attackNames = new string[0];
    public bool[] open = new bool[0];




    public Texture Activate(int attack)
    {
        if (attack < genericAttacks.Length && genericAttacks[attack] != null)
        {
            return genericAttacks[attack].Activate();
        }

        return null;
    }


    public Texture Activate(string attack)
    {
        int index = Array.IndexOf(genericAttackTypes, attack);
        if (index != -1 && genericAttacks[index] != null)
        {
            return genericAttacks[index].Activate();
        }

        return null;
    }
}