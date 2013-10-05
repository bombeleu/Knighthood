// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;

/// <summary>
/// Health component for objects that can recieve hits and die.
/// </summary>
public class Health : BaseMono
{
    #region Public Fields

    public int maxHealth;
    public int currentHealth;// { get; protected set; }
    public bool invincible;// { get; private set; }
    public float[] statusEffectivenesses = {1f, 1f, 1f, 1f, 1f};

    #endregion

    #region Private Fields

    private int lastHitID;

    #endregion

    #region Events

    /// <summary>Triggered when the owner recieves a hit and not invincible.</summary>
    public EventHandler<HitEventArgs> HitEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        Initialize();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set health to max health.
    /// </summary>
    public void Initialize()
    {
        currentHealth = maxHealth;
    }


    /// <summary>
    /// Change the amount of health.
    /// </summary>
    /// <param name="amount">Amount to change health by. Gets added.</param>
    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }


    /// <summary>
    /// Toggle the object's invincibility.
    /// </summary>
    /// <param name="invincible">Should the object be invincible?</param>
    public void ToggleInvincible(bool invincible)
    {
        this.invincible = invincible;
    }

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Recieve an attack.
    /// </summary>
    /// <param name="sender">Who sent the attack.</param>
    /// <param name="hitInfo">Attack info associated with the attack.</param>
    public virtual void RecieveHit(object sender, int hitID, HitInfo hitInfo)
    {
        if (invincible) return;
        if (hitID == lastHitID) return;
        lastHitID = hitID;

        ChangeHealth(-hitInfo.damage);
        GameResources.Instance.DamageIndicator_Pool.nextFree.GetComponent<DamageIndicator>().Initiate(transform, hitInfo.damage);

        if (HitEvent != null)
        {
            HitEvent(sender, new HitEventArgs(hitInfo, currentHealth == 0));
        }
    }

    #endregion
}