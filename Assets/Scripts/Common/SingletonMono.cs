using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _Instance;

    public static T Instance 
    {
        get
        {
            return _Instance;
        }
    }

    private void Awake() {
        _Instance = this as T;
    }
}
