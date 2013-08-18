// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for any character.
/// </summary>
public class Character : BaseMono
{
  public enum States { Spawning, Idling, Moving, Jumping, Falling, Flinching, Dying }
  public States currentState { get; protected set; }
  private States initialState;
  private Dictionary<States, Action<Dictionary<string, object>>> EnterMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
  private Dictionary<States, Action<Dictionary<string, object>>> ExitMethods = new Dictionary<States, Action<Dictionary<string, object>>>();


  /// <summary>
  /// Create a new currentState.
  /// </summary>
  /// <param name="stateName">State.</param>
  /// <param name="EnterMethod">Enter method for currentState.</param>
  /// <param name="ExitMethod">Exit method for currentState.</param>
  private void CreateState(States stateName, Action<Dictionary<string, object>> EnterMethod, Action<Dictionary<string, object>> ExitMethod)
  {
    EnterMethods.Add(stateName, EnterMethod);
    ExitMethods.Add(stateName, ExitMethod);
  } // end CreateState


  /// <summary>
  /// Exit the current state and enter the new one.
  /// </summary>
  /// <param name="stateName">State to transition to.</param>
  /// <param name="info">Info to pass to the exit and enter states.</param>
  private void SetState(States stateName, Dictionary<string, object> info)
  {
    ExitMethods[currentState](info);
    currentState = stateName;
    EnterMethods[currentState](info);
  } // end SetState

} // end Character class