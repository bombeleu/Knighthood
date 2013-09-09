// Steve Yeager
// 8.22.2013

using UnityEngine;
using System.Collections;
using System.Xml;

/// <summary>
/// Holds all stat info for a myCharacter.
/// </summary>
public class StatManager
{
  #region Stat Fields

  public readonly int health;
  public readonly int physicalAttack;
  public readonly int physicalDefense;
  public readonly int magicAttack;
  public readonly int magicDefense;
  public readonly int magicMax;
  public readonly int attackSpeed;

  #endregion


  #region Data Methods

  //
  public void LoadNPC(string name)
  {
    XmlDocument statSheet = new XmlDocument();
    statSheet.LoadXml(GameResources.Instance.NPCStats.text);
    XmlNode root = statSheet.SelectSingleNode("/Enemies/Enemy/"+name);
    foreach (XmlNode stat in root)
    {
      Debugger.Log(stat.Name + ": " + stat.InnerXml);
    }
  } // end LoadNPC

  #endregion

} // end StatManager class