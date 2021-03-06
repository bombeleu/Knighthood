﻿// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Level manager for the Test Room.
/// </summary>
public class TestRoomLevelManager : LevelManager
{
    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        CreatePlayers();
    }

    #endregion

    #region LevelManager Overrides

    public override void RecieveTrigger(string method)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}