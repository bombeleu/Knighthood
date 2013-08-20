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
  protected List<Transform> playerTransforms = new List<Transform>();

  #endregion

  #region Camera Fields

  public CameraInfo cameraInfo;
  new protected Camera camera;
  protected Transform cameraTransform;

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
    cameraTransform = camera.transform;
  } // end Awake


  private void Update()
  {
    UpdateCamera();
  } // end Update

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
    Transform playerParent = (new GameObject().transform);
    playerParent.name = "Players";

    for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
    {
      GameObject player = (GameObject)Instantiate(GameResources.Instance.Player_Prefab, new Vector3(-17f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
      playerUsernames.Add(GameData.Instance.playerUsernames[i]);
      playerTransforms.Add(player.transform);
      player.GetSafeComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
      player.transform.parent = playerParent;
    }
  } // end CreatePlayers

  #endregion

  #region Camera Methods

  /// <summary>
  /// Update the camera to keep the players on screen.
  /// </summary>
  protected void UpdateCamera()
  {
    if (cameraInfo.locked) return;

    bool moveRight = false, moveLeft = false;
    float speed = 0f;

    foreach (Transform player in playerTransforms)
    {
      float screenXPercent = camera.WorldToScreenPoint(player.position).x / Screen.width;

      // move left
      if (screenXPercent <= cameraInfo.boundaryDynamic)
      {
        moveLeft = true;
        speed = Mathf.Abs(player.GetComponent<Player>().velocity.x);
      }
      // move right
      else if (screenXPercent >= (1f - cameraInfo.boundaryDynamic))
      {
        moveRight = true;
        speed = Mathf.Abs(player.GetComponent<Player>().velocity.x);
      }
    }

    if (moveLeft && !moveRight)
    {
      cameraTransform.position += Vector3.left * speed * GameTime.deltaTime;
    }
    else if (moveRight && !moveLeft)
    {
      cameraTransform.position += Vector3.right * speed * GameTime.deltaTime;
    }
  } // end UpdateCamera

  #endregion

} // end LevelManger class