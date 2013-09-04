// Steve Yeager
// 8.21.2013

using UnityEngine;

/// <summary>
/// Base class for all attacks.
/// </summary>
public abstract class AttackSystem : BaseMono
{
    protected Transform myTransform;
    protected Character character;
    protected bool attacking = false;


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        myTransform = transform;
        character = GetSafeComponent<Character>();
    } // end Start

    #endregion

    #region Abstract Methods

    public abstract bool Initiate();

    public abstract void Attack();

    #endregion

    #region Protected Methods

    /// <summary>
    /// Clean up the current attack. Set character state to idle.
    /// </summary>
    protected void EndAttack()
    {
        character.EndAttack();
    } // end EndAttack

    #endregion

} // end AttackSystem class