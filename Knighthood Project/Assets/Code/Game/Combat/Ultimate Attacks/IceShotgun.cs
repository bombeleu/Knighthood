// Steve Yeager
// 10.22.2013

using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Ultimate 4: Harold, Chad
/// </summary>
public class IceShotgun : UltimateAttack
{
    #region Public Fields

    public GameObject Wall_Prefab;
    public GameObject Shard_Prefab;
    public int shardCount;
    public float maxYRot;
    public float minYRot;
    public float shardSpeed;
    public float shardTime;
    public HitInfo hitInfo;

    public float wallTime;
    public float punchTime;
    public float wallCenter;

    public float HaroldX;
    public Texture HaroldPunchAnim;
    public float ChadX;
    public Texture ChadConjureAnim;

    #endregion


    #region UltimateAttack Overrides

    public override void Activate()
    {
        StartCoroutine(Attack());
    }

    #endregion

    #region Private Methods

    private IEnumerator Attack()
    {
        Vector3 startPosition = UltimateAttacks.attackPivot.position;
        Quaternion startRotation = UltimateAttacks.attackPivot.rotation;

        // characters
        UltimateAttacks.players[0].transform.position = startPosition + UltimateAttacks.attackPivot.forward * ChadX;
        UltimateAttacks.players[0].PlayAnimation(ChadConjureAnim);
        UltimateAttacks.players[2].transform.position = startPosition + UltimateAttacks.attackPivot.forward * HaroldX;
        UltimateAttacks.players[2].PlayAnimation(HaroldPunchAnim);

        yield return WaitForTime(wallTime);
        var Wall = (GameObject)Instantiate(Wall_Prefab, startPosition + Vector3.up*wallCenter, startRotation);
        yield return WaitForTime(punchTime);

        GameObject shard;
        for (int i = 0; i < shardCount; i++)
        {
            shard = (GameObject)Instantiate(Shard_Prefab, startPosition + Vector3.up * wallCenter, startRotation * Quaternion.Euler(Random.Range(minYRot, maxYRot), 0f, 0f));
            shard.transform.Align();
            shard.GetComponent<GroupHitbox>().Initialize(UltimateAttacks.participants, hitInfo.Attack(UltimateAttacks.participants.Select(p => p.myStats.abilityStrength.value)), shardTime, 1, shard.transform.forward*shardSpeed, true);
        }
        Destroy(Wall);
    }

    #endregion
}