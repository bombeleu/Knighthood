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

  protected List<string> playerUsernames = new List<string>();
  public List<Transform> playerTransforms { get; private set; }

  #endregion

  #region Camera Fields

  new public Camera camera { get; private set; }

  #endregion

  #region Pause Fields

  public bool paused { get; private set; }

  #endregion

  #region Events

  public EventHandler<PauseEventArgs> PauseEvent;

  #endregion


  #region MonoBehaviour Overrides

  protected virtual void Awake()
  {
    // get references
    camera = Camera.main;
  } // end Awake

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

  /// <summary>
  /// Create the players at the beginning of the level.
  /// </summary>
  protected void CreatePlayers()
  {
    playerTransforms = new List<Transform>();

    Transform playerParent = (new GameObject().transform);
    playerParent.name = "Players";

    for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
    {
      GameObject player = (GameObject)Instantiate(GameResources.Instance.Player_Prefabs[i], new Vector3(-17f + 2f * i, 0.5f, 0f), Quaternion.Euler(0f, 90f, 0f));
      playerUsernames.Add(GameData.Instance.playerUsernames[i]);
      playerTransforms.Add(player.transform);
      player.GetSafeComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
      player.transform.parent = playerParent;
    }
  } // end CreatePlayers

  #endregion

} // end LevelManger class