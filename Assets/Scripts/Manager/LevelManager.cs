using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{
    #region enum
    public enum LevelDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    #endregion

    #region params
    List<GameObject> _Beats;
    List<ObjectItem> _Objects;
    ObjectItem _ActiveObject;
    float _ObjectOffset = 2.5f;
    int _Index;
    int _RangeMin;
    int _RangeMax1;
    int _RangeMax2;
    int _RangeMax3;
    #endregion

    #region  unity methods
    private void Start() {
        if (_Beats == null)
            _Beats = new List<GameObject>();
        if (_Objects == null)
            _Objects = new List<ObjectItem>();
        _Index = -1;
        _RangeMin = 3;
        _RangeMax1 = 6;
        _RangeMax2 = 9;
        _RangeMax3 = 12;
    }

    private void Update() {
        if (_ActiveObject != null)
        {
            if (_ActiveObject.IsIdle())
                _ActiveObject.SetObjectFocus();
        }
    }
    #endregion

    #region private methods
    void ClearLevel ()
    {
        foreach (var obj in _Beats)
            PoolManager.Instance.KillGameObject(obj);
        _Beats.Clear();
        _Objects.Clear();
    }

    void GenerateBeats (int quota)
    {
        this.ClearLevel();

        int start = (int)PoolManager.PoolObject.None + 1;
        int end = (int)PoolManager.PoolObject.All;
        float offsetX = (quota / 2) * _ObjectOffset * -1;
        if (quota % 2 == 0)
            offsetX += (_ObjectOffset / 2);
        for (int i = 0; i < quota; ++i) 
        {
            int type = Random.Range(start, end);
            Vector3 pos = new Vector3(offsetX + _ObjectOffset * i, 0, 0);
            GameObject obj = PoolManager.Instance.Spawn((PoolManager.PoolObject)type, pos);
            _Beats.Add(obj);
            ObjectItem script = obj.GetComponent<ObjectItem>();
            _Objects.Add(script);
        }
        _Index = 0;
        _ActiveObject = _Objects[_Index];
    }
    #endregion

    #region public methods
    public void GenerateLevel (LevelDifficulty dif = LevelDifficulty.Easy)
    {
        int range = _RangeMax1;
        switch (dif)
        {
            case LevelDifficulty.Medium:
            {
                range = _RangeMax2;
                break;
            }

            case LevelDifficulty.Hard:
            {
                range = _RangeMax3;
                break;
            }

            default:
            {
                range = _RangeMax1;
                break;
            }
        }

        int quota = Random.Range(_RangeMin, range + 1);
        this.GenerateBeats(quota);
    }

    public bool IsProccessingLevel ()
    {
        return _Index >= 0 && _Index < _Beats.Count;
    }

    public void ProccessCommand (PoolManager.PoolObject type)
    {

    }
    #endregion
}
