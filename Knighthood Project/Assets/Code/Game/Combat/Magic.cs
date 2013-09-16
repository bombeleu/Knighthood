// Steve Yeager
// 8.25.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// Manages a character's magic.
/// </summary>
public class Magic : BaseMono
{
    #region Public Fields

    public int maxMagic;
    public int currentMagic;
    public float regenInterval;

    #endregion

    #region Private Fields

    private bool regenerating;

    #endregion


    #region Public Methods

    /// <summary>
    /// Get magic ready for new character.
    /// </summary>
    public void Initialize()
    {
        currentMagic = maxMagic;
    }


    /// <summary>
    /// Get magic ready for new character that has a StatManager.
    /// </summary>
    /// <param name="stats"></param>
    public void Initialize(StatManager stats)
    {
        maxMagic = stats.magicMax;
        currentMagic = maxMagic;
    }


    /// <summary>
    /// Use magic. Start magic regen.
    /// </summary>
    /// <param name="amount">Amount of magic needed.</param>
    /// <returns>True, if had enough magic.</returns>
    public bool CastMagic(int amount)
    {
        if (amount > currentMagic) return false;
        Debug.Log("Cast");
        currentMagic -= amount;
        StopCoroutine("RegenerateMagic");
        StartCoroutine("RegenerateMagic");
        return true;
    }


    /// <summary>
    /// Does the character have enough magic to cast attack?
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
        currentMagic = Mathf.Clamp(currentMagic + amount, 0, maxMagic);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Regenerates magic overtime.
    /// </summary>
    private IEnumerator RegenerateMagic()
    {
        if (regenerating) yield break;
        regenerating = true;
        while (currentMagic < maxMagic)
        {
            
            yield return WaitForTime(regenInterval);
            currentMagic++;
        }
        currentMagic = maxMagic;
    }

    #endregion
}