using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private StageManager stageScript;
    private GAMESTATE currentState;
    enum GAMESTATE
    {
        GS_INIT,
        GS_CINEMATIC,
        GS_PREPAIR,
        GS_DANCE,
        GS_STATS
    }
    void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        stageScript = GetComponent<StageManager>();
        currentState = GAMESTATE.GS_INIT;
    }

    public void InitGame()
    {
        stageScript.SetupStage();
    }

    public void UpdateGame()
    {
        switch(currentState)
        {
            case GAMESTATE.GS_INIT:
                InitGame();
                currentState = GAMESTATE.GS_CINEMATIC;
            break;
            case GAMESTATE.GS_CINEMATIC:
                currentState = GAMESTATE.GS_PREPAIR;
            break;
            case GAMESTATE.GS_PREPAIR:
                stageScript.GenerateButtons(stageScript.buttons, 2, 6);
                stageScript.LayoutButtons();
                currentState = GAMESTATE.GS_DANCE;
            break;
            case GAMESTATE.GS_DANCE:
            break;
            case GAMESTATE.GS_STATS:
            break;
        }

    }
    void Update()
    {
        UpdateGame();
    }
}
