// Steve Yeager
// 8.18.2013

/// <summary>
/// Holds user player info.
/// </summary>
public struct PlayerInfo
{
  /// <summary>Player's username.</summary>
  public readonly string username;
  /// <summary>Player's number. 1 based.</summary>
  public readonly int playerNumber;
  /// <summary>Player's zero based number. Always 1 less than playerNumber.</summary>
  public readonly int zeroNumber;


  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="username">Player's username.</param>
  /// <param name="playerNumber">Player's number. 0 based.</param>
  public PlayerInfo(string username, int zeroNumber)
  {
    this.username = username;
    this.zeroNumber = zeroNumber;
    playerNumber = zeroNumber + 1;
  }
}