using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    #region params 
    public PoolManager.PoolObject _PoolObject;
    public int _Quota;
    public GameObject _Prefab;
    List<GameObject> _PooledObjects;
    List<GameObject> _LiveObjects;
    #endregion

    #region unity methods
    private void Start() {
        if (_PooledObjects == null)
        {
            _PooledObjects = new List<GameObject>();
            if (_Prefab != null)
            {
                for (int i = 0; i < _Quota; ++i)
                {
                    GameObject obj = Instantiate(_Prefab, Vector3.zero, Quaternion.identity, transform);
                    obj.SetActive(false);
                    ObjectItem item = obj.GetComponent<ObjectItem>();
                    item.ObjectType = _PoolObject;
                    _PooledObjects.Add(obj);
                }
            }
        }

        if (_LiveObjects == null)
         _LiveObjects = new List<GameObject>();
    }
    #endregion

    #region public methods
    public GameObject Spawn (Vector3 pos, Quaternion rot)
    {
        GameObject obj = null;
        obj = _PooledObjects.Find(o => !o.activeSelf);
        if (obj == null)
        {
            obj = Instantiate(_Prefab, transform);
            Debug.Log("Instantiate object");
        }

        obj.SetActive(true);
        _LiveObjects.Add(obj);

        obj.transform.position = pos;
        obj.transform.rotation = rot;

        return obj;
    }

    public void KillGameObject (GameObject obj) 
    {
        int index = _LiveObjects.FindIndex(o => o == obj);
        if (index != -1)
        {
            _LiveObjects.RemoveAt(index);
            _PooledObjects.Add(obj);
            obj.SetActive(false);       
        }
    }

    public bool IsResponsibleForObj (GameObject obj) 
    {
        int index = _LiveObjects.FindIndex(o => o == obj);
        return index != -1;
    }
    #endregion
}
