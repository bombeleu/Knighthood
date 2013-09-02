// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Manager for the second splash screen.
/// </summary>
public class SplashGameManager : BaseMono
{
    public float time;


    #region MonoBehaviour Overrides

    private void Awake()
    {
        InvokeAction(() => Application.LoadLevel("Main Menu"), time);
    } // end Start


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
    } // end Update

    #endregion

} // end SplashGameManager class