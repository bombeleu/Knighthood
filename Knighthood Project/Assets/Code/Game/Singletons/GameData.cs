// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public string previousScene { get; private set; }
    public string nextScene { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    } // end Start


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
                playerUsernames.Add("Player " + (playerUsernames.Count+1).ToString());
            }
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
        previousScene = Application.loadedLevelName;
        this.nextScene = nextScene;

        Application.LoadLevel(load ? "Loading Screen" : nextScene);
    } // end LoadScene

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

    #region SignIn Methods

    private int GetSignInInput()
    {

        return -1;
    } // end GetSignInInput

    #endregion

} // end GameData class