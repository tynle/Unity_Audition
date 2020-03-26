using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region params
    public bool _CanPlay;
    bool _StartGame;
    private StageManager stageScript;
    private GAMESTATE currentState;
    #endregion
    
    enum GAMESTATE
    {
        GS_INIT,
        GS_CINEMATIC,
        GS_PREPAIR,
        GS_DANCE,
        GS_STATS
    }

    #region unity methods
    [RuntimeInitializeOnLoadMethod]
    static void OnInitGameManagerStart ()
    {
        Instance._StartGame = false;
        Instance._CanPlay = false;
        
        Instance.stageScript = Instance.GetComponent<StageManager>();
        Instance.currentState = GAMESTATE.GS_INIT;
    }

    private void Update() {
        // if (_CanPlay)
        // {
        //     if (!LevelManager.Instance.IsProccessingLevel())
        //     {
        //         LevelManager.Instance.GenerateLevel();
        //     }
        // }
        UpdateGame();
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
    public void UpdateGame()
    {
        switch(currentState)
        {
            case GAMESTATE.GS_INIT:
                currentState = GAMESTATE.GS_CINEMATIC;
                InitGame();
            break;
            case GAMESTATE.GS_CINEMATIC:
                currentState = GAMESTATE.GS_PREPAIR;
            break;
            case GAMESTATE.GS_PREPAIR:
                stageScript.GenerateButtons(stageScript.buttons, 2, 6);
                stageScript.LayoutButtons();currentState = GAMESTATE.GS_DANCE;
            break;
            case GAMESTATE.GS_DANCE:
            break;
            case GAMESTATE.GS_STATS:
            break;
        }

    }
	
	public void InitGame()
    {
        stageScript.SetupStage();
    }

    #endregion
}
