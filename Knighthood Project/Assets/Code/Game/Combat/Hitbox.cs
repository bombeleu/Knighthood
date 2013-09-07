// Steve Yeager
// 8.22.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Pass hit info from sender to receiver.
/// </summary>
public class Hitbox : BaseMono
{
    #region References

    protected Transform myTransform;

    #endregion

    #region Combat Fields

    private static int HitIDs = 1;
    private int hitID;
    private Character sender;
    public HitInfo hitInfo { get; private set; }
    private bool oneHit;
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

        Health opponentHealth = other.GetComponent<Health>();
        if (opponentHealth != null)
        {
            opponentHealth.RecieveHit(sender, hitID, hitInfo);
            if (oneHit)
            {
                gameObject.SetActive(false);
            }
        }
    }


    private void OnDisable()
    {
        moveJob.Kill();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="sender">Character who sent the attack.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    /// <param name="hitNumber">How many hits to perform. Usually 1.</param>
    /// <param name="oneHit">Does the hitbox get destroyed after landing one hit?</param>
    public void Initialize(Character sender, HitInfo hitInfo, float time, int hitNumber, bool oneHit = false)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;
        this.oneHit = oneHit;

        gameObject.tag = sender.gameObject.tag;
        SetHitID();
        if (hitNumber > 1)
        {
            StartCoroutine(MultiHit(time, hitNumber));
        }

        InvokeMethod("End", time);
    }


    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="sender">Character who sent the attack.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    /// <param name="hitNumber">How many hits to perform. Usually 1.</param>
    /// <param name="shootVector">Move vector for hitbox.</param>
    /// <param name="oneHit">Does the hitbox get destroyed after landing one hit?</param>
    public void Initialize(Character sender, HitInfo hitInfo, float time, int hitNumber, Vector3 shootVector, bool oneHit = false)
    {
        moveJob = new Job(Move(shootVector), time);
        Initialize(sender, hitInfo, time, hitNumber, oneHit);
    }


    /// <summary>
    /// End the current attack.
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

        float buffer = time/hitNumber;
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
            myTransform.position += initialVector*GameTime.deltaTime;
            yield return null;
        }
    }

    #endregion

} // end Hitbox class