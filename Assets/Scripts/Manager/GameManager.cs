using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region params
    public bool _CanPlay;
    bool _StartGame;
    private StageManager stageScript;
    public static LeaderboardManager gameLeaderBoard;
    private GAMESTATE currentState;
    #endregion
    
    enum GAMESTATE
    {
        GS_IDLE,
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
        GameManager.gameLeaderBoard = Instance.GetComponent<LeaderboardManager>();

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
        GAMESTATE state = currentState;
        currentState = GAMESTATE.GS_IDLE;
        
        switch(state)
        {
            case GAMESTATE.GS_INIT:
                InitGame();
            break;

            case GAMESTATE.GS_CINEMATIC:
                Invoke("PlayCinematic", 3.0f);
            break;

            case GAMESTATE.GS_PREPAIR:
                stageScript.GenerateButtons(stageScript.buttons, 2, 6);
                stageScript.LayoutButtons();
                currentState = GAMESTATE.GS_DANCE;
            break;

            case GAMESTATE.GS_DANCE:
                GameStart();
            break;

            case GAMESTATE.GS_STATS:
                stageScript.PlayOutro();
            break;

            case GAMESTATE.GS_IDLE:
            default:
            break;
        }

    }
	
	public void InitGame()
    {
        stageScript.SetupStage();
        currentState = GAMESTATE.GS_CINEMATIC;
    }
	public void PlayCinematic()
    {
        stageScript.PlayIntro();
        currentState = GAMESTATE.GS_PREPAIR;
    }
	public void GameStart()
    {
        float playTime = stageScript.GameStart();

        Invoke("GameEnd", playTime);
    }

    public void GameEnd()
    {
        currentState = GAMESTATE.GS_STATS;
    }

    #endregion
}
