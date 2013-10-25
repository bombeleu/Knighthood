// Steve Yeager
// 10.19.2013

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager for player Ultimate Attacks.
/// </summary>
public class UltimateAttackManager : BaseMono
{
    #region References

    private Transform myTransform;
    private Player myPlayer;

    #endregion

    #region Public Fields

    public Texture acceptAnim;

    #endregion

    #region Private Fields

    private Vector3 previousPosition;
    private int[] attacks;
    private bool ready = true;

    #endregion

    #region Static Fields

    public static readonly Dictionary<int, int[]> participantAttacks = new Dictionary<int, int[]>
    {
        { 1, new[] { 0, 2, 4, 6, 8, 10, 12, 14 } },
        { 2, new[] { 1, 2, 5, 6, 9, 10, 13, 14 } },
        { 4, new[] { 3, 4, 5, 6, 11, 12, 13, 14 } },
        { 8, new[] { 7, 8, 9, 10, 11, 12, 13, 14 } }
    };

    #endregion

    #region Const Fields

    private const float cooldown = 120f;

    #endregion

    #region Properties

    public int playerValue { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        myTransform = transform;
        myPlayer = GetComponent<Player>();

        switch (GetComponent<Player>().character)
        {
            case Player.Characters.Chad:
                playerValue = 1;
                break;
            case Player.Characters.Eva:
                playerValue = 2;
                break;
            case Player.Characters.Harold:
                playerValue = 4;
                break;
            case Player.Characters.Jules:
                playerValue = 8;
                break;
        }

        attacks = participantAttacks[playerValue];
    }

    #endregion

    #region Public Methods

    public bool CanActivate()
    {
        return ready;
    }


    public void Activate()
    {
        ready = false;
        myPlayer.PlayAnimation(acceptAnim);
        previousPosition = myTransform.position;
        UltimateAttacks.Instance.Register(myTransform, playerValue);
        UltimateAttacks.UnleashedEvent += UnleashedHandler;
        UltimateAttacks.AttackOverEvent += AttackOverHandler;
    }

    #endregion

    #region Event Handlers

    private void UnleashedHandler(object sender, UltimateAttackUnleashArgs args)
    {
        UltimateAttacks.UnleashedEvent -= UnleashedHandler;
        InvokeAction(() => ready = true, cooldown);
    }


    private void AttackOverHandler(object sender, EventArgs args)
    {
        UltimateAttacks.AttackOverEvent -= AttackOverHandler;
        myTransform.position = previousPosition;
        GetComponent<Player>().SetState(Player.IdlingState, new Dictionary<string, object>());
    }

    #endregion
}