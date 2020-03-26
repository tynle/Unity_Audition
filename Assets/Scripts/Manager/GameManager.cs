using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region params
    public bool _CanPlay;
    bool _StartGame;
    #endregion

    #region unity methods
    [RuntimeInitializeOnLoadMethod]
    static void OnInitGameManagerStart ()
    {
        Instance._StartGame = false;
        Instance._CanPlay = false;
    }

    private void Update() {
        if (_CanPlay)
        {
            if (!LevelManager.Instance.IsProccessingLevel())
            {
                LevelManager.Instance.GenerateLevel();
            }
        }
    }
    
    #endregion

    #region public methods
    public bool CanPlay ()
    {
        return this._CanPlay;
    }

    public void ProcessCommand (PoolManager.PoolObject type)
    {
        LevelManager.Instance.ProccessCommand(type);
    }
    #endregion
}
