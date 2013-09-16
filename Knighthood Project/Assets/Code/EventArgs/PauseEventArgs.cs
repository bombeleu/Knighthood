// Steve Yeager
// 8.18.2013

using System;

/// <summary>
/// Event args for the game pausing/unpausing.
/// </summary>
public class PauseEventArgs : EventArgs
{
    /// <summary>Is the game paused?</summary>
    public readonly bool paused;
    /// <summary>Player that paused/unpaused the game.</summary>
    public readonly int playerNumber;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="paused">Is the game paused?</param>
    /// <param name="playerNumber">Player that paused/unpaused the game.</param>
    public PauseEventArgs(bool paused, int playerNumber)
    {
        this.paused = paused;
        this.playerNumber = playerNumber;
    }
}