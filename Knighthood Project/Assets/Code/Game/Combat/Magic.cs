// Steve Yeager
// 8.25.2013

using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Magic : BaseMono
{
    public int maxMagic;
    public int currentMagic;
    public float regenInterval;
    private bool regenerating;


    public void Initialize()
    {
        currentMagic = maxMagic;
    }


    public void Initialize(StatManager stats)
    {
        maxMagic = stats.magicMax;
        currentMagic = maxMagic;
    }


    /// <summary>
    /// Use magic. Start magic regen.
    /// </summary>
    /// <param name="amount">Amount of magic needed.</param>
    /// <returns>True, if had enough magic.</returns>
    public bool CastMagic(int amount)
    {
        if (amount > currentMagic) return false;
        Debug.Log("Cast");
        currentMagic -= amount;
        StopCoroutine("RegenerateMagic");
        StartCoroutine("RegenerateMagic");
        return true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool EnoughMagic(int amount)
    {
        return amount <= currentMagic;
    }


    /// <summary>
    /// Regenerates magic overtime.
    /// </summary>
    private IEnumerator RegenerateMagic()
    {
        if (regenerating) yield break;
        regenerating = true;
        while (currentMagic < maxMagic)
        {
            
            yield return WaitForTime(regenInterval);
            currentMagic++;
        }
        currentMagic = maxMagic;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeMagic(int amount)
    {
        currentMagic = Mathf.Clamp(currentMagic + amount, 0, maxMagic);
    }

}