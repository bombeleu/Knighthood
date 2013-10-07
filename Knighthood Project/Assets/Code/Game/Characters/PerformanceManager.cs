// Steve Yeager
// 10.5.2013

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Holds data for how the user has played throught his career.
/// </summary>
public class PerformanceManager
{
    #region Private Fields

    private string username;

    #endregion

    #region Properties

    public Dictionary<string, int> kills { get; private set; }
    public int deaths { get; private set; }
    public int totalMoney { get; private set; }
    public float timePlayed { get; private set; }

    #endregion

    #region Const Fields

    private const string KILLSPATH = ": Kills ";
    private const string DEATHSPATH = ": Deaths";
    private const string TOTALMONEYPATH = ": Total Money";
    private const string TIMEPLAYEDPATH = ": Time Played";

    #endregion


    #region Public Methods

    public PerformanceManager(string username)
    {
        this.username = username;
        kills = new Dictionary<string, int>();
        foreach (var enemy in LevelManager.Instance.enemyTypes)
        {
            kills.Add(enemy.ToString(), 0);
        }
    }


    public int IncreaseKill(string enemy)
    {
        return kills[enemy]++;
    }


    public int IncreaseDeaths()
    {
        return deaths++;
    }


    public int IncreaseMoney(int amount)
    {
        totalMoney += amount;
        return totalMoney;
    }


    public float IncreaseTimePlayed(float seconds)
    {
        timePlayed += seconds;
        return timePlayed;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetTotals()
    {
        if (string.IsNullOrEmpty(username)) Debugger.LogError("Username can't be empty. Initialize first.");

        var data = new Dictionary<string, object>();

        var totalKills = Enum.GetNames(typeof (Enemy.EnemyTypes)).ToDictionary(enemy => enemy, enemy => PlayerPrefs.GetInt(username + KILLSPATH + enemy));
        data.Add("Kills", totalKills);
        data.Add("Deaths", PlayerPrefs.GetInt(username + DEATHSPATH));
        data.Add("Total Money", PlayerPrefs.GetInt(username + TOTALMONEYPATH));
        data.Add("Time Played", PlayerPrefs.GetInt(username + TIMEPLAYEDPATH));

        return data;
    }


    /// <summary>
    /// 
    /// </summary>
    public void Save()
    {
        foreach (var enemy in LevelManager.Instance.enemyTypes)
        {
            PlayerPrefs.SetInt(username + KILLSPATH + enemy, PlayerPrefs.GetInt(username + KILLSPATH + enemy) + kills[enemy.ToString()]);
        }
        PlayerPrefs.SetInt(username + DEATHSPATH, PlayerPrefs.GetInt(username + DEATHSPATH) + deaths);
        PlayerPrefs.SetInt(username + TOTALMONEYPATH, PlayerPrefs.GetInt(username + TOTALMONEYPATH) + totalMoney);
        PlayerPrefs.SetFloat(username + TIMEPLAYEDPATH, PlayerPrefs.GetInt(username + TIMEPLAYEDPATH) + timePlayed);
    }

    #endregion
}