// Steve Yeager
// 8.20.2013

using UnityEngine;
using System.Collections;

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
  public float zoomMin = 11f;
  public float zoomMax = 15f;
  private float zoomDiff;
  public float distMin = 20f;
  public float distMax = 40f;
  private float distDiff;
  public float zoomOutSpeed = 20f;
  public float zoomInSpeed = 1f;
  private bool canZoomIn = false;
  private bool zoomInTimer = false;
  public float zoomInBuffer;

  #endregion



  #region MonoBehaviour Overrides

  private void Awake()
  {
    // get references
    myCamera = camera;
    myTransform = transform;

    // set up
    zoomDiff = zoomMax - zoomMin;
    distDiff = distMax - distMin;
  } // end Start


  private void Update()
  {
    float speed = 0f;
    bool moveLeft = false, moveRight = false;
    float minX = 0f, maxX = 0f;

    foreach (Transform player in LevelManager.Instance.playerTransforms)
    {
      float screenXPercent = myCamera.WorldToScreenPoint(player.position).x / Screen.width;

      // player on left
      if (screenXPercent <= boundaryHorizontalInner)
      {
        moveLeft = true;
        if (speed == 0 || speed < Mathf.Abs(player.rigidbody.velocity.x))
        {
          speed = Mathf.Abs(player.rigidbody.velocity.x);
        }
      }
      // player on right
      else if (screenXPercent >= (1f - boundaryHorizontalInner))
      {
        moveRight = true;
        if (speed == 0 || speed < Mathf.Abs(player.rigidbody.velocity.x))
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
    else
    {
      // zoom
      //Zoom(maxX - minX);
    }
  } // end LateUpdate


  #endregion

  #region Movement Methods

  /// <summary>
  /// Move the camera left/right to keep players on screen.
  /// </summary>
  /// <param name="speed">Speed to move camera.</param>
  /// <param name="moveRight">Move camera to the right?</param>
  private void MoveHorizontal(float speed, bool moveRight)
  {
    if (speed == 0) speed = baseSpeed;
    Log(speed);
    myTransform.position += (moveRight ? Vector3.right : Vector3.left) * speed * Time.fixedDeltaTime;
    //myTransform.Translate((moveRight ? Vector3.right : Vector3.left) * speed * Time.deltaTime);

  } // end MoveHorizontal


  /// <summary>
  /// 
  /// </summary>
  /// <param name="dist"></param>
  private void Zoom(float dist)
  {
    dist = Mathf.Clamp(dist, distMin, distMax);
    float ratio = (dist - distDiff) / distDiff;
    float zoom = zoomDiff * ratio + zoomMin;
    zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
    if (zoom < myCamera.orthographicSize)
    {
      if (canZoomIn)
      {
        myCamera.orthographicSize = Mathf.Lerp(myCamera.orthographicSize, zoom, zoomInSpeed * GameTime.deltaTime);
        //float zero = 0;
        //myCamera.orthographicSize = Mathf.SmoothDamp(myCamera.orthographicSize, zoom, ref zero, zoomInSpeed * GameTime.deltaTime);
      }
      else if (!zoomInTimer)
      {
        StartCoroutine("ZoomInTimer");
      }
    }
    else if (zoom > myCamera.orthographicSize)
    {
      StopCoroutine("ZoomInTimer");
      canZoomIn = false;
      zoomInTimer = false;
      //float zero = 0;
      myCamera.orthographicSize = Mathf.Lerp(myCamera.orthographicSize, zoom, zoomOutSpeed * GameTime.deltaTime);
      //myCamera.orthographicSize = Mathf.SmoothDamp(myCamera.orthographicSize, zoom, ref zero, zoomOutSpeed * GameTime.deltaTime);
    }
  } // end Zoom


  /// <summary>
  /// Wait before zooming in.
  /// </summary>
  private IEnumerator ZoomInTimer()
  {
    zoomInTimer = true;
    yield return new WaitForSeconds(zoomInBuffer);
    zoomInTimer = false;
    canZoomIn = true;
  } // end ZoomInTimer

  #endregion

  void DrawLines()
  {
    // outside
    Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalOuter, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalOuter, Screen.height, 10f)), Color.red);
    Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalOuter, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalOuter, Screen.height, 10f)), Color.red);

    // inside
    Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalInner, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width * boundaryHorizontalInner, Screen.height, 10f)), Color.blue);
    Debug.DrawLine(myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalInner, 0f, 10f)), myCamera.ScreenToWorldPoint(new Vector3(Screen.width - Screen.width * boundaryHorizontalInner, Screen.height, 10f)), Color.blue);
  }

} // end LevelCamera