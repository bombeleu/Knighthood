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
    private UltimateAttackManager ultimateManager;

    #endregion

    #region State Fields

    private const string JumpingState = "Jumping";
    private const string DefendingState = "Defending";
    private const string DodgeRollingState = "DodgeRolling";
    private const string AirDodgingState = "AirDodging";
    private const string UltimateState = "Ultimate";

    public float spawnTime;
    public float fastFallSpeed;
    private bool fallingThrough;
    private const float fallingThroughTime = 0.2f;
    public float extraJumpTime;
    private bool canExtraJump = false;
    private bool prematureJump;
    public float prematureJumpTime;
    /// <summary>How long the myCharacter can jump.</summary>
    public float climbTime = 0.3f;

    /// <summary>How long perfectShield lasts after Defending starts.</summary>
    public float perfectShieldTime;
    /// <summary>If hit during perfectShield enemy will flinch.<summary>
    private bool perfectShield;
    /// <summary>How much damage the player can take while defending.</summary>
    public int shieldHealthMax;
    /// <summary>Current damage taken while defending.</summary>
    private int shieldHealth;
    /// <summary>Time in seconds to increase shieldHealth.</summary>
    public float shieldRegenTime;
    /// <summary>Multiplied by normal speed.</summary>
    public float defendSpeedModifier = 0.5f;

    /// <summary>How long the air dodge lasts.</summary>
    public float airDodgeTime;
    /// <summary>Buffer time between air dodges.</summary>
    public float airDodgeBuffer;
    /// <summary>Can air dodge?</summary>
    private bool canAirDodge = true;

    /// <summary>Speed of roll.</summary>
    public float dodgeRollSpeed;
    /// <summary>How long the dodge roll lasts.</summary>
    public float dodgeRollTime;
    /// <summary>Buffer time between dodge rolls.</summary>
    public float dodgeRollBuffer;
    /// <summary>Can dodge roll?</summary>
    private bool canDodgeRoll = true;

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

    #region Stat Fields

    public enum Characters { Chad = 0, Eva = 1, Harold = 2, Jules = 3 }
    public Characters character;
    public ExperienceManager myExperience;
    public MoneyManager myMoney;
    public PerformanceManager myPerformance;
    public ScoreManager myScore;
    private const float SCOREEXPMODIFIER = 1.5f;
    private const float SCOREMONEYMODIFIER = 5.5f;

    #endregion

    #region Events

    public static EventHandler<MoneyEventAgs> MoneyEvent;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(SpawningState, SpawningEnter, info => {});
        CreateState(IdlingState, IdlingEnter, IdlingExit);
        CreateState(JumpingState, JumpingEnter, info => {});
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(FallingState, FallingEnter, FallingExit);
        CreateState(AttackingState, AttackingEnter, info => {});
        CreateState(UltimateState, UltimateEnter, UltimateExit);
        CreateState(DefendingState, DefendingEnter, DefendingExit);
        CreateState(DodgeRollingState, DodgeRollingEnter, DodgeRollingExit);
        CreateState(AirDodgingState, AirDodgingEnter, AirDodgingExit);
        CreateState(FlinchingState, FlinchingEnter, FlinchingExit);
        // dying state
        
        initialState = SpawningState;

        // combat
        attackManager = GetComponent<AttackManager>();
        attackManager.Initialize(this);
        comboManager = GetComponent<ComboManager>();
        comboManager.Initialize(this);
        tauntManager = GetComponent<TauntManager>();
        ultimateManager = GetComponent<UltimateAttackManager>();
    }


    protected override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        //UnityEditor.Selection.objects = new UnityEngine.Object[1] { gameObject };
#endif

        // events
        MoneyEvent += CollectMoney;
        tauntManager.OnTauntOver += (x, y) => SetState(IdlingState, new Dictionary<string,object>());
        attackManager.OnAttackOver += OnAttackOver;
        comboManager.OnAttackOver += OnAttackOver;
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

    #region Static Methods

    /// <summary>
    /// Trigger the MoneyEvent.
    /// </summary>
    /// <param name="worth"></param>
    public static void AllCollectMoney(object collector, int worth)
    {
        MoneyEvent(collector, new MoneyEventAgs(worth));
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

        // set up
        myExperience.Load(0);
        myMoney = new MoneyManager(playerInfo.username);
        myPerformance = new PerformanceManager(playerInfo.username);
        myScore = new ScoreManager(playerInfo.username);

        StartInitialState(null);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="experience"></param>
    public void RecieveKill(Enemy.EnemyTypes enemy, int experience)
    {
        myExperience.Increase(experience);
        myPerformance.IncreaseKill(enemy.ToString());
        myScore.IncreaseScore(Mathf.CeilToInt(experience*SCOREEXPMODIFIER));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    public void Retag(int number)
    {
        tag = "Player" + number;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handler for MoneyEvent. Add money to manager.
    /// </summary>
    private void CollectMoney(object collector, MoneyEventAgs args)
    {
        myMoney.Transaction(args.worth);
        myPerformance.IncreaseMoney(args.worth);
        myScore.IncreaseScore(Mathf.CeilToInt(args.worth*SCOREMONEYMODIFIER));
    }

    #endregion

    #region State Methods

    private void SpawningEnter(Dictionary<string, object> info)
    {
        SetState(IdlingState, new Dictionary<string, object>());
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
        myMotor.ClearVelocity();

        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        while (true)
        {
            // defending state
            if (GetDefendingInput())
            {
                SetState(DefendingState, new Dictionary<string, object>());
                yield break;
            }

            // dodge roll
            if (GetDodgeRollingInput() != 0f)
            {
                SetState(DodgeRollingState, new Dictionary<string, object> { { "velocity", GetDodgeRollingInput() } });
                yield break;
            }

            // fall through
            if (CanFallThrough())
            {
                SetState(FallingState, new Dictionary<string, object>{{"fallingThrough", true}});
                yield break;
            }

            // ultimate state
            if (GetUltimateInput())
            {
                if (ultimateManager.CanActivate())
                {
                    ultimateManager.Activate();
                    SetState(UltimateState, new Dictionary<string, object>());
                }
            }

            // attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            // jumping state
            if (GetJumpingInput())
            {
                SetState(JumpingState, null);
                yield break;
            }

            // moving state
            if (GetMovingInput().x != 0f)
            {
                SetState(MovingState, null);
                yield break;
            }

            // falling state
            if (!myMotor.IsGrounded())
            {
                SetState(FallingState, null);
                yield break;
            }

            // taunting state
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


    private void MovingEnter(Dictionary<string, object> info)
    {
        // attackAnimation
        PlayAnimation(MovingState);

        currentStateJob = new Job(MovingUpdate());
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

            // dodge roll
            if (GetDodgeRollingInput() != 0f)
            {
                SetState(DodgeRollingState, new Dictionary<string, object> { { "velocity", GetDodgeRollingInput() } });
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
            myMotor.MoveX(moveVector.x);

            // enter falling state
            if (!myMotor.IsGrounded(true))
            {
                var info = new Dictionary<string, object> {{"extraJump", true}};
                SetState(FallingState, info);
                yield break;
            }

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        myMotor.SetVelocityY(0f);
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
        float inputX;
        while (GetJumpingInput(true))
        {
            // air dodge
            if (GetAirDodgeInput())
            {
                SetState(AirDodgingState, new Dictionary<string, object>());
                yield break;
            }

            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            inputX = GetMovingInput().x;
            myMotor.SetRotation(inputX);
            myMotor.MoveX(inputX);
            myMotor.SetVelocityY(myMotor.jumpStrength);

            yield return null;
        }
    }


    private IEnumerator Float()
    {
        float inputX = 0f;
        while (myMotor.velocity.y > 0.1f)
        {
            // air dodge
            if (GetAirDodgeInput())
            {
                SetState(AirDodgingState, new Dictionary<string, object>());
                yield break;
            }

            // enter attacking state
            AttackTypes attack = GetAttackingInput();
            if (attack != AttackTypes.None)
            {
                CalculateAttack(attack, new Dictionary<string, object>());
                yield break;
            }

            inputX = GetMovingInput().x;
            myMotor.SetRotation(inputX);
            myMotor.ApplyGravity();
            myMotor.MoveX(inputX);

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
        float inputX = 0f;
        while (true)
        {
            // enter idling or jumping state
            if (!fallingThrough && myMotor.IsGrounded(true))
            {
                SetState((prematureJump ? JumpingState : IdlingState), new Dictionary<string, object>());
                yield break;
            }

            // air dodge
            if (GetAirDodgeInput())
            {
                SetState(AirDodgingState, new Dictionary<string, object>());
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
                myMotor.AddVelocityY(fastFallSpeed * GameTime.deltaTime);
            }

            // move
            inputX = GetMovingInput().x;
            myMotor.SetRotation(inputX);
            myMotor.ApplyGravity();
            myMotor.MoveX(inputX);

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
                myMotor.ApplyGravity();
            }
            else
            {
                myMotor.ClearVelocity();
            }

            yield return null;
        }
    }


    private void UltimateEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;
    }


    private void UltimateExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
    }


    private void DefendingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;
        StartCoroutine("PerfectShieldTimer");
        StopCoroutine("ShieldRegen");

        PlayAnimation(myMotor.IsGrounded(true) ? "Defend Ground" : "Defend Air");

        currentStateJob = new Job(DefendingUpdate());
    }
    

    private IEnumerator DefendingUpdate()
    {
        bool grounded = myMotor.IsGrounded(true);
        while (true)
        {
            if (myMotor.IsGrounded(true))
            {
                // land
                if (!grounded)
                {
                    grounded = true;
                    myMotor.SetVelocityY(0f);
                    PlayAnimation("Defend Ground");
                }

                // dodge roll
                if (GetDodgeRollingInput() != 0f)
                {
                    SetState(DodgeRollingState, new Dictionary<string, object> { { "velocity", GetDodgeRollingInput() } });
                    yield break;
                }

                // enter idle state
                if (!GetDefendingInput())
                {
                    SetState(IdlingState, new Dictionary<string, object>());
                    yield break;
                }

                // move
                myMotor.MoveX(GetMovingInput().x*defendSpeedModifier);
            }
            else
            {
                // enter falling state
                if (!GetDefendingInput())
                {
                    SetState(FallingState, new Dictionary<string, object>());
                    yield break;
                }

                // air dodge
                if (GetDodgeRollingInput() != 0f)
                {
                    SetState(AirDodgingState, new Dictionary<string, object>());
                    yield break;
                }

                // move
                myMotor.ApplyGravity();
                myMotor.MoveX(GetMovingInput().x * defendSpeedModifier);
            }

            yield return null;
        }
    }


    private void DefendingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
        StopCoroutine("PerfectShieldTimer");
        StartCoroutine("RegenShield");
    }


    private IEnumerator RegenShield()
    {
        while (shieldHealth < shieldHealthMax)
        {
            yield return WaitForTime(shieldRegenTime);
            shieldHealth++;
        }

        shieldHealth = shieldHealthMax;
    }


    private void DodgeRollingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;

        PlayAnimation("Dodge Roll");
        myMotor.SetVelocity((float)info["velocity"], 0f);

        currentStateJob = new Job(DodgeRollingUpdate());
    }


    private IEnumerator DodgeRollingUpdate()
    {
        float time = dodgeRollTime;
        while (time > 0f)
        {
            time -= GameTime.deltaTime;

            // fall
            if (!myMotor.IsGrounded(true))
            {
                SetState(FallingState, new Dictionary<string, object>());
                yield break;
            }

            yield return null;
        }

        SetState(IdlingState, new Dictionary<string, object>());
    }


    private void DodgeRollingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;
        myMotor.ClearVelocity();
        StartCoroutine("DodgeRollBuffer");
    }


    private IEnumerator DodgeRollBuffer()
    {
        canDodgeRoll = false;
        yield return WaitForTime(dodgeRollBuffer);
        canDodgeRoll = true;
    }


    private void AirDodgingEnter(Dictionary<string, object> info)
    {
        myHealth.invincible = true;

        PlayAnimation("Air Dodge");

        currentStateJob = new Job(AirDodgingUpdate());
    }


    private IEnumerator AirDodgingUpdate()
    {
        float time = airDodgeTime;
        while (time > 0f)
        {
            time -= GameTime.deltaTime;

            // land
            if (myMotor.IsGrounded(true))
            {
                SetState(IdlingState, new Dictionary<string, object>());
                yield break;
            }

            // move
            myMotor.ApplyGravity();
            myMotor.MoveX(GetMovingInput().x);

            yield return null;
        }

        SetState(FallingState, new Dictionary<string, object>());
    }


    private void AirDodgingExit(Dictionary<string, object> info)
    {
        myHealth.invincible = false;

        StartCoroutine("AirDodgingBuffer");
    }


    private IEnumerator AirDodgingBuffer()
    {
        canAirDodge = false;
        yield return WaitForTime(airDodgeBuffer);
        canAirDodge = true;
    }


    private void FlinchingEnter(Dictionary<string, object> info)
    {
        var knockBack = (Vector3)info["knockBack"];

        if (myMotor.IsGrounded() && knockBack.y == 0f)
        {
            PlayAnimation("Flinch Ground");
        }
        else
        {
            PlayAnimation("Flinch Fall");
        }

        currentStateJob = new Job(FlinchingUpdate(knockBack, flinchTimeBase + knockBack.magnitude * knockBackMultiplier));
    }


    private IEnumerator FlinchingUpdate(Vector3 knockBack, float flinchTime)
    {
        Log(flinchTime);
        bool falling = !myMotor.IsGrounded();
        float time = flinchTime;
        float fallSpeed = 0;

        while (time > 0f)
        {
            time -= GameTime.deltaTime;
            bool grounded = myMotor.IsGrounded();
            myMotor.ClearVelocity();

            // hit the floor
            if (grounded && falling)
            {
                myMotor.Ground();
                PlayAnimation("Flinch Land");
                yield return WaitForTime(1f);
                break;
            }

            // start falling
            if (!grounded && !falling)
            {
                falling = true;
                PlayAnimation("Flinch Fall");
            }

            // fall
            if (!grounded)
            {
                fallSpeed -= myMotor.gravity * GameTime.deltaTime;
                myMotor.SetVelocityY(fallSpeed);
            }

            // knockback
            myMotor.AddVelocity(knockBack);

            // recover
            knockBack.x -= Mathf.Sign(knockBack.x) * knockBackRecoverySpeed * GameTime.deltaTime;

            yield return null;
        }

        SetState(myMotor.IsGrounded(true) ? IdlingState : FallingState, new Dictionary<string, object>());
    }


    private void FlinchingExit(Dictionary<string, object> info)
    {
        StartCoroutine("Invincible", flinchInvincibleTime);
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
    /// Detect attackValue input.
    /// </summary>
    /// <returns>Returns corresponding attackValue type.</returns>
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
    /// Detect dodge input.
    /// </summary>
    /// <returns>Dodge Roll speed in correct direction.</returns>
    private float GetDodgeRollingInput()
    {
        if (!canDodgeRoll) return 0f;

        if (keyboard)
        {
            return 0f;
        }
        else
        {
            // right
            if (Input.GetAxis("R_XAxis_" + playerInfo.playerNumber) > 0.2f)
            {
                return dodgeRollSpeed;
            }
            // left
            else if (Input.GetAxis("R_XAxis_" + playerInfo.playerNumber) < -0.2f)
            {
                return -dodgeRollSpeed;
            }
        }

        return 0f;
    }


    /// <summary>
    /// Detect air dodging input.
    /// </summary>
    /// <returns>True, if recieving air dodging input.</returns>
    public bool GetAirDodgeInput()
    {
        if (!canAirDodge) return false;

        if (keyboard)
        {
            return false;
        }
        else
        {
            return Mathf.Abs(Input.GetAxis("R_XAxis_" + playerInfo.playerNumber)) > 0.2f;
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


    /// <summary>
    /// Detect Ultimate Attack input.
    /// </summary>
    /// <returns>True, if Ultimate Attack input pressed.</returns>
    private bool GetUltimateInput()
    {
        if (keyboard)
        {
            return Input.GetKeyDown(KeyCode.E);
        }
        else
        {
            return Input.GetButtonDown("RB_" + playerInfo.playerNumber);
        }
    }

    #endregion

    #region Combat Methods

    /// <summary>
    /// Activate either combo or attackValue manager and set state to Attacking.
    /// </summary>
    /// <param name="attackValue">Attack performed.</param>
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


    /// <summary>
    /// Activate perfectShield for a time.
    /// </summary>
    private IEnumerator PerfectShieldTimer()
    {
        perfectShield = true;
        yield return WaitForTime(perfectShieldTime);
        perfectShield = false;
    }

    #endregion

    #region Event Handlers

    protected override void HitHandler(object sender, HitEventArgs args)
    {
        // end attacks
        attackManager.Cancel();
        comboManager.Cancel();
        // cancel taunt

        base.HitHandler(sender, args);
    }

    #endregion
}