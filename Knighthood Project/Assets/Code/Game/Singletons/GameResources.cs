// Steve Yeager
// 8.18.2013

using System;
using UnityEngine;

/// <summary>
/// Singleton to hold references to widely used objects.
/// </summary>
public class GameResources : Singleton<GameResources>
{
    #region Player Fields

    public GameObject[] Player_Prefabs;

    #endregion

    #region GUI

    public GameObject DamageIndicator_Prefab;
    public ObjectRecycler DamageIndicator_Pool;
    public GameObject Hitbox_Prefab;
    public ObjectRecycler Hitbox_Pool;

    #endregion

    #region Doc Fields

    public TextAsset NPCStats;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        DamageIndicator_Pool = new ObjectRecycler(DamageIndicator_Prefab);
        Hitbox_Pool = new ObjectRecycler(Hitbox_Prefab);
    } // end Awake


    private void Start()
    {
        // set events
        GameData.LoadSceneEvent += LoadSceneHandler;
    }

    #endregion

    #region Event Handlers

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

} // end GameResources class