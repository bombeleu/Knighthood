// Steve Yeager
// 9.1.2013

using UnityEngine;

/// <summary>
/// Manager for the second splash screen.
/// </summary>
public class SplashGameManager : BaseMono
{
    #region Public Fields

    public float time;

    #endregion

    #region MonoBehaviour Overrides

    private void Awake()
    {
        InvokeAction(() => Application.LoadLevel("Main Menu"), time);
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Application.LoadLevel("Main Menu");
        }

        for (int i = 1; i <= Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetButtonUp("Start_" + i))
            {
                Application.LoadLevel("Main Menu");
            }
        }
    }

    #endregion

}