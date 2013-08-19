// Steve Yeager
// 8.17.2013

using UnityEngine;

/// <summary>
/// Wrapper for Unity Time class.
/// </summary>
public static class GameTime
{
  public static float deltaTime
  {
    get
    {
      return (LevelManager.Instance.paused ? 0f : UnityEngine.Time.deltaTime);
    }
  }

} // end GameTime class