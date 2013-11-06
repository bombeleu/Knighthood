// Steve Yeager
// 10.30.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manager for the Combat Arena Level.
/// </summary>
public class CombatArenaManager : LevelManager
{
    #region Public Fields

    public Transform[] spawnPoints = new Transform[4];

    #endregion


    #region LevelManager Overrides

    public override void RecieveTrigger(string method)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        // get references
        camera = Camera.main;
        levelCamera = camera.GetComponent<LevelCamera>();

        CreatePlayers();
        AssignPlayerTeams();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initially create the players.
    /// </summary>
    protected override void CreatePlayers()
    {
        PlayerTransforms = new List<Transform>();
        Transform PlayerParent = new GameObject("Players").transform;
        for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
        {
            Transform player = ((GameObject)Instantiate(GameResources.Instance.Player_Prefabs[GameData.Instance.playerCharacters[i]],
                                                        spawnPoints[i].position,
                                                        Quaternion.Euler(0f, 90f, 0f))).transform;
            player.GetComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
            player.parent = PlayerParent;
            PlayerTransforms.Add(player);
        }
    }

    #endregion
}