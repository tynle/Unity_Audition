using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class StageManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject[] buttons;
    public float buttonSpacing = 0.5f;

    public GameObject[] dancers;
    private Vector3[] dancerPosition = {
        new Vector3(2f, 0f, 10f),
        new Vector3(0f, 0f, 8f),
        new Vector3(-2f, 0f, 10f)
    };
    private List<PlayerController> m_dancerControl;

    public GameObject[] playList;
    private BGMObject m_bgmInPlay;
    private float m_startInvokeTime;

    public GameObject danceStage;
    private DanceStageController m_danceStageControl;

    private Transform buttonHolder;
    private Transform dancerHolder;
    private bool isIntroPlaying;

    public void SetupStage()
    {
        buttonHolder = new GameObject("ButtonHolder").transform;
        dancerHolder = new GameObject("DancerHolder").transform;

        m_dancerControl = new List<PlayerController>(dancerPosition.Length);

        LoadBGM();
        LoadDanceStage();
        LoadDancers();
        LoadUI();
    }
    
    private void LoadBGM()
    {
        int randNum = Random.Range(0, playList.Length);
        GameObject audioSource = Instantiate(playList[randNum], Vector3.zero, Quaternion.identity);
        m_bgmInPlay = audioSource.GetComponent<BGMObject>();
    }
    
    
    private void LoadDanceStage()
    {
        GameObject ds = Instantiate(danceStage, Vector3.zero, Quaternion.identity);
        m_danceStageControl = ds.GetComponent<DanceStageController>();
        m_danceStageControl.Intro.stopped += OnIntroStop => {isIntroPlaying = false; Debug.Log("intro stopped");};
    }

    public bool IsIntroPlaying()
    {
        return isIntroPlaying;
    }
    private void LoadUI()
    {
        GameObject ui = Instantiate(canvas, Vector3.zero, Quaternion.identity);
        /*ui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        ui.GetComponent<Canvas>().worldCamera = Camera.main;*/
    }

    private void LoadDancers()
    {
        for(int i = 0; i < dancers.Length; i ++)
        {
            GameObject dancer = Instantiate(dancers[i], dancerPosition[i], Quaternion.Euler(0.0f, 180.0f, 0.0f), dancerHolder);
            
            PlayerController control = (dancer.GetComponent<PlayerController>());
            control.Setup(m_bgmInPlay.danceRoutine, m_bgmInPlay.musicSpeed, m_bgmInPlay.danceCallTime);
            control.smoothReturn *= m_bgmInPlay.musicSpeed;

            m_dancerControl.Add(control);
        }

        m_danceStageControl.manInLeftSpot = m_dancerControl[0];
        m_danceStageControl.manInMainSpot = m_dancerControl[1];
        m_danceStageControl.manInRightSpot = m_dancerControl[2];
    }

    public void PlayIntro()
    {
        m_bgmInPlay.PlayBGM();
        m_startInvokeTime = Time.time;
        
        m_danceStageControl.LightsOn();
        m_danceStageControl.Intro.Play();
        isIntroPlaying = true;
        foreach(PlayerController control in m_dancerControl)
        {
            control.MoveToDanceSpot();
        }
    }

    
    public void GenerateButtons(GameObject[] arrButtons, int min, int max)
    {
        int btnCount = Random.Range(min, max + 1);
        for(int i = 0; i < btnCount; i ++)
        {
            GameObject btnChoice = arrButtons[Random.Range(0, arrButtons.Length)];
            Instantiate(btnChoice, new Vector3(0f, 0f, 0f), Quaternion.identity, buttonHolder);
        }
        LayoutButtons();
    }
    public void LayoutButtons()
    {
        buttonHolder.position = new Vector3(0.0f, 2.0f, 0.0f);
        float btnWidth = buttonHolder.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
        float totalWidth = buttonHolder.childCount * btnWidth + (buttonHolder.childCount - 1) *buttonSpacing;
        float startX = -totalWidth/2 + btnWidth/2;
        for(int i = 0; i < buttonHolder.childCount; i ++)
        {
            Transform button = buttonHolder.GetChild(i);
            button.position += new Vector3(startX + i*(btnWidth + buttonSpacing), 0, 0);
        }
    }

    public void GameStart() {
        float delay = m_bgmInPlay.danceStartTime - (Time.time - m_startInvokeTime);
        if(delay > 0) {
            Invoke("startDancers", delay);
        } else {
            startDancers();
        }
        
        Invoke("stopDancers", m_bgmInPlay.danceEndTime - (Time.time - m_startInvokeTime));
    }

    private void startDancers() {
        foreach(PlayerController control in m_dancerControl) {
            control.StartDancing();
        }
        StartCoroutine("randomGamePlay");
    }
    
    private IEnumerator randomGamePlay() {
        while(true) {
            foreach(PlayerController control in m_dancerControl) {
                int result = Random.Range(1, 5);
                switch(result) {
                    case 1:
                        control.TriggerMiss();
                        Debug.Log("Miss!");
                    break;

                    case 2:
                    case 3:
                    case 4:
                        control.TriggerDance();
                    break;
                }
            }
            yield return new WaitForSeconds(m_bgmInPlay.danceCallTime / 2);
        }
    }

    private void stopDancers() {
        StopCoroutine("randomGamePlay");

        // highest score
        int maxScore = 0;
        foreach(PlayerController control in m_dancerControl) {
            if (maxScore < control.score) {
                maxScore = control.score;
            }
        }

        // trigger ending animation
        foreach(PlayerController control in m_dancerControl) {
            control.TriggerEnd(control.score == maxScore);
        }
    }
}