// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Singleton to hold references to widely used objects.
/// </summary>
public class GameResources : Singleton<GameResources>
{
  #region Player Fields

  public GameObject Player_Prefab;

  #endregion

  #region MonoBehaviour Overrides

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  } // end Awake

  #endregion

} // end GameResources class