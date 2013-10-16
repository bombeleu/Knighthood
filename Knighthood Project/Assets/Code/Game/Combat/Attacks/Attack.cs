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

    public float windUp;
    public float attackTime;
    public float windDown;
    public float cooldown;
    protected bool canActivate = true;


    #region Abstract Methods

    /// <summary>
    /// Can the attack be activated?
    /// </summary>
    /// <returns>True, if the attack can be activated at this time.</returns>
    public abstract bool CanActivate();

    /// <summary>
    /// Start the attack.
    /// </summary>
    /// <returns>True, if the attack was successfully started.</returns>
    public abstract Texture Activate();

    #endregion


    #region Public Methods

    /// <summary>
    /// Get attack ready for combat for specific character.
    /// </summary>
    /// <param name="manager"></param>
    public void Initialize(CombatManager manager)
    {
        this.manager = manager;
    }


    public HitInfo TransformKnockBack()
    {
        return new HitInfo();
    }

    #endregion
}