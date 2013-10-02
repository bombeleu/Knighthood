// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for any myCharacter.
/// </summary>
[RequireComponent(typeof(Health))]
public class Character : BaseMono
{
    #region Reference Fields

    protected CharacterMotor myMotor;
    protected Transform myTransform;
    protected Rigidbody myRigidbody;
    protected CharacterHealth myHealth;
    private Material myMaterial;

    #endregion

    #region State Fields

    public const string SpawningState = "Spawning";
    public const string IdlingState = "Idling";
    public const string MovingState = "Moving";
    public const string FallingState = "Falling";
    public const string AttackingState = "Attacking";

    protected string currentState;
    protected string initialState;
    private readonly Dictionary<string, Action<Dictionary<string, object>>> enterMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly Dictionary<string, Action<Dictionary<string, object>>> exitMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    protected Job currentStateJob;

    #endregion

    #region Movement Fields

    public float moveSpeed;
    protected Vector3 velocity;
    public float gravity;
    public float terminalVelocity;
    /// <summary>How fast the myCharacter moves upward.</summary>
    public float jumpSpeed = 21;
    /// <summary>How long the myCharacter can jump.</summary>
    public float climbTime = 0.3f;

    #endregion

    #region Stat Fields

    public StatManager StatManager;// { get; protected set; }

    #endregion

    #region Animation Fields

    public string[] animationNames;
    public Texture[] animationTex;

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        myMotor = GetComponent<CharacterMotor>();
        myTransform = transform;
        myRigidbody = rigidbody;
        myHealth = GetComponent<CharacterHealth>();
        myMaterial = myTransform.FindChild("Body").renderer.materials[0];
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Create a new currentState.
    /// </summary>
    /// <param name="stateName">State.</param>
    /// <param name="enterMethod">Enter method for currentState.</param>
    /// <param name="exitMethod">Exit method for currentState.</param>
    protected void CreateState(string stateName, Action<Dictionary<string, object>> enterMethod, Action<Dictionary<string, object>> exitMethod)
    {
        enterMethods.Add(stateName, enterMethod);
        exitMethods.Add(stateName, exitMethod);
    }


    /// <summary>
    /// Exit the current state and enter the new one.
    /// </summary>
    /// <param name="stateName">State to transition to.</param>
    /// <param name="info">Info to pass to the exit and enter states.</param>
    public void SetState(string stateName, Dictionary<string, object> info)
    {
        // save previous state
        if (info == null)
        {
            info = new Dictionary<string, object>();
        }
        info.Add("previous state", currentState);

        // exit state
        exitMethods[currentState](info);
        if (currentStateJob != null) currentStateJob.Kill();

        // enter state
        Log(name + " Entering: " + stateName + " - " + Time.timeSinceLevelLoad);
        currentState = stateName;
        enterMethods[currentState](info);
    }


    /// <summary>
    /// Start the initial state. Doesn't call any exit methods.
    /// </summary>
    /// <param name="info">Info to pass to state enter method.</param>
    protected void StartInitialState(Dictionary<string, object> info)
    {
        currentState = initialState;
        enterMethods[initialState](info);
    }

    #endregion

    #region Velocity Methods

    /// <summary>
    /// Set CharacterMotor velocity to zero.
    /// </summary>
    protected void ClearVelocity()
    {
        myMotor.velocity = Vector3.zero;
    }


    /// <summary>
    /// Set new velocity for CharacterMotor.
    /// </summary>
    /// <param name="velocity">New velocity value.</param>
    protected void SetVelocity(Vector3 velocity)
    {
        myMotor.velocity = velocity;
    }


    /// <summary>
    /// Set new velocity for CharacterMotor.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    /// <param name="y">Vertical speed. Up is positive.</param>
    protected void SetVelocity(float x, float y)
    {
        myMotor.velocity = new Vector3(x, y, 0f);
    }


    /// <summary>
    /// Set CharacterMotor's velocity's horizontal speed.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    protected void SetVelocityX(float x)
    {
        myMotor.velocity = new Vector3(x, myMotor.velocity.y, 0f);
    }


    /// <summary>
    /// Set CharacterMotor's velocity's vertical speed.
    /// </summary>
    /// <param name="y">Vertical speed. Up is positive.</param>
    protected void SetVelocityY(float y)
    {
        myMotor.velocity = new Vector3(myMotor.velocity.x, y, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity's vertical speed.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    /// <param name="y">Vertical speed. Up is positive.</param>
    protected void AddVelocity(float x, float y)
    {
        myMotor.velocity += new Vector3(x, y, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity's horizontal speed.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    protected void AddVelocityX(float x)
    {
        myMotor.velocity += new Vector3(x, 0f, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity's vertical speed.
    /// </summary>
    /// <param name="y">Vertical speed. Up is positive.</param>
    protected void AddVelocityY(float y)
    {
        myMotor.velocity += new Vector3(0f, y, 0f);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Change the character to Attacking state.
    /// </summary>
    /// <param name="attackTexture">Texture corresponding to the attack being performed.</param>
    public void Attack(Texture attackTexture)
    {
        SetState(AttackingState, new Dictionary<string, object> { { "attackTexture", attackTexture } });
    }


    /// <summary>
    /// End the current attack state. Set to Idle.
    /// </summary>
    public void EndAttack()
    {
        SetState(IdlingState, null);
    }

    #endregion

    #region Animation Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationName"></param>
    protected void PlayAnimation(string animationName)
    {
        myMaterial.mainTexture = animationTex[Array.IndexOf(animationNames, animationName)];
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationTexure"></param>
    protected void PlayAnimation(Texture animationTexure)
    {
        myMaterial.mainTexture = animationTexure;
    }

    #endregion
}