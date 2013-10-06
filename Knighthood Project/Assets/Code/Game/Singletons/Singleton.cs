// Steve Yeager
// 8.17.2013

using UnityEngine;

/// <summary>
/// Base class for all singletons.
/// </summary>
/// <typeparam name="T">Inherited class.</typeparam>
public class Singleton<T> : BaseMono where T : MonoBehaviour
{
    protected static T instance;


    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject i = new GameObject(typeof(T).ToString());
                    i.AddComponent<T>();
                    instance = i.GetComponent<T>();
                }
            }

            return instance;
        }
    }

}