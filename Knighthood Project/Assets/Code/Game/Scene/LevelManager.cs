// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Base level manager singleton.
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
  #region Player Fields

  protected Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

  #endregion

  #region Pause Fields

  public bool paused { get; private set; }

  #endregion

  #region Events

  public EventHandler<PauseEventArgs> PauseEvent;

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
  } // end TogglePause

  #endregion

  #region Spawn Methods

  protected void CreatePlayers()
  {
    Transform playerParent = (new GameObject().transform);
    playerParent.name = "Players";

    for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
    {
      GameObject player = (GameObject)Instantiate(GameResources.Instance.Player_Prefab, new Vector3(-17f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
      players.Add(GameData.Instance.playerUsernames[i], player);
      player.GetSafeComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
      player.transform.parent = playerParent;
    }
  } // end CreatePlayers

  #endregion

} // end LevelManger class