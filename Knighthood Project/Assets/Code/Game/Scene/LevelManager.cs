// Steve Yeager
// 8.18.2013

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base level manager singleton.
/// </summary>
public abstract class LevelManager : Singleton<LevelManager>
{
    #region Static Fields

    protected Type managerType;

    #endregion

    #region Player Fields

    public List<Transform> PlayerTransforms { get; private set; }

    #endregion

    #region Enemy Fields

    private Transform EnemyParent;
    public Enemy.EnemyTypes[] enemyTypes;
    private Dictionary<Enemy.EnemyTypes, ObjectRecycler> EnemyPools; 
    private string enemiesClearedMethod;
    private int enemiesLeft;

    #endregion

    #region Camera Fields

    public new Camera camera { get; private set; }
    protected LevelCamera levelCamera;

    #endregion

    #region Events

    public EventHandler<FinishedLevelEventArgs> FinishedEvent;

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        camera = Camera.main;
        levelCamera = camera.GetComponent<LevelCamera>();

        // set up
        EnemyParent = new GameObject("Enemies").transform;
        EnemyPools = new Dictionary<Enemy.EnemyTypes, ObjectRecycler>();
        foreach (var enemy in enemyTypes)
        {
            EnemyPools.Add(enemy, new ObjectRecycler(GameResources.Instance.EnemyPrefab(enemy), EnemyParent));
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Players have successfully finished the level. Fires FinishedEvent and load overview screen.
    /// </summary>
    public void FinishLevel()
    {
        if (FinishedEvent != null) FinishedEvent(this, new FinishedLevelEventArgs());

        var levelData = new Dictionary<string, object>();
        foreach (var player in PlayerTransforms)
        {
            Player cont = player.GetComponent<Player>();
            Dictionary<string, object> playerData = new Dictionary<string, object>();
            
            // score
            playerData.Add("Score", cont.myScore.score);
            playerData.Add("New Highscore", cont.myScore.Save());

            // money
            playerData.Add("Money", cont.myMoney.money);
            cont.myMoney.Save();

            // kills
            var killData = enemyTypes.ToDictionary(enemy => enemy, enemy => cont.myPerformance.kills[enemy.ToString()]);
            playerData.Add("Kills", killData);

            // deaths
            playerData.Add("Deaths", cont.myPerformance.deaths);
            cont.myPerformance.Save();

            levelData.Add(cont.playerInfo.username, playerData);
        }
        
        InvokeAction(() => GameData.Instance.LoadScene("Overview Screen", levelData), 5f);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="spawnPoints"></param>
    /// <param name="clearMethod"></param>
    public void SpawnEnemies(EnemySpawnPoint[] spawnPoints, string startMethod, string clearMethod)
    {
        Invoke(string.IsNullOrEmpty(startMethod) ? "LockCamera" : startMethod, 0f);
        enemiesClearedMethod = string.IsNullOrEmpty(clearMethod) ? "UnlockCamera" : clearMethod;
        enemiesLeft = spawnPoints.Length;

        // spawn enemies and register event
        foreach (var point in spawnPoints)
        {
            var enemy = EnemyPools[point.enemy].nextFree;
            enemy.transform.position = point.transform.position;
            enemy.GetComponent<CharacterHealth>().HitEvent += HitHandler;
        }
    }


    /// <summary>
    /// Lock the level camera.
    /// </summary>
    public void LockCamera()
    {
        levelCamera.locked = true;
    }


    /// <summary>
    /// Unlock the level camera.
    /// </summary>
    public void UnlockCamera()
    {
        levelCamera.locked = false;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Create the players at the beginning of the level.
    /// </summary>
    protected void CreatePlayers()
    {
        PlayerTransforms = new List<Transform>();

        Transform playerParent = (new GameObject().transform);
        playerParent.name = "Players";

        for (int i = 0; i < GameData.Instance.playerUsernames.Count; i++)
        {
            GameObject player = (GameObject)Instantiate(GameResources.Instance.Player_Prefabs[GameData.Instance.playerCharacters[i]],
                                                        new Vector3(10f + 2f * i, 0.5f, 0f),
                                                        Quaternion.Euler(0f, 90f, 0f));
            PlayerTransforms.Add(player.transform);
            player.GetSafeComponent<Player>().Initialize(GameData.Instance.playerUsernames[i], i);
            player.transform.parent = playerParent;
        }
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    public abstract void RecieveTrigger(string method);

    #endregion

    #region Event Handlers

    public void HitHandler(object sender, HitEventArgs args)
    {
        if (!args.dead) return;

        enemiesLeft--;
        if (enemiesLeft == 0)
        {
            Invoke(enemiesClearedMethod, 0f);
        }
    }

    #endregion
}