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


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        player = GetComponent<Player>();
        health = GetComponent<Health>();
        magic = GetComponent<Magic>();
    }


    private void OnGUI()
    {
        // temp
        GUILayout.Label(player.playerInfo.username);
        GUILayout.Label("Health: " + health.currentHealth);
        GUILayout.Label("Magic: " + magic.currentMagic);
    }

    #endregion

}