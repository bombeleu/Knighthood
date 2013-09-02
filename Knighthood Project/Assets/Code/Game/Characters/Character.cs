// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for any character.
/// </summary>
[RequireComponent(typeof(Health))]
public class Character : BaseMono
{
    #region Reference Fields

    protected CharacterMotor CM;
    protected Transform myTransform;
    protected Rigidbody myRigidbody;

    #endregion

    #region State Fields

    public enum States { Spawning, Idling, Moving, Jumping, Falling, Attacking, Flinching, Dying }
    public States currentState;// { get; protected set; }
    protected States initialState;
    private Dictionary<States, Action<Dictionary<string, object>>> EnterMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
    private Dictionary<States, Action<Dictionary<string, object>>> ExitMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
    protected Job currentStateJob;

    #endregion

    #region Movement Fields

    public float moveSpeed;
    public Vector3 velocity;
    public float gravity;
    public float terminalVelocity;
    public JumpingInfo jumpingInfo;

    #endregion

    #region Stat Fields

    public StatManager statManager { get; protected set; }

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        CM = GetSafeComponent<CharacterMotor>();
        myTransform = transform;
        myRigidbody = rigidbody;
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
    public void SetState(States stateName, Dictionary<string, object> info)
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

    #region Public Methods

    public void EndAttack()
    {
        SetState(States.Idling, null);
    } // end EndAttack

    #endregion

} // end Character class