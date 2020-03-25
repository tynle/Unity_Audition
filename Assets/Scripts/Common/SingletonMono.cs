using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _Instance;

    public T Instance 
    {
        get
        {
            if (_Instance == null)
                _Instance = this as T;
            return _Instance;
        }
    }

    private void Awake() {
        _Instance = this as T;
    }
}
