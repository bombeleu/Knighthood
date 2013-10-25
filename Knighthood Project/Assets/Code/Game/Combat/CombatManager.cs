// Steve Yeager
// 9.10.2013

using System;
using UnityEngine;

/// <summary>
/// Base class for managers for combat.
/// </summary>
public abstract class CombatManager : BaseMono
{
    #region References

    protected Character myCharacter;

    #endregion

    #region Public Fields

    public Attack[] attacks = new Attack[0];
    public string[] inputs = new string[0];
    public bool[] open = new bool[0];

    #endregion

    #region Private Fields

    private int activated = -1;

    #endregion

    #region Events

    public EventHandler<AttackOverArgs> OnAttackOver;

    #endregion


    #region Virtual Methods

    /// <summary>
    /// Checks the status of the corresponding attackValue.
    /// </summary>
    /// <param name="attackValue">Index of attackValue to activate</param>
    /// <returns>True, if the attackValue can activate.</returns>
    public virtual bool CanActivate(int attack)
    {
        if (attacks[attack] != null)
        {
            return attacks[attack].CanActivate();
        }

        return false;
    }


    /// <summary>
    /// Checks the status of the corresponding attackValue.
    /// </summary>
    /// <param name="attackInput">Input pressed.</param>
    /// <returns>True, if the attackValue can activate.</returns>
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
    /// Activates the correct attackValue.
    /// </summary>
    /// <param name="attackValue">Index of the attackValue to activate.</param>
    /// <returns>Texture corresponding to the attackValue. Null if nothing activated.</returns>
    public virtual Texture Activate(int attack)
    {
        if (attack < attacks.Length && attacks[attack] != null)
        {
            activated = attack;
            return attacks[attack].Activate();
        }

        return null;
    }


    /// <summary>
    /// Activates an attackValue corresponding to the attackValue input.
    /// </summary>
    /// <param name="attackInput">Name of the attackValue input.</param>
    /// <returns>Texture corresponding to the attackValue. Null if nothing activated.</returns>
    public virtual Texture Activate(string attackInput)
    {
        int index = Array.IndexOf(inputs, attackInput);
        if (index != -1 && attacks[index] != null)
        {
            activated = index;
            return attacks[index].Activate();
        }

        return null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancelled"></param>
    public virtual void EndAttack(bool cancelled)
    {
        activated = -1;
        if (OnAttackOver != null) OnAttackOver(this, new AttackOverArgs(cancelled));
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


    /// <summary>
    /// 
    /// </summary>
    public virtual void Cancel()
    {
        if (activated == -1) return;
        attacks[activated].Cancel();
    }

    #endregion
}