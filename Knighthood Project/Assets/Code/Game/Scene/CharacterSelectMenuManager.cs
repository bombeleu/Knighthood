// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the Character Selection Menu.
/// </summary>
public class CharacterSelectMenuManager : BaseMono
{
    #region MonoBehaviour Overrides

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width/4, Screen.height/3, Screen.width/2, Screen.height/3));
        {
            GUILayout.BeginHorizontal();
            {
                for (int i = 0; i < GameResources.Instance.Player_Prefabs.Length; i++)
                {
                    if (GUILayout.Button(GameResources.Instance.Player_Prefabs[i].name, GUILayout.Height(Screen.height/3)))
                    {
                        GameData.Instance.playerCharacters.Add(i);
                        GameData.Instance.LoadScene("Level Select Menu", true);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    #endregion
}