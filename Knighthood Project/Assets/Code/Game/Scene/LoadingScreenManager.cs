// Steve Yeager
// 9.1.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Loads the next scene and displays loading images.
/// </summary>
public class LoadingScreenManager : BaseMono
{
    #region Public Fields

    public TextAsset tips;
    public GUIText tip;
    public GUIText nextScene;
    public float loadTime;

    #endregion


    #region MonoBehaviour Overrides

    private void Start()
    {
        string[] tipsLines = tips.text.Split('\n');
        tip.text = tipsLines[Random.Range(0, tipsLines.Length)];
        nextScene.text = GameData.Instance.NextScene;

        InvokeAction(() => Application.LoadLevel(GameData.Instance.NextScene), loadTime);
    } // end Start

    #endregion

} // end LoadingScreenManager class