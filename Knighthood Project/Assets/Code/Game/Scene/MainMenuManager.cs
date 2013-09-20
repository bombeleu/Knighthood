// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for Main Menu.
/// </summary>
public class MainMenuManager : BaseMono
{
    #region MonoBehaviour Overrides

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width/2-100, Screen.height/3, 200, Screen.height/3));
        {
            // play
            if (GUILayout.Button("Play"))
            {
                GameData.Instance.LoadScene("Character Select Menu");
            }

            // options
            if (GUILayout.Button("Options"))
            {
                GameData.Instance.LoadScene("Options Menu");
            }

            // quit
            if (GUILayout.Button("Quit"))
            {
                if (Application.isEditor)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
                else
                {
                    Application.Quit();
                }
            }
        }
        GUILayout.EndArea();
    }

    #endregion
}