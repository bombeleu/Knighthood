// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Base class for players. Interacts with Character
/// </summary>
public class Player : Character
{
    #region References

    private PlayerAttacks attackSystems;

    #endregion

    #region State Fields

    public float spawnTime;
    public float fastFallSpeed;
    public float extraJumpTime;
    private bool canExtraJump = false;

    #endregion

    #region Player Fields

    private PlayerInfo playerInfo;
    public bool keyboard = false;
    public float attackDeadZone = 0.7f;
    public float magicModifierDeadZone = 0.8f;
    public float moveBuffer = 0.1f;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(States.Spawning, SpawningEnter, info => { Log("Spawning Exit"); });
        CreateState(States.Idling, IdlingEnter, IdlingExit);
        CreateState(States.Jumping, JumpingEnter, info => { Log("Jumping Exit"); });
        CreateState(States.Moving, info => { Log("Moving Enter"); currentStateJob = new Job(MovingUpdate()); }, MovingExit);
        CreateState(States.Falling, FallingEnter, info => { Log("Falling Exit"); });
        CreateState(States.Attacking, AttackingEnter, AttackingExit);
        initialState = States.Spawning;

        // combat
        attackSystems = GetSafeComponent<PlayerAttacks>();

        // test
        log = false;
    } // end Start


    //private void OnDrawGizmos()
    //{
    //  Gizmos.DrawSphere(myTransform.position + Vector3.up + myTransform.forward * Mathf.Abs(velocity.x * GameTime.deltaTime), 0.5f);
    //} // end OnDrawGizmos

    #endregion

    #region Create Methods

    /// <summary>
    /// Set player info.
    /// </summary>
    /// <param name="username">Player's username.</param>
    /// <param name="playerNumber">Player's number.</param>
    public void Initialize(string username, int playerNumber)
    {
        playerInfo = new PlayerInfo(username, playerNumber);
        name = username;

        StartInitialState(null);
    } // end Initialize

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        Log("Spawning Enter.");

        currentStateJob = new Job(SpawningUpdate());
    } // end SpawningEnter


    private IEnumerator SpawningUpdate()
    {
        yield return WaitForTime(spawnTime);

        SetState(States.Idling, null);
    } // end SpawningUpdate


    private void IdlingEnter(Dictionary<string, object> info)
    {
        Log("Idling Enter");

        // reset values
        velocity = Vector3.zero;
        SetVelocity(velocity);

        currentStateJob = new Job(IdlingUpdate());
    } // end IdlingEnter


    private IEnumerator IdlingUpdate()
    {
        while (true)
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttacks.Attacks.None)
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("attack", GetAttackingInput());
                SetState(States.Attacking, info);
                yield break;
            }

            // enter jumping state
            if (GetJumpingInput())
            {
                SetState(States.Jumping, null);
                yield break;
            }

            // enter moving state
            if (GetMovingInput().x != 0f)
            {
                SetState(States.Moving, null);
                yield break;
            }

            // enter falling state
            if (!CM.IsGrounded())
            {
                SetState(States.Falling, null);
                yield break;
            }

            yield return null;
        }
    } // end IdlingUpdate


    private void IdlingExit(Dictionary<string, object> info)
    {
        velocity.y = 0f;
    } // end IdlingExit


    private IEnumerator MovingUpdate()
    {
        yield return WaitForTime(moveBuffer);
        Vector3 moveVector;

        while (true)
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttacks.Attacks.None)
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("attack", GetAttackingInput());
                SetState(States.Attacking, info);
                yield break;
            }

            // enter jumping state
            if (GetJumpingInput())
            {
                SetState(States.Jumping, null);
                yield break;
            }

            moveVector = GetMovingInput();

            // enter idling state
            if (moveVector.x == 0f)
            {
                SetState(States.Idling, null);
                yield break;
            }

            // rotate
            SetRotation();

            // move
            velocity.x = moveVector.x * moveSpeed;
            SetVelocity(velocity);

            // enter falling state
            if (!CM.IsGrounded())
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("extraJump", true);
                SetState(States.Falling, info);
                yield break;
            }

            yield return null;
        }
    } // end MovingUpdate


    private void MovingExit(Dictionary<string, object> info)
    {
        velocity.y = 0f;
    } // end MovingExit


    private void JumpingEnter(Dictionary<string, object> info)
    {
        Log("Jumping Enter");

        currentStateJob = new Job(JumpingUpdate(), false);
        currentStateJob.CreateChildJob(Climb(), jumpingInfo.climbTime);
        currentStateJob.CreateChildJob(Float());

        currentStateJob.JobCompleteEvent += (killed) =>
                                            {
                                                info = new Dictionary<string, object>();
                                                info.Add("fromJump", true);
                                                SetState(States.Falling, info);
                                            };
        currentStateJob.Start();
    } // end JumpingEnter


    private IEnumerator JumpingUpdate()
    {
        yield return null;
    } // end JumpingUpdate


    private IEnumerator Climb()
    {
        Log("Climb");
        velocity.y = jumpingInfo.jumpSpeed;

        while (GetJumpingInput(true))
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttacks.Attacks.None)
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("attack", GetAttackingInput());
                SetState(States.Attacking, info);
                yield break;
            }

            SetRotation();
            velocity.x = GetMovingInput().x * moveSpeed;
            SetVelocity(velocity);
            yield return null;
        }
    } // end Climb


    private IEnumerator Float()
    {
        Log("Float");
        while (velocity.y > 0.1f)
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttacks.Attacks.None)
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("attack", GetAttackingInput());
                SetState(States.Attacking, info);
                yield break;
            }

            SetRotation();
            velocity.x = GetMovingInput().x * moveSpeed;
            velocity.y -= gravity * GameTime.deltaTime;
            SetVelocity(velocity);
            yield return null;
        }
    } // end Float


    private void FallingEnter(Dictionary<string, object> info)
    {
        Log("Falling Enter");

        // make sure falling
        if (CM.IsGrounded())
        {
            SetState(States.Idling, null);
            return;
        }

        // give extra jump waitTime
        if (info != null && info.ContainsKey("extraJump"))
        {
            canExtraJump = true;
            InvokeAction(() => { canExtraJump = false; }, extraJumpTime);
        }

        currentStateJob = new Job(FallingUpdate());
    } // end FallingEnter


    private IEnumerator FallingUpdate()
    {
        while (true)
        {
            // enter idling state
            if (CM.IsGrounded())
            {
                SetState(States.Idling, null);
                yield break;
            }

            // enter attacking state
            if (GetAttackingInput() != PlayerAttacks.Attacks.None)
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info.Add("attack", GetAttackingInput());
                SetState(States.Attacking, info);
                yield break;
            }

            // enter jumping state
            if (canExtraJump && GetJumpingInput())
            {
                Log("EXTRA JUMP!!!!!!!!!!!!!!!!!!!!!!!!!");
                SetState(States.Jumping, null);
                yield break;
            }

            // fast fall
            if (GetMovingInput().y < -0.9f)
            {
                Log("Fast falling");
                velocity.y -= fastFallSpeed * GameTime.deltaTime;
            }

            // move
            SetRotation();
            velocity.x = GetMovingInput().x * moveSpeed;
            velocity.y -= gravity * GameTime.deltaTime;
            if (velocity.y < -terminalVelocity)
            {
                Log("Terminal");
                velocity.y = -terminalVelocity;
            }
            SetVelocity(velocity);

            yield return null;
        }

    } // end Falling Update


    private void AttackingEnter(Dictionary<string, object> info)
    {
        Log("Attacking Enter");

        if (attackSystems.Activate((PlayerAttacks.Attacks)info["attack"]))
        {
            //currentStateJob = new Job(AttackingUpdate());
            velocity.x = 0;
            SetVelocity(velocity);
        }
        else
        {
            SetState(States.Idling, null);
        }
    } // end AttackingEnter


    private void AttackingExit(Dictionary<string, object> info)
    {
        Log("Attacking Exit");
    } // end AttackingExit

    #endregion

    #region Input Methods

    /// <summary>
    /// Detect jumping input.
    /// </summary>
    /// <param name="held">Does the button need to be held.</param>
    /// <returns>True, if the jump button has been pressed/held.</returns>
    private bool GetJumpingInput(bool held = true)
    {
        if (held)
        {
            if (keyboard)
            {
                return Input.GetKey(KeyCode.Space);
            }
            else
            {
                return Input.GetButton("A_" + playerInfo.playerNumber);
            }
        }
        else
        {
            if (keyboard)
            {
                return Input.GetKeyDown(KeyCode.Space);
            }
            else
            {
                return Input.GetButtonDown("A_" + playerInfo.playerNumber);
            }
        }
    } // end GetJumpingInput


    /// <summary>
    /// Detect movement input.
    /// </summary>
    /// <returns>Movement vector.</returns>
    private Vector3 GetMovingInput()
    {
        if (keyboard)
        {
            return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        }
        else
        {
            return new Vector3(Input.GetAxis("L_XAxis_" + playerInfo.playerNumber), Input.GetAxis("L_YAxis_" + playerInfo.playerNumber), 0f);
        }
    } // end GetMovingInput


    /// <summary>
    /// Set correct y rotation based on GetMovingInput.
    /// </summary>
    private void SetRotation()
    {
        float x = GetMovingInput().x;
        if (x > 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (x < 0)
        {
            myTransform.rotation = Quaternion.Euler(0f, 270f, 0f);
        }
    } // end SetRotation


    /// <summary>
    /// Detect attack input.
    /// </summary>
    /// <returns>Returns corresponding attack type.</returns>
    private PlayerAttacks.Attacks GetAttackingInput()
    {
        if (keyboard)
        {
            // magic
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    return PlayerAttacks.Attacks.MagicLeft;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    return PlayerAttacks.Attacks.MagicUp;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    return PlayerAttacks.Attacks.MagicRight;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    return PlayerAttacks.Attacks.MagicDown;
                }
            }

            Vector2 joystick = GetMovingInput();

            // light
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightSide;
                }

                return PlayerAttacks.Attacks.LightNormal;
            }

            // heavy
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavyUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavyDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavySide;
                }

                return PlayerAttacks.Attacks.HeavyNormal;
            }

            // ranged
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedSide;
                }

                return PlayerAttacks.Attacks.RangedNormal;
            }
        }
        else
        {
            // magic
            if (Input.GetAxis("TriggersR_" + playerInfo.playerNumber) >= magicModifierDeadZone)
            {
                if (Input.GetButtonDown("X_" + playerInfo.playerNumber))
                {
                    return PlayerAttacks.Attacks.MagicLeft;
                }
                if (Input.GetButtonDown("Y_" + playerInfo.playerNumber))
                {
                    return PlayerAttacks.Attacks.MagicUp;
                }
                if (Input.GetButtonDown("B_" + playerInfo.playerNumber))
                {
                    return PlayerAttacks.Attacks.MagicRight;
                }
                if (Input.GetButtonDown("A_" + playerInfo.playerNumber))
                {
                    return PlayerAttacks.Attacks.MagicDown;
                }
            }

            Vector2 joystick = GetMovingInput();

            // light
            if (Input.GetButtonDown("X_" + playerInfo.playerNumber))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.LightSide;
                }

                return PlayerAttacks.Attacks.LightNormal;
            }

            // heavy
            if (Input.GetButtonDown("Y_" + playerInfo.playerNumber))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavyUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavyDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.HeavySide;
                }

                return PlayerAttacks.Attacks.HeavyNormal;
            }

            // ranged
            if (Input.GetButtonDown("B_" + playerInfo.playerNumber))
            {
                if (joystick.y > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedUp;
                }
                if (joystick.y < -attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedDown;
                }
                if (Mathf.Abs(joystick.x) > attackDeadZone)
                {
                    return PlayerAttacks.Attacks.RangedSide;
                }

                return PlayerAttacks.Attacks.RangedNormal;
            }
        }

        return PlayerAttacks.Attacks.None;
    } // end GetAttackingInput

    #endregion

    #region Movement Methods

    /// <summary>
    /// Set the CharacterMotor velocity if allow.
    /// </summary>
    /// <param name="moveVector">Move vector not multiplied by deltaTime.</param>
    private void SetVelocity(Vector3 moveVector)
    {
        float screenX = LevelManager.Instance.camera.WorldToScreenPoint(myTransform.position).x / Screen.width;

        // screen right
        if (GetMovingInput().x > 0 && screenX >= (1 - LevelCamera.Instance.boundaryHorizontalOuter))
        {
            moveVector.x = 0f;
        }
        // screen left
        else if (GetMovingInput().x < 0 && screenX <= LevelCamera.Instance.boundaryHorizontalOuter)
        {
            moveVector.x = 0f;
        }

        // blocked
        RaycastHit rayInfo;
        if (Physics.SphereCast(myTransform.position + Vector3.up, 0.5f, myTransform.forward, out rayInfo, Mathf.Abs(moveVector.x * GameTime.deltaTime), 1 << LayerMask.NameToLayer("Terrain")))
        {
            //moveVector.x = 0f;
        }

        CM.velocity = moveVector;
    } // end SetVelocity

    #endregion

} // end Player class