// Steve Yeager
// 9.15.2013

using UnityEngine;

/// <summary>
/// Manager for the Level Selection Menu.
/// </summary>
public class LevelSelectMenuManager : BaseMono
{
    #region Private Fields

    private readonly string[] levels =
    {
        "Combat Arena",
        "Test Room",
        "Level 0"
    };
    private int levelCursor;

    #endregion

    #region MonoBehaviour Overrides

    private void Update()
    {
        // move cursor
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (levelCursor < levels.Length-1) levelCursor++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (levelCursor > 0) levelCursor--;
        }
    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 40, 200, 40), levels[levelCursor]))
        {
            GameData.Instance.LoadScene(levels[levelCursor], true);
        }
    }

    #endregion
}