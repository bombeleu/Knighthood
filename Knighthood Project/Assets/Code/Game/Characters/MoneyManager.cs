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

    public int money;

    #endregion

    #region Const Fields

    private const string MONEYPATH = ": Money";

    #endregion


    #region Public Methods

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
    /// Load saved money amount for a player.
    /// </summary>
    /// <param name="username"></param>
    public void Load(string username)
    {
        this.username = username;
        money = PlayerPrefs.GetInt(username + MONEYPATH);
    }


    /// <summary>
    /// Save current money amount.
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetInt(username + MONEYPATH, money);
    }

    #endregion
}