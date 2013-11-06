// Steve Yeager
// 8.22.2013

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Holds all stat info for a myCharacter.
/// </summary>
[Serializable]
public class StatManager
{
    #region Public Fields

    public enum Stats
    {
        AttackPhysical,
        AttackMagic,
        DefenseStoutness,
        DefenseShield,
        SpeedMovement,
        SpeedAgility,
        MagicPool,
        MagicRegen,
        AbilityStrength,
        AbilityRegen
    };

    public TextAsset statsFile;

    public double Attack { get { return (attackPhysical.value + attackMagic.value) / 2d; } }
    /// <summary>Strength of physical attacks.</summary>
    public Stat attackPhysical;
    /// <summary>Strength of magic attacks.</summary>
    public Stat attackMagic;

    public double Defense { get { return (defenseStoutness.value + defenseShield.value) / 2d; } }
    /// <summary>Used to counter attackPhysical and attackMagic.</summary>
    public Stat defenseStoutness;
    /// <summary>Shield max health.</summary>
    public Stat defenseShield;

    public double Speed { get { return (speedMovement.value + speedAgility.value) / 2d; } }
    /// <summary>Normal and shield speed.</summary>
    public Stat speedMovement;
    /// <summary>Attack speed.</summary>
    public Stat speedAgility;

    public double Magic { get { return (magicPool.value + magicRegen.value) / 2d; } }
    /// <summary>Amount of magic available.</summary>
    public Stat magicPool;
    /// <summary>How fast magic regenerates.</summary>
    public Stat magicRegen;

    public double Ability { get { return (abilityStrength.value + abilityRegen.value) / 2d; } }
    /// <summary>How strong special abilites are.</summary>
    public Stat abilityStrength;
    /// <summary>How fast the abilities regenerate.</summary>
    public Stat abilityRegen;

    #endregion

    #region Properties Fields

    public string user { get; private set; }
    
    #endregion

    #region Const Fields

    private const int ATTACKPHYSICALINITIALVALUE = 1;
    private const int ATTACKPHYSICALLEVELVALUE = 1;
    private const string ATTACKPHYSICALPATH = ": Attack Physical";
    private const int ATTACKMAGICINITIALVALUE = 1;
    private const int ATTACKMAGICLEVELVALUE = 1;
    private const string ATTACKMAGICPATH = ": Attack Magic";

    private const int DEFENSESTOUTNESSINITIALVALUE = 1;
    private const int DEFENSESTOUTNESSLEVELVALUE = 1;
    private const string DEFENSESTOUTNESSPATH = ": Defense Stoutness";
    private const int DEFENSESHIELDINITIALVALUE = 100;
    private const int DEFENSESHIELDLEVELVALUE = 2;
    private const string DEFENSESHIELDPATH = ": Defense Shield";

    private const int SPEEDMOVEMENTINITIALVALUE = 1;
    private const float SPEEDMOVEMENTLEVELVALUE = 0.1f;
    private const string SPEEDMOVEMENTPATH = ": Speed Movement";
    private const int SPEEDAGILITYINITIALVALUE = 1;
    private const int SPEEDAGILITYLEVELVALUE = 1;
    private const string SPEEDAGILITYPATH = ": Speed Agility";

    private const int MAGICPOOLINITIALVALUE = 100;
    private const int MAGICPOOLLEVELVALUE = 2; 
    private const string MAGICPOOLPATH = ": Magic Pool";
    private const int MAGICREGENINITIALVALUE = 1;
    private const float MAGICREGENLEVELVALUE = 0.1f;
    private const string MAGICREGENPATH = ": Magic Regen";

    private const int ABILITYSTRENGTHINITIALVALUE = 1;
    private const int ABILITYSTRENGTHLEVELVALUE = 1; 
    private const string ABILITYSTRENGTHPATH = ": Ability Strength";
    private const int ABILITYREGENPATHINITIALVALUE = 1;
    private const int ABILITYREGENPATHLEVELVALUE = 1; 
    private const string ABILITYREGENPATH = ": Ability Regen";

    #endregion


    #region Public Methods

    /// <summary>
    /// Set stats to initial.
    /// </summary>
    public void Initialize()
    {
        XmlDocument statSheet = new XmlDocument();
        statSheet.LoadXml(statsFile.text);
        XmlNode root = statSheet.SelectSingleNode("/stats");
        XmlNode cursor;

        cursor = root.SelectSingleNode("AttackPhysical");
        attackPhysical = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  ATTACKPHYSICALINITIALVALUE, ATTACKPHYSICALLEVELVALUE);
        cursor = root.SelectSingleNode("AttackMagic");
        attackMagic = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  ATTACKMAGICINITIALVALUE, ATTACKMAGICLEVELVALUE);

        cursor = root.SelectSingleNode("DefenseStoutness");
        defenseStoutness = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  DEFENSESTOUTNESSINITIALVALUE, DEFENSESTOUTNESSLEVELVALUE);
        cursor = root.SelectSingleNode("DefenseShield");
        defenseShield = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  DEFENSESHIELDINITIALVALUE, DEFENSESHIELDLEVELVALUE);

        cursor = root.SelectSingleNode("SpeedMovement");
        speedMovement = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  SPEEDMOVEMENTINITIALVALUE, SPEEDMOVEMENTLEVELVALUE);
        cursor = root.SelectSingleNode("SpeedAgility");
        speedAgility = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  SPEEDAGILITYINITIALVALUE, SPEEDAGILITYLEVELVALUE);

        cursor = root.SelectSingleNode("MagicPool");
        magicPool = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  MAGICPOOLINITIALVALUE, MAGICPOOLLEVELVALUE);
        cursor = root.SelectSingleNode("MagicRegen");
        magicRegen = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  MAGICREGENINITIALVALUE, MAGICREGENLEVELVALUE);

        cursor = root.SelectSingleNode("AbilityStrength");
        abilityStrength = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  ABILITYSTRENGTHINITIALVALUE, ABILITYSTRENGTHLEVELVALUE);
        cursor = root.SelectSingleNode("AbilityRegen");
        abilityRegen = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                  int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                  ABILITYREGENPATHINITIALVALUE, ABILITYREGENPATHLEVELVALUE);
    }


    /// <summary>
    /// Get stats from save data if applicable or set to initial.
    /// </summary>
    /// <param name="user">Player username.</param>
    public void Initialize(string user)
    {
        this.user = user;

        // no data saved
        if (PlayerPrefs.GetInt(user + ATTACKPHYSICALPATH, -1) == -1)
        {
            Initialize();
        }
        // found saved data
        else
        {
            XmlDocument statSheet = new XmlDocument();
            statSheet.LoadXml(statsFile.text);
            XmlNode root = statSheet.SelectSingleNode("/stats");
            XmlNode cursor;

            cursor = root.SelectSingleNode("AttackPhysical");
            attackPhysical = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      ATTACKPHYSICALINITIALVALUE, ATTACKPHYSICALLEVELVALUE,
                                      PlayerPrefs.GetInt(user + ATTACKPHYSICALPATH));
            cursor = root.SelectSingleNode("AttackMagic");
            attackMagic = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      ATTACKMAGICINITIALVALUE, ATTACKMAGICLEVELVALUE,
                                      PlayerPrefs.GetInt(user + ATTACKMAGICPATH));

            cursor = root.SelectSingleNode("DefenseStoutness");
            defenseStoutness = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      DEFENSESTOUTNESSINITIALVALUE, DEFENSESTOUTNESSLEVELVALUE,
                                      PlayerPrefs.GetInt(user + DEFENSESTOUTNESSPATH));
            cursor = root.SelectSingleNode("DefenseShield");
            defenseShield = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      DEFENSESHIELDINITIALVALUE, DEFENSESHIELDLEVELVALUE,
                                      PlayerPrefs.GetInt(user + DEFENSESHIELDPATH));

            cursor = root.SelectSingleNode("SpeedMovement");
            speedMovement = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      SPEEDMOVEMENTINITIALVALUE, SPEEDMOVEMENTLEVELVALUE,
                                      PlayerPrefs.GetInt(user + SPEEDMOVEMENTPATH));
            cursor = root.SelectSingleNode("SpeedAgility");
            speedAgility = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      SPEEDAGILITYINITIALVALUE, SPEEDAGILITYLEVELVALUE,
                                      PlayerPrefs.GetInt(user + SPEEDAGILITYPATH));

            cursor = root.SelectSingleNode("MagicPool");
            magicPool = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      MAGICPOOLINITIALVALUE, MAGICPOOLLEVELVALUE,
                                      PlayerPrefs.GetInt(user + MAGICPOOLPATH));
            cursor = root.SelectSingleNode("MagicRegen");
            magicRegen = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      MAGICREGENINITIALVALUE, MAGICREGENLEVELVALUE,
                                      PlayerPrefs.GetInt(user + MAGICREGENPATH));

            cursor = root.SelectSingleNode("AbilityStrength");
            abilityStrength = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      ABILITYSTRENGTHINITIALVALUE, ABILITYSTRENGTHLEVELVALUE,
                                      PlayerPrefs.GetInt(user + ABILITYSTRENGTHPATH));
            cursor = root.SelectSingleNode("AbilityRegen");
            abilityRegen = new Stat(int.Parse(cursor.SelectSingleNode("initialStat").InnerXml),
                                      int.Parse(cursor.SelectSingleNode("finalStat").InnerXml),
                                      ABILITYREGENPATHINITIALVALUE, ABILITYREGENPATHLEVELVALUE,
                                      PlayerPrefs.GetInt(user + ABILITYREGENPATH));
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
            case Stats.AttackPhysical:
                attackPhysical.ChangeLevel(amount);
                break;
            case Stats.AttackMagic:
                attackMagic.ChangeLevel(amount);
                break;
            case Stats.DefenseStoutness:
                defenseStoutness.ChangeLevel(amount);
                break;
            case Stats.DefenseShield:
                defenseShield.ChangeLevel(amount);
                break;
            case Stats.SpeedMovement:
                speedMovement.ChangeLevel(amount);
                break;
            case Stats.SpeedAgility:
                speedAgility.ChangeLevel(amount);
                break;
            case Stats.MagicPool:
                magicPool.ChangeLevel(amount);
                break;
            case Stats.MagicRegen:
                magicRegen.ChangeLevel(amount);
                break;
            case Stats.AbilityStrength:
                abilityStrength.ChangeLevel(amount);
                break;
            case Stats.AbilityRegen:
                abilityRegen.ChangeLevel(amount);
                break;
        }
    }

    #endregion
}