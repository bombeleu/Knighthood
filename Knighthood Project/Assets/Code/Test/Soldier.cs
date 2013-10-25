// Steve Yeager
// 10.4.2013

using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public sealed class Soldier : Enemy
{
    #region Patrolling Fields

    /// <summary>Horizontal distance player is detected when Patrolling.</summary>
    public float attentionRadius = 5f;
    /// <summary>Max distance Soldier can move before turning.</summary>
    public float maxPatrolDistance = 10f;
    /// <summary>Distance to check ahead for object to turn around.</summary>
    public float patrollingCast = 2f;
    /// <summary>How long it takes to turn around.</summary>
    public float patrolTurnTime = 1f;
    /// <summary>Patrolling speed.</summary>
    public float patrollingSpeed = 5f;

    #endregion

    #region State Fields

    public const string PatrollingState = "Patrolling";

    public const string JumpingState = "Jumping";
    private bool fallingThrough;
    private float gravity;
    public float attackDistance = 3f;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(SpawningState, info => SetState(PatrollingState, new Dictionary<string, object>()), info => { });
        CreateState(PatrollingState, PatrollingEnter, info => { });
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(JumpingState, JumpingEnter, info => { });
        CreateState(FallingState, FallingEnter, FallingExit);
        CreateState(AttackState, AttackingEnter, info => { });
        CreateState(IdlingState, IdlingEnter, info => { });
        CreateState(FlinchingState, FlinchingEnter, FlinchingExit);
        CreateState(DyingState, DyingEnter, info => { });

        initialState = SpawningState;

        // set up
        gravity = myMotor.gravity;
    }


    protected override void Start()
    {
        base.Start();

        // register events
        attackManager.OnAttackOver += OnAttackOver;

        StartInitialState(new Dictionary<string, object>());
    }

    #endregion

    #region State Methods

    private void PatrollingEnter(Dictionary<string, object> info)
    {
        PlayAnimation("Moving");

        currentStateJob = new Job(PatrollingUpdate());
    }


    private IEnumerator PatrollingUpdate()
    {
        float currentDistance = 0f;
        while (true)
        {
            // player is close enough
            foreach (var player in LevelManager.Instance.PlayerTransforms)
            {
                if (Mathf.Abs((player.position - myTransform.position).x) <= attentionRadius)
                {
                    currentTarget = player;
                    SetState(MovingState, new Dictionary<string,object>());
                    yield break;
                }
            }

            // turn around: max distance, infront of object, over edge
            if (currentDistance >= maxPatrolDistance ||
                Physics.Raycast(myTransform.position + Vector3.up, myTransform.forward, patrollingCast, 1<<LayerMask.NameToLayer("Terrain")) ||
                !Physics.Raycast(myTransform.position + Vector3.up + myTransform.forward * patrollingCast/2f, Vector3.down, 1.1f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                currentDistance = 0f;
                myMotor.ClearVelocity();
                PlayAnimation("Idling");

                float time = patrolTurnTime;
                while (time > 0)
                {
                    time -= GameTime.deltaTime;

                    // player here
                    foreach (var player in LevelManager.Instance.PlayerTransforms)
                    {
                        if (Mathf.Abs((player.position - myTransform.position).x) <= attentionRadius)
                        {
                            currentTarget = player;
                            SetState(MovingState, new Dictionary<string, object>());
                            yield break;
                        }
                    }

                    yield return null;
                }

                myMotor.SetRotation(-myTransform.forward.x);
                PlayAnimation("Moving");
            }


            // move
            myMotor.SetVelocityForward(patrollingSpeed);
            currentDistance += patrollingSpeed * GameTime.deltaTime;

            yield return null;
        }
    }


    private void MovingEnter(Dictionary<string, object> info)
    {
        myNavAgent.StartNav(currentTarget);

        PlayAnimation(MovingState);

        currentStateJob = new Job(MovingUpdate());
    }


    private IEnumerator MovingUpdate()
    {
        Vector3 nodePosition;
        while (true)
        {
            // attackValue
            if (Mathf.Abs((currentTarget.position - myTransform.position).x) <= attackDistance)
            {
                if (attackManager.CanActivate(0))
                {
                    SetState(AttackState, new Dictionary<string, object>());
                    yield break;
                }
                else
                {
                    SetState(IdlingState, new Dictionary<string, object>());
                    yield break;
                }
            }

            // idle
            if (!myNavAgent.GetNextNodePosition(out nodePosition))
            {
                nodePosition = currentTarget.position;
            }

            // rotate
            myMotor.SetRotation((nodePosition - myTransform.position).x);

            // jump
            if (nodePosition.y - myTransform.position.y >= myNavAgent.stepHeight)
            {
                SetState(JumpingState, new Dictionary<string, object> { { "target", nodePosition } });
                yield break;
            }

            // fall
            if (!myMotor.IsGrounded() || (nodePosition.y - myTransform.position.y < -myNavAgent.stepHeight && myMotor.OverTranslucent()))
            {
                SetState(FallingState, new Dictionary<string, object> { { "target", nodePosition }, { "translucent", true } });
                yield break;
            }

            // move
            myMotor.MoveX(Mathf.Sign((nodePosition - myTransform.position).x));

            // next node
            if (Vector3.Distance(myTransform.position, nodePosition) <= myNavAgent.allowedRadius)
            {
                Vector3 nextNode;
                myNavAgent.Continue(out nextNode);
            }

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        myNavAgent.EndNav();
    }


    private void JumpingEnter(Dictionary<string, object> info)
    {
        myMotor.ClearVelocity();

        PlayAnimation(JumpingState);

        currentStateJob = new Job(JumpingUpdate((Vector3)info["target"]));
    }


    private IEnumerator JumpingUpdate(Vector3 target)
    {
        Vector3 dist = target - myTransform.position;

        float velIntY = Mathf.Sqrt(2 * gravity * (dist.y + 1));
        float timeY = (-velIntY - Mathf.Sqrt(velIntY * velIntY - 2 * gravity * dist.y)) / -gravity;

        float velX = dist.x / timeY;

        Vector3 initialVel = new Vector3(velX, velIntY, 0f);

        while (initialVel.y > 0)
        {
            timeY -= GameTime.deltaTime;
            myMotor.SetVelocity(initialVel);
            initialVel += Vector3.down * gravity * GameTime.deltaTime;

            yield return null;
        }

        myNavAgent.Continue();
        SetState(FallingState, new Dictionary<string, object> { { "target", target }, { "speed", velX } });

        yield return null;
    }


    private void FallingEnter(Dictionary<string, object> info)
    {
        PlayAnimation(FallingState);

        if (info.ContainsKey("translucent"))
        {
            fallingThrough = true;
            InvokeAction(() => fallingThrough = false, 0.2f);
        }

        currentStateJob = new Job(FallingUpdate((Vector3)info["target"], info.ContainsKey("speed") ? (float)info["speed"] : -1f));
    }


    private IEnumerator FallingUpdate(Vector3 target, float speedX)
    {
        if (speedX == -1f)
        {
            
        }
        myMotor.SetVelocityX(speedX);

        while (true)
        {
            // land
            if (!fallingThrough && myMotor.IsGrounded(true))
            {
                SetState(MovingState, new Dictionary<string, object> { { "Target", LevelManager.Instance.PlayerTransforms[0] } });
                yield break;
            }

            // move
            //myMotor.MoveX((target - myTransform.position).x / myMotor.moveSpeed);
            myMotor.ApplyGravity();

            yield return null;
        }
    }


    private void FallingExit(Dictionary<string, object> info)
    {
        fallingThrough = false;
    }


    private void AttackingEnter(Dictionary<string, object> info)
    {
        // stop
        myMotor.ClearVelocity();

        // face
        myMotor.SetRotation((currentTarget.position - myTransform.position).x);

        // attackValue
        Texture attack = attackManager.Activate(0);
        PlayAnimation(attack);
    }

     
    private void IdlingEnter(Dictionary<string, object> info)
    {
        myMotor.ClearVelocity();

        PlayAnimation(IdlingState);

        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        while (true)
        {
            // attacking
            if (Mathf.Abs((currentTarget.position - myTransform.position).x) <= attackDistance)
            {
                if (attackManager.CanActivate(0))
                {
                    SetState(AttackState, new Dictionary<string, object>());
                    yield break;
                }

                yield return null;
            }

            // moving
            if (Mathf.Abs((currentTarget.position - myTransform.position).x) > attackDistance)
            {
                // and path is greater than one
                SetState(MovingState, new Dictionary<string, object>());
                yield break;
            }

            yield return null;
        }
    }


    private void FlinchingEnter(Dictionary<string, object> info)
    {
        var knockBack = (Vector3)info["knockBack"];

        if (myMotor.IsGrounded() && knockBack.y == 0f)
        {
            PlayAnimation("Stand Flinch");
        }
        else
        {
            PlayAnimation("Fall Flinch");
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
                PlayAnimation("Splat");
                yield return WaitForTime(1f);
                break;
            }

            // start falling
            if (!grounded && !falling)
            {
                falling = true;
                PlayAnimation("Fall Flinch");
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

        SetState(myMotor.IsGrounded(true) ? MovingState : FallingState, new Dictionary<string, object>());
    }


    private void FlinchingExit(Dictionary<string, object> info)
    {
        StartCoroutine("Invincible", flinchInvincibleTime);
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        myGameObject.SetActive(false);
    }

    #endregion

    #region Event Handlers

    protected override void HitHandler(object sender, HitEventArgs args)
    {
        // end attacks
        attackManager.Cancel();

        base.HitHandler(sender, args);
    }

    #endregion
}