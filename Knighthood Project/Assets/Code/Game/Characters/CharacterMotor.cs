// Steve Yeager
// 8.22.2013

using System.Linq;
using System.Runtime.Serialization.Formatters;
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

    private const float GroundedRayDist = 0.5f;
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
    }


    private void OnCollisionEnter(Collision info)
    {
        //if (IsGrounded()) return;

        //if (!info.contacts.Any(contact => contact.point.y > myTransform.position.y)) return;
        //velocity = new Vector3(velocity.x, 0f, 0f);
        //GetComponent<Character>().SetState(Character.FallingState, null);
    }

    #endregion

    #region Movement Methods

    /// <summary>
    /// If the myCharacter is close enough to the ground to be considered on it. Snaps to ground.
    /// </summary>
    /// <returns>True, if on ground.</returns>
    public bool IsGrounded()
    {
        if (velocity.y > 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f, terrainLayer))
        {
            velocity = new Vector3(velocity.x, 0f, 0f);
            myTransform.position += Vector3.down * (rayInfo.distance - 0.1f);
            return true;
        }
        return false;
    }


    /// <summary>
    /// If the character is on top of a Translucent platform.
    /// </summary>
    /// <returns>True, if on a platform and it is Translucent.</returns>
    public bool OverTranslucent()
    {
        if (velocity.y > 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f, terrainLayer))
        {
            return rayInfo.collider.tag == "Translucent";
        }
        return false;
    }


    /// <summary>
    /// Set correct y rotation based on GetMovingInput.
    /// </summary>
    public void SetRotation(float x)
    {
        if (x > 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 90f, 0f);
            myTransform.Align();
        }
        else if (x < 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 270f, 0f);
            myTransform.Align();
        }
    }

    #endregion
}