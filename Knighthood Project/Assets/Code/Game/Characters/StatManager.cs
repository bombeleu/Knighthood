// Steve Yeager
// 8.22.2013

using System;
using System.Xml;

/// <summary>
/// Holds all stat info for a myCharacter.
/// </summary>
[Serializable]
public class StatManager
{
    #region Stat Fields

    public enum Stats
    {
        Health,
        PhysicalAttack,
        PhysicalDefense,
        MagicAttack,
        MagicDefense,
        MagicMax,
        AttackSpeed
    };

    public int health;// { get; private set; }
    public int physicalAttack = 1;// { get; private set; }
    public int physicalDefense = 1;// { get; private set; }
    public int magicAttack = 1;// { get; private set; }
    public int magicDefense = 1;// { get; private set; }
    public int magicMax;// { get; private set; }
    public int attackSpeed;// { get; private set; }

    #endregion


    #region Public Methods

    /// <summary>
    /// Load NPC stats.
    /// </summary>
    /// <param name="name">Name of the NPC</param>
    [Obsolete]
    public void LoadNPC(string name)
    {
        XmlDocument statSheet = new XmlDocument();
        statSheet.LoadXml(GameResources.Instance.NPCStats.text);
        XmlNode root = statSheet.SelectSingleNode("/Enemies/Enemy/" + name);
        foreach (XmlNode stat in root)
        {
            Debugger.Log(stat.Name + ": " + stat.InnerXml);
        }
    }


    /// <summary>
    /// Can change a stat but it will not be saved.
    /// </summary>
    /// <param name="stat">Stat to change.</param>
    /// <param name="amount">Amount to change stat by.</param>
    public void TempChangeStat(Stats stat, int amount)
    {
        switch (stat)
        {
            case Stats.Health:
                health += amount;
                break;
            case Stats.PhysicalAttack:
                physicalAttack += amount;
                break;
            case Stats.PhysicalDefense:
                physicalDefense += amount;
                break;
            case Stats.MagicAttack:
                magicAttack += amount;
                break;
            case Stats.MagicDefense:
                magicDefense += amount;
                break;
            case Stats.MagicMax:
                magicMax += amount;
                break;
            case Stats.AttackSpeed:
                attackSpeed += amount;
                break;
        }
    }

    #endregion
}