// Steve Yeager
// 10.21.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Pass hit info from a group of senders to receiver.
/// </summary>
public class GroupHitbox : BaseMono
{
    #region Reference Fields

    protected Transform myTransform;

    #endregion

    #region Public Fields

    public HitInfo hitInfo { get; private set; }

    #endregion

    #region Private Fields

    private static int HitIDs = 1;
    private int hitID;
    private List<Character> senders;
    private bool oneShot;
    private Job moveJob;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tag) return;

        // hit opponent
        Health opponentHealth = other.GetComponent<Health>();
        if (opponentHealth != null)
        {
            opponentHealth.RecieveHit(senders, hitID, hitInfo.TransformKnockBack(opponentHealth.transform.position, myTransform.position));
            if (oneShot)
            {
                gameObject.SetActive(false);
            }
        }

        // hit terrain
        if (oneShot && other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }


    private void OnDisable()
    {
        if (moveJob != null) moveJob.Kill();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="senders">Characters who sent the attackValue.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    /// <param name="hitNumber">How many hits to perform. Usually 1.</param>
    /// <param name="oneShot">Does the hitbox get destroyed after landing one hit?</param>
    public void Initialize(List<Character> senders, HitInfo hitInfo, float time, int hitNumber, bool oneShot = false)
    {
        this.senders = senders;
        this.hitInfo = hitInfo;
        this.oneShot = oneShot;

        tag = senders[0].gameObject.tag;
        SetHitID();
        if (hitNumber > 1)
        {
            StartCoroutine(MultiHit(time, hitNumber));
        }

        hitInfo.FactorMagicAttack((int)senders.Average(s => s.myStats.magicAttack));

        InvokeMethod("End", time);
    }


    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="senders">Characters who sent the attackValue.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    /// <param name="hitNumber">How many hits to perform. Usually 1.</param>
    /// <param name="shootVector">Move vector for hitbox.</param>
    /// <param name="oneShot">Does the hitbox get destroyed after landing one hit?</param>
    public void Initialize(List<Character> senders, HitInfo hitInfo, float time, int hitNumber, Vector3 shootVector, bool oneShot = false)
    {
        moveJob = new Job(Move(shootVector), time);
        Initialize(senders, hitInfo, time, hitNumber, oneShot);
    }


    /// <summary>
    /// End the current attackValue.
    /// </summary>
    public void End()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Increase HitIDs total and assign hitID to next value;
    /// </summary>
    private void SetHitID()
    {
        hitID = ++HitIDs;
    }


    /// <summary>
    /// Increase the hitID to allow for subsequent hits to the same Health component.
    /// </summary>
    /// <param name="time">How long the hitbox will last.</param>
    /// <param name="hitNumber">How many hits to perform.</param>
    private IEnumerator MultiHit(float time, float hitNumber)
    {
        if (hitNumber <= 1) yield break;

        float buffer = time / hitNumber;
        for (int i = 1; i < hitNumber; i++)
        {
            yield return WaitForTime(buffer);
            SetHitID();
        }
    }


    /// <summary>
    /// Move the hitbox over time.
    /// </summary>
    /// <param name="initialVector">Move vector.</param>
    private IEnumerator Move(Vector3 initialVector)
    {
        while (true)
        {
            myTransform.position += initialVector * GameTime.deltaTime;
            yield return null;
        }
    }

    #endregion
}