// Steve Yeager
// 8.17.2013

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

  #region Invoke Methods

  //
  public void InvokeAction(Action action, float time)
  {
    StartCoroutine(InvokedAction(action, time));
  } // end InvokeAction


  private IEnumerator InvokedAction(Action action, float time)
  {
    yield return new WaitForSeconds(time);
    action.Invoke();
  } // end InvokedAction

  #endregion

  #region Coroutine Methods

  /// <summary>
  /// Wait for a set amount of GameTime.
  /// </summary>
  /// <param name="waitTime">Time to wait in seconds.</param>
  public Coroutine WaitForTime(float waitTime)
  {
    return StartCoroutine(Wait(waitTime));
  } // end WaitForTime


  /// <summary>
  /// Wait for seconds in GameTime.
  /// </summary>
  /// <param name="waitTime">Time in seconds.</param>
  private IEnumerator Wait(float waitTime)
  {
    Debug.Log("Wait");
    float timer = 0f;
    while (timer < waitTime)
    {
      timer += GameTime.deltaTime;
      yield return null;
    }
  } // end Wait

  #endregion

} // end BaseMono class