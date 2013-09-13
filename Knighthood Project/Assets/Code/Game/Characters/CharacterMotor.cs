// Steve Yeager
// 8.22.2013

using System.Linq;
using UnityEngine;

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

    private const float GroundedRayDist = 0.3f;
    private int terrainLayer;
    public Vector3 Velocity { get { return myRigidbody.velocity; } set { myRigidbody.velocity = value; } }

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

        if (!info.contacts.Any(contact => contact.point.y > myTransform.position.y)) return;
        Velocity = new Vector3(Velocity.x, 0f, 0f);
        GetComponent<Character>().SetState(Character.States.Falling, null);
        return;
    } // end OnCollisionEnter

    #endregion

    #region Movement Methods

    /// <summary>
    /// If the myCharacter is close enough to the ground to be considered on it. Snaps to ground.
    /// </summary>
    /// <returns>True, if on ground.</returns>
    public bool IsGrounded()
    {
        if (Velocity.y > 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f, terrainLayer))
        {
            Velocity = new Vector3(Velocity.x, 0f, 0f);
            myTransform.position += Vector3.down * (rayInfo.distance - 0.1f);
            return true;
        }
        return false;
    }

    #endregion

}