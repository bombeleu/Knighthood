// Steve Yeager
// 10.4.2013

using System.Collections.Generic;
using UnityEngine;
using System.Collections;


/// <summary>
/// 
/// </summary>
public sealed class Dummy : Enemy
{
    #region State Fields

    public const string JumpingState = "Jumping";
    private bool fallingThrough;
    private float gravity;

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
            if (Vector3.Distance(myTransform.position, player.position) > 5f)
            {
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

        currentStateJob = new Job(MovingUpdate());
    }


    private IEnumerator MovingUpdate()
    {
        Vector3 nodePosition;
        while (true)
        {
            // idle
            if (!myNavAgent.GetNextNodePosition(out nodePosition))
            {
                SetState(IdlingState, new Dictionary<string, object>());
                yield break;
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

        while (timeY > 0f)
        {
            Debug.DrawRay(myTransform.position, initialVel);
            timeY -= GameTime.deltaTime;
            myMotor.SetVelocity(initialVel);
            initialVel += Vector3.down * gravity * GameTime.deltaTime;

            yield return null;
        }

        myNavAgent.Continue();
        SetState(MovingState, new Dictionary<string, object> { { "Target", LevelManager.Instance.PlayerTransforms[0] } });

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

        currentStateJob = new Job(FallingUpdate((Vector3)info["target"]));
    }


    private IEnumerator FallingUpdate(Vector3 target)
    {
        while (true)
        {
            // land
            if (!fallingThrough && myMotor.IsGrounded(true))
            {
                SetState(MovingState, new Dictionary<string, object> { { "Target", LevelManager.Instance.PlayerTransforms[0] } });
                yield break;
            }

            // move
            myMotor.MoveX((target - myTransform.position).x / myMotor.moveSpeed);
            myMotor.ApplyGravity();

            yield return null;
        }
    }


    private void FallingExit(Dictionary<string, object> info)
    {
        fallingThrough = false;
    }


    private void DyingEnter(Dictionary<string, object> info)
    {
        myGameObject.SetActive(false);
    }

    #endregion
}