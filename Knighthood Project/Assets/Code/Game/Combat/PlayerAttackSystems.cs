// Steve Yeager
// 8.21.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class PlayerAttackSystems : BaseMono
{
  public enum Attacks
  {
    LightNormal = 0, LightSide = 1, LightUp = 2, LightDown = 3,
    HeavyNormal = 4, HeavySide = 5, HeavyUp = 6, HeavyDown = 7,
    RangedNormal = 8, RangedSide = 9, RangedUp = 10, RangedDown = 11,
    MagicLeft = 12, MagicUp = 13, MagicRight = 14, MagicDown = 15,
    None
  }
  public AttackSystem[] attackSystems = new AttackSystem[16];


  public bool Initiate(Attacks attack)
  {
    Log(attack);
    return attackSystems[(int)attack].Initiate();
  } // end Initiate

} // end PlayerAttackSystems class