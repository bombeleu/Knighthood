// Steve Yeager
// 9.12.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class TauntManager : BaseMono
{
    private Character myCharacter;
    public Texture[] textures = new Texture[4];
    private bool[] used = new bool[4];
    public int healthRestore;
    public int magicRestore;
    public int combatIncrease;
    public float increaseTime;
    public float tauntTime;


    private void Awake()
    {
        // get references
        myCharacter = GetComponent<Character>();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="taunt"></param>
    /// <returns></returns>
    public Texture Activate(int taunt)
    {
        if (!used[taunt])
        {
            switch (taunt)
            {
                // replenish health
                case 0:
                    GetComponent<Health>().ChangeHealth(healthRestore);
                    break;
                // replenish magic
                case 1:
                    GetComponent<Magic>().ChangeMagic(magicRestore);
                    break;
                // defense increase
                case 2:
                    myCharacter.StatManager.TempChangeStat(StatManager.Stats.MagicDefense, combatIncrease);
                    myCharacter.StatManager.TempChangeStat(StatManager.Stats.PhysicalDefense, combatIncrease);
                    StartCoroutine("DefenseIncrease");
                    break;
                // strength increase
                case 3:
                    myCharacter.StatManager.TempChangeStat(StatManager.Stats.MagicAttack, combatIncrease);
                    myCharacter.StatManager.TempChangeStat(StatManager.Stats.PhysicalAttack, combatIncrease);
                    StartCoroutine("AttackIncrease");
                    break;
            }

            used[taunt] = true;
        }

        StartCoroutine("Taunt");
        return textures[taunt];
    }


    private IEnumerator DefenseIncrease()
    {
        yield return WaitForTime(increaseTime);
        myCharacter.StatManager.TempChangeStat(StatManager.Stats.MagicDefense, -combatIncrease);
        myCharacter.StatManager.TempChangeStat(StatManager.Stats.PhysicalDefense, -combatIncrease);
    }


    private IEnumerator AttackIncrease()
    {
        yield return WaitForTime(increaseTime);
        myCharacter.StatManager.TempChangeStat(StatManager.Stats.MagicAttack, -combatIncrease);
        myCharacter.StatManager.TempChangeStat(StatManager.Stats.PhysicalAttack, -combatIncrease);
    }


    private IEnumerator Taunt()
    {
        yield return WaitForTime(tauntTime);
        myCharacter.EndAttack();
    }
}