// Steve Yeager
// 8.25.2013

using System.Collections;

/// <summary>
/// 
/// </summary>
public class Magic : BaseMono
{
    public int maxMagic;
    public int currentMagic;
    public float regenInterval;


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
        while (currentMagic < maxMagic)
        {
            yield return WaitForTime(regenInterval);
            currentMagic++;
        }
        currentMagic = maxMagic;
    }

}