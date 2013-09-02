// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the first splash screen.
/// </summary>
public class SplashDevManager : BaseMono
{
    public float time;


    #region MonoBehaviour Overrides

    private void Awake()
    {
        InvokeAction(() => Application.LoadLevel("Splash Game"), time);
    } // end Awake


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
    } // end Update

    #endregion

} // end SplashDevManager class