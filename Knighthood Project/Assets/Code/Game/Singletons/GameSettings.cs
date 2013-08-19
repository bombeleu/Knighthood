// Steve Yeager
// 8.17.2013

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Singleton to hold all game settings.
/// </summary>
public class GameSettings : Singleton<GameSettings>
{
  #region MonoBehaviour Overrides

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
  } // end Awake

  #endregion

} // end GameSettings class