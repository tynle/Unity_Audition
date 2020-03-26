using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public GameObject canvas;
    
    public void SetupStage()
    {
        GameObject ui = Instantiate(canvas, new Vector3(0f,0f,0f), Quaternion.identity) as GameObject;
        ui.transform.Find("bgImage").SetAsFirstSibling();
        ui.transform.Find("Hub").SetAsLastSibling();
    }
    
}
