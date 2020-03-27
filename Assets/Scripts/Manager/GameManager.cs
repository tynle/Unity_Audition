using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    #region params
    public bool _CanPlay;
    bool _StartGame;
    private StageManager stageScript;
    public static LeaderboardManager leaderboard;
    private GAMESTATE currentState;
    private float m_timeMilestone;
    private float m_gameDuration;
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
        GameManager.leaderboard = Instance.GetComponent<LeaderboardManager>();

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
                if (Time.time - m_timeMilestone >= 3.0f) { //skip 3s to easy see the intro :)
                    currentState = GAMESTATE.GS_PREPAIR;
                    stageScript.PlayIntro();
                }
            break;

            case GAMESTATE.GS_PREPAIR:
                if(stageScript.IsIntroPlaying())
                    break;

                currentState = GAMESTATE.GS_DANCE;
                GameStart();
            break;

            case GAMESTATE.GS_DANCE:
            break;

            case GAMESTATE.GS_STATS:
                currentState = GAMESTATE.GS_IDLE;
                stageScript.PlayOutro(GameManager.leaderboard.getWinners());
            break;

            case GAMESTATE.GS_IDLE:
            default:
            break;
        }

    }
	
	public void InitGame()
    {
        List<PlayerController> dancers = stageScript.SetupStage();
        
        GameManager.leaderboard.reset();
        for (int i = 0; i < dancers.Count; i++) {
            GameManager.leaderboard.register(i, dancers[i].name);
        }
    }
	public void GameStart()
    {
        //stageScript.GenerateButtons(stageScript.buttons, 2, 6);
        m_timeMilestone = Time.time;
        m_gameDuration = stageScript.GameStart();
    }

    #endregion
}
