// Steve Yeager
// 8.17.2013

using UnityEngine;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// Handles all debugging input.
/// </summary>
public class DebugManager : Singleton<DebugManager>
{
  #region MonoBehaviour Overrides

  private void LateUpdate()
  {
    if (Input.GetKeyDown(KeyCode.F1))
    {
      StartCoroutine(TakeScreenshot());
    }
  } // end LateUpate

  #endregion

  #region Screenshot Methods

  /// <summary>
  /// Capture current camera render.
  /// </summary>
  /// <param name="camera">What camera to capture. If null Camera.main is selected.</param>
  public IEnumerator TakeScreenshot(Camera camera = null)
  {
    yield return new WaitForEndOfFrame();

    int width = Screen.width;
    int height = Screen.height;

    if (camera == null)
    {
      camera = Camera.main;
    }

    RenderTexture texture = new RenderTexture(width, height, 24);
    camera.targetTexture = texture;
    Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
    camera.Render();
    RenderTexture.active = texture;
    screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    camera.targetTexture = null;
    RenderTexture.active = null;
    Destroy(texture);
    byte[] bytes = screenshot.EncodeToPNG();

    if (!Directory.Exists(Application.dataPath + "/screenshots"))
    {
      Directory.CreateDirectory(Application.dataPath + "/screenshots");
    }
    string path = String.Format("{0}/screenshots/screen_{1}.png", Application.dataPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    File.WriteAllBytes(path, bytes);
    Debug.Log("Screenshot taken: " + path);
  } // end TakeScreenshot

  #endregion

} // end DebugManager class