// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Health component for objects that can recieve hits and die.
/// </summary>
public class Health : BaseMono
{
  public int maxHealth;
  public int currentHealth { get; private set; }
  public bool invincible { get; private set; }

  public EventHandler<HitEventArgs> HitEvent;


  private void Awake()
  {
    Initialize();
  } // end Awake


  /// <summary>
  /// Set health to max health.
  /// </summary>
  public void Initialize()
  {
    currentHealth = maxHealth;
  } // end Initialize


  /// <summary>
  /// Change the amount of health.
  /// </summary>
  /// <param name="amount">Amount to change health by. Gets added.</param>
  public void ChangeHealth(int amount)
  {
    currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
  } // end ChangeHealth


  /// <summary>
  /// Toggle the object's invincibility.
  /// </summary>
  /// <param name="invincible">Should the object be invincible?</param>
  public void ToggleInvincible(bool invincible)
  {
    this.invincible = invincible;
  } // end ToggleInvincible


  /// <summary>
  /// Recieve an attack.
  /// </summary>
  /// <param name="sender">Who sent the attack.</param>
  /// <param name="hitInfo">Attack info associated with the attack.</param>
  public void RecieveHit(object sender, HitInfo hitInfo)
  {
    if (invincible) return;

    ChangeHealth(hitInfo.damage);

    if (HitEvent != null)
    {
      HitEvent(sender, new HitEventArgs(hitInfo, currentHealth == 0));
    }
  } // end RecieveHit

} // end Health class