// Steve Yeager
// 8.25.2013

/// <summary>
/// Health for characters.
/// </summary>
public class CharacterHealth : Health
{
    #region Reference Fields

    private StatManager statManager;

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
        base.RecieveHit(sender, hitID, hitInfo.LocalizeDamage(statManager));
    }

    #endregion
}