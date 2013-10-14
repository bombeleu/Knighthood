// Steve Yeager
// 10.8.2013

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class SpawnTrigger : BaseMono
{
    #region Public Fields

    public EnemySpawnPoint[] spawnPoints;
    public string startMethod;
    public string clearMethod;

    #endregion

    #region MonoBehaviour Overrides

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        LevelManager.Instance.SpawnEnemies(spawnPoints, startMethod, clearMethod);
        Destroy(gameObject);
    }

    #endregion
}