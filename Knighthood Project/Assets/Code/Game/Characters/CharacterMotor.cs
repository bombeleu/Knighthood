// Steve Yeager
// 8.22.2013

using UnityEngine;

/// <summary>
/// Handles all Character movement.
/// </summary>
public class CharacterMotor : BaseMono
{
    #region Reference Fields

    protected Transform myTransform;
    private Rigidbody myRigidbody;

    #endregion

    #region Public Fields

    public float moveSpeed = 13f;
    public float gravity = 70f;
    public float terminalVelocity = 40f;
    public float jumpStrength = 21f;

    #endregion

    #region Private Fields

    private int terrainLayer;
    private const float GroundedRayDist = 0.5f;

    #endregion

    #region Properties

    public Vector3 velocity
    {
        get { return myRigidbody.velocity; }
        set { myRigidbody.velocity = value; }
    }

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

    #region Public Methods

    /// <summary>
    /// If the myCharacter is close enough to the ground to be considered on it. Snaps to ground.
    /// </summary>
    /// <param name="ground">Should the character snap to the ground?</param>
    /// <returns>True, if character is close enough to be considered on the ground.</returns>
    public bool IsGrounded(bool ground = false)
    {
        if (velocity.y > 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up*0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f,
            terrainLayer))
        {
            if (ground)
            {
                velocity = new Vector3(velocity.x, 0f, 0f);
                myTransform.position += Vector3.down*(rayInfo.distance - 0.1f);
            }
            return true;
        }
        return false;
    }


    /// <summary>
    /// Ground the character if close to terrain.
    /// </summary>
    public void Ground()
    {
        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f, terrainLayer))
        {
            velocity = new Vector3(velocity.x, 0f, 0f);
            myTransform.position += Vector3.down * (rayInfo.distance - 0.1f);
        }
    }


    /// <summary>
    /// If the character is on top of a Translucent platform.
    /// </summary>
    /// <returns>True, if on a platform and it is Translucent.</returns>
    public bool OverTranslucent()
    {
        if (velocity.y > 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up*0.1f, Vector3.down, out rayInfo, GroundedRayDist + 0.1f,
            terrainLayer))
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


    /// <summary>
    /// Increase gravity if not at terminal velocity.
    /// </summary>
    public void ApplyGravity()
    {
        if (velocity.y > -terminalVelocity)
        {
            AddVelocityY(-gravity * GameTime.deltaTime);
        }
    }


    /// <summary>
    /// Move character by fraction of moveSpeed.
    /// </summary>
    /// <param name="input">[-1, 1]</param>
    public virtual void MoveX(float input)
    {
        input = Mathf.Clamp(input, -1f, 1f);
        velocity = new Vector3(moveSpeed*input, velocity.y, 0f);
    }


    /// <summary>
    /// Set CharacterMotor velocity to zero.
    /// </summary>
    public void ClearVelocity()
    {
        velocity = Vector3.zero;
    }


    /// <summary>
    /// Set new velocity for CharacterMotor.
    /// </summary>
    /// <param name="velocity">New velocity value.</param>
    public virtual void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }


    /// <summary>
    /// Set new velocity for CharacterMotor.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    /// <param name="y">Vertical speed. Up is positive.</param>
    public virtual void SetVelocity(float x, float y)
    {
        velocity = new Vector3(x, y, 0f);
    }


    /// <summary>
    /// Set CharacterMotor's velocity's horizontal speed.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    public virtual void SetVelocityX(float x)
    {
        velocity = new Vector3(x, velocity.y, 0f);
    }


    /// <summary>
    /// Set CharacterMotor's velocity's vertical speed.
    /// </summary>
    /// <param name="y">Vertical speed. Up is positive.</param>
    public void SetVelocityY(float y)
    {
        velocity = new Vector3(velocity.x, y, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    /// <param name="y">Vertical speed. Up is positive.</param>
    public virtual void AddVelocity(float x, float y)
    {
        velocity += new Vector3(x, y, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity.
    /// </summary>
    /// <param name="velocity">Velocity to add to current velocity.</param>
    public virtual void AddVelocity(Vector3 velocity)
    {
        this.velocity += velocity;
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity's horizontal speed.
    /// </summary>
    /// <param name="x">Horizontal speed.</param>
    public virtual void AddVelocityX(float x)
    {
        velocity += new Vector3(x, 0f, 0f);
    }


    /// <summary>
    /// Add to the CharacterMotor's velocity's vertical speed.
    /// </summary>
    /// <param name="y">Vertical speed. Up is positive.</param>
    public void AddVelocityY(float y)
    {
        velocity += new Vector3(0f, y, 0f);
    }


    /// <summary>
    /// Set x velocity in the forward direction.
    /// </summary>
    /// <param name="x">Forward speed.</param>
    public void SetVelocityForward(float x)
    {
        velocity = myTransform.forward * x;
    }

    #endregion
}