﻿// Steve Yeager
// 8.18.2013

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base level manager singleton.
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    #region Player Fields

    protected List<string> playerUsernames = new List<string>();
    public List<Transform> PlayerTransforms { get; private set; }

    #endregion

    #region Camera Fields

    public new Camera camera { get; private set; }

    #endregion

    #region Events

    public EventHandler<FinishedLevelEventArgs> FinishedEvent;

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        camera = Camera.main;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Players have successfully finished the level. Fires FinishedEvent and load overview screen.
    /// </summary>
    public void FinishLevel()
    {
        if (FinishedEvent != null) FinishedEvent(this, new FinishedLevelEventArgs());

        // get player stats and if they leveled up and score

        InvokeAction(() => GameData.Instance.LoadScene("Level Overview Screen", true), 5f);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Create the players at the beginning of the level.
    /// </summary>
    protected void CreatePlayers()
    {
        PlayerTransforms = new List<Transform>();

        Transform playerParent = (new GameObject().transform);
        playerParent.name = "Players";

        for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
        {
            GameObject player = (GameObject)Instantiate(GameResources.Instance.Player_Prefabs[GameData.Instance.playerCharacters[i]],
                                                        new Vector3(10f + 2f * i, 0.5f, 0f),
                                                        Quaternion.Euler(0f, 90f, 0f));
            playerUsernames.Add(GameData.Instance.playerUsernames[i]);
            PlayerTransforms.Add(player.transform);
            player.GetSafeComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
            player.transform.parent = playerParent;
        }
    }

    #endregion
}