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
    base.Awake();

    // create states
    CreateState(States.Spawning, SpawningEnter, info => { Log("Spawning Exit"); });
    CreateState(States.Idling, IdlingEnter, info => { Log("Idling Exit"); });
    CreateState(States.Jumping, JumpingEnter, info => { Log("Jumping Exit"); });
    CreateState(States.Moving, info => { Log("Moving Enter"); currentStateJob = new Job(MovingUpdate()); }, info => { Log("Moving Exit"); });
    CreateState(States.Falling, info => { Log("Falling Enter"); }, info => { Debugger.Log("Falling Exit"); });
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
    Log("Spawning Enter.");

    currentStateJob = new Job(SpawningUpdate());
    currentStateJob.JobCompleteEvent += (killed) => SetState(States.Idling, null);
  } // end SpawningEnter


  private IEnumerator SpawningUpdate()
  {
    yield return new WaitForSeconds(spawnTime);
  } // end SpawningUpdate


  private void IdlingEnter(Dictionary<string, object> info)
  {
    Log("Idling Enter");

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

      // enter moving state
      if (GetMovingInput().x != 0f)
      {
        SetState(States.Moving, null);
        yield break;
      }

      yield return null;
    }
  } // end IdlingUpdate


  private IEnumerator MovingUpdate()
  {
    Vector3 moveVector;
      
    while (true)
    {
      // enter jumping state
      if (GetJumpingInput())
      {
        SetState(States.Jumping, null);
        yield break;
      }

      moveVector = GetMovingInput();

      // enter idling state
      if (moveVector.x == 0f)
      {
        SetState(States.Idling, null);
        yield break;
      }

      // rotate
      myTransform.rotation = Quaternion.LookRotation(new Vector3(moveVector.x, 0f, 0f));

      // move
      velocity.x = moveVector.x * moveSpeed;
      velocity.y = -gravity;
      CC.Move(velocity * GameTime.deltaTime);

      // enter falling state
      if (!CC.isGrounded)
      {
        SetState(States.Falling, null);
        yield break;
      }

      yield return null;
    }
  } // end MovingUpdate


  private void JumpingEnter(Dictionary<string, object> info)
  {
    Log("Jumping Enter");

    currentStateJob = new Job(JumpingUpdate(), false);
    
    currentStateJob.CreateChildJob(Leap(), jumpingInfo.leapTime);
    currentStateJob.CreateChildJob(Climb(), jumpingInfo.climbTime);
    currentStateJob.CreateChildJob(Float(), jumpingInfo.floatTime);

    currentStateJob.JobCompleteEvent += (killed) => SetState(States.Falling, null);
    currentStateJob.Start();
  } // end JumpingEnter


  private IEnumerator JumpingUpdate()
  {
    yield return null;
  } // end JumpingUpdate


  private IEnumerator Leap()
  {
    velocity.y = jumpingInfo.jumpSpeed;

    while (true)
    {
      CC.Move(velocity * GameTime.deltaTime);
      yield return null;
    }
  } // end Leap


  private IEnumerator Climb()
  {
    velocity.y = jumpingInfo.jumpSpeed;

    while (GetJumpingInput(true))
    {
      CC.Move(velocity * GameTime.deltaTime);
      yield return null;
    }
  } // end Climb


  private IEnumerator Float()
  {
    float vel = 0f;
    while (true)
    {
      velocity.y = Mathf.SmoothDamp(velocity.y, 0f, ref vel, jumpingInfo.floatTime);
      CC.Move(velocity * GameTime.deltaTime);
      yield return null;
    }
  } // end Float

  #endregion

  #region Input Methods

  /// <summary>
  /// Detect jumping input.
  /// </summary>
  /// <param name="held">Does the button need to be held.</param>
  /// <returns>True, if the jump button has been pressed/held.</returns>
  private bool GetJumpingInput(bool held = false)
  {
    if (held)
    {
      if (keyboard)
      {
        return Input.GetKey(KeyCode.Space);
      }
      else
      {
        return Input.GetButton("A_" + playerInfo.playerNumber);
      }
    }
    else
    {
      if (keyboard)
      {
        return Input.GetKeyDown(KeyCode.Space);
      }
      else
      {
        return Input.GetButtonDown("A_" + playerInfo.playerNumber);
      }
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