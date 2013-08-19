// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


/// <summary>
/// Base class for players. Interacts with Character
/// </summary>
public class Player : Character
{
  #region Reference Fields

  public float spawnTime;

  #endregion

  #region Player Fields

  private PlayerInfo playerInfo;
  public bool keyboard = false;

  #endregion


  #region MonoBehaviour Overrides

  protected override void Awake()
  {
    // create states
    CreateState(States.Spawning, SpawningEnter, info => { Debugger.Log("Spawning Exit"); });
    CreateState(States.Idling, IdlingEnter, info => { Debugger.Log("Idling Exit"); });
    CreateState(States.Jumping, info => { Debugger.Log("Jumping Enter"); }, info => { Debugger.Log("Jumping Exit"); });
    initialState = States.Spawning;
  } // end Awake

  #endregion

  #region Create Methods

  /// <summary>
  /// Set player info.
  /// </summary>
  /// <param name="username">Player's username.</param>
  /// <param name="playerNumber">Player's number.</param>
  public void Initialize(string username, int playerNumber)
  {
    playerInfo = new PlayerInfo(username, playerNumber);
    name = username;

    StartInitialState(null);
  } // end Initialize

  #endregion

  #region State Methods

  private void SpawningEnter(Dictionary<string, object> info)
  {
    Debugger.Log("Spawning Enter.");

    currentStateJob = new Job(SpawningUpdate());
    currentStateJob.JobCompleteEvent += (killed) => SetState(States.Idling, null);
  } // end SpawningEnter


  private IEnumerator SpawningUpdate()
  {
    yield return new WaitForSeconds(spawnTime);
  } // end SpawningUpdate


  private void IdlingEnter(Dictionary<string, object> info)
  {
    Debugger.Log("Idling Enter");

    currentStateJob = new Job(IdlingUpdate());
  } // end IdlingEnter


  private IEnumerator IdlingUpdate()
  {
    while (true)
    {
      // enter jumping state
      if (GetJumpingInput())
      {
        SetState(States.Jumping, null);
        yield break;
      }
      yield return null;
    }
  } // end IdlingUpdate

  #endregion

  #region Input Methods

  /// <summary>
  /// Detect jumping input.
  /// </summary>
  /// <returns>True, if the jump button has been pressed.</returns>
  private bool GetJumpingInput()
  {
    if (keyboard)
    {
      return Input.GetKeyDown(KeyCode.Space);
    }
    else
    {
      return Input.GetButtonDown("A_" + playerInfo.playerNumber);
    }
  } // end GetJumpingInput


  /// <summary>
  /// Detect movement input.
  /// </summary>
  /// <returns>Movement vector.</returns>
  private Vector3 GetMovingInput()
  {
    if (keyboard)
    {
      return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
    }
    else
    {
      return new Vector3(Input.GetAxis("L_XAxis_" + playerInfo.playerNumber), Input.GetAxis("L_YAxis_" + playerInfo.playerNumber), 0f);
    }
  } // end GetMovingInput

  #endregion

} // end Player class