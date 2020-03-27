using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : SingletonMono<GameManager>
{
    public enum GAMESCORE {
        MISS = -10,
        COOL = 15,
        GREAT = 25,
        PERFECT = 50
    }

    public struct Record : IComparer<Record>
    {
        public int id;
        public string name;
        public int score;

        public void Set (int _id, string _name, int _score = 0) {
            id = _id;
            name = _name;
            score = _score;
        }
        public int Compare(Record a, Record b) {
            return a.score.CompareTo(b.score);
        } 
    }

    public List<Record> m_scoreBoard;

    void Awake() {
        m_scoreBoard = new List<Record>();
    }

    public void register(int _id, string _name) {
        Record rec = new Record();
        rec.Set(_id, _name);
        m_scoreBoard.Add(rec);
    }

    public void reset() {
        for (int i = 0; i < m_scoreBoard.Count; i++) {
            Record reset = m_scoreBoard[i];
            reset.score = 0;

            m_scoreBoard[i] = reset;
        }
    }

    public void score(int _id, GAMESCORE type) {
        for (int i = 0; i < m_scoreBoard.Count; i++) {
            if (m_scoreBoard[i].id == _id) {
                Record update = m_scoreBoard[i];
                update.score += (int)type;

                m_scoreBoard[i] = update;
                return;
            }
        }
        
        // Sort
        m_scoreBoard.Sort(new Record());
    }

    public bool resultOf(int _id) {
        int highestScore = m_scoreBoard[0].score;
        foreach (Record rec in m_scoreBoard) {
            if (rec.id == _id) {
                return (rec.score == highestScore);
            }
        }
        return false;
    }
}