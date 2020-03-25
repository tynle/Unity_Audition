using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // component refs
    private Transform m_body;
    private Transform m_wrapper;
    private Animator m_anim;

    // define values
    private static string TRIGGER_SWITCH = "Switch";
    private static string TRIGGER_WIN = "Win";
    private static string TRIGGER_LOSE = "Lose";
    private static string INT_TYPE = "Type";

    // public parameters
    public bool RunTestOnPlay;
    public Camera sceneCamera;
    public Vector3 danceSpot;
    public int numberOfDancingMove;
    public float smoothReturn;

    // private parameters
    private Transform m_originBodyTransform;
    private Coroutine m_reposCoroutine = null;
    
    ///////////////
    // system events
    void Awake() {
        m_wrapper = this.transform;

        GameObject character = m_wrapper.Find("Body").gameObject;
        m_body = character.GetComponent<Transform>();
        m_anim = character.GetComponent<Animator>();
        m_originBodyTransform = m_body;
        
        if (RunTestOnPlay) {
            setMusicSpeed(2.0f);
            m_wrapper.position = new Vector3(m_wrapper.position.x, m_wrapper.position.y, m_wrapper.position.z + 5.0f);
            MoveToDanceSpot();
            StartCoroutine("autoSwitchDance");
        }
    }
    

    ///////////////
    // cutscene
    public void MoveToDanceSpot() {
        m_body = m_originBodyTransform;
        SmoothRePositioning(m_wrapper, danceSpot, false);
    }
    
    public void SwitchDanceSpot(Vector3 spot) {
        danceSpot = spot;
        MoveToDanceSpot();
    }

    ///////////////
    // game events
    public void TriggerDance(int mode) {
        if (mode < 0) mode = 0;
        if (mode > numberOfDancingMove) mode = numberOfDancingMove;

        m_anim.SetInteger(INT_TYPE, mode);
        m_anim.SetTrigger(TRIGGER_SWITCH);
    }

    public void TriggerEnd(bool isWinner) {
        if (isWinner) {
            m_anim.SetTrigger(TRIGGER_WIN);
        } else {
            m_anim.SetTrigger(TRIGGER_LOSE);
        }
    }

    ///////////////
    // others
    public void setMusicSpeed(float spd) {
        m_anim.speed *= spd;
    }
    
    void SmoothRePositioning(Transform me, Vector3 target, bool faceToTarget) {
        if (m_reposCoroutine == null) {
            m_reposCoroutine = StartCoroutine(doReposition(me, target, faceToTarget));
        } else {
            me.position = target;
            LookStraight(me, sceneCamera.transform.position);
        }
    }

    IEnumerator doReposition(Transform me, Vector3 target, bool faceToTarget) {
        while (me.position != target) {
            if (faceToTarget) LookStraight(me, target);
            me.position = Vector3.MoveTowards(me.position, target, smoothReturn * Time.deltaTime);
            yield return null;
        }

        LookStraight(me, sceneCamera.transform.position);
        
        // self kill
        StopCoroutine(m_reposCoroutine);
    }

    void LookStraight(Transform me, Vector3 target) {
        Vector3 lookPoint = target;
        lookPoint.y = me.position.y;

        me.LookAt(lookPoint);
    }
    IEnumerator autoSwitchDance() {
        int i = numberOfDancingMove;
        while (i > -3) {
            yield return new WaitForSeconds(5.0f);
            if (i == -1) {
                Debug.Log("Win");
                TriggerEnd(true);
            } else if (i == -2) {
                Debug.Log("Lose");
                TriggerEnd(false);
            } else {
                Debug.Log("Switch");
                TriggerDance(i);
            }
            i--;
        }
        StopCoroutine("autoSwitchDance");
    }
}
