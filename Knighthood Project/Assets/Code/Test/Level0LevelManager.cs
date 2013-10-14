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

        managerType = GetType();

        CreatePlayers();
    }

    #endregion

    #region LevelManager Overrides

    public override void RecieveTrigger(string method)
    {
        Log(method);
        Invoke(method, 0f);
    }

    #endregion

    #region Trigger Methods

    

    #endregion
}