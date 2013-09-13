// Steve Yeager
// 9.10.2013

using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class CombatManager : BaseMono
{
    protected Character myCharacter;
    public Attack[] attacks = new Attack[0];
    public string[] inputs = new string[0];
    public bool[] open = new bool[0]; // possibly move to Editor



    public virtual bool CanActivate(string attackInput)
    {
        int index = Array.IndexOf(inputs, attackInput);
        if (index != -1 && attacks[index] != null)
        {
            return attacks[index].CanActivate();
        }

        return false;
    }
    

    public virtual Texture Activate(int attack)
    {
        if (attack < attacks.Length && attacks[attack] != null)
        {
            return attacks[attack].Activate();
        }

        return null;
    }


    public virtual Texture Activate(string attackInput)
    {
        int index = Array.IndexOf(inputs, attackInput);
        if (index != -1 && attacks[index] != null)
        {
            return attacks[index].Activate();
        }

        return null;
    }


    public virtual void EndAttack()
    {
        myCharacter.EndAttack();
    }

    public void Initialize(Character character)
    {
        myCharacter = character;
        foreach (var attack in attacks)
        {
            attack.Initialize(this);
        }
    }
}