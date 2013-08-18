// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Holds references to already instantiate objects.
/// </summary>
public class ObjectRecycler
{
  private readonly GameObject prefab;
  private List<GameObject> objects = new List<GameObject>();
  private readonly Transform parent;


  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="prefab">Type of gameobject to create.</param>
  /// <param name="parent">Should the objects be parented to a pool?</param>
  /// <param name="premade">Does the pool have some objects already created? Must have non-null parent.</param>
  public ObjectRecycler(GameObject prefab, Transform parent = null, bool premade = false)
  {
    this.prefab = prefab;
    this.parent = parent;

    if (premade)
    {
      for (int i = 0; i < parent.childCount; i++)
      {
        objects.Add(parent.GetChild(i).gameObject);
      }
    }
  } // end ObjectRecycler


  /// <summary>Return first inactive object.</summary>
  public GameObject nextFree
  {
    get
    {
      // found inactive object
      foreach (GameObject gameObject in objects)
      {
        if (!gameObject.activeInHierarchy)
        {
          gameObject.SetActive(true);
          return gameObject;
        }
      }

      // must create a new object
      GameObject newObject = (GameObject)MonoBehaviour.Instantiate(prefab);
      objects.Add(newObject);
      newObject.transform.parent = parent;
      return newObject;
    }
  } // end nextFree

} // end ObjectRecycler class