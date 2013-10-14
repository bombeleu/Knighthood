﻿// Steve Yeager
// 8.22.2013

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base enemy class.
/// </summary>
public class Enemy : Character
{
    #region Reference Fields

    protected NavAgent myNavAgent;
    protected AttackManager attackManager;

    #endregion

    #region Stat Fields

    public enum EnemyTypes
    {
        Dummy
    }
    public EnemyTypes enemyType;
    public int experience;

    #endregion

    #region State Fields

    protected const string AttackState = "Attacking";

    #endregion

    #region Nav Fields

    public float navBuffer = 0.5f;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // get references
        myNavAgent = GetComponent<NavAgent>();
        attackManager = GetComponent<AttackManager>();
        attackManager.Initialize(this);

        // set up
        myHealth.Initialize();
    }


    protected virtual void Start()
    {
        // register events
        myHealth.HitEvent += HitHandler;
    }

    #endregion

    #region Event Handlers

    private void HitHandler(object sender, HitEventArgs args)
    {
        if (args.dead)
        {
            Log("Killed by: " + sender, Debugger.LogTypes.Combat);

            Player player = (Player)sender;
            player.RecieveKill(enemyType, experience);

            SetState(DyingState, new Dictionary<string, object>());
        }
    }

    #endregion
}