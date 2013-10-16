// Steve Yeager
// 8.18.2013

using System.Collections;
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
    private string[] statusMethods = new string[5];

    #endregion

    #region Events

    /// <summary>Triggered when the owner recieves a hit and not invincible.</summary>
    public EventHandler<HitEventArgs> HitEvent;
    public EventHandler<StatusEffectEventArgs> FireEvent;
    public EventHandler<StatusEffectEventArgs> LightningEvent;
    public EventHandler<StatusEffectEventArgs> AcidEvent;
    public EventHandler<StatusEffectEventArgs> EarthEvent;
    public EventHandler<StatusEffectEventArgs> IceEvent;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        Initialize();
    }


    private void Start()
    {
        // status methods
        statusMethods[0] = "FireEffect";
        statusMethods[1] = "LightningEffect";
        statusMethods[2] = "AcidEffect";
        statusMethods[3] = "EarthEffect";
        statusMethods[4] = "IceEffect";
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

        var damage = hitInfo.damage;

        // status effect
        if (hitInfo.effect != HitInfo.Effects.None)
        {
            damage = Mathf.CeilToInt(damage*statusEffectivenesses[(int)hitInfo.effect]);
            if (damage > 0)
            {
                Log(hitInfo.effect + ":" + (int) hitInfo.effect, Debugger.LogTypes.Combat);
                StopAllCoroutines();
                StartCoroutine(statusMethods[(int)hitInfo.effect], damage);
            }
        }

        ChangeHealth(-damage);
        CreateIndicator(damage);

        if (HitEvent != null)
        {
            HitEvent(sender, new HitEventArgs(hitInfo, currentHealth == 0));
        }
    }


    protected virtual IEnumerator FireEffect(int damage)
    {
        if (FireEvent != null) FireEvent(null, new StatusEffectEventArgs(statusEffectivenesses[0] > 1f, 2.5f));

        var material = GetComponentInChildren<Renderer>().material;

        damage = Mathf.CeilToInt(damage / 10f);

        var originalColor = material.color;
        material.color = Color.red;

        for (int i = 0; i < 5; i++)
        {
            yield return WaitForTime(0.5f);
            ChangeHealth(-damage);
            CreateIndicator(damage);
        }

        material.color = originalColor;
    }


    protected virtual IEnumerator IceEffect(int damage)
    {
        if (IceEvent != null) IceEvent(null, new StatusEffectEventArgs(statusEffectivenesses[4] > 1f, 2.5f));

        var material = GetComponentInChildren<Renderer>().material;
        var originalColor = material.color;
        material.color = Color.blue;
        yield return WaitForTime(2.5f);
        material.color = originalColor;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Create a damage indicator.
    /// </summary>
    /// <param name="damage">Damage to be displayed.</param>
    private void CreateIndicator(int damage)
    {
        GameResources.Instance.DamageIndicator_Pool.nextFree.GetComponent<DamageIndicator>().Initiate(transform, damage);
    }

    #endregion
}