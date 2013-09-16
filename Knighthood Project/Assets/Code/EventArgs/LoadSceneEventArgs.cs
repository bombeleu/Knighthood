// Steve Yeager
// 9.6.2013

using System;

/// <summary>
/// Event args for the game loading a scene.
/// </summary>
public class LoadSceneEventArgs : EventArgs
{
    public readonly string previousScene;
    public readonly string nextScene;


    /// <summary>
    /// Args for LoadEvent.
    /// </summary>
    /// <param name="previousScene">Scene leaving.</param>
    /// <param name="nextScene">Scene entering.</param>
    public LoadSceneEventArgs(string previousScene, string nextScene)
    {
        this.previousScene = previousScene;
        this.nextScene = nextScene;
    }
}