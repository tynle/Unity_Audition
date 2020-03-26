using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DanceStageController : MonoBehaviour
{
    // characters
    public Transform manInMainSpot;
    public Transform manInLeftSpot;
    public Transform manInRightSpot;

    // game variable
    public float gameSpeed;
    public Color frontLightStart;
    public Color frontLightEnd;
    public Color behindLightStart;
    public Color behindLightEnd;

    public bool TestOnPlay;

    // components
    private GameObject m_spotLightGroup;
    private GameObject m_frontLightGroup;
    private GameObject m_behindLightGroup;

    private Transform m_mainSpotLight;
    private Transform m_leftSpotLight;
    private Transform m_rightSpotLight;

    public PlayableDirector Intro;
    public PlayableDirector Outro;


    ///////////////
    // system events
    void Awake() {
        // Cinematics
        Intro = transform.Find("Cinematic/Intro").gameObject.GetComponent<PlayableDirector>();
        Outro = transform.Find("Cinematic/Outro").gameObject.GetComponent<PlayableDirector>();

        // front lights
        m_frontLightGroup = transform.Find("Lights/FrontLights").gameObject;
        m_frontLightGroup.SetActive(false);

        // behind lights
        m_behindLightGroup = transform.Find("Lights/BehindLights").gameObject;
        m_behindLightGroup.SetActive(false);

        // spot lights
        m_spotLightGroup = transform.Find("Lights/SpotLights").gameObject;
        //m_spotLightGroup.SetActive(false);

        m_mainSpotLight = m_spotLightGroup.transform.Find("Main Spot");
        m_leftSpotLight = m_spotLightGroup.transform.Find("Left Spot");
        m_rightSpotLight = m_spotLightGroup.transform.Find("Right Spot");

        if (TestOnPlay) {
            Invoke("LightsOn", 1.0f);
        }
    }
    
    void Update() {
        // Follow the character
        m_mainSpotLight.LookAt(manInMainSpot);
        m_leftSpotLight.LookAt(manInLeftSpot);
        m_rightSpotLight.LookAt(manInRightSpot);

        // blinking lights
        float t = Mathf.PingPong(Time.time, 0.5f / gameSpeed) / (0.5f / gameSpeed);
        
        for(int i = 0; i < m_frontLightGroup.transform.childCount; i++) {
            Light l = m_frontLightGroup.transform.GetChild(i).GetComponent<Light>();
            l.color = Color.Lerp(frontLightStart, frontLightEnd, t);
        }

        for(int i = 0; i < m_behindLightGroup.transform.childCount; i++) {
            Light l = m_behindLightGroup.transform.GetChild(i).GetComponent<Light>();
            l.color = Color.Lerp(behindLightStart, behindLightEnd, t);
        }
    }

    ///////////////
    // Lights
    public void LightsOn() {
        Invoke("TurnOnBehindLight", 1.0f / gameSpeed);
        Invoke("TurnOnFrontLight", 2.0f / gameSpeed);
    }

    private void TurnOnFrontLight() {
        m_frontLightGroup.SetActive(true);
    }
    private void TurnOnBehindLight() {
        m_behindLightGroup.SetActive(true);
    }
}
