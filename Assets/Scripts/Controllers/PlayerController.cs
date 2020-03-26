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
    public Vector3 danceSpot;
    public float smoothReturn;

    // private parameters
    private Transform m_originBodyTransform;
    private Coroutine m_reposCoroutine = null;
    private List<int> m_danceRoutine;
    private int m_posInRoutine = -1;
    private bool m_missedMove = false;
    private float m_timePerMove = -1;
    private int m_numOfMove = -1;
    
    ///////////////
    // system events
    void Awake() {
        m_wrapper = this.transform;

        GameObject character = m_wrapper.Find("Body").gameObject;
        m_body = character.GetComponent<Transform>();
        m_anim = character.GetComponent<Animator>();
        m_originBodyTransform = m_body;
        m_numOfMove = m_anim.runtimeAnimatorController.animationClips.Length - 3;
        
        if (RunTestOnPlay) {
            List<int> routine = new List<int>();
            routine.Add(1);
            routine.Add(2);
            routine.Add(3);
            routine.Add(4);
            routine.Add(5);
            routine.Add(6);

            Setup(routine, 1.5f, 2.5f);
            m_wrapper.position = new Vector3(m_wrapper.position.x, m_wrapper.position.y, m_wrapper.position.z + 5.0f);
            MoveToDanceSpot();

            Invoke("TriggerDance", 2.0f);
            Invoke("TriggerMiss", 5.0f);
        }
    }

    ///////////////
    // game setup
    public void Setup(List<int> danceRoutine, float gameSpeed, float timePerMove) {
        m_anim.speed *= gameSpeed;
        m_danceRoutine = danceRoutine;
        m_timePerMove = timePerMove;

        for(int i = 0; i < m_danceRoutine.Count;) {
            if (m_danceRoutine[i] < 0 || m_danceRoutine[i] > m_numOfMove) {
                m_danceRoutine.RemoveAt(i);
            } else {
                i++;
            }
        }
    }

    ///////////////
    // dance spot
    public void MoveToDanceSpot() {
        m_body = m_originBodyTransform;
        SmoothRePositioning(m_wrapper, danceSpot, false);
    }
    
    public void SwitchDanceSpot(Vector3 spot) {
        danceSpot = spot;
        MoveToDanceSpot();
    }

    ///////////////
    // dance routine
    public void TriggerDance() {
        m_posInRoutine = 0;
        StartCoroutine("DanceWithRoutine");
    }

    IEnumerator DanceWithRoutine() {
        while (m_danceRoutine[m_posInRoutine] >= 0 && m_danceRoutine[m_posInRoutine] <= m_numOfMove) {
            if (m_missedMove) {
                m_missedMove = false;
            } else {
                SmoothRePositioning(m_body, danceSpot, true);
                m_anim.SetTrigger(TRIGGER_SWITCH);
                m_anim.SetInteger(INT_TYPE, m_danceRoutine[m_posInRoutine]);
            }
            
            // set next move
            m_posInRoutine++;
            if (m_posInRoutine >= m_danceRoutine.Count) m_posInRoutine = 0;

            yield return new WaitForSeconds(m_timePerMove);
        }
    }

    public void TriggerMiss() {
        m_missedMove = true;
        m_anim.SetInteger(INT_TYPE, 0);
    }

    public void TriggerEnd(bool isWinner) {
        m_posInRoutine = -1;
        StopCoroutine("DanceWithRoutine");
        SmoothRePositioning(m_body, danceSpot, true);
        if (isWinner) {
            m_anim.SetTrigger(TRIGGER_WIN);
        } else {
            m_anim.SetTrigger(TRIGGER_LOSE);
        }
    }

    ///////////////
    // others    
    void SmoothRePositioning(Transform me, Vector3 target, bool faceToTarget) {
        if (m_reposCoroutine == null) {
            m_reposCoroutine = StartCoroutine(doReposition(me, target, faceToTarget));
        } else {
            me.position = target;
            LookStraight(me, me.position + me.forward);
        }
    }

    IEnumerator doReposition(Transform me, Vector3 target, bool faceToTarget) {
        while (me.position != target) {
            if (faceToTarget) LookStraight(me, target);
            me.position = Vector3.MoveTowards(me.position, target, smoothReturn * Time.deltaTime);
            yield return null;
        }

        LookStraight(me, me.position + me.forward);
        
        // self kill
        StopCoroutine(m_reposCoroutine);
    }

    void LookStraight(Transform me, Vector3 target) {
        Vector3 lookPoint = target;
        lookPoint.y = me.position.y;

        me.LookAt(lookPoint);
    }
}
