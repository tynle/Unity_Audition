using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectItem : MonoBehaviour
{
    #region  enum
    public enum ObjectState
    {
        Idle,
        Focus2,
        Gray,
        Focus
    }
    #endregion

    #region  params
    const string TRANS_OBJECT_STATE = "ObjectState";
    const string CLIP_FOCUS2 = "Focus2";
    Animator _AnimCtrl;

    PoolManager.PoolObject _PoolObject;
    #endregion
    // Start is called before the first frame update
    #region unity methods
    void Start()
    {
        if (_AnimCtrl == null)
        {
            _AnimCtrl = GetComponent<Animator>();
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Idle);
        }
    }

    private void OnEnable() {
        if (_AnimCtrl != null)
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Idle);
    }

    // Update is called once per frame
    #endregion

    #region property 
    public PoolManager.PoolObject ObjectType
    {
        get
        {
            return _PoolObject;
        }
        set
        {
            if (_PoolObject == PoolManager.PoolObject.None)
                _PoolObject = value;
        }
    }
    #endregion

    #region public methods
    public void SetObjectFocus () 
    {
        if (_AnimCtrl != null)
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Focus2);
    }

    public void SetObjectTouch ()
    {
        if (_AnimCtrl != null)
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Focus);
    }

    public bool CanTouch ()
    {
        return _AnimCtrl.GetNextAnimatorStateInfo(0).IsName(CLIP_FOCUS2);
    }
    #endregion

    #region on animation controller 
    void OnFocus2Exit ()
    {
        _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Gray);
    }
    #endregion
}
