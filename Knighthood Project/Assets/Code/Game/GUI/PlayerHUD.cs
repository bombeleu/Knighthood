// Steve Yeager
// 9.5.2013

using UnityEngine;

/// <summary>
/// HUD for player.
/// </summary>
public class PlayerHUD : BaseMono
{
    private Player player;
    private Health health;
    private Magic magic;
    private StatManager statManager;


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        player = GetComponent<Player>();
        health = GetComponent<Health>();
        magic = GetComponent<Magic>();
        statManager = player.StatManager;
    }


    private void OnGUI()
    {
        // temp
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label(player.playerInfo.username);
                GUILayout.Label("Health: " + health.currentHealth);
                GUILayout.Label("Magic: " + magic.currentMagic);
            }
            GUILayout.EndVertical();
            GUILayout.Space(10f);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("HMax: " + statManager.health);
                GUILayout.Label("PAttack: " + statManager.physicalAttack);
                GUILayout.Label("PDefense: " + statManager.physicalDefense);
                GUILayout.Label("MAttack: " + statManager.magicAttack);
                GUILayout.Label("MDefense: " + statManager.magicDefense);
                GUILayout.Label("MMax: " + statManager.magicMax);
                GUILayout.Label("ASpeed: " + statManager.attackSpeed);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    #endregion

}