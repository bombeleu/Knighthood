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
        statManager = GetSafeComponent<Character>().StatManager;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 
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
        hitInfo.FactorDefendStats(statManager);
        base.RecieveHit(sender, hitID, hitInfo);
    }

    #endregion
}