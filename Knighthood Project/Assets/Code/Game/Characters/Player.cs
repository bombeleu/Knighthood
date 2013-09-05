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

    private PlayerAttackManager attackManager;

    #endregion

    #region State Fields

    public float spawnTime;
    public float fastFallSpeed;
    public float extraJumpTime;
    private bool canExtraJump = false;
    private bool prematureJump;
    public float prematureJumpTime;

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
        CreateState(States.Spawning, SpawningEnter, info => {});
        CreateState(States.Idling, IdlingEnter, IdlingExit);
        CreateState(States.Jumping, JumpingEnter, info => {});
        CreateState(States.Moving, info => { currentStateJob = new Job(MovingUpdate()); }, MovingExit);
        CreateState(States.Falling, FallingEnter, info => {});
        CreateState(States.Attacking, AttackingEnter, info => {});
        initialState = States.Spawning;

        // combat
        attackManager = GetComponentInChildren<PlayerAttackManager>();

        // test
        //log = false;
    } // end Awake


    private void Start()
    {
        UnityEditor.Selection.objects = new UnityEngine.Object[1] { gameObject };
    } // end Start


    //private void OnDrawGizmos()
    //{
    //  Gizmos.DrawSphere(myTransform.position + Vector3.up + myTransform.forward * Mathf.Abs(Velocity.x * GameTime.deltaTime), 0.5f);
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
        currentStateJob = new Job(SpawningUpdate());
    } // end SpawningEnter


    private IEnumerator SpawningUpdate()
    {
        yield return WaitForTime(spawnTime);

        SetState(States.Idling, null);
    } // end SpawningUpdate


    private void IdlingEnter(Dictionary<string, object> info)
    {
        // make sure in correct state
        if (!myMotor.IsGrounded())
        {
            SetState(States.Falling, null);
            return;
        }
        else
        {
            if (GetMovingInput().x != 0f)
            {
                SetState(States.Moving, null);
            }
        }

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
            if (GetAttackingInput() != PlayerAttackManager.AttackTypes.None)
            {
                var info = new Dictionary<string, object> {{"attack", GetAttackingInput()}};
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
            if (!myMotor.IsGrounded())
            {
                SetState(States.Falling, null);
                yield break;
            }

            yield return null;
        }
    } // end IdlingUpdate


    private void IdlingExit(Dictionary<string, object> info)
    {
    } // end IdlingExit


    private IEnumerator MovingUpdate()
    {
        yield return WaitForTime(moveBuffer);
        Vector3 moveVector;

        while (true)
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttackManager.AttackTypes.None)
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
            if (!myMotor.IsGrounded())
            {
                var info = new Dictionary<string, object> {{"extraJump", true}};
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
        currentStateJob = new Job(JumpingUpdate(), false);
        currentStateJob.CreateChildJob(Climb(), jumpingInfo.climbTime);
        currentStateJob.CreateChildJob(Float());

        currentStateJob.JobCompleteEvent += (killed) =>
                                            {
                                                if (killed) return;
                                                info = new Dictionary<string, object> {{"fromJump", true}};
                                                SetState(States.Falling, info);
                                            };
        currentStateJob.Start();
    } // end JumpingEnter


    private IEnumerator JumpingUpdate()
    {
        // play the substates Climb and Float
        yield return null;
    } // end JumpingUpdate


    private IEnumerator Climb()
    {
        velocity.y = jumpingInfo.jumpSpeed;

        while (GetJumpingInput(true))
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttackManager.AttackTypes.None)
            {
                var info = new Dictionary<string, object> {{"attack", GetAttackingInput()}};
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
        while (velocity.y > 0.1f)
        {
            // enter attacking state
            if (GetAttackingInput() != PlayerAttackManager.AttackTypes.None)
            {
                var info = new Dictionary<string, object> {{"attack", GetAttackingInput()}};
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
        // make sure falling
        if (myMotor.IsGrounded())
        {
            SetState((GetMovingInput().x == 0f ? States.Idling : States.Moving), null);
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
            // enter idling or jumping state
            if (myMotor.IsGrounded())
            {
                SetState((prematureJump ? States.Jumping : States.Idling), null);
                yield break;
            }

            // enter attacking state
            if (GetAttackingInput() != PlayerAttackManager.AttackTypes.None)
            {
                var info = new Dictionary<string, object> {{"attack", GetAttackingInput()}};
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

            // get premature jumping input
            if (!prematureJump && GetJumpingInput())
            {
                prematureJump = true;
                InvokeAction(() => { prematureJump = false; }, prematureJumpTime);
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
                velocity.y = -terminalVelocity;
            }
            SetVelocity(velocity);

            yield return null;
        }

    } // end Falling Update


    private void AttackingEnter(Dictionary<string, object> info)
    {
        // attack
        if (attackManager.Activate((PlayerAttackManager.AttackTypes)info["attack"]))
        {
            currentStateJob = new Job(AttackingUpdate());
        }
        // return to previous state
        else
        {
            SetState((States)info["previous state"], null);
        }
    } // end AttackingEnter


    private IEnumerator AttackingUpdate()
    {
        while (true)
        {
            if (!myMotor.IsGrounded())
            {
                velocity.y -= gravity * GameTime.deltaTime;
                SetVelocity(velocity);
            }
            else
            {
                velocity.x = 0f;
                SetVelocity(velocity);
            }

            yield return null;
        }
    } // end AttackingUpdate

    #endregion

    #region Input Methods

    /// <summary>
    /// Detect jumping input.
    /// </summary>
    /// <param name="held">Does the button need to be held.</param>
    /// <returns>True, if the jump button has been pressed/held.</returns>
    private bool GetJumpingInput(bool held = false)
    {
        if (held)
        {
            if (keyboard)
            {
                return Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift);
            }
            else
            {
                return Input.GetButton("A_" + playerInfo.playerNumber) && Input.GetAxis("TriggersR_" + playerInfo.playerNumber) < magicModifierDeadZone;
            }
        }
        else
        {
            if (keyboard)
            {
                return Input.GetKeyDown(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift);
            }
            else
            {
                return Input.GetButtonDown("A_" + playerInfo.playerNumber) && Input.GetAxis("TriggersR_" + playerInfo.playerNumber) < magicModifierDeadZone;
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
    private PlayerAttackManager.AttackTypes GetAttackingInput()
    {
        if (keyboard)
        {
            // magic
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    return PlayerAttackManager.AttackTypes.SuperLeft;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    return PlayerAttackManager.AttackTypes.SuperHeavy;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    return PlayerAttackManager.AttackTypes.SuperStun;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    return PlayerAttackManager.AttackTypes.SuperJump;
                }
            }

            // left
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return PlayerAttackManager.AttackTypes.Light;
            }

            // up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return PlayerAttackManager.AttackTypes.Heavy;
            }

            // right
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return PlayerAttackManager.AttackTypes.Stun;
            }
        }
        else
        {
            // magic
            if (Input.GetAxis("TriggersR_" + playerInfo.playerNumber) >= magicModifierDeadZone)
            {
                if (Input.GetButtonDown("X_" + playerInfo.playerNumber))
                {
                    return PlayerAttackManager.AttackTypes.SuperLeft;
                }
                if (Input.GetButtonDown("Y_" + playerInfo.playerNumber))
                {
                    return PlayerAttackManager.AttackTypes.SuperHeavy;
                }
                if (Input.GetButtonDown("B_" + playerInfo.playerNumber))
                {
                    return PlayerAttackManager.AttackTypes.SuperStun;
                }
                if (Input.GetButtonDown("A_" + playerInfo.playerNumber))
                {
                    return PlayerAttackManager.AttackTypes.SuperJump;
                }
            }

            // left
            if (Input.GetButtonDown("X_" + playerInfo.playerNumber))
            {
                return PlayerAttackManager.AttackTypes.Light;
            }

            // up
            if (Input.GetButtonDown("Y_" + playerInfo.playerNumber))
            {
                return PlayerAttackManager.AttackTypes.Heavy;
            }

            // right
            if (Input.GetButtonDown("B_" + playerInfo.playerNumber))
            {
                return PlayerAttackManager.AttackTypes.Stun;
            }
        }

        return PlayerAttackManager.AttackTypes.None;
    } // end GetAttackingInput

    #endregion

    #region Movement Methods

    /// <summary>
    /// Set the CharacterMotor Velocity if allow.
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

        myMotor.Velocity = moveVector;
    } // end SetVelocity

    #endregion

} // end Player class