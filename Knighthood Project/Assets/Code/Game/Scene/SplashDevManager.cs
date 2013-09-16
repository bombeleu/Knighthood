// Steve Yeager
// 9.1.2013

using UnityEngine;

/// <summary>
/// Manager for the first splash screen.
/// </summary>
public class SplashDevManager : BaseMono
{
    #region Public Fields

    public float time;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        InvokeAction(() => Application.LoadLevel("Splash Game"), time);
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Application.LoadLevel("Splash Game");
        }

        for (int i = 1; i <= Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetButtonUp("Start_" + i))
            {
                Application.LoadLevel("Splash Game");
            }
        }
    }

    #endregion
}