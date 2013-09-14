// Steve Yeager
// 8.25.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Health for characters.
/// </summary>
public class CharacterHealth : Health
{
    private StatManager statManager;


    private void Awake()
    {
        statManager = GetSafeComponent<Character>().StatManager;
    }


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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="hitID"></param>
    /// <param name="hitInfo"></param>
    public override void RecieveHit(object sender, int hitID, HitInfo hitInfo)
    {
        hitInfo.FactorDefendStats(statManager);
        base.RecieveHit(sender, hitID, hitInfo);
    }

}