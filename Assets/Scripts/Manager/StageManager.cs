using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject[] buttons;
    public float buttonSpacing = 0.5f;
    private Transform buttonHolder;

    public void SetupStage()
    {
        buttonHolder = new GameObject("ButtonHolder").transform;
        GameObject ui = Instantiate(canvas, new Vector3(0f,0f,0f), Quaternion.identity) as GameObject;
        ui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        GenerateButtons(buttons, 2, 6);
        LayoutButtons();
    }
    
    public void GenerateButtons(GameObject[] arrButtons, int min, int max)
    {
        int btnCount = Random.Range(min, max + 1);
        for(int i = 0; i < btnCount; i ++)
        {
            GameObject btnChoice = arrButtons[Random.Range(0, arrButtons.Length)];
            GameObject instance = Instantiate(btnChoice, new Vector3(0f, 0f, 0f), Quaternion.identity);
            instance.transform.SetParent(buttonHolder);
        }
    
    }
    void LayoutButtons()
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