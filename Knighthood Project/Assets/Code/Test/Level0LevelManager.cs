// Steve Yeager
// 10.6.2013

/// <summary>
/// 
/// </summary>
public class Level0LevelManager : LevelManager
{
    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        CreatePlayers();
    }

    #endregion
}