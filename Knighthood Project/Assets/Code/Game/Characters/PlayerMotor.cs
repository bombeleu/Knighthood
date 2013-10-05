// Steve Yeager
// 10.4.2013

using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerMotor : CharacterMotor
{
    #region CharacterMotor Overrides

    public override void SetVelocity(Vector3 velocity)
    {
        velocity.x = ClampToCamera(velocity.x);
        base.SetVelocity(velocity);
    }


    public override void SetVelocity(float x, float y)
    {
        ClampToCamera(x);
        base.SetVelocity(x, y);
    }


    public override void SetVelocityX(float x)
    {
        ClampToCamera(x);
        base.SetVelocityX(x);
    }


    public override void AddVelocity(float x, float y)
    {
        ClampToCamera(x);
        base.AddVelocity(x, y);
    }


    public override void AddVelocityX(float x)
    {
        ClampToCamera(x);
        base.AddVelocityX(x);
    }


    public override void MoveX(float input)
    {
        input = Mathf.Clamp(input, -1f, 1f);
        velocity = new Vector3(ClampToCamera(moveSpeed*input), velocity.y, 0f);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float ClampToCamera(float x)
    {
        float screenX = LevelManager.Instance.camera.WorldToScreenPoint(myTransform.position).x / Screen.width;

        // screen right
        if (x > 0 && screenX >= (1 - LevelCamera.Instance.boundaryHorizontalOuter))
        {
            x = 0f;
        }
        // screen left
        else if (x < 0 && screenX <= LevelCamera.Instance.boundaryHorizontalOuter)
        {
            x = 0f;
        }

        return x;
    }

    #endregion
}