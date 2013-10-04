// Steve Yeager
// 8.18.2013

using UnityEngine;

/// <summary>
/// Singleton to hold references to widely used objects.
/// </summary>
public class GameResources : Singleton<GameResources>
{
    #region Player Fields

    public GameObject[] Player_Prefabs;

    #endregion

    #region Pool Fields

    public GameObject DamageIndicator_Prefab;
    public ObjectRecycler DamageIndicator_Pool;
    public GameObject Hitbox_Prefab;
    public ObjectRecycler Hitbox_Pool;
    public GameObject Money_Prefab;
    public ObjectRecycler Money_Pool;

    #endregion

    #region Doc Fields

    public TextAsset NPCStats;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // pools
        DamageIndicator_Pool = new ObjectRecycler(DamageIndicator_Prefab);
        Hitbox_Pool = new ObjectRecycler(Hitbox_Prefab);
        Money_Pool = new ObjectRecycler(Money_Prefab, new GameObject("Money").transform);
    }


    private void Start()
    {
        // set events
        GameData.LoadSceneEvent += LoadSceneHandler;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handler for LoadEvent. Reset's ObjectRecyclers.
    /// </summary>
    /// <param name="sender">GameData.</param>
    /// <param name="loadSceneEventArgs">LoadEvent args.</param>
    private void LoadSceneHandler(object sender, LoadSceneEventArgs loadSceneEventArgs)
    {
        if (DamageIndicator_Pool != null)
        {
            DamageIndicator_Pool.Clear();
        }
        if (Hitbox_Pool != null)
        {
            Hitbox_Pool.Clear();
        }
    }

    #endregion
}