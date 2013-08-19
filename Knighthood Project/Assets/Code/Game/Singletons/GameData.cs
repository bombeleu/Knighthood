// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton to hold game data.
/// </summary>
public class GameData : Singleton<GameData>
{
  #region Player Fields

  public List<string> playerUsernames;// { get; private set; }

  #endregion


  #region MonoBehaviour Overrides

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  } // end Awake

  #endregion

} // end GameData class