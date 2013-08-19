// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Wrapper for Unity's Debug class.
/// </summary>
public static class Debugger
{
  public static void Log(object message)
  {
    Debug.Log(message);
  } // end Log


  public static void Log(object message, Object context)
  {
    Debug.Log(message, context);
  } // end Log


  public static void LogWarning(object message)
  {
    Debug.LogWarning(message);
  } // end LogWarning


  public static void LogWarning(object message, Object context)
  {
    Debug.LogWarning(message, context);
  } // end LogWarning


  public static void LogError(object message)
  {
    Debug.LogError(message);
  } // end LogError


  public static void LogError(object message, Object context)
  {
    Debug.LogError(message, context);
  } // end LogError


  public static void LogException(System.Exception exception)
  {
    Debug.LogException(exception);
  } // end LogException


  public static void LogException(System.Exception exception, Object context)
  {
    Debug.LogException(exception, context);
  } // end LogException

} // end Debugger class