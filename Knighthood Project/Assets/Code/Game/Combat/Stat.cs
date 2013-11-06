// Steve Yeager
// 11.3.2013

using System;
using UnityEngine;

/// <summary>
/// Used in StatManager.
/// </summary>
[Serializable]
public class Stat
{
    #region Readonly Fields

    public readonly int levelMin;
    public readonly int levelMax;
    public readonly float initialValue;
    public readonly float levelValue;

    #endregion

    #region Properties

    public int level;// { get; private set; }
    public float value;
    //{
    //    get
    //    {
    //        return initialValue + level * levelValue;
    //    }
    //}

    #endregion


    #region Initialization

    /// <remarks>Sets level to levelMin.</remarks>
    public Stat(int levelMin, int levelMax, float initialValue, float levelValue)
    {
        this.levelMin = levelMin;
        this.levelMax = levelMax;
        this.initialValue = initialValue;
        this.levelValue = levelValue;

        level = levelMin;
        value = initialValue + level * levelValue;
    }


    public Stat(int levelMin, int levelMax, float initialValue, float levelValue, int level)
    {
        this.levelMin = levelMin;
        this.levelMax = levelMax;
        this.initialValue = initialValue;
        this.levelValue = levelValue;

        this.level = level;
        value = initialValue + level * levelValue;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Changes the current level the stat is at.
    /// </summary>
    /// <param name="amount">Amount to be added to level.</param>
    public void ChangeLevel(int amount)
    {
        level += amount;

        value = initialValue + level * levelValue;
    }

    #endregion
}