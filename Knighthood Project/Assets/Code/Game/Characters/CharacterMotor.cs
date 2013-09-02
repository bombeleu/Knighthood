﻿// Steve Yeager
// 8.22.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all Character movement.
/// </summary>
public class CharacterMotor : BaseMono
{
  #region Reference Fields

  private Transform myTransform;
  private Rigidbody myRigidbody;

  #endregion

  #region Movement Fields

  private const float groundedRayDist = 0.3f;
  private int terrainLayer;
  public Vector3 velocity { get { return myRigidbody.velocity; } set { myRigidbody.velocity = value; } }

  #endregion


  #region MonoBehaviour Overrides

  private void Awake()
  {
    // get references
    myTransform = transform;
    myRigidbody = rigidbody;

    terrainLayer = 1 << LayerMask.NameToLayer("Terrain");
  } // end Start


  private void OnCollisionEnter(Collision info)
  {
    if (IsGrounded()) return;

    foreach (ContactPoint contact in info.contacts)
    {
      if (contact.point.y > myTransform.position.y)
      {
        velocity = new Vector3(velocity.x, 0f, 0f);
        GetComponent<Character>().SetState(Character.States.Falling, null);
        return;
      }
    }
  } // end OnCollisionEnter

  #endregion

  #region Movement Methods

  /// <summary>
  /// If the character is close enough to the ground to be considered on it. Snaps to ground.
  /// </summary>
  /// <returns>True, if on ground.</returns>
  public bool IsGrounded()
  {
    RaycastHit rayInfo;
    if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, groundedRayDist + 0.1f, terrainLayer))
    {
      velocity = new Vector3(velocity.x, 0f, 0f);
      myTransform.position += Vector3.down * (rayInfo.distance - 0.1f);
      return true;
    }
    return false;
  } // end IsGrounded

  #endregion

} // end CharacterMotor class