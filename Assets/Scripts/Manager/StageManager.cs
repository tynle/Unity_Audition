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
    private Vector3[] dancerStartPose = {
        new Vector3(2f, 0f, 10f),
        new Vector3(0f, 0f, 8f),
        new Vector3(-2f, 0f, 10f)
    };
    private Vector3[] dancerSpots = {
        new Vector3(4f, 0f, 3f),
        new Vector3(0f, 0f, 0f),
        new Vector3(-4f, 0f, 3f)
    };
    private Vector3 bestDanceSpot = Vector3.zero;
    private int m_bestDancerID = 1;
    private List<PlayerController> m_dancerControl;

    public GameObject[] playList;
    private BGMObject m_bgmInPlay;
    private float m_startInvokeTime;

    public GameObject danceStage;
    private DanceStageController m_danceStageControl;
    
    private GameObject m_ui;

    private Transform buttonHolder;
    private Transform dancerHolder;
    private bool isIntroPlaying;

    public List<PlayerController> SetupStage()
    {
        if (buttonHolder == null) buttonHolder = new GameObject("ButtonHolder").transform;
        if (dancerHolder == null) dancerHolder = new GameObject("DancerHolder").transform;

        if (m_dancerControl == null) m_dancerControl = new List<PlayerController>(dancerSpots.Length);

        LoadBGM();
        LoadDanceStage();
        LoadDancers();
        LoadUI();

        return m_dancerControl;
    }
    
    private void LoadBGM()
    {
        if (m_bgmInPlay != null) Destroy(m_bgmInPlay.gameObject);

        int randNum = Random.Range(0, playList.Length);
        GameObject audioSource = Instantiate(playList[randNum], Vector3.zero, Quaternion.identity);
        m_bgmInPlay = audioSource.GetComponent<BGMObject>();
    }
    
    
    private void LoadDanceStage()
    {
        if (m_danceStageControl == null) {
            GameObject ds = Instantiate(danceStage, Vector3.zero, Quaternion.identity);
            m_danceStageControl = ds.GetComponent<DanceStageController>();
            m_danceStageControl.Intro.stopped += OnIntroStop => {isIntroPlaying = false; Debug.Log("intro stopped");};
        }
    }
    
    private void LoadUI()
    {
        if (m_ui == null) {
            m_ui = Instantiate(canvas, Vector3.zero, Quaternion.identity);
            m_ui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            m_ui.GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }

    private void LoadDancers()
    {
        if (m_dancerControl.Count <= 0) {
            // load prefabs
            for(int i = 0; i < dancers.Length; i ++)
            {
                GameObject dancer = Instantiate(dancers[i], dancerStartPose[i], Quaternion.Euler(0.0f, 180.0f, 0.0f), dancerHolder);
                
                PlayerController control = (dancer.GetComponent<PlayerController>());
                control.Setup(m_bgmInPlay.danceRoutine, m_bgmInPlay.musicSpeed, m_bgmInPlay.danceCallTime);
                control.moveSpeed *= m_bgmInPlay.musicSpeed;
                control.danceSpot = dancerSpots[i];

                m_dancerControl.Add(control);
            }

            // spot lights
            m_danceStageControl.manInLeftSpot = m_dancerControl[0];
            m_danceStageControl.manInMainSpot = m_dancerControl[1];
            m_danceStageControl.manInRightSpot = m_dancerControl[2];
        }
    }

    public bool IsIntroPlaying()
    {
        return isIntroPlaying;
    }

    public void PlayIntro()
    {
        m_bgmInPlay.PlayBGM();
        m_startInvokeTime = Time.time;
        
        m_danceStageControl.LightsOn();
        m_danceStageControl.Intro.Play();
        m_danceStageControl.Intro.playableGraph.GetRootPlayable(0).SetSpeed(35.0f/m_bgmInPlay.danceStartTime);

        isIntroPlaying = true;
        foreach(PlayerController control in m_dancerControl)
        {
            control.MoveToDanceSpot();
        }
    }

    public void PlayOutro(List<int> winners)
    {
        StopCoroutine("randomGamePlay");

        // cinematic
        m_danceStageControl.Outro.Play();

        // trigger ending animation
        for (int i = 0; i < m_dancerControl.Count; i++) {
            m_dancerControl[i].TriggerEnd(winners.IndexOf(i) != -1);
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

    public float GameStart() {
        float delay = m_bgmInPlay.danceStartTime - (Time.time - m_startInvokeTime);
        if(delay > 0) {
            Invoke("startDancers", delay);
        } else {
            startDancers();
        }

        return m_bgmInPlay.danceEndTime - (Time.time - m_startInvokeTime);
    }

    private void startDancers() {
        foreach(PlayerController control in m_dancerControl) {
            control.StartDancing();
        }
        StartCoroutine("randomGamePlay");
    }
    
    private IEnumerator randomGamePlay() {
        while(true) {
            for(int i = 0; i < m_dancerControl.Count; i++) {
                int result = Random.Range(1, 5);
                switch(result) {
                    case 1:
                        m_dancerControl[i].TriggerMiss();
                        GameManager.leaderboard.score(i, LeaderboardManager.GAMESCORE.MISS);
                    break;

                    case 2:
                        m_dancerControl[i].TriggerDance();
                        GameManager.leaderboard.score(i, LeaderboardManager.GAMESCORE.COOL);
                    break;

                    case 3:
                        m_dancerControl[i].TriggerDance();
                        GameManager.leaderboard.score(i, LeaderboardManager.GAMESCORE.GREAT);
                    break;

                    case 4:
                        m_dancerControl[i].TriggerDance();
                        GameManager.leaderboard.score(i, LeaderboardManager.GAMESCORE.PERFECT);
                    break;
                }
            }
            SwitchDanceSpot();
            yield return new WaitForSeconds(m_bgmInPlay.danceCallTime / 2);
        }
    }

    private void SwitchDanceSpot() {
        List<int> winners = GameManager.leaderboard.getWinners();
        if (winners.IndexOf(m_bestDancerID) != -1) {
            // Best dancer is still best
        } else {
            m_dancerControl[m_bestDancerID].SwitchDanceSpot(m_dancerControl[winners[0]].danceSpot);
            m_dancerControl[winners[winners.Count - 1]].SwitchDanceSpot(bestDanceSpot);
            m_bestDancerID = winners[winners.Count - 1];
        }
    }
}