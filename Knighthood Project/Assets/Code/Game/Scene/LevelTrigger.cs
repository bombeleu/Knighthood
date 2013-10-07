// Steve Yeager
// 10.6.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class LevelTrigger : BaseMono
{
    #region Public Fields

    public string method;

    #endregion


    #region MonoBehaviour Overrides

    private void OnTriggerEnter(Collider other)
    {
        LevelManager.Instance.Invoke(method, 0f);
    }

    #endregion
}