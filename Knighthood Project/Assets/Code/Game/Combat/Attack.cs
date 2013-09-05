// Steve Yeager
// 9.1.2013

using UnityEngine;

/// <summary>
/// Base class for all attacks.
/// </summary>
public abstract class Attack : BaseMono
{
    #region Protected Fields

    protected Transform myTransform;
    protected Character character;

    #endregion

    #region Public Fields

    public PlayerAttackManager.AttackTypes attack;
    public HitInfo hitInfo;
    public float hitboxTime;
    public Vector2 offset;
    public float windup;
    public float attackTime;
    public float cooldown;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform.parent;
        character = myTransform.GetComponent<Character>();
    } // end Start

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Start the attack if possible.
    /// </summary>
    /// <returns>True, if the attack was successfully started.</returns>
    public abstract bool Activate();

    #endregion

    #region Protected Methods

    /// <summary>
    /// End the current attack.
    /// </summary>
    protected void EndAttack()
    {
        character.EndAttack();
    } // end EndAttack

    #endregion

} // end Attack class