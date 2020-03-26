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
    const string CLIP_IDLE = "Idle";
    bool _IsFinished;
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
        _IsFinished = false;
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

    public void SetObjectTouched ()
    {
        if (_AnimCtrl != null)
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Focus);
    }

    public bool CanTouch ()
    {
        return _AnimCtrl.GetCurrentAnimatorStateInfo(0).IsName(CLIP_FOCUS2);
    }

    public bool IsIdle ()
    {
        if (_AnimCtrl != null)
            return _AnimCtrl.GetCurrentAnimatorStateInfo(0).IsName(CLIP_IDLE);
        return false;
        // _AnimCtrl.GetCurrentAnimatorStateInfo(0).IsName
    }

    public void SetObjectIdle()
    {
        if (_AnimCtrl != null)
            _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Idle);
    }

    public bool IsFinished ()
    {
        return _IsFinished;
    }
    #endregion

    #region on animation controller 
    void OnFocus2Exit ()
    {
        _AnimCtrl.SetInteger(TRANS_OBJECT_STATE, (int)ObjectState.Gray);
    }

    void OnGrayExit ()
    {
        _IsFinished = true;
    }

    void OnFocusExit ()
    {
        _IsFinished = true;
    }
    #endregion
}
