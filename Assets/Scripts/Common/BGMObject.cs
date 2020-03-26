using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMObject : MonoBehaviour
{
    public float danceStartTime;
    public float danceEndTime;
    public List<int> danceRoutine;
    public float musicSpeed;
    public float danceCallTime;

    private AudioSource m_soundSource;

    // Start is called before the first frame update
    void Awake() {
        m_soundSource = GetComponent<AudioSource>();
    }

    public void PlayBGM() {
        m_soundSource.Play();
    }
}
