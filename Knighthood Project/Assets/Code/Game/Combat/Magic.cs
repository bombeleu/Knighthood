// Steve Yeager
// 8.25.2013

using UnityEngine;
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
  } // end Initialize


  public void Initialize(StatManager stats)
  {
    maxMagic = stats.magicMax;
    currentMagic = maxMagic;
  } // end Initialize


  /// <summary>
  /// 
  /// </summary>
  /// <param name="amount"></param>
  /// <returns></returns>
  public bool CastMagic(int amount)
  {
    if (amount > currentMagic)
    {
      return false;
    }
    else
    {
      currentMagic -= amount;
      StartCoroutine("RegenerateMagic");
      return true;
    }
  } // end CastMagic


  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  private IEnumerator RegenerateMagic()
  {
    WaitForSeconds interval = new WaitForSeconds(regenInterval);

    while (currentMagic < maxMagic)
    {
      yield return interval;
      currentMagic++;
    }
  } // end RegenerateMagic

} // end Magic class