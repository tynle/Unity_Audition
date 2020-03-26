using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // component refs
    private Transform m_body;
    private Transform m_bodyIdle;
    private Transform m_wrapper;
    private Animator m_anim;
    private Animator m_animIdle;

    // define values
    private static string TRIGGER_RESTART = "Restart";
    private static string TRIGGER_SWITCH = "Switch";
    private static string TRIGGER_WIN = "Win";
    private static string TRIGGER_LOSE = "Lose";
    private static string INT_TYPE = "Type";

    // public parameters
    public bool RunTestOnPlay;
    public Vector3 danceSpot;
    public float smoothReturn;
    public int score;

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

        GameObject body = m_wrapper.Find("Body").gameObject;
        m_body = body.GetComponent<Transform>();
        m_anim = body.GetComponent<Animator>();

        GameObject bodyIdle = m_wrapper.Find("Body_Idle").gameObject;
        m_bodyIdle = bodyIdle.GetComponent<Transform>();
        m_animIdle = bodyIdle.GetComponent<Animator>();

        Reset();

        m_originBodyTransform = m_body;
        m_numOfMove = m_anim.runtimeAnimatorController.animationClips.Length - 3;
        
        if (RunTestOnPlay) {
            List<int> routine = new List<int>();
            routine.Add(6);
            routine.Add(3);
            routine.Add(5);
            routine.Add(2);
            routine.Add(1);
            routine.Add(6);
            routine.Add(4);
            routine.Add(2);
            routine.Add(3);
            routine.Add(4);

            Setup(routine, 1.5f, 5.0f);
            m_wrapper.position = new Vector3(m_wrapper.position.x, m_wrapper.position.y, m_wrapper.position.z + 8.0f);
            MoveToDanceSpot();

            Invoke("TriggerDance", 10.3f);
        }
    }

    ///////////////
    // game setup
    public void Reset() {
        m_anim.speed = 1.0f;
        m_animIdle.speed = 1.0f;

        m_missedMove = false;

        switchRenderIdle(false);

        m_posInRoutine = -1;

        m_anim.enabled = true;
        m_anim.SetTrigger(TRIGGER_RESTART);
        m_anim.SetInteger(INT_TYPE, -1);
        
        m_animIdle.enabled = true;
        m_animIdle.SetTrigger(TRIGGER_RESTART);
    }
    public void Setup(List<int> danceRoutine, float gameSpeed, float timePerMove) {
        m_anim.speed *= gameSpeed;
        m_animIdle.speed *= gameSpeed;
        m_danceRoutine = danceRoutine;
        m_timePerMove = timePerMove;
        score = 0;

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
        m_anim.SetInteger(INT_TYPE, 0);
        SmoothRePositioning(m_wrapper, danceSpot, false);
    }
    
    public void SwitchDanceSpot(Vector3 spot) {
        danceSpot = spot;
        MoveToDanceSpot();
    }

    ///////////////
    // dance routine
    public void StartDancing() {
        m_posInRoutine = 0;
        StartCoroutine("DanceWithRoutine");
    }

    IEnumerator DanceWithRoutine() {
        while (m_danceRoutine[m_posInRoutine] > 0 && m_danceRoutine[m_posInRoutine] <= m_numOfMove) {
            
            m_body = m_originBodyTransform;

            m_anim.SetTrigger(TRIGGER_SWITCH);
            m_anim.SetInteger(INT_TYPE, m_danceRoutine[m_posInRoutine]);
            
            // set next move
            m_posInRoutine++;
            if (m_posInRoutine >= m_danceRoutine.Count) m_posInRoutine = 0;

            yield return new WaitForSeconds(m_timePerMove);
        }
    }

    public void TriggerDance() {
        if (m_missedMove) {
            switchRenderIdle(false);
            m_missedMove = false;
        }
    }

    public void TriggerMiss() {
        m_missedMove = true;
        switchRenderIdle(true);
    }

    public void TriggerEnd(bool isWinner) {
        m_posInRoutine = -1;
        StopCoroutine("DanceWithRoutine");

        if(!m_missedMove) {
            switchRenderIdle(true);
        }

        m_anim.enabled = false;
        if (isWinner) {
            m_animIdle.SetTrigger(TRIGGER_WIN);
        } else {
            m_animIdle.SetTrigger(TRIGGER_LOSE);
        }
    }

    ///////////////
    // others
    public Transform MyBodyRef() {
        if (m_bodyIdle.GetComponentInChildren<SkinnedMeshRenderer>().enabled) {
            return m_bodyIdle;
        } else {
            return m_body;
        }
    }
    void SmoothRePositioning(Transform me, Vector3 target, bool look = false) {
        if (m_reposCoroutine == null) {
            m_reposCoroutine = StartCoroutine(doReposition(me, target, look));
        } else {
            me.position = target;
            if (look) LookStraight(me, me.position + me.forward);
        }
    }

    IEnumerator doReposition(Transform me, Vector3 target, bool look) {
        while (me.position != target) {
            if (look) LookStraight(me, target);
            me.position = Vector3.MoveTowards(me.position, target, smoothReturn * Time.deltaTime);
            yield return null;
        }

        if (look) LookStraight(me, me.position + me.forward);
        
        // self kill
        StopCoroutine(m_reposCoroutine);
    }

    void LookStraight(Transform me, Vector3 target) {
        Vector3 lookPoint = target;
        lookPoint.y = me.position.y;

        me.LookAt(lookPoint);
    }

    void switchRenderIdle(bool idle) {
        SkinnedMeshRenderer[] renders = m_body.GetComponentsInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] renderIdles = m_bodyIdle.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer r in renders) {
            r.enabled = !idle;
        }

        foreach (SkinnedMeshRenderer r in renderIdles) {
            r.enabled = idle;
        }
    }
}
