using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private StageManager stageScript;
    void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        stageScript = GetComponent<StageManager>();
        InitGame();
    }

    public void InitGame()
    {
        stageScript.SetupStage();
    }
}
