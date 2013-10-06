// Steve Yeager
// 10.5.2013

using System;
using System.Collections.Generic;
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
    public Dictionary<string, int> tempKills { get; private set; }
    public int deaths { get; private set; }
    public int tempDeaths { get; private set; }
    public int totalMoney { get; private set; }
    public int tempTotalMoney { get; private set; }
    public float timePlayed { get; private set; }
    public float tempTimePlayed { get; private set; }

    #endregion

    #region Const Fields

    private const string KILLSPATH = ": Kills ";
    private const string DEATHSPATH = ": Deaths";
    private const string TOTALMONEYPATH = ": Total Money";
    private const string TIMEPLAYEDPATH = ": Time Played";

    #endregion


    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enemy"></param>
    public void IncreaseKill(string enemy)
    {
        tempKills[enemy]++;
        kills[enemy]++;
    }


    /// <summary>
    /// 
    /// </summary>
    public void IncreaseDeaths()
    {
        tempDeaths++;
        deaths++;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseMoney(int amount)
    {
        tempTotalMoney += amount;
        totalMoney += amount;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="seconds"></param>
    public void IncreaseTimePlayed(float seconds)
    {
        tempTimePlayed += seconds;
        timePlayed += seconds;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    public void Load(string username)
    {
        kills = new Dictionary<string, int>();
        foreach (var enemy in Enum.GetNames(typeof (Enemy.EnemyTypes)))
        {
            kills.Add(enemy, PlayerPrefs.GetInt(username + KILLSPATH + enemy));
        }
        deaths = PlayerPrefs.GetInt(username + DEATHSPATH);
        totalMoney = PlayerPrefs.GetInt(username + TOTALMONEYPATH);
        timePlayed = PlayerPrefs.GetFloat(username + TIMEPLAYEDPATH);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="clearTemp"></param>
    public void Save(bool clearTemp = true)
    {
        if (clearTemp)
        {
            tempKills.Clear();
            tempDeaths = 0;
            tempTotalMoney = 0;
            tempTimePlayed = 0f;
        }

        foreach (var enemy in Enum.GetNames(typeof (Enemy.EnemyTypes)))
        {
            PlayerPrefs.SetInt(username + KILLSPATH + enemy, kills[enemy]);
        }
        PlayerPrefs.SetInt(username + DEATHSPATH, deaths);
        PlayerPrefs.SetInt(username + TOTALMONEYPATH, totalMoney);
        PlayerPrefs.SetFloat(username + TIMEPLAYEDPATH, timePlayed);
    }

    #endregion
}