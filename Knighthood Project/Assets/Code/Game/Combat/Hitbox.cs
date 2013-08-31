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
  public HitInfo hitInfo;
  public float time;

  #endregion


  //
  public void Initialize(Character sender)
  {
    this.sender = sender;
    gameObject.layer = sender.gameObject.layer;
    hitID = ++hitIDs;
    Invoke("End", time);
  } // end Initialize


  //
  public void End()
  {
    gameObject.SetActive(false);
  } // end End

  #region MonoBehaviour Overrides

  private void Awake()
  {
    // get references
    myTransform = transform;
  } // end Awake


  private void OnTriggerStay(Collider other)
  {
    Health opponentHealth = other.GetComponent<Health>();
    if (opponentHealth != null)
    {
      opponentHealth.RecieveHit(sender, hitID, hitInfo);
    }
  } // end OnTriggerStay

  #endregion

} // end Hitbox class