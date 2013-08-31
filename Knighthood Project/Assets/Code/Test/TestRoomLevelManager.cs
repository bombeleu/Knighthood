// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Level manager for the Test Room.
/// </summary>
public class TestRoomLevelManager : LevelManager
{
  #region Test Fields

  public float fireballSpawnMin;
  public float fireballSpawnMax;

  #endregion


  #region MonoBehaviour Overrides

  protected override void Awake()
  {
    base.Awake();

    CreatePlayers();
  } // end Awake

  #endregion

  #region Test Methods

  private IEnumerator Fireballs()
  {
    while (true)
    {

      yield return new WaitForSeconds(Random.Range(fireballSpawnMin, fireballSpawnMax));
    }
  } // end Fireballs

  #endregion

} // end TestRoomLevelManager class