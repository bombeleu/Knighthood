// Steve Yeager
// 10.20.2013

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Ultimate 0: Chad
/// </summary>
public class IceSpear : UltimateAttack
{
    #region Public Fields

    public GameObject IceSpear_Prefab;
    public Vector2 offset;
    public HitInfo hitInfo;

    #endregion


    #region UltimateAttack Overrides

    public override void Activate()
    {
        var spear = (GameObject)Instantiate(IceSpear_Prefab, UltimateAttacks.attackPivot.position + UltimateAttacks.participants[0].transform.TransformDirection(0f, offset.y, offset.x), UltimateAttacks.participants[0].transform.rotation);
        spear.transform.Align();
        spear.GetComponent<Hitbox>().Initialize(UltimateAttacks.participants[0].GetComponent<Player>(), hitInfo, attackTime, 1);
    }

    #endregion
}