// Steve Yeager
// 9.5.2013

using UnityEngine;

/// <summary>
/// HUD for player.
/// </summary>
public class PlayerHUD : BaseMono
{
    #region Private Fields

    private Player player;
    private Health health;
    private Magic magic;
    //private StatManager statManager;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        player = GetComponent<Player>();
        health = GetComponent<Health>();
        magic = GetComponent<Magic>();
        //statManager = player.myStats;
    }


    private void OnGUI()
    {
        // temp
        GUI.BeginGroup(new Rect(player.playerInfo.playerNumber * 200, 10, 200, 500));
        {
            GUI.Label(new Rect(0, 0, 200, 100), player.playerInfo.username);
            GUI.Label(new Rect(0, 40, 200, 100), "Health: " + health.currentHealth);
            GUI.Label(new Rect(0, 80, 200, 100), "Magic: " + magic.currentMagic);
            GUI.Label(new Rect(0, 120, 200, 100), "Shield: " + player.shieldHealth);

            //GUILayout.Space(10f);
            //GUILayout.BeginVertical();
            //{
            //    GUILayout.Label("HMax: " + statManager.health);
            //    GUILayout.Label("PAttack: " + statManager.physicalAttack);
            //    GUILayout.Label("PDefense: " + statManager.physicalDefense);
            //    GUILayout.Label("MAttack: " + statManager.magicAttack);
            //    GUILayout.Label("MDefense: " + statManager.magicDefense);
            //    GUILayout.Label("MMax: " + statManager.magicMax);
            //    GUILayout.Label("ASpeed: " + statManager.attackSpeed);
            //}
            //GUILayout.EndVertical();
        }
        GUI.EndGroup();
    }

    #endregion
}