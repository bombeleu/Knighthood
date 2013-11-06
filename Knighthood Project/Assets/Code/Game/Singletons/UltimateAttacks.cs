// Steve Yeager
// 10.20.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 
/// </summary>
public class UltimateAttacks : Singleton<UltimateAttacks>
{
    #region Public Fields

    public UltimateAttack[] attacks = new UltimateAttack[15];

    #endregion

    #region Static Fields

    public static Transform attackPivot { get; private set; }
    private static bool open;
    private static int attackValue;
    public static readonly Player.Characters[][] attackParcipants = 
    {
        new[] { Player.Characters.Chad },
        new[] { Player.Characters.Eva },
        new[] { Player.Characters.Chad, Player.Characters.Eva },
        new[] { Player.Characters.Harold },
        new[] { Player.Characters.Chad, Player.Characters.Harold },
        new[] { Player.Characters.Eva, Player.Characters.Harold },
        new[] { Player.Characters.Chad, Player.Characters.Eva, Player.Characters.Harold },
        new[] { Player.Characters.Jules },
        new[] { Player.Characters.Chad, Player.Characters.Jules },
        new[] { Player.Characters.Eva, Player.Characters.Jules },
        new[] { Player.Characters.Chad, Player.Characters.Eva, Player.Characters.Jules },
        new[] { Player.Characters.Harold, Player.Characters.Jules },
        new[] { Player.Characters.Chad, Player.Characters.Harold, Player.Characters.Jules },
        new[] { Player.Characters.Eva, Player.Characters.Harold, Player.Characters.Jules },
        new[] { Player.Characters.Chad, Player.Characters.Eva, Player.Characters.Harold, Player.Characters.Jules },
    };
    public static List<Character> participants = new List<Character>();
    public static Player[] players = new Player[4];

    #endregion

    #region Const Fields

    private const float waitTime = 2f;

    #endregion

    #region Events

    public static EventHandler<UltimateAttackUnleashArgs> UnleashedEvent;
    public static EventHandler AttackOverEvent;

    #endregion


    #region Public Methods

    public void Register(Transform player, int playerValue)
    {
        if (!open)
        {
            attackPivot = new GameObject("Ultimate Attack Pivot").transform;
            attackPivot.transform.position = player.position;
            attackPivot.transform.rotation = player.rotation;
            open = true;
            attackValue = playerValue;

            StartCoroutine(Attack());
        }
        else
        {
            attackValue += playerValue;
        }

        var cont = player.GetComponent<Player>();
        participants.Add(cont);
        players[(int)cont.character] = cont;
    }

    #endregion

    #region Private Methods

    private IEnumerator Attack()
    {
        Vector3 startPosition = attackPivot.position;
        Quaternion startRotation = attackPivot.rotation;

        attackValue--;
        yield return WaitForTime(waitTime);
        UnleashedEvent(participants[0], new UltimateAttackUnleashArgs(startPosition, startRotation, attackValue));
        attacks[attackValue].Activate();
        yield return WaitForTime(attacks[attackValue].attackTime);
        AttackOverEvent(participants[0], null);
        ClearPlayers();
        participants.Clear();
        Destroy(attackPivot.gameObject);
        open = false;
    }


    /// <summary>
    /// Empty participants after attack.
    /// </summary>
    private static void ClearPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            players[i] = null;
        }
    }

    #endregion
}