// Steve Yeager
// 10.4.2013

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public sealed class Dummy : Enemy
{
    #region State Fields

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
        CreateState(SpawningState, info => SetState(IdlingState, new Dictionary<string, object>()), info => { });
        CreateState(IdlingState, IdlingEnter, info => { });
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(JumpingState, JumpingEnter, info => { });
        CreateState(FallingState, FallingEnter, FallingExit);
        CreateState(AttackState, AttackingEnter, info => { });
        CreateState(DyingState, DyingEnter, info => { });

        initialState = IdlingState;

        // set up
        gravity = myMotor.gravity;
    }


    protected override void Start()
    {
        base.Start();

        StartInitialState(new Dictionary<string, object> { { "Target", LevelManager.Instance.PlayerTransforms[0] } });
    }

    #endregion

    #region State Methods

    private void IdlingEnter(Dictionary<string, object> info)
    {
        myMotor.ClearVelocity();

        PlayAnimation(IdlingState);

        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        Transform player = LevelManager.Instance.PlayerTransforms[0];
        while (true)
        {
            // attacking
            if (Mathf.Abs((player.position - myTransform.position).x) <= attackDistance)
            {
                if (attackManager.CanActivate(0))
                {
                    SetState(AttackState, new Dictionary<string, object> {{"Target", player}});
                    yield break;
                }

                yield return null;
            }

            // moving
            if (Mathf.Abs((player.position - myTransform.position).x) > attackDistance)
            {
                // and path is greater than one
                Log("Idling->Moving: Outside attackDist.");
                SetState(MovingState, new Dictionary<string, object> { { "Target", player } });
                yield break;
            }

            yield return null;
        }
    }


    private void MovingEnter(Dictionary<string, object> info)
    {
        myNavAgent.StartNav((Transform)info["Target"]);

        PlayAnimation(MovingState);

        currentStateJob = new Job(MovingUpdate((Transform)info["Target"]));
    }


    private IEnumerator MovingUpdate(Transform Target)
    {
        Vector3 nodePosition;
        while (true)
        {
            // attack
            if (Mathf.Abs((Target.position - myTransform.position).x) <= attackDistance)
            {
                if (attackManager.CanActivate(0))
                {
                    SetState(AttackState, new Dictionary<string, object> {{"Target", Target}});
                    yield break;
                }
                else
                {
                    Log("Moving->Idling: Within attackDist but can't attack.");
                    SetState(IdlingState, new Dictionary<string, object>());
                    yield break;
                }
            }

            // idle
            if (!myNavAgent.GetNextNodePosition(out nodePosition))
            {
                Log("Moving->Idling: No more nodes in path.");
                //SetState(IdlingState, new Dictionary<string, object>());
                nodePosition = Target.position;
                //yield break;
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

        // attack
        Texture attack = attackManager.Activate(0);
        PlayAnimation(attack);
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        myGameObject.SetActive(false);
    }

    #endregion
}