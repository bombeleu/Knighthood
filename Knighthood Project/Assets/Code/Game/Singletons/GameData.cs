// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

/// <summary>
/// Singleton to hold game data and handle player sign in.
/// </summary>
public class GameData : Singleton<GameData>
{
    #region Player Fields

    public List<string> playerUsernames;// { get; private set; }
    public List<string> allUsernames; // { get; private set; }

    #endregion

    #region Loading Fields

    public string PreviousScene { get; private set; }
    public string NextScene { get; private set; }

    #endregion

    #region Events

    /// <summary>When a new scene is being loaded.</summary>
    public static EventHandler<LoadSceneEventArgs> LoadSceneEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    } // end Awake


    private void Update()
    {
        // player disconnect
        if (playerUsernames.Count > Input.GetJoystickNames().Length)
        {
            Log("Player disconnected");
        }

        // new player
        if (playerUsernames.Count < Input.GetJoystickNames().Length)
        {
            Log("New Player");

            // set first player to default username
            if (playerUsernames.Count == 0 && !string.IsNullOrEmpty(PlayerPrefs.GetString("Default Username", null)))
            {
                playerUsernames.Add(PlayerPrefs.GetString("Default Username"));
            }
            else
            {
                playerUsernames.Add("Player " + (playerUsernames.Count + 1));
            }
        }

        // test
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));

            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    } // end Update

    #endregion

    #region Loading Methods

    /// <summary>
    /// Load the next scene.
    /// </summary>
    /// <param name="nextScene">Name of next scene.</param>
    /// <param name="load">Should the Loading Screen be loaded first?</param>
    public void LoadScene(string nextScene, bool load = false)
    {
        PreviousScene = Application.loadedLevelName;
        this.NextScene = nextScene;

        if (LoadSceneEvent != null)
        {
            LoadSceneEvent(this, new LoadSceneEventArgs(PreviousScene, NextScene));
        }

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    } // end LoadScene


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// </summary>
    public void ReloadScene()
    {
        if (LoadSceneEvent != null)
        {
            LoadSceneEvent(this, new LoadSceneEventArgs(PreviousScene, NextScene));
        }

        Application.LoadLevel(Application.loadedLevel);
    }

    #endregion

    #region Data Methods

    /// <summary>
    /// Gets all previously saved usernames.
    /// </summary>
    private void LoadAllUsernames()
    {
        allUsernames = PlayerPrefs.GetString("All Usernames").Split('|').ToList();
    } // end LoadAllUsernames


    /// <summary>
    /// Add a new username. Saves to PlayerPrefs. Set Default Username if applicable.
    /// </summary>
    /// <param name="newUsername">Username to add.</param>
    public void AddUsername(string newUsername)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("Default Username", null)))
        {
            PlayerPrefs.SetString("Default Username", newUsername);
        }

        allUsernames.Add(newUsername);
        PlayerPrefs.SetString("All Usernames", string.Join("|", allUsernames.ToArray()));
    } // end AddUsername

    #endregion

}

// end GameData class