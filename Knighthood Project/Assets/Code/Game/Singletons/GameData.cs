// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton to hold game data.
/// </summary>
public class GameData : Singleton<GameData>
{
    #region Player Fields

    public List<string> playerUsernames;// { get; private set; }

    #endregion

    #region Loading Fields

    public string previousScene { get; private set; }
    public string nextScene { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    } // end Start

    #endregion


    #region Loading Methods

    public void LoadScene(string nextScene, bool load = false)
    {
        previousScene = Application.loadedLevelName;
        this.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    } // end LoadScene

    #endregion

} // end GameData class