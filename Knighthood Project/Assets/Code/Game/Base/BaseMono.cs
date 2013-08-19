// Steve Yeager
// 8.17.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Wrapper for Unity MonoBehaviour.
/// </summary>
public class BaseMono : MonoBehaviour
{
  #region Component Methods

  /// <summary>
  /// Get a component that implements certain interface.
  /// </summary>
  /// <typeparam name="I">Interface being implemented.</typeparam>
  /// <returns>The component that implements the interface.</returns>
  public I GetInterfaceComponent<I>() where I : class
  {
    return GetComponent(typeof(I)) as I;
  } // end GetInterfaceComponent


  /// <summary>
  /// Find all objects that implement a certain interface.
  /// </summary>
  /// <typeparam name="I">Interface to be implemented.</typeparam>
  /// <returns>List of references to components that implement the interface.</returns>
  public static List<I> FindObjectsOfInterface<I>() where I : class
  {
    MonoBehaviour[] monoBehaviours = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
    List<I> list = new List<I>();

    foreach (MonoBehaviour mono in monoBehaviours)
    {
      I component = mono.GetComponent(typeof(I)) as I;
      if (component != null)
      {
        list.Add(component);
      }
    }

    return list;
  } // end FindObjectsOfInterface


  /// <summary>
  /// Get component, Logs Error if not found.
  /// </summary>
  /// <typeparam name="T">Component type.</typeparam>
  /// <returns>Component if found</returns>
  public T GetSafeComponent<T>() where T : class
  {
    return gameObject.GetSafeComponent<T>();
  } // end GetSafeComponent

  #endregion

  #region Log Methods

  protected void Log(object message)
  {
    Debugger.Log(message, this);
  } // end Log

  #endregion

} // end BaseMono class