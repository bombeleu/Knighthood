// Steve Yeager
// 10.3.2013

using System;
using UnityEngine;

/// <summary>
/// Manages a players money.
/// </summary>
[Serializable]
public class MoneyManager
{
    #region Private Fields

    private string username;

    #endregion

    #region Public Fields

    public int money { get; private set; }

    #endregion

    #region Const Fields

    private const string MONEYPATH = ": Money";

    #endregion


    #region Public Methods

    public MoneyManager(string username)
    {
        this.username = username;
    }


    /// <summary>
    /// Does the player have enough money?
    /// </summary>
    /// <param name="amount">Amount to check against saved money.</param>
    /// <returns>True, if player has enough money.</returns>
    public bool Available(int amount)
    {
        return money >= amount;
    }


    /// <summary>
    /// Increase money.
    /// </summary>
    /// <param name="amount">Amount to add to saved money.</param>
    public void Transaction(int amount)
    {
        money += amount;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int Load()
    {
        if (string.IsNullOrEmpty(username)) Debugger.LogError("Needs username. Initialize first.");
        return PlayerPrefs.GetInt(username + MONEYPATH);
    }


    /// <summary>
    /// Adds the current money to the saved money and saves the new value. Clears current money.
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetInt(username + MONEYPATH, Load() + money);
        money = 0;
    }

    #endregion
}