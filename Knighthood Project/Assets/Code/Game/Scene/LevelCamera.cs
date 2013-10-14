// Steve Yeager
// 8.20.2013

using UnityEngine;

/// <summary>
/// Base class for cameras in Levels.
/// </summary>
public class LevelCamera : Singleton<LevelCamera>
{
    #region Reference Fields

    private Camera myCamera;
    private Transform myTransform;

    #endregion

    #region Status Fields

    public bool locked;
    public float boundaryHorizontalOuter = 0.05f;
    public float boundaryHorizontalInner = 0.25f;
    public float boundaryVerticalOuter;
    public float boundaryVerticalInner;

    #endregion

    #region Movement Fields

    public float baseSpeed = 8f;
    public float distMin = 20f;
    public float distMax = 40f;

    #endregion



    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myCamera = camera;
        myTransform = transform;
    }


    private void Update()
    {
        if (locked) return;

        float speed = 0f;
        bool moveLeft = false, moveRight = false;
        float minX = 0f, maxX = 0f;

        foreach (Transform player in LevelManager.Instance.PlayerTransforms)
        {
            float screenXPercent = myCamera.WorldToScreenPoint(player.position).x / Screen.width;

            // player on left
            if (screenXPercent <= boundaryHorizontalInner)
            {
                moveLeft = true;
                if (speed == 0f || speed < Mathf.Abs(player.rigidbody.velocity.x))
                {
                    speed = Mathf.Abs(player.rigidbody.velocity.x);
                }
            }
            // player on right
            else if (screenXPercent >= (1f - boundaryHorizontalInner))
            {
                moveRight = true;
                if (speed == 0f || speed < Mathf.Abs(player.rigidbody.velocity.x))
                {
                    speed = Mathf.Abs(player.rigidbody.velocity.x);
                }
            }

            // farthest left
            if (minX == 0 || minX > player.position.x)
            {
                minX = player.position.x;
            }
            // farthest left
            if (maxX == 0 || maxX < player.position.x)
            {
                maxX = player.position.x;
            }
        }

        // move left
        if (moveLeft && !moveRight)
        {
            MoveHorizontal(speed, false);
        }
        // move right
        else if (moveRight && !moveLeft)
        {

            MoveHorizontal(speed, true);
        }
    }


    #endregion

    #region Movement Methods

    /// <summary>
    /// MoveX the camera left/right to keep players on screen.
    /// </summary>
    /// <param name="speed">Speed to move camera.</param>
    /// <param name="moveRight">MoveX camera to the right?</param>
    private void MoveHorizontal(float speed, bool moveRight)
    {
        if (speed == 0) speed = baseSpeed;
        myTransform.position += (moveRight ? Vector3.right : Vector3.left) * speed * Time.fixedDeltaTime;

    }

    #endregion

    #region Debug Methods

    private void DrawLines()
    {
        // outside
        Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalOuter, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalOuter, Screen.height, 10f)), Color.red);
        Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalOuter, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalOuter, Screen.height, 10f)), Color.red);

        // inside
        Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalInner, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalInner, Screen.height, 10f)), Color.blue);
        Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalInner, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalInner, Screen.height, 10f)), Color.blue);
    }

    #endregion
}