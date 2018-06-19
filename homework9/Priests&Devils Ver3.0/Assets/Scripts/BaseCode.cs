using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.mygame;


namespace Com.mygame {

    // 以枚举型表示游戏状态
    //public enum GameState { OnTheRight, ToLeft, ToRight, OnTheLeft, Win, Lose }; 
    public interface GameState
    {
        bool isMoving();
        void setMoving(bool state);
        string isMessage();
        void setMessage(string msg);
    }


    // 玩家操作指令
    public interface IUact
    {
        void HumanOn();
        void EvilOn();
        void BoatMove();
        void LeftOff();
        void RightOff();
        void Restart();
        void Auto();
    }

    // 游戏场景控制类
    public class GameSceneController: System.Object, GameState, IUact {

        public static GameSceneController instance;
        private BaseCode base_code;
        private GenGameObject gen_game_obj;
        //public GameState state = GameState.OnTheRight;
        private bool moving = false;
        private string msg = "";

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

        // GameStatus
        public bool isMoving() { return moving; }
        public void setMoving(bool state) { moving = state;  }
        public string isMessage() { return msg;  }
        public void setMessage(string msg) { this.msg = msg;  }

        // 根据用户指令调用对应的动作函数

        public void HumanOn() {  gen_game_obj.HumanGetOnBoat();  }

        public void EvilOn()  {  gen_game_obj.EvilsGetOnBoat();  }

        public void BoatMove() { gen_game_obj.moveBoat();  }

        public void LeftOff()  { gen_game_obj.GetOff(0);  }

        public void RightOff() { gen_game_obj.GetOff(1);  }

        public void Auto() { gen_game_obj.AutoAct(); }

        public void Restart()
        {
            msg = "";
            moving = false;
            Application.LoadLevel(Application.loadedLevelName);
            //state = GameState.OnTheRight;
        }

    }

}

public class BaseCode : MonoBehaviour {
	void Start () {
        GameSceneController controller = GameSceneController.GetInstance();
        controller.setBaseCode(this);
	}
}
