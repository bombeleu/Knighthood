// Steve Yeager
// 8.25.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Manages a character's magic.
/// </summary>
public class Magic : BaseMono
{
    #region Reference Fields

    private StatManager myStats;

    #endregion

    #region Public Fields

    public float currentMagic;

    #endregion

    #region Private Fields

    private bool regenerating;
    private Job regenerate;

    #endregion

    #region Const Fields

    private float REGENINTERVAL = 0.3f;

    #endregion


    #region Public Methods

    /// <summary>
    /// Get magic ready for new character that has a myStats.
    /// </summary>
    /// <param name="stats"></param>
    public void Initialize(StatManager stats)
    {
        myStats = stats;
        currentMagic = (int)stats.magicPool.value;
    }


    /// <summary>
    /// Use magic. Start magic regen.
    /// </summary>
    /// <param name="amount">Amount of magic needed.</param>
    /// <returns>True, if had enough magic.</returns>
    public bool CastMagic(int amount)
    {
        if (amount > currentMagic) return false;

        currentMagic -= amount;
        if (!regenerating) regenerate = new Job(RegenerateMagic());

        return true;
    }


    /// <summary>
    /// Does the character have enough magic to cast attackValue?
    /// </summary>
    /// <param name="amount">Amount needed.</param>
    /// <returns>True, if there is enough magic in reserve.</returns>
    public bool EnoughMagic(int amount)
    {
        return amount <= currentMagic;
    }


    /// <summary>
    /// Change the magic amount.
    /// </summary>
    /// <param name="amount">Amount to add to magic reserve.</param>
    public void ChangeMagic(int amount)
    {
        currentMagic = Mathf.Clamp(currentMagic + amount, 0, (int)myStats.magicPool.value);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Regenerates magic overtime.
    /// </summary>
    private IEnumerator RegenerateMagic()
    {
        regenerating = true;
        while (currentMagic < myStats.magicPool.value)
        {
            yield return WaitForTime(REGENINTERVAL);
            currentMagic += myStats.magicRegen.value;
        }
        currentMagic = myStats.magicPool.value;
        regenerating = false;
    }

    #endregion
}