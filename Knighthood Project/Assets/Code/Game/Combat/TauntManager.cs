// Steve Yeager
// 9.12.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Manager for activating player taunts.
/// </summary>
public class TauntManager : BaseMono
{
    #region Public Fields

    public Texture[] textures = new Texture[4];
    public int healthRestore;
    public int magicRestore;
    public int combatIncrease;
    public float increaseTime;
    public float tauntTime;

    #endregion

    #region Private Fields

    private Character myCharacter;
    private readonly bool[] used = new bool[4];

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myCharacter = GetComponent<Character>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activate corresponding taunt.
    /// </summary>
    /// <param name="taunt">Index of taunt to activate. 0-3.</param>
    /// <returns>Texture of the corresponding taunt. Null if none activated.</returns>
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
                    myCharacter.myStats.TempChangeStat(StatManager.Stats.MagicDefense, combatIncrease);
                    myCharacter.myStats.TempChangeStat(StatManager.Stats.PhysicalDefense, combatIncrease);
                    StartCoroutine("DefenseIncrease");
                    break;
                // strength increase
                case 3:
                    myCharacter.myStats.TempChangeStat(StatManager.Stats.MagicAttack, combatIncrease);
                    myCharacter.myStats.TempChangeStat(StatManager.Stats.PhysicalAttack, combatIncrease);
                    StartCoroutine("AttackIncrease");
                    break;
            }

            used[taunt] = true;
        }

        StartCoroutine("Taunt");
        return textures[taunt];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns defense stats back to normal after set time.
    /// </summary>
    private IEnumerator DefenseIncrease()
    {
        yield return WaitForTime(increaseTime);
        myCharacter.myStats.TempChangeStat(StatManager.Stats.MagicDefense, -combatIncrease);
        myCharacter.myStats.TempChangeStat(StatManager.Stats.PhysicalDefense, -combatIncrease);
    }


    /// <summary>
    /// Returns attack stats back to normal after set time.
    /// </summary>
    private IEnumerator AttackIncrease()
    {
        yield return WaitForTime(increaseTime);
        myCharacter.myStats.TempChangeStat(StatManager.Stats.MagicAttack, -combatIncrease);
        myCharacter.myStats.TempChangeStat(StatManager.Stats.PhysicalAttack, -combatIncrease);
    }


    /// <summary>
    /// Tells the character when the taunt is over.
    /// </summary>
    private IEnumerator Taunt()
    {
        yield return WaitForTime(tauntTime);
        myCharacter.EndAttack();
    }

    #endregion
}