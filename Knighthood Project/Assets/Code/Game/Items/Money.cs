// Steve Yeager
// 10.3.2013

using UnityEngine;

/// <summary>
/// Money to be picked up by the players.
/// </summary>
public class Money : BaseMono
{
    #region Reference Fields

    private Collider myCollider;

    #endregion

    #region Public Fields

    public int worth;

    #endregion


    #region Const Fields

    private const float COLLECTDELAY = 1f;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // references
        myCollider = collider;
    }


    private void OnEnable()
    {
        InvokeAction(() => myCollider.enabled = true, COLLECTDELAY);   
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;

        Player.AllCollectMoney(collision.gameObject.GetComponent<Player>(), worth);

        myCollider.enabled = false;
        gameObject.SetActive(false);
    }

    #endregion
}