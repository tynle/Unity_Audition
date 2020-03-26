using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject[] buttons;
    public float buttonSpacing = 0.5f;

    public GameObject[] dancers;
    private Vector3[] dancerPosition = {
        new Vector3(-2f, 0f, 4f),
        new Vector3(0f, 0f, 0f),
        new Vector3(2f, 0f, 4f)
    };

    public GameObject[] playList;
    private BGMObject m_bgmInPlay;

    public GameObject danceStage;
    private GameObject m_danceStageInstance;

    private Transform buttonHolder;
    private Transform dancerHolder;

    public void SetupStage()
    {
        buttonHolder = new GameObject("ButtonHolder").transform;
        dancerHolder = new GameObject("DancerHolder").transform;
        
        int randNum = Random.Range(0, playList.Length);
        m_bgmInPlay = Instantiate(playList[randNum], Vector3.zero, Quaternion.identity).GetComponent<BGMObject>();
        m_bgmInPlay.PlayBGM();
        m_danceStageInstance = Instantiate(danceStage, Vector3.zero, Quaternion.identity);

        GameObject ui = Instantiate(canvas, Vector3.zero, Quaternion.identity);
        ui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        ui.GetComponent<Canvas>().worldCamera = m_danceStageInstance.transform.Find("Cameras/Camera Brain").GetComponent<Camera>();

        ShowDancers();
    }
    
    public void ShowDancers()
    {
        for(int i = 0; i < dancers.Length; i ++)
        {
            GameObject dancer = Instantiate(dancers[i], dancerPosition[i], Quaternion.identity, dancerHolder);
        }
        dancerHolder.Rotate(0, 180, 0);
    }
    public void GenerateButtons(GameObject[] arrButtons, int min, int max)
    {
        int btnCount = Random.Range(min, max + 1);
        for(int i = 0; i < btnCount; i ++)
        {
            GameObject btnChoice = arrButtons[Random.Range(0, arrButtons.Length)];
            GameObject instance = Instantiate(btnChoice, new Vector3(0f, 0f, 0f), Quaternion.identity, buttonHolder);
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
}