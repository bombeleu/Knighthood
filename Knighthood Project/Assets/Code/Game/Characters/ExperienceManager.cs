// Steve Yeager
// 10.1.2013

using System;
using UnityEngine;

/// <summary>
/// Manages a player's experience and leveling up while playing a level.
/// </summary>
[Serializable]
public class ExperienceManager
{
    #region Public Fields
    /// <summary>How much exp the player currently has.</summary>
    public int currentEXP;
    /// <summary>How much exp needed to get to the next level.</summary>
    public int neededEXP;
    /// <summary>Player's current level.</summary>
    public int level;

    #endregion

    #region Private Fields

    /// <summary>Total exp player has ever accumulated. Used for saving/loading.</summary>
    private int totalEXP;

    #endregion

    #region Properties

    /// <summary>How many points the player has gotten towards stat progression by leveling up in the current level.</summary>
    public int statPoints { get; private set; }

    #endregion

    #region Const Fields

    private const string TOTALEXPERIENCEPATH = ": Total Experience";

    #endregion


    #region Public Methods

    /// <summary>
    /// LoadHighscore total exp and calculate level.
    /// </summary>
    /// <param name="username">Player's username.</param>
    public void Load(string username)
    {
        totalEXP = PlayerPrefs.GetInt(username + TOTALEXPERIENCEPATH);
        int exp = totalEXP;

        while (exp >= neededEXP)
        {
            level++;
            exp -= neededEXP;
            neededEXP = CalculateNeededEXP(level+1);
        }

        currentEXP = exp;
    }


    /// <summary>
    /// Give the player a set amount of exp. For Debugging.
    /// </summary>
    /// <param name="experience"></param>
    public void Load(int experience)
    {
        totalEXP = experience;
        int exp = totalEXP;

        while (exp >= neededEXP)
        {
            level++;
            exp -= neededEXP;
            neededEXP = CalculateNeededEXP(level + 1);
        }

        currentEXP = exp;
    }


    /// <summary>
    /// Save the total exp.
    /// </summary>
    /// <param name="username">Player's username.</param>
    public void Save(string username)
    {
        PlayerPrefs.SetInt(username + TOTALEXPERIENCEPATH, totalEXP);
    }


    /// <summary>
    /// Add exp. Levels up if necessary.
    /// </summary>
    /// <param name="amount">Amount of experience to add.</param>
    public void Increase(int amount)
    {
        currentEXP += amount;
        while (currentEXP >= neededEXP)
        {
            // increase level points
            level++;
            currentEXP -= neededEXP;
            neededEXP = CalculateNeededEXP(level + 1);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Caculates how much exp is needed to pass a level.
    /// </summary>
    /// <param name="level">Level to pass.</param>
    /// <returns>How much exp is needed to pass level.</returns>
    private int CalculateNeededEXP(int level)
    {
        level--;
        if (level <= 0) return 0;
        return Mathf.FloorToInt(115f * Mathf.Pow(1.35f, level));
    }


    /// <summary>
    /// Debug. Shows exp needed for range of levels.
    /// </summary>
    private void TestCurve()
    {
        for (int i = 0; i <= 10; i++)
        {
            Debug.Log(i + ":" + CalculateNeededEXP(i));
        }
    }

    #endregion
}