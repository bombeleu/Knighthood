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

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    } // end Start


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tag) return;

        Health opponentHealth = other.GetComponent<Health>();
        if (opponentHealth != null)
        {
            opponentHealth.RecieveHit(sender, hitID, hitInfo);
        }
    } // end OnTriggerStay

    #endregion

    #region Public Methods

    /// <summary>
    /// Create a new hitbox with new data.
    /// </summary>
    /// <param name="sender">Character who sent the attack.</param>
    /// <param name="hitInfo">HitInfo to pass along to the reciever's Health.</param>
    /// <param name="time">How long the hitbox should last.</param>
    /// <para name="hitNumber">How many hits to perform. Usually 1.</para>
    public void Initialize(Character sender, HitInfo hitInfo, float time, int hitNumber)
    {
        this.sender = sender;
        this.hitInfo = hitInfo;

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
    /// <para name="hitNumber">How many hits to perform. Usually 1.</para>
    /// <para name="hitNumber">How many hits to perform. Usually 1.</para>
    public void Initialize(Character sender, HitInfo hitInfo, float time, int hitNumber, Vector3 shootVector)
    {
        //StartCoroutine(Move(shootVector));
        Job moveJob = new Job(Move(shootVector), time);
        Initialize(sender, hitInfo, time, hitNumber);
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