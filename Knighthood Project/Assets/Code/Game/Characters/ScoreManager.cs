// Steve Yeager
// 10.6.2013

using UnityEngine;

/// <summary>
/// Manages a player's score in a level and can access highscores.
/// </summary>
public sealed class ScoreManager
{
    #region Private Fields

    private string username;

    #endregion

    #region Properties

    public int score { get; private set; }

    #endregion

    #region Const Fields

    private const string HIGHSCOREPATH = ": Highscore ";

    #endregion


    #region Public Methods

    public ScoreManager(string username)
    {
        this.username = username;
    }


    /// <summary>
    /// Increase current score obtained in current level.
    /// </summary>
    /// <param name="amount">Amount to increase score by.</param>
    /// <returns>New score value.</returns>
    public int IncreaseScore(int amount)
    {
        score += amount;
        return score;
    }


    /// <summary>
    /// LoadHighscore the highscore for a level.
    /// </summary>
    /// <param name="username">Player's username.</param>
    /// <param name="level">Level to get highscore for.</param>
    /// <returns>Level's highscore.</returns>
    public int LoadHighscore(string level)
    {
        return PlayerPrefs.GetInt(username + HIGHSCOREPATH + level);
    }


    /// <summary>
    /// Save a player's score if it is a new highscore.
    /// </summary>
    /// <param name="username">Player's username.</param>
    /// <returns>True, if a new highscore has been set.</returns>
    public bool Save()
    {
        if (score > LoadHighscore(Application.loadedLevelName))
        {
            PlayerPrefs.SetInt(username + HIGHSCOREPATH + Application.loadedLevelName, score);
            return true;
        }

        return false;
    }

    #endregion
}