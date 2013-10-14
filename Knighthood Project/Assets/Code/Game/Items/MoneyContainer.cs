// Steve Yeager
// 10.3.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof (Health))]
public class MoneyContainer : BaseMono
{
    #region Reference Fields

    private Transform myTransform;

    #endregion

    #region Public Fields

    public int minAmount;
    public int maxAmount;
    public bool random = true;
    public Vector2 force;
    public float rotation;

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        // references
        myTransform = transform;

        // events
        GetComponent<Health>().HitEvent += HitHandler;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Triggered by HitEvent. Creates and shoots money.
    /// </summary>
    private void HitHandler(object sender, HitEventArgs args)
    {
        if (args.dead)
        {
            int amount = random ? Random.Range(minAmount, maxAmount) : minAmount;

            while (amount > 0)
            {
                var money = GameResources.Instance.Money_Pool.nextFree;
                money.transform.position = myTransform.position + Vector3.up;
                money.rigidbody.velocity = new Vector3(0f, Random.Range(0f, force.y), Random.Range(-force.x, force.x));
                money.rigidbody.angularVelocity = new Vector3(Random.Range(-rotation, rotation), Random.Range(-rotation, rotation), Random.Range(-rotation, rotation));
                amount--;
            }
        }
    }

    #endregion
}