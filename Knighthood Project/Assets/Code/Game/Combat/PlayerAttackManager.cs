// Steve Yeager
// 9.2.2013

/// <summary>
/// Manages player attacks.
/// </summary>
public class PlayerAttackManager : BaseMono
{
    public Attack[] attacks = new Attack[7];
    public enum AttackTypes
    {
        None = -1,
        Light = 0,
        Heavy = 1,
        Stun = 2,
        SuperLeft = 3,
        SuperHeavy = 4,
        SuperStun = 5,
        SuperJump = 6,
    }



    /// <summary>
    /// Activate the correct Attack.
    /// </summary>
    /// <param name="attack">Attack to activate.</param>
    /// <returns>True, if Attack is not null and Attack successfully activates.</returns>
    public bool Activate(AttackTypes attack)
    {
        Log(attack);
        return attacks[(int)attack] != null && attacks[(int)attack].Activate();
    } // end Activate
	
} // end PlayerAttackManager class