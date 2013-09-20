// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using UnityEditor;

/// <summary>
/// Singleton to hold game data and handle player sign in.
/// </summary>
public class GameData : Singleton<GameData>
{
    #region Player Fields

    public List<string> playerUsernames;// { get; private set; }
    public List<string> allUsernames; // { get; private set; }
    public List<int> playerCharacters; 

    #endregion

    #region Loading Fields

    public string PreviousScene { get; private set; }
    public string nextScene { get; private set; }

    #endregion

    #region Pause Fields

    public bool paused { get; private set; }

    #endregion

    #region Events

    /// <summary>When a new scene is being loaded.</summary>
    public static EventHandler<LoadSceneEventArgs> LoadSceneEvent;
    public EventHandler<PauseEventArgs> PauseEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }


    private void Update()
    {
        // player disconnect
        if (playerUsernames.Count > Input.GetJoystickNames().Length)
        {
            //Log("Player disconnected");
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

#if UNITY_EDITOR
        // test
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));

            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
#endif
    }

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
        this.nextScene = nextScene;

        if (LoadSceneEvent != null)
        {
            LoadSceneEvent(this, new LoadSceneEventArgs(PreviousScene, this.nextScene));
        }

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    }


    /// <summary>
    /// Reloads the current scene without the Loading Screen.
    /// </summary>
    public void ReloadScene()
    {
        if (LoadSceneEvent != null)
        {
            LoadSceneEvent(this, new LoadSceneEventArgs(PreviousScene, nextScene));
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
    }


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
    }

    #endregion

    #region Pause Methods

    /// <summary>
    /// Toggle pause.
    /// </summary>
    /// <param name="player">Player that paused the game.</param>
    public void TogglePause(int player)
    {
        paused = !paused;
        if (PauseEvent != null)
        {
            PauseEvent(this, new PauseEventArgs(paused, player));
        }
    }

    #endregion
}