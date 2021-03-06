﻿// Steve Yeager
// 9.10.2013

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manger for all combos for a character.
/// </summary>
public class ComboManager : CombatManager
{
    #region Public Fields

    public string[] comboStrings = new string[0];

    #endregion

    #region Private Fields

    private string currentComboString = "";
    private Queue<int> comboQueue = new Queue<int>();
    private bool attacking;

    #endregion


    #region CombatManager Overrides

    public override bool CanActivate(string attackInput)
    {
        if (attacking)
        {

            int attackIndex = Array.IndexOf(inputs, attackInput);
            if (Array.IndexOf(comboStrings, currentComboString + attackIndex) != -1)
            {
                comboQueue.Enqueue(attackIndex);
            }
            return false;
        }
        else
        {
            int attackIndex = GetAttackIndexFromInput(attackInput);
            if (attackIndex != -1 && attacks[attackIndex] != null)
            {
                return attacks[attackIndex].CanActivate();
            }

            return false;
        }
    }


    public override Texture Activate(string attackInput)
    {
        int attackIndex = GetAttackIndexFromInput(attackInput);
        currentComboString += attackIndex;
        StartCoroutine("OpenQueue", attacks[attackIndex].attackTime);
        return base.Activate(attackIndex);
    }


    public override void EndAttack(bool cancelled)
    {
        if (comboQueue.Count == 0)
        {
            base.EndAttack(false);
            currentComboString = "";
        }
        else
        {
            ActivateComboNext();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Timer for setting toggling the queue open status.
    /// </summary>
    /// <param name="openTime">Attack time of an attackValue.</param>
    private IEnumerator OpenQueue(float openTime)
    {
        attacking = true;
        yield return WaitForTime(openTime);
        attacking = false;
    }


    /// <summary>
    /// Starts the next attackValue if one is queued up.
    /// </summary>
    private void ActivateComboNext()
    {
        currentComboString += comboQueue.Dequeue();
        int comboStringIndex = GetAttackIndexFromComboString(currentComboString);
        Texture attackTexture = Activate(comboStringIndex);
        myCharacter.SetState(Character.AttackingState, new Dictionary<string, object> { { "attackTexture", attackTexture } });
    }


    /// <summary>
    /// Get the index of the next possible attackValue.
    /// </summary>
    /// <param name="attackInput">Name of input.</param>
    /// <returns>Index of the attackValue corresponding with the next combo.</returns>
    private int GetAttackIndexFromInput(string attackInput)
    {
        int inputIndex = Array.IndexOf(inputs, attackInput);
        if (inputIndex == -1) return -1;

        int comboStringIndex = Array.IndexOf(comboStrings, currentComboString + inputIndex);
        return comboStringIndex;
    }


    /// <summary>
    /// Get index of attackValue.
    /// </summary>
    /// <param name="comboString">Combo string for attackValue.</param>
    /// <returns>Index of the corresponding attackValue.</returns>
    private int GetAttackIndexFromComboString(string comboString)
    {
        return Array.IndexOf(comboStrings, comboString);
    }

    #endregion
}