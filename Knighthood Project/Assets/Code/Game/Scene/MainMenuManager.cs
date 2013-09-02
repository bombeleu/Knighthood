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

            }

            // options
            if (GUILayout.Button("Options"))
            {

            }

            // quit
            if (GUILayout.Button("Quit"))
            {
                Application.Quit();
            }
        }
        GUILayout.EndArea();
    } // end OnGUI

    #endregion

} // end MainMenuManager class