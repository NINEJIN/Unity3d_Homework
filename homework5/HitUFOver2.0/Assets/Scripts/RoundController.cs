using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFO;

namespace HitUFO
{
    public interface UI
    {
        void throwUfo();
        void setMode(bool mode);
        // For mode: true is Dynamics, and false is Kinematics
    }

    public interface GameState
    {
        bool isShooting();
        int getRound();
        int getScore();
        void toNextRound();
        void setScore(int o);
    }


    public class SceneController : System.Object, UI, GameState
    {
        private static SceneController instance;
        private RoundController basecode;
        private Manager manager;

        private int round = 0;
        private int score;

        public static SceneController getInstance()
        {
            if (instance == null) instance = new SceneController();
            return instance;
        }

        public void setManager (Manager obj) { manager = obj; }
        internal Manager getManager() { return manager; }

        public void setRoundController(RoundController obj) { basecode = obj; }
        internal RoundController getRoundController() { return basecode; }


        public void throwUfo()
        {
            manager.Ready();
        }

        public void setMode(bool mode)
        {
            manager.setMode(mode);
        }
        
        public bool isShooting() { return manager.isShooting(); }
        public int getRound() { return round; }
        public int getScore() { return score; }


        public void setScore(int i) { score = i; }
        public void toNextRound()
        {
            score = 0;
            basecode.loadUfo(++round);
        }
    }
}


public class RoundController : MonoBehaviour {
    private Color color;
    private Vector3 pos;
    private Vector3 dir;
    private float speed = 0.03f;

    private void Awake()
    {
        SceneController.getInstance().setRoundController(this);
    }

    public void loadUfo(int round)
    {
        // 随机生成颜色
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        color = new Color(r, g, b);
        pos = new Vector3(0, 0, 0);
        SceneController.getInstance().getManager().setting(1, color, pos, speed, round);
    }
}
