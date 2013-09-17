// Steve Yeager
// 8.22.2013

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base enemy class.
/// </summary>
public class Enemy : Character
{
    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // create states
        CreateState(SpawningState, info => SetState(IdlingState, new Dictionary<string, object>()), info => { });
        CreateState(IdlingState, IdlingEnter, info => {});

        initialState = SpawningState;
    }


    protected void Start()
    {
        StartInitialState(new Dictionary<string, object>());
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

    #endregion
}