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
  public bool paused { get; private set; }

  public EventHandler<PauseEventArgs> PauseEvent;


  /// <summary>
  /// Toggle pause.
  /// </summary>
  /// <param name="player">Player that paused the game.</param>
  public void TogglePause(int player)
  {
    paused = !paused;
    if (PauseEvent != null)
    {
      PauseEvent(this, new PauseEventArgs(paused, player));
    }
  } // end TogglePause

} // end GameSettings class