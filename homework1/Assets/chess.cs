using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chess : MonoBehaviour
{
    public int steps;   // 表示棋子步数
    public int player;  // 表示棋手顺序
    private int[,] room = new int[3, 3]; // 用数组存放棋盘信息

    public Texture2D bg;    // 背景图
    public Texture2D img1;  // 选手1棋子
    public Texture2D img2;  // 选手二棋子

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    // Use this to reset the game
    private void Reset()
    {
        player = 1;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                room[i, j] = 0;
        steps = 0;
    }

    // Use this to check the status of the game
    private int Check()
    {
        for (int i = 0; i < 3; i++)
        {
            //横向判断
            if (room[i, 0] == room[i, 1] && room[i, 0] == room[i, 2])
                return room[i, 0];
            //纵向判断
            if (room[0, i] == room[1, i] && room[0, i] == room[2, i])
                return room[0, i];
        }

        //斜向判断
        if ((room[0, 0] == room[1, 1] && room[0, 0] == room[2, 2]) || (room[0, 2] == room[1, 1] && room[2, 0] == room[1, 1]))
            return room[1, 1];

        if (steps == 9) return 3;  // 若九个格子已满且还未决出胜负则为平局
        return 0; // 未结束
    }

    private void OnGUI()
    {
        GUIStyle bgstyle = new GUIStyle();
        bgstyle.normal.background = bg;
        GUI.Label(new Rect(0, 0, 1024, 781), "", bgstyle);
        // 设置背景

        GUIStyle style1 = new GUIStyle();
        style1.fontSize = 20;
        style1.normal.textColor = Color.black;
        style1.fontStyle = FontStyle.Bold;
        //设计字体样式

        if (GUI.Button(new Rect(415, 350, 100, 50), "Reset"))
            Reset();
        //reset按钮重置游戏       

        switch (Check())
        {
            case 1:
                GUI.Label(new Rect(400, 50, 100, 50), "   小火龙胜出", style: style1);
                break;
            case 2:
                GUI.Label(new Rect(400, 50, 100, 50), "   杰尼龟胜出", style: style1);
                break;
            case 3:
                GUI.Label(new Rect(400, 50, 100, 50), "       平局   ", style: style1);
                break;
        }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (room[i, j] == 1) GUI.Button(new Rect(360 + i * 70, 100 + j * 70, 70, 70), img1);
                else if (room[i, j] == 2) GUI.Button(new Rect(360 + i * 70, 100 + j * 70, 70, 70), img2);
                //添加棋盘+渲染

                if (GUI.Button(new Rect(360 + i * 70, 100 + j * 70, 70, 70), "")) //补充剩余棋盘
                    if (Check() == 0)            //若游戏未结束即可下子
                    {
                        if (player == 1) room[i, j] = 1;    
                            else room[i, j] = 2;            // 下子
                        steps++;
                        player = 1 - player;                //切换棋手
                    }
             }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
