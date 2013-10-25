// Steve Yeager
// 8.25.2013

using UnityEngine;

/// <summary>
/// Health for characters.
/// </summary>
public class CharacterHealth : Health
{
    #region Reference Fields

    private StatManager statManager;

    #endregion

    #region Public Fields

    public enum AttackArmor { None, Weak, Strong }
    public AttackArmor attackArmor;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        statManager = GetSafeComponent<Character>().myStats;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set current and max health through the StatManager.
    /// </summary>
    /// <param name="statManager"></param>
    public void Initialize(StatManager stats)
    {
        maxHealth = stats.health;
        currentHealth = maxHealth;
        this.statManager = stats;
    }

    #endregion

    #region Health Overrides

    public override void RecieveHit(object sender, int hitID, HitInfo hitInfo)
    {
        if (attackArmor == AttackArmor.Strong) return;
        if (invincible) return;
        if (hitID == lastHitID) return;
        lastHitID = hitID;

        var damage = hitInfo.damage;

        // status effect
        if (hitInfo.effect != HitInfo.Effects.None)
        {
            damage = Mathf.CeilToInt(damage * statusEffectivenesses[(int)hitInfo.effect]);
            if (damage > 0)
            {
                Log(hitInfo.effect + ":" + (int)hitInfo.effect, Debugger.LogTypes.Combat);
                StopAllCoroutines();
                StartCoroutine(statusMethods[(int)hitInfo.effect], damage);
            }
        }

        ChangeHealth(-damage);
        CreateIndicator(damage);

        if (attackArmor == AttackArmor.None)
        {
            if (HitEvent != null)
            {
                HitEvent(sender, new HitEventArgs(hitInfo, currentHealth == 0));
            }
        }
    }


    public override void RecieveHit(object[] senders, int hitID, HitInfo hitInfo)
    {
        if (attackArmor == AttackArmor.Strong) return;
        if (invincible) return;
        if (hitID == lastHitID) return;
        lastHitID = hitID;

        var damage = hitInfo.damage;

        // status effect
        if (hitInfo.effect != HitInfo.Effects.None)
        {
            damage = Mathf.CeilToInt(damage * statusEffectivenesses[(int)hitInfo.effect]);
            if (damage > 0)
            {
                Log(hitInfo.effect + ":" + (int)hitInfo.effect, Debugger.LogTypes.Combat);
                StopAllCoroutines();
                StartCoroutine(statusMethods[(int)hitInfo.effect], damage);
            }
        }

        ChangeHealth(-damage);
        CreateIndicator(damage);

        if (attackArmor == AttackArmor.None)
        {
            OnGroupHitEvent(senders, new HitEventArgs(hitInfo, currentHealth == 0));
        }
    }

    #endregion
}