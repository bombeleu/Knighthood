// Steve Yeager
// 9.10.2013

using System;
using UnityEngine;

/// <summary>
/// Base class for managers for combat.
/// </summary>
public abstract class CombatManager : BaseMono
{
    protected Character myCharacter;
    public Attack[] attacks = new Attack[0];
    public string[] inputs = new string[0];
    public bool[] open = new bool[0];


    #region Virtual Methods

    /// <summary>
    /// Checks the status of the corresponding attack.
    /// </summary>
    /// <param name="attack">Index of attack to activate</param>
    /// <returns>True, if the attack can activate.</returns>
    public virtual bool CanActivate(int attack)
    {
        if (attacks[attack] != null)
        {
            return attacks[attack].CanActivate();
        }

        return false;
    }


    /// <summary>
    /// Checks the status of the corresponding attack.
    /// </summary>
    /// <param name="attackInput">Input pressed.</param>
    /// <returns>True, if the attack can activate.</returns>
    public virtual bool CanActivate(string attackInput)
    {
        int index = Array.IndexOf(inputs, attackInput);
        if (index != -1 && attacks[index] != null)
        {
            return attacks[index].CanActivate();
        }

        return false;
    }
    

    /// <summary>
    /// Activates the correct attack.
    /// </summary>
    /// <param name="attack">Index of the attack to activate.</param>
    /// <returns>Texture corresponding to the attack. Null if nothing activated.</returns>
    public virtual Texture Activate(int attack)
    {
        if (attack < attacks.Length && attacks[attack] != null)
        {
            return attacks[attack].Activate();
        }

        return null;
    }


    /// <summary>
    /// Activates an attack corresponding to the attack input.
    /// </summary>
    /// <param name="attackInput">Name of the attack input.</param>
    /// <returns>Texture corresponding to the attack. Null if nothing activated.</returns>
    public virtual Texture Activate(string attackInput)
    {
        int index = Array.IndexOf(inputs, attackInput);
        if (index != -1 && attacks[index] != null)
        {
            return attacks[index].Activate();
        }

        return null;
    }


    /// <summary>
    /// Relays to the character that the attack has ended.
    /// </summary>
    public virtual void EndAttack()
    {
        myCharacter.EndAttack();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize manager and all of its attacks.
    /// </summary>
    /// <param name="character"></param>
    public void Initialize(Character character)
    {
        myCharacter = character;
        foreach (var attack in attacks)
        {
            attack.Initialize(this);
        }
    }

    #endregion
}