using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.mygame;

public class IUserInterface : MonoBehaviour
{

    GameSceneController my;
    IUact action;
    GameState state;

    // width 代表 按钮宽度， height 代表 按钮高度， x 代表 第一个按钮的横坐标， y 代表 第一个按钮的纵坐标
    float width = Screen.width / 10 , height = Screen.height / 12;
    float x = Screen.width / 2.5f, y = Screen.height / 16;

    private void Start()
    {
        my = GameSceneController.GetInstance();
        action = GameSceneController.GetInstance() as IUact;
        state = GameSceneController.GetInstance() as GameState;
    }

    private void OnGUI()
    {
        // 显示当前状态
        string msg = state.isMessage();

        if (msg != "")
        {
            if (GUI.Button(new Rect(x, (height + 5f) * 2 + y, width * 2 + 5f, height), msg))
                action.Restart();            
        }
        else
        {
            if (!state.isMoving())
            {
                if (GUI.Button(new Rect(x, (height + 5f) * 2 + y,  width, height ), "Go"))
                {
                    action.BoatMove();
                }
                if (GUI.Button(new Rect(x + width + 5f, (height + 5f) * 2 + y, width, height), "AutoNext"))
                {
                    action.LeftOff();
                    action.RightOff();

                    
                    action.Auto();
                    action.BoatMove();
                  
                    action.LeftOff();
                    action.RightOff();
                }
                if (GUI.Button(new Rect(x, y, width, height), "Evil On"))
                {
                    action.EvilOn();
                }
                if (GUI.Button(new Rect(x + width + 5f, y, width, height), "Human On"))
                {
                    action.HumanOn();
                }
                if (GUI.Button(new Rect(x, y + height + 5f, width, height), "Left Off"))
                {
                    action.LeftOff();
                }
                if (GUI.Button(new Rect(x + width + 5f, y + height + 5f, width, height), "Right Off"))
                {
                    action.RightOff();
                }
            }
        }
    }
}
