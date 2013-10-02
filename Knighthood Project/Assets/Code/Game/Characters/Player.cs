// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for players. Interacts with Character
/// </summary>
public class Player : Character
{
    #region Reference Fields

    private ComboManager comboManager;
    private AttackManager attackManager;
    private TauntManager tauntManager;

    #endregion

    #region State Fields

    private const string JumpingState = "Jumping";
    private const string DefendingState = "Defending";

    public float spawnTime;
    public float fastFallSpeed;
    private bool fallingThrough;
    private const float fallingThroughTime = 0.2f;
    public float extraJumpTime;
    private bool canExtraJump = false;
    private bool prematureJump;
    public float prematureJumpTime;

    #endregion

    #region Player Fields

    public PlayerInfo playerInfo { get; private set; }
    public bool keyboard = false;
    public float attackDeadZone = 0.7f;
    public float superModifierDeadZone = 0.8f;
    public float moveBuffer = 0.1f;
    private enum AttackTypes
    {
        None = -1,
        Light = 0,
        Heavy = 1,
        Stun = 2,
        SuperLight = 3,
        SuperHeavy = 4,
        SuperStun = 5,
        SuperJump = 6
    };

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(SpawningState, SpawningEnter, info => {});
        CreateState(IdlingState, IdlingEnter, IdlingExit);
        CreateState(JumpingState, JumpingEnter, info => { });
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(FallingState, FallingEnter, FallingExit);
        CreateState(AttackingState, AttackingEnter, info => {});
        CreateState(DefendingState, DefendingEnter, DefendingExit);
        initialState = SpawningState;

        // combat
        attackManager = GetComponent<AttackManager>();
        attackManager.Initialize(this);
        comboManager = GetComponent<ComboManager>();
        comboManager.Initialize(this);
        tauntManager = GetComponent<TauntManager>();
    }


    private void Start()
    {
#if UNITY_EDITOR
        //UnityEditor.Selection.objects = new UnityEngine.Object[1] { gameObject };
#endif
    }


    private void Update()
    {
        // temp reload scene
        if (Input.GetButtonDown("Back_" + playerInfo.playerNumber) || Input.GetKeyDown(KeyCode.Return))
        {
            GameData.Instance.ReloadScene();
        }
    }

    #endregion

    #region Public Methods

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
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        SetState(IdlingState, new Dictionary<string, object>());
        //currentStateJob = new Job(SpawningUpdate());
    }


    private IEnumerator SpawningUpdate()
    {
        yield return WaitForTime(spawnTime);

        SetState(IdlingState, null);
    }


    private void IdlingEnter(Dictionary<string, object> info)
    {
        // make sure in correct state
        if (!myMotor.IsGrounded())
        {
            SetState(FallingState, null);
            return;
        }
        else
        {
            if (GetMovingInput().x != 0f)
            {
                SetState(MovingState, null);
            }
        }

        // attackAnimation
        PlayAnimation(IdlingState);

        // reset values
        velocity = Vector3.zero;
        SetVelocity(velocity);

        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        while (true)
        {
            // fall through
            if (CanFallThrough())
            {
                SetState(FallingState, new Dictionary<string, object>{{"fallingThrough", true}});
                yield break;
            }

            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            // enter defending state
            if (GetDefendingInput())
            {
                SetState(DefendingState, new Dictionary<string, object>());
                yield break;
            }

            // enter jumping state
            if (GetJumpingInput())
            {
                SetState(JumpingState, null);
                yield break;
            }

            // enter moving state
            if (GetMovingInput().x != 0f)
            {
                SetState(MovingState, null);
                yield break;
            }

            // enter falling state
            if (!myMotor.IsGrounded())
            {
                SetState(FallingState, null);
                yield break;
            }

            // taunt
            Texture tauntTexture = GetTauntingInput();
            if (tauntTexture != null)
            {
                SetState(AttackingState, new Dictionary<string, object>{{"attackTexture", tauntTexture}});
                yield break;
            }

            yield return null;
        }
    }


    private void IdlingExit(Dictionary<string, object> info)
    {
    }


    private IEnumerator MovingUpdate()
    {
        yield return WaitForTime(moveBuffer);
        Vector3 moveVector;

        while (true)
        {
            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            // enter defending state
            if (GetDefendingInput())
            {
                SetState(DefendingState, new Dictionary<string, object>());
                yield break;
            }

            // enter jumping state
            if (GetJumpingInput())
            {
                SetState(JumpingState, null);
                yield break;
            }

            // fall through
            if (CanFallThrough())
            {
                SetState(FallingState, new Dictionary<string, object> { { "fallingThrough", true } });
                yield break;
            }

            moveVector = GetMovingInput();

            // enter idling state
            if (moveVector.x == 0f)
            {
                SetState(IdlingState, null);
                yield break;
            }

            // taunt
            Texture tauntTexture = GetTauntingInput();
            if (tauntTexture != null)
            {
                SetState(AttackingState, new Dictionary<string, object> { { "attackTexture", tauntTexture } });
                yield break;
            }

            // rotate
            myMotor.SetRotation(GetMovingInput().x);

            // move
            velocity.x = moveVector.x * moveSpeed;
            SetVelocity(velocity);

            // enter falling state
            if (!myMotor.IsGrounded())
            {
                var info = new Dictionary<string, object> {{"extraJump", true}};
                SetState(FallingState, info);
                yield break;
            }

            yield return null;
        }
    }


    private void MovingEnter(Dictionary<string, object> info)
    {
        // attackAnimation
        PlayAnimation(MovingState);
    
        currentStateJob = new Job(MovingUpdate());
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        velocity.y = 0f;
    }


    private void JumpingEnter(Dictionary<string, object> info)
    {
        // attackAnimation
        PlayAnimation(JumpingState);

        currentStateJob = new Job(JumpingUpdate(), false);
        currentStateJob.CreateChildJob(Climb(), climbTime);
        currentStateJob.CreateChildJob(Float());

        //
        currentStateJob.JobCompleteEvent += (killed) =>
                                            {
                                                if (killed) return;
                                                info = new Dictionary<string, object> {{"fromJump", true}};
                                                SetState(FallingState, info);
                                            };
        currentStateJob.Start();
    }


    private IEnumerator JumpingUpdate()
    {
        // play the substates Climb and Float
        yield return null;
    }


    private IEnumerator Climb()
    {
        velocity.y = jumpSpeed;

        while (GetJumpingInput(true))
        {
            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            myMotor.SetRotation(GetMovingInput().x);
            velocity.x = GetMovingInput().x * moveSpeed;
            SetVelocity(velocity);
            yield return null;
        }
    }


    private IEnumerator Float()
    {
        while (velocity.y > 0.1f)
        {
            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            myMotor.SetRotation(GetMovingInput().x);
            velocity.x = GetMovingInput().x * moveSpeed;
            velocity.y -= gravity * GameTime.deltaTime;
            SetVelocity(velocity);
            yield return null;
        }
    }


    private void FallingEnter(Dictionary<string, object> info)
    {
        // make sure falling
        if (info.ContainsKey("fallingThrough"))
        {
            fallingThrough = true;
            InvokeAction(() => fallingThrough = false, fallingThroughTime);
        }
        else
        {
            if (myMotor.IsGrounded(true))
            {
                SetState((GetMovingInput().x == 0f ? IdlingState : MovingState), new Dictionary<string, object>());
                return;
            }
        }

        // fall animation
        PlayAnimation(FallingState);

        // give extra jump waitTime
        if (info != null && info.ContainsKey("extraJump"))
        {
            canExtraJump = true;
            InvokeAction(() => { canExtraJump = false; }, extraJumpTime);
        }

        currentStateJob = new Job(FallingUpdate());
    }


    private IEnumerator FallingUpdate()
    {
        while (true)
        {
            // enter idling or jumping state
            if (!fallingThrough && myMotor.IsGrounded(true))
            {
                SetState((prematureJump ? JumpingState : IdlingState), new Dictionary<string, object>());
                yield break;
            }

            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            // enter defending state
            if (GetDefendingInput())
            {
                SetState(DefendingState, new Dictionary<string, object>());
                yield break;
            }

            // enter jumping state
            if (canExtraJump && GetJumpingInput())
            {
                SetState(JumpingState, new Dictionary<string, object>());
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
                velocity.y -= fastFallSpeed * GameTime.deltaTime;
            }

            // move
            myMotor.SetRotation(GetMovingInput().x);
            velocity.x = GetMovingInput().x * moveSpeed;
            Fall();
            SetVelocity(velocity);

            yield return null;
        }
    }


    private void FallingExit(Dictionary<string, object> info)
    {
        StopCoroutine("Fall");
    }


    private void AttackingEnter(Dictionary<string, object> info)
    {
        Texture attackTexture = (Texture)info["attackTexture"];

        PlayAnimation(attackTexture);
        
        currentStateJob = new Job(AttackingUpdate());
    }


    private IEnumerator AttackingUpdate()
    {
        while (true)
        {
            GetAttackingInput();

            if (!myMotor.IsGrounded())
            {
                velocity.y -= gravity * GameTime.deltaTime;
                SetVelocity(velocity);
            }
            else
            {
                velocity = Vector2.zero;
                SetVelocity(velocity);
            }

            yield return null;
        }
    }


    private void DefendingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;

        currentStateJob = new Job(DefendingUpdate());
    }


    private IEnumerator DefendingUpdate()
    {
        while (true)
        {
            // enter idle state
            if (!GetDefendingInput())
            {
                SetState(IdlingState, new Dictionary<string, object>());
                yield break;
            }

            // stop movement
            if (myMotor.IsGrounded(true))
            {
                velocity.x = 0f;
                SetVelocity(velocity);
            }

            myMotor.SetRotation(GetMovingInput().x);

            yield return null;
        }
    }


    private void DefendingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
    }

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
                return Input.GetButton("A_" + playerInfo.playerNumber) && Input.GetAxis("TriggersR_" + playerInfo.playerNumber) < superModifierDeadZone;
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
                return Input.GetButtonDown("A_" + playerInfo.playerNumber) && Input.GetAxis("TriggersR_" + playerInfo.playerNumber) < superModifierDeadZone;
            }
        }
    }


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
    }


    /// <summary>
    /// Detect attack input.
    /// </summary>
    /// <returns>Returns corresponding attack type.</returns>
    private AttackTypes GetAttackingInput()
    {
        if (keyboard)
        {
            if (Input.GetKey(KeyCode.LeftControl)) return AttackTypes.None;
            
            // super
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) && attackManager.CanActivate(AttackTypes.SuperLight.ToString()))
                {
                    return AttackTypes.SuperLight;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) && attackManager.CanActivate(AttackTypes.SuperHeavy.ToString()))
                {
                    return AttackTypes.SuperHeavy;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) && attackManager.CanActivate(AttackTypes.SuperStun.ToString()))
                {
                    return AttackTypes.SuperStun;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow) && attackManager.CanActivate(AttackTypes.SuperJump.ToString()))
                {
                    return AttackTypes.SuperJump;
                }
            }
            // normal
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) && comboManager.CanActivate(AttackTypes.Light.ToString()))
                {
                    return AttackTypes.Light;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) && comboManager.CanActivate(AttackTypes.Heavy.ToString()))
                {
                    return AttackTypes.Heavy;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) && comboManager.CanActivate(AttackTypes.Stun.ToString()))
                {
                    return AttackTypes.Stun;
                }
            }
        }
        else
        {
            // super
            if (Input.GetAxis("TriggersR_" + playerInfo.playerNumber) >= superModifierDeadZone)
            {
                if (Input.GetButtonDown("X_" + playerInfo.playerNumber) && attackManager.CanActivate(AttackTypes.SuperLight.ToString()))
                {
                    return AttackTypes.SuperLight;
                }
                if (Input.GetButtonDown("Y_" + playerInfo.playerNumber) && attackManager.CanActivate(AttackTypes.SuperHeavy.ToString()))
                {
                    return AttackTypes.SuperHeavy;
                }
                if (Input.GetButtonDown("B_" + playerInfo.playerNumber) && attackManager.CanActivate(AttackTypes.SuperStun.ToString()))
                {
                    return AttackTypes.SuperStun;
                }
                if (Input.GetButtonDown("A_" + playerInfo.playerNumber) && attackManager.CanActivate(AttackTypes.SuperJump.ToString()))
                {
                    return AttackTypes.SuperJump;
                }
            }
            // normal
            else
            {
                if (Input.GetButtonDown("X_" + playerInfo.playerNumber) && comboManager.CanActivate(AttackTypes.Light.ToString()))
                {
                    return AttackTypes.Light;
                }
                if (Input.GetButtonDown("Y_" + playerInfo.playerNumber) && comboManager.CanActivate(AttackTypes.Heavy.ToString()))
                {
                    return AttackTypes.Heavy;
                }
                if (Input.GetButtonDown("B_" + playerInfo.playerNumber) && comboManager.CanActivate(AttackTypes.Stun.ToString()))
                {
                    return AttackTypes.Stun;
                }
            }
        }

        return AttackTypes.None;
    }


    /// <summary>
    /// Detect player defending input.
    /// </summary>
    /// <returns>True, if recieving defending input.</returns>
    private bool GetDefendingInput()
    {
        if (keyboard)
        {
            return Input.GetKey(KeyCode.F);
        }
        else
        {
            return Input.GetAxis("TriggersL_" + playerInfo.playerNumber) >= superModifierDeadZone;
        }
    }


    /// <summary>
    /// Can the player fall through the platform?
    /// </summary>
    /// <returns>True, if player is pressing down and the platform is Translucent.</returns>
    private bool CanFallThrough()
    {
        float inputY = GetMovingInput().y;

        if (inputY >= 0) return false;

        RaycastHit rayInfo;
        if (Physics.Raycast(myTransform.position + Vector3.up * 0.1f, Vector3.down, out rayInfo, 0.2f, 1 << LayerMask.NameToLayer("Terrain")))
        {
            return rayInfo.transform.tag == "Translucent";
        }

        return false;
    }


    /// <summary>
    /// Detect player input that registers taunting.
    /// </summary>
    /// <returns>Correct texture if taunting or null if not.</returns>
    private Texture GetTauntingInput()
    {
        if (keyboard)
        {
            if (!Input.GetKey(KeyCode.LeftControl)) return null;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return tauntManager.Activate(0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                return tauntManager.Activate(1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return tauntManager.Activate(2);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                return tauntManager.Activate(3);
            }
        }
        else
        {
            float xInput = Input.GetAxis("DPad_XAxis_" + playerInfo.playerNumber);
            if (xInput <= -0.2f)
            {
                return tauntManager.Activate(0);
            }
            if (xInput >= 0.2f)
            {
                return tauntManager.Activate(2);
            }
            float yInput = Input.GetAxis("DPad_YAxis_" + playerInfo.playerNumber);
            if (yInput <= -0.2f)
            {
                return tauntManager.Activate(3);
            }
            if (yInput >= 0.2f)
            {
                return tauntManager.Activate(1);
            }
        }

        return null;
    }

    #endregion

    #region Combat Methods

    /// <summary>
    /// Activate either combo or attack manager and set state to Attacking.
    /// </summary>
    /// <param name="attack">Attack performed.</param>
    /// <param name="info">Info to pass to the attacking state.</param>
    private void CalculateAttack(AttackTypes attack, Dictionary<string, object> info)
    {
        if (attack == AttackTypes.None) return;

        Texture attackTexture;

        if ((int) attack < 3)
        {
            attackTexture = comboManager.Activate(attack.ToString());
        }
        else
        {
            attackTexture = attackManager.Activate(attack.ToString());
        }

        info.Add("attackTexture", attackTexture);
        SetState(AttackingState, info);
    }

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

        myMotor.velocity = moveVector;
    }


    /// <summary>
    /// Add gravity to velocity for falling.
    /// </summary>
    private void Fall()
    {
        velocity.y -= gravity*GameTime.deltaTime;
        if (velocity.y < -terminalVelocity)
        {
            velocity.y = -terminalVelocity;
        }
        SetVelocity(velocity);
    }

    #endregion
}