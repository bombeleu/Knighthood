﻿// Steve Yeager
// 8.18.2013

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for any myCharacter.
/// </summary>
//[RequireComponent(typeof(Health))]
public class Character : BaseMono
{
    #region Reference Fields

    protected CharacterMotor myMotor;
    protected Transform myTransform;
    protected GameObject myGameObject;
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
    public const string DyingState = "Dying";
    public const string FlinchingState = "Flinching";

    protected string currentState;
    protected string initialState;
    private readonly Dictionary<string, Action<Dictionary<string, object>>> enterMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    private readonly Dictionary<string, Action<Dictionary<string, object>>> exitMethods = new Dictionary<string, Action<Dictionary<string, object>>>();
    protected Job currentStateJob;

    public float flinchTimeBase;
    public float knockBackMultiplier;
    public float knockBackRecoverySpeed;
    public float flinchInvincibleTime;

    #endregion

    #region Stat Fields

    public StatManager myStats;// { get; protected set; }

    #endregion

    #region Animation Fields

    public string[] animationNames;
    public Texture[] animationTex;

    #endregion

    #region Events

    public EventHandler DieEvent;

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        myMotor = GetComponent<CharacterMotor>();
        myTransform = transform;
        myGameObject = gameObject;
        myRigidbody = rigidbody;
        myHealth = GetComponent<CharacterHealth>();
        myMaterial = myTransform.FindChild("Body").renderer.materials[0];
    }


    protected virtual void Start()
    {
        // events
        myHealth.HitEvent += HitHandler;
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

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationName"></param>
    public void PlayAnimation(string animationName)
    {
        myMaterial.mainTexture = animationTex[Array.IndexOf(animationNames, animationName)];
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationTexure"></param>
    public void PlayAnimation(Texture animationTexure)
    {
        myMaterial.mainTexture = animationTexure;
    }


    /// <summary>
    /// Change the character to Attacking state.
    /// </summary>
    /// <param name="attackTexture">Texture corresponding to the attackValue being performed.</param>
    public void Attack(Texture attackTexture)
    {
        SetState(AttackingState, new Dictionary<string, object> { { "attackTexture", attackTexture } });
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Make character invincible for set time.
    /// </summary>
    /// <param name="time">Invincible duration.</param>
    protected IEnumerator Invincible(float time)
    {
        myHealth.invincible = true;
        yield return WaitForTime(time);
        myHealth.invincible = false;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Start flinching after being hit.
    /// </summary>
    protected virtual void HitHandler(List<object> senders, HitEventArgs args)
    {
        if (args.damage > 0)
        {
            if (args.health > 0)
            {
                SetState(FlinchingState, new Dictionary<string, object> { { "knockBack", args.hitInfo.knockBack } });
            }
            else
            {
                SetState(DyingState, new Dictionary<string, object>());
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected void OnAttackOver(object sender, AttackOverArgs args)
    {
        if (!args.cancelled && currentState == AttackingState)
        {
            SetState(IdlingState, new Dictionary<string, object>());
        }
    }

    #endregion
}