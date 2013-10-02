// Steve Yeager
// 10.1.2013

using System;
using UnityEngine;

[Serializable]
public class ExperienceManager
{
    #region Public Fields

    public int currentEXP;
    public int neededEXP;
    public int level;

    #endregion

    #region Private Fields

    private int totalEXP;

    #endregion

    #region Const Fields

    private const string TOTALEXPERIENCEPATH = ": Total Experience";

    #endregion


    #region Public Methods

    /// <summary>
    /// 
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
    /// 
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
    /// 
    /// </summary>
    /// <param name="username">Player's username.</param>
    public void Save(string username)
    {
        PlayerPrefs.SetInt(username + TOTALEXPERIENCEPATH, totalEXP);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount">Amount of experience to add.</param>
    public void Increase(int amount)
    {
        currentEXP += amount;
        while (currentEXP >= neededEXP)
        {
            level++;
            currentEXP -= neededEXP;
            neededEXP = CalculateNeededEXP(level + 1);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private int CalculateNeededEXP(int level)
    {
        level--;
        if (level <= 0) return 0;
        return Mathf.FloorToInt(115f * Mathf.Pow(1.35f, level));
    }


    /// <summary>
    /// 
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