using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.mygame;


namespace Com.mygame {

    // 以枚举型表示游戏状态
    public enum GameState { OnTheRight, ToLeft, ToRight, OnTheLeft, Win, Lose };

    // 玩家操作指令
    public interface IUact
    {
        void HumanOn();
        void EvilOn();
        void BoatMove();
        void LeftOff();
        void RightOff();
        void Restart();
    }

    // 游戏场景控制类
    public class GameSceneController: System.Object,IUact {

        public static GameSceneController instance;
        private BaseCode base_code;
        private GenGameObject gen_game_obj;            
        public GameState state = GameState.OnTheRight;

        public static GameSceneController GetInstance()
        {
            if (instance == null)
                instance = new GameSceneController();
            return instance;
        }

        public BaseCode getBaseCode()
        {
            return base_code;
        }

        internal void setBaseCode(BaseCode it)
        {
            if (it == null)
                base_code = it;
        }

        public GenGameObject getGenGameObject()
        {
            return gen_game_obj;
        }

        internal void setGenGameObject(GenGameObject it)
        {
            if (gen_game_obj == null)
                gen_game_obj = it;
        }

        public void HumanOn()
        {
            gen_game_obj.HumanGetOnBoat();
        }

        public void EvilOn()
        {
            gen_game_obj.EvilsGetOnBoat();
        }

        public void BoatMove()
        {
            gen_game_obj.Go();
        }

        public void LeftOff()
        {
            gen_game_obj.GetOff(0);
        }

        public void RightOff()
        {
            gen_game_obj.GetOff(1);
        }

        public void Restart()
        {
            Application.LoadLevel(Application.loadedLevelName);
            state = GameState.OnTheRight;
        }
    }

    
}

public class BaseCode : MonoBehaviour {
	void Start () {
        GameSceneController controller = GameSceneController.GetInstance();
        controller.setBaseCode(this);
	}
}
