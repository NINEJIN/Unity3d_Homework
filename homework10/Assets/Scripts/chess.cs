using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class chess : NetworkBehaviour
{
    //四个锚点位置，用于计算棋子落点
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;

    //主摄像机
    public Camera cam;

    //锚点在屏幕上的映射位置
    public Vector3 LTPos;
    public Vector3 RTPos;
    public Vector3 LBPos;
    public Vector3 RBPos;

    // 记录棋子状态
    public SyncListInt cs = new SyncListInt();

    // 记录游戏状态
    [SyncVar]
    public int winner = 0;             //获胜方，1为黑子，-1为白子
    [SyncVar]
    public bool isPlaying = true;      //是否处于对弈状态

    enum turn { black, white };
    [SyncVar]
    turn chessTurn;             //落子顺序

    [SyncVar]
    public bool isRestart;

    //------------------------------------------------------------------

    Vector3 PointPos;           //当前点选的位置
    float gridWidth = 1;        //棋盘网格宽度
    float gridHeight = 1;       //棋盘网格高度
    float minGridDis;           //网格宽和高中较小的一个

    Vector2[,] chessPos;        //存储棋盘上所有可以落子的位置
    int[,] chessState;          //存储棋盘位置上的落子状态

    public Texture2D white;     //白棋子
    public Texture2D black;     //黑棋子

    

    void Start()
    {

        for (int i = 0; i < 225; i++)
            cs.Add(0);

        LeftTop = GameObject.Find("Main Camera/Plane/LeftTop");
        RightTop = GameObject.Find("Main Camera/Plane/RightTop");
        LeftBottom = GameObject.Find("Main Camera/Plane/LeftBottom");
        RightBottom = GameObject.Find("Main Camera/Plane/RightBottom");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        //计算锚点位置
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);

        chessPos = new Vector2[15, 15];
        chessState = new int[15, 15];
        chessTurn = turn.black;

        //计算网格宽度
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;

        //计算落子点位置
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                chessPos[i, j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (chessTurn == turn.black && !isServer)
            return;

        if (chessTurn == turn.white && isServer)
           return;
           

        /*
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
                cs[i * 15 + j] = chessState[i, j];
                //CmdSend(i, j, chessState[i, j]);
                */
        //检测鼠标输入并确定落子状态
        if (isPlaying && Input.GetMouseButtonDown(0))
        {
            PointPos = Input.mousePosition;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    //找到最接近鼠标点击位置的落子点，如果空则落子
                    if (Dis(PointPos, chessPos[i, j]) < minGridDis / 2 && chessState[i, j] == 0)
                    {
                        //根据下棋顺序确定落子颜色
                        chessState[i, j] = chessTurn == turn.black ? 1 : -1;

                        // 更新全局变量
                        cs[i * 15 + j] = chessState[i, j];                            
                        //CmdSend(i, j, chessState[i, j]);

                        //落子成功，更换下棋顺序
                        chessTurn = chessTurn == turn.black ? turn.white : turn.black;
                    }
                }
            }
            //调用判断函数，确定是否有获胜方
            int re = result();
            if (re == 1)
            {
                winner = 1;
                isPlaying = false;
            }
            else if (re == -1)
            {
                winner = -1;
                isPlaying = false;
            }
        }

        if (isRestart)
            restart();
    }
    //计算平面距离函数
    float Dis(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    void OnGUI()
    {

        // 绘制棋子
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (cs[i * 15 + j] == 1)
                {
                    //GUI.Button(new Rect(chessPos[i, j].x - gridWidth / 2, Screen.height - chessPos[i, j].y - gridHeight / 2, gridWidth, gridHeight), "B");
                    GUI.DrawTexture(new Rect(chessPos[i, j].x - gridWidth / 2, Screen.height - chessPos[i, j].y - gridHeight / 2, gridWidth, gridHeight), black);
                }
                if (cs[i * 15 + j] == -1)
                {
                    GUI.DrawTexture(new Rect(chessPos[i, j].x - gridWidth / 2, Screen.height - chessPos[i, j].y - gridHeight / 2, gridWidth, gridHeight), white);
                }
            }
        }
        //根据获胜状态，弹出相应的胜利图片
        if (winner == 1)
        {
            if (GUI.Button(new Rect(Screen.width * 0.45f, Screen.height * 0.45f, Screen.width * 0.1f, Screen.height * 0.1f), "黑子胜利！"))
                isRestart = true;
        }
        else if (winner == -1)
        {
            if (GUI.Button(new Rect(Screen.width * 0.45f, Screen.height * 0.45f, Screen.width * 0.1f, Screen.height * 0.1f), "白子胜利！"))
                isRestart = true;
        }
        else
            isRestart = false;


    }

    // 游戏重置
    void restart()
    {
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
            {
                chessState[i, j] = 0;
                cs[i * 15 + j] = 0;
            }
                
        isPlaying = true;
        chessTurn = turn.black;
        winner = 0;
    }

    //检测是够获胜的函数，不含黑棋禁手检测
    int result()
    {
        for (int i = 0; i < 15; i++)
            for (int j = 0; j < 15; j++)
            {
                int sum = 0;
                if (j < 11)
                {
                    //横向 →
                    sum = chessState[i, j] + chessState[i, j + 1] + chessState[i, j + 2] + chessState[i, j + 3] + chessState[i, j + 4];
                    if (sum == 5)  return 1;    // 黑子胜
                    if (sum == -5) return -1;   // 白子胜
                }
                if (i < 11)
                {
                    //纵向 ↓
                    sum = chessState[i, j] + chessState[i + 1, j] + chessState[i + 2, j] + chessState[i + 3, j] + chessState[i + 4, j];
                    if (sum == 5) return 1;
                    if (sum == -5) return -1;
                }
                if (i < 11 && j < 11)
                {
                    // 右斜线 ↘
                    sum = chessState[i, j] + chessState[i + 1, j + 1] + chessState[i + 2, j + 2] + chessState[i + 3, j + 3] + chessState[i + 4, j + 4];
                    if (sum == 5) return 1;
                    if (sum == -5) return -1;
                }
                if (i >= 4 && j < 11)
                {
                    // 左斜线 ↗
                    sum = chessState[i, j] + chessState[i - 1, j + 1] + chessState[i - 2, j + 2] + chessState[i - 3, j + 3] + chessState[i - 4, j + 4];
                    if (sum == 5) return 1;
                    if (sum == -5) return -1;
                }
            }
        return 0; // 胜负未分
    }

    [Command]
    void CmdSend(int i, int j, int state)
    {
        RpcPlus1(i, j, state);
    }

    [ClientRpc]
    void RpcPlus1(int i, int j, int state)
    {
        cs[i * 15 + j] = state;
    }
}