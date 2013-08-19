// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CharacterController))]

/// <summary>
/// Base class for any character.
/// </summary>
public class Character : BaseMono
{
  #region Reference Fields

  protected CharacterController CC;
  protected Transform myTransform;

  #endregion

  #region State Fields

  public enum States { Spawning, Idling, Moving, Jumping, Falling, Flinching, Dying }
  public States currentState;// { get; protected set; }
  protected States initialState;
  private Dictionary<States, Action<Dictionary<string, object>>> EnterMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
  private Dictionary<States, Action<Dictionary<string, object>>> ExitMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
  protected Job currentStateJob;

  #endregion

  #region Movement Fields

  public float moveSpeed;
  protected Vector3 velocity;
  public float gravity;
  public JumpingInfo jumpingInfo;

  #endregion


  #region MonoBehaviour Overrides

  protected virtual void Awake()
  {
    // get references
    CC = GetSafeComponent<CharacterController>();
    myTransform = transform;
  } // end Awake

  #endregion

  #region State Methods

  /// <summary>
  /// Create a new currentState.
  /// </summary>
  /// <param name="stateName">State.</param>
  /// <param name="EnterMethod">Enter method for currentState.</param>
  /// <param name="ExitMethod">Exit method for currentState.</param>
  protected void CreateState(States stateName, Action<Dictionary<string, object>> EnterMethod, Action<Dictionary<string, object>> ExitMethod)
  {
    EnterMethods.Add(stateName, EnterMethod);
    ExitMethods.Add(stateName, ExitMethod);
  } // end CreateState


  /// <summary>
  /// Exit the current state and enter the new one.
  /// </summary>
  /// <param name="stateName">State to transition to.</param>
  /// <param name="info">Info to pass to the exit and enter states.</param>
  protected void SetState(States stateName, Dictionary<string, object> info)
  {
    ExitMethods[currentState](info);
    currentStateJob.Kill();
    currentState = stateName;
    EnterMethods[currentState](info);
  } // end SetState


  /// <summary>
  /// Start the initial state. Doesn't call any exit methods.
  /// </summary>
  /// <param name="info">Info to pass to state enter method.</param>
  protected void StartInitialState(Dictionary<string, object> info)
  {
    currentState = initialState;
    EnterMethods[initialState](info);
  } // end StartInitialState

  #endregion

} // end Character class