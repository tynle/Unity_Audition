using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMono<PoolManager>
{
    #region enum
    public enum PoolObject
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    Dictionary<PoolObject, Pool> _PoolDict;
    #endregion


    #region unity methods
    private void Start() {
        if (_PoolDict == null)
        {
            _PoolDict = new Dictionary<PoolObject, Pool>();
            Pool[] pools = GetComponentsInChildren<Pool> ();
            foreach (var p in pools)
            {
                _PoolDict[p._PoolObject] = p;
            }
        }
        
    }
    #endregion

    #region  public methods
    public GameObject Spawn (PoolManager.PoolObject type, Vector3 pos, Quaternion rot) 
    {
        GameObject obj = null;
        if (_PoolDict.ContainsKey(type))
        {
            obj = _PoolDict[type].Spawn(pos, rot);
        }
        return obj;
    }

    public GameObject Spawn (PoolObject type, Vector3 pos)
    {
        return this.Spawn(type, pos, Quaternion.identity);
    }

    public void KillGameObject (GameObject obj) 
    {
        bool exist = false;
        foreach (var p in _PoolDict)
        {
            exist = p.Value.IsResponsibleForObj(obj);
            if (exist)
            {
                p.Value.KillGameObject(obj);
                exist = true;
                break;
            }
        }

        if (!exist)
            Destroy(obj);
    }
    #endregion
}
