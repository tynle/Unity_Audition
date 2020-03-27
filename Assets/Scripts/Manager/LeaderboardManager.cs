using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : SingletonMono<GameManager>
{
    private List<Record> m_scoreBoard;
    public List<string> debugScoreBoard;
    public List<int> debugScore;

    public enum GAMESCORE {
        MISS = -10,
        COOL = 15,
        GREAT = 25,
        PERFECT = 50
    }

    public struct Record
    {
        public int id;
        public string name;
        public int score;

        public void Set (int _id, string _name, int _score = 0) {
            id = _id;
            name = _name;
            score = _score;
        } 
    }

    void Awake() {
        m_scoreBoard = new List<Record>();
        debugScoreBoard = new List<string>();
        debugScore = new List<int>();
    }

    void Update() {
        debug();
    }

    void debug() {
        debugScoreBoard.Clear();
        debugScore.Clear();
        foreach(Record rec in m_scoreBoard) {
            debugScoreBoard.Add(rec.name);
            debugScore.Add(rec.score);
        }
    }

    public void register(int _id, string _name) {
        Record rec = new Record();
        rec.Set(_id, _name);
        m_scoreBoard.Add(rec);
    }

    public void reset() {
        m_scoreBoard.Clear();
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
    }

    private int getHighestScore() {
        int highestScore = m_scoreBoard[0].score;
        foreach(Record rec in m_scoreBoard) {
            if (rec.score > highestScore) {
                highestScore = rec.score;
            }
        }
        return highestScore;
    }

    public bool resultOf(int _id) {
        int highestScore = getHighestScore();
        foreach (Record rec in m_scoreBoard) {
            if (rec.id == _id) {
                return (rec.score == highestScore);
            }
        }
        return false;
    }

    public List<int> getWinners() {
        int highestScore = getHighestScore();
        List<int> winners = new List<int>();
        
        foreach (Record rec in m_scoreBoard) {
            if (rec.score == highestScore) {
                winners.Add(rec.id);
            }
        }
        return winners;
    }
}