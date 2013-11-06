// Steve Yeager
// 8.22.2013

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        Soldier,
        Magician,

    }
    public EnemyTypes enemyType;
    public int experience;

    #endregion

    #region State Fields

    protected const string AttackState = "Attacking";

    #endregion

    #region Nav Fields

    public float navBuffer = 0.5f;
    protected Transform currentTarget;

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


    protected override void Start()
    {
        base.Start();
    }

    #endregion

    #region Event Handlers

    protected override void HitHandler(List<object> senders, HitEventArgs args)
    {
        if (args.health == 0)
        {
            foreach (var player in senders)
            {
                ((Player)player).RecieveKill(enemyType, experience);
            }

            SetState(DyingState, new Dictionary<string, object>());
        }
        else
        {
            currentTarget = ((Character)senders[0]).transform;
            SetState(FlinchingState, new Dictionary<string, object> { { "knockBack", args.hitInfo.knockBack } });
        }
    }

    #endregion
}