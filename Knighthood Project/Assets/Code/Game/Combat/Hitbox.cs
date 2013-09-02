// Steve Yeager
// 8.22.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Pass hit info from sender to receiver.
/// </summary>
public class Hitbox : BaseMono
{
    #region References

    protected Transform myTransform;

    #endregion

    #region Combat Fields

    private static int hitIDs;
    private int hitID;
    private Character sender;
    public HitInfo hitInfo { get; private set; }

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    } // end Start


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tag) return;

        Health opponentHealth = other.GetComponent<Health>();
        if (opponentHealth != null)
        {
            opponentHealth.RecieveHit(sender, hitID, hitInfo);
        }
    } // end OnTriggerStay

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="sender">Character who sent the attack.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    public void Initialize(Character sender, HitInfo hitInfo, float time)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;

        gameObject.tag = sender.gameObject.tag;
        hitID = ++hitIDs;

        InvokeMethod("End", time);
    } // end Initialize


    /// <summary>
    /// End the current attack.
    /// </summary>
    public void End()
    {
        gameObject.SetActive(false);
    } // end End

    #endregion

} // end Hitbox class