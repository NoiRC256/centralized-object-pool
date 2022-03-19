using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if(_instance == null)
                {
                    Debug.LogWarning("Missing singleton of type " + typeof(T).ToString());
                    /*
                    // Create if missing.
                    GameObject newGO = new GameObject();
                    _instance = newGO.AddComponent<T>();
                    */
                    return null;
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
}
