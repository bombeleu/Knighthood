// Steve Yeager
// 9.15.2013

using UnityEngine;

/// <summary>
/// Manager for the Level Selection Menu.
/// </summary>
public class LevelSelectMenuManager : BaseMono
{
    #region MonoBehaviour Overrides

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 40, 200, 40), "Test Room"))
        {
            GameData.Instance.LoadScene("Test Room", true);
        }
    }

    #endregion
}