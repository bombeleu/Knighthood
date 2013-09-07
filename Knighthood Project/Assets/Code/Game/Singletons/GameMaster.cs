// Steve Yeager
// 9.6.2013

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GameMaster : BaseMono
{
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(gameObject);
        }
    }
	
}