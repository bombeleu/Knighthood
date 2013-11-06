// Steve Yeager
// 8.18.2013

using System;
using System.Collections;
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

    public List<Transform> PlayerTransforms { get; protected set; }

    #endregion

    #region Enemy Fields

    private Transform EnemyParent;
    public Enemy.EnemyTypes[] enemyTypes;
    private Dictionary<Enemy.EnemyTypes, ObjectRecycler> EnemyPools; 
    private string enemiesClearedMethod;
    private int enemiesLeft;

    #endregion

    #region Camera Fields

    public new Camera camera { get; protected set; }
    protected LevelCamera levelCamera;

    #endregion

    #region Overview Fields

    public bool displayOverview;
    public float overviewDelay = 3f;
    public Vector2 panelSizePercent;
    public float panelYPosPercent;
    public float usernameHeightPercent;

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


    protected void OnGUI()
    {
        if (!displayOverview) return;

        float W = Screen.width;
        float H = Screen.height;
        float xSpace = (W - panelSizePercent.x*W*4f)/5f;

        for (int i = 0; i < 4; i++)
        {
            GUI.BeginGroup(new Rect(xSpace * (i + 1) + (panelSizePercent.x*W*i), panelYPosPercent * H, panelSizePercent.x * W, panelSizePercent.y * H));
            {
                // username
                GUI.Label(new Rect(0, 0, panelSizePercent.x*W, usernameHeightPercent*H), "yeagz7");

                // sheet
                GUI.Box(new Rect(0, usernameHeightPercent * H, panelSizePercent.x * W, (panelSizePercent.y - usernameHeightPercent) * H), "");
            }
            GUI.EndGroup();
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
        levelData.Add("userData", new Dictionary<string, object>());
        levelData.Add("statPoints", new int[] { 0, 0, 0, 0 });
        foreach (var player in PlayerTransforms)
        {
            Player cont = player.GetComponent<Player>();
            Dictionary<string, object> playerData = new Dictionary<string, object>();
            
            // score
            //playerData.Add("Score", cont.myScore.score);
            //playerData.Add("New Highscore", cont.myScore.Save());

            // kills
            var killData = enemyTypes.ToDictionary(enemy => enemy, enemy => cont.myPerformance.kills[enemy.ToString()]);
            playerData.Add("Kills", killData);

            // deaths
            playerData.Add("Deaths", cont.myPerformance.deaths);
            cont.myPerformance.Save();

            // money
            playerData.Add("Money", cont.myMoney.money);
            cont.myMoney.Save();

            // exp
            ((int[])levelData["statPoints"])[cont.playerInfo.zeroNumber] = cont.myExperience.statPoints;

            ((Dictionary<string, object>)levelData["userData"]).Add(cont.playerInfo.username, playerData);
        }
        
        //InvokeAction(() => GameData.Instance.LoadScene("Overview Screen", levelData), 5f);
        StartCoroutine(LevelOverview(levelData));
    }


    /// <summary>
    /// Recieve spawn call from Spawn Trigger.
    /// </summary>
    /// <param name="spawnPoints">List of spawnpoints.</param>
    /// <param name="clearMethod">Method to call once all the enemies are killed.</param>
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


    /// <summary>
    /// Change the tag for every player for PvP.
    /// </summary>
    public void AssignPlayerTeams()
    {
        for (int i = 0; i < PlayerTransforms.Count; i++)
        {
            PlayerTransforms[i].GetComponent<Player>().Retag("Team " + (i+1));
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Create the players at the beginning of the level.
    /// </summary>
    protected virtual void CreatePlayers()
    {
        PlayerTransforms = new List<Transform>();

        Transform playerParent = new GameObject("Players").transform;

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

    #region Private Methods

    private IEnumerator LevelOverview(Dictionary<string, object> levelData)
    {
        yield return WaitForTime(overviewDelay);

        displayOverview = true;

        yield return WaitForTime(3f);

        int[] statPoints = (int[])levelData["statPoints"];
        if (statPoints.Max() > 0)
        {
            // load level up screen
            GameData.Instance.LoadScene("Levelup Menu", statPoints);
        }
        else
        {
            GameData.Instance.LoadScene("Level Selection Menu", true);
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
        if (args.health > 0) return;

        enemiesLeft--;
        if (enemiesLeft == 0)
        {
            Invoke(enemiesClearedMethod, 0f);
        }
    }

    #endregion
}