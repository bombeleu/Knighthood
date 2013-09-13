// Steve Yeager
// 9.10.2013

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ComboManager : CombatManager
{
    public string[] comboStrings = new string[0];
    private string currentComboString = "";
    Queue<int> comboQueue = new Queue<int>();
    private bool attacking;



    public override bool CanActivate(string attackInput)
    {
        if (attacking)
        {
            
            int attackIndex = Array.IndexOf(inputs, attackInput);
            if (Array.IndexOf(comboStrings, currentComboString + attackIndex) != -1)
            {
                comboQueue.Enqueue(attackIndex);
                Debug.Log(comboQueue.Peek());
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


    public override void EndAttack()
    {
        if (comboQueue.Count == 0)
        {
            myCharacter.EndAttack();
            currentComboString = "";
        }
        else
        {
            ActivateComboNext();
        }
    }


    private IEnumerator OpenQueue(float openTime)
    {
        attacking = true;
        yield return WaitForTime(openTime);
        attacking = false;
    }


    private void ActivateComboNext()
    {
        currentComboString += comboQueue.Dequeue();
        int comboStringIndex = GetAttackIndexFromComboString(currentComboString);
        Texture attackTexture = Activate(comboStringIndex);
        myCharacter.SetState(Character.States.Attacking, new Dictionary<string, object> { { "attackTexture", attackTexture } });
    }


    private int GetAttackIndexFromInput(string attackInput)
    {
        int inputIndex = Array.IndexOf(inputs, attackInput);
        if (inputIndex == -1) return -1;

        int comboStringIndex = Array.IndexOf(comboStrings, currentComboString + inputIndex);
        return comboStringIndex;
    }


    private int GetAttackIndexFromComboString(string comboString)
    {
        return Array.IndexOf(comboStrings, comboString);
    }
}