// Steve Yeager
// 9.1.2013

using UnityEngine;

/// <summary>
/// Base class for all attacks.
/// </summary>
public abstract class Attack : BaseMono
{
    public string attackInput;
    public string attackName;
    protected CombatManager manager;
    protected CharacterHealth myHealth;

    public float windUp;
    public float attackTime;
    public float windDown;
    public float cooldown;
    protected bool canActivate = true;
    public CharacterHealth.AttackArmor attackArmor;
    protected Job attackJob;


    #region Abstract Methods

    /// <summary>
    /// Can the attackValue be activated?
    /// </summary>
    /// <returns>True, if the attackValue can be activated at this time.</returns>
    public abstract bool CanActivate();

    /// <summary>
    /// Start the attackValue.
    /// </summary>
    /// <returns>True, if the attackValue was successfully started.</returns>
    public abstract Texture Activate();


    /// <summary>
    /// 
    /// </summary>
    public abstract void Cancel();

    #endregion

    #region Public Methods

    /// <summary>
    /// Get attackValue ready for combat for specific character.
    /// </summary>
    /// <param name="manager"></param>
    public void Initialize(CombatManager manager)
    {
        this.manager = manager;
        myHealth = manager.GetComponent<CharacterHealth>();
    }

    #endregion
}