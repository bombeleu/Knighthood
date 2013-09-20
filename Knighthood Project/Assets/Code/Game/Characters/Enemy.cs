// Steve Yeager
// 8.22.2013

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base enemy class.
/// </summary>
public class Enemy : Character
{
    #region Reference Fields

    private NavAgent myNavAgent;

    #endregion

    #region State Fields

    public const string JumpingState = "Jumping";
    public float navBuffer = 0.5f;
    public float navStopRange = 3f;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // get references
        myNavAgent = GetComponent<NavAgent>();

        // create states
        CreateState(SpawningState, info => SetState(IdlingState, new Dictionary<string, object>()), info => {});
        CreateState(IdlingState, IdlingEnter, info => {});
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(JumpingState, JumpingEnter, JumpingExit);

        initialState = MovingState;
    }


    protected void Start()
    {
        StartInitialState(new Dictionary<string, object>{{"Target", LevelManager.Instance.PlayerTransforms[0]}});
    }

    #endregion

    #region State Methods

    private void IdlingEnter(Dictionary<string, object> info)
    {
        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        while (true)
        {

            yield return null;
        }
    }


    private void MovingEnter(Dictionary<string, object> info)
    {
        currentStateJob = new Job(MovingUpdate((Transform)info["Target"]));
        StartCoroutine("CalculatePath", info["Target"]);
    }


    private IEnumerator MovingUpdate(Transform Target)
    {
        while (true)//Vector3.Distance(myTransform.position, Target.position) > navStopRange)
        {
            // jump
            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        StopCoroutine("CalculatePath");
    }


    private void JumpingEnter(Dictionary<string, object> info)
    {
        
    }


    private IEnumerator JumpingUpdate()
    {


        yield return null;
    }


    private void JumpingExit(Dictionary<string, object> info)
    {
        
    }

    #endregion

    #region Movement Methods

    /// <summary>
    /// Recalculate the NavAgent path at intervals.
    /// </summary>
    /// <param name="Target">Transform of destination.</param>
    private IEnumerator CalculatePath(Transform Target)
    {
        while (true)
        {
            myNavAgent.FindPath(Target.position);
            yield return WaitForTime(navBuffer);
        }
    }

    #endregion
}