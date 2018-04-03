using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.mygame;

public class GenGameObject : MonoBehaviour
{

    // 使用stack记录两岸的人/鬼
    Stack<GameObject> HumansOnLeft = new Stack<GameObject>();  
    Stack<GameObject> HumansOnRight = new Stack<GameObject>();
    Stack<GameObject> EvilsOnLeft = new Stack<GameObject>();
    Stack<GameObject> EvilsOnRight = new Stack<GameObject>();

    // 预设草地、河流以及船的position
    Vector3 GrassRightPos = new Vector3(17.5f, -2, 0);
    Vector3 GrassLeftPos = new Vector3(-17.5f, -2, 0);
    Vector3 RiverPos = new Vector3(0, -2.2f, 0);
    Vector3 BoatRightPos = new Vector3(7, 0, 0);
    Vector3 BoatLeftPos = new Vector3(-7, 0, 0);

    // 设关于船的各种变量，包括船对象、船上对象、船上人数、船的速度、船的行驶距离
    GameObject boat;
    GameObject[] OnBoat = new GameObject[2];    
    public int Boater = 0;    
    public float speed = 25f;
    public float distance = 2f;

    // 场景控制my
    GameSceneController my;

    // 游戏开始时
    private void Start()
    {
        my = GameSceneController.GetInstance();
        my.setGenGameObject(this);
        LoadSrc();
    }

   // 加载场景
    void LoadSrc()
    {
        // 加载两岸及河流
        Instantiate(Resources.Load("Prefabs/Grass"), GrassRightPos, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/Grass"), GrassLeftPos, Quaternion.identity);
        Instantiate(Resources.Load("Prefabs/River"), RiverPos, Quaternion.identity);

        // 加载船
        boat = Instantiate(Resources.Load("Prefabs/Boat"), BoatRightPos, Quaternion.identity) as GameObject;

        // 加载人与恶魔
        for (int i = 0; i < 3; i++)
        {
            HumansOnRight.Push(Instantiate(Resources.Load("Prefabs/human"), new Vector3(12 + 2 * i, 1, 0), Quaternion.Euler(0, -90, 0)) as GameObject);
            EvilsOnRight.Push(Instantiate(Resources.Load("Prefabs/evil"), new Vector3(18 + 2 * i, 1, 0), Quaternion.Euler(0, -90, 0)) as GameObject);
        }

        // 加载灯光
        Instantiate(Resources.Load("Prefabs/Light"));

    }

    // 设置传入对象(stack)中各对象的位置
    void SetPosition(Stack<GameObject> it, Vector3 pos)
    {
        GameObject[] arr = it.ToArray();
        for (int i = 0; i < it.Count; i++)
            arr[i].transform.position = new Vector3(pos.x + 2 * i, pos.y, pos.z);
    }

    // 指定一个对象上船
    void GetOn(GameObject obj)
    {
        if (Boater == 2) return;    // 若船上满人则拒绝操作

        obj.transform.parent = boat.transform; //将船设置为上船对象的父体

        if (OnBoat[0] == null)  //左侧上船
        {
            OnBoat[0] = obj;
            obj.transform.localPosition = new Vector3(-0.2f, 5, 0);
        }
        else    // 右侧下船
        {
            OnBoat[1] = obj;
            obj.transform.localPosition = new Vector3(0.2f, 5, 0);
        }
        
        Boater++;
    }

    // 开船
    public void Go()
    {
        if (Boater != 0)
        {
            if (my.state == GameState.OnTheRight)
                my.state = GameState.ToLeft;
            else if (my.state == GameState.OnTheLeft)
                my.state = GameState.ToRight;
        }
    }

    // 指定船上一个对象下船
    public void GetOff(int BoatNum)
    {
        if (Boater == 0) return;

        OnBoat[BoatNum].transform.parent = null;    // 取消父系关系

        if (my.state == GameState.OnTheRight)       // 在右岸下船
        {
            if (OnBoat[BoatNum].tag == "Evil")
                EvilsOnRight.Push(OnBoat[BoatNum]);
            else if (OnBoat[BoatNum].tag == "Human")
                HumansOnRight.Push(OnBoat[BoatNum]);            
        }
        else if (my.state == GameState.OnTheLeft)   // 在左岸下船
        {
            if (OnBoat[BoatNum].tag == "Evil")
                EvilsOnLeft.Push(OnBoat[BoatNum]);
            else if (OnBoat[BoatNum].tag == "Human")
                HumansOnLeft.Push(OnBoat[BoatNum]);            
        }

        OnBoat[BoatNum] = null;
        Boater--;
    }

    void Check()
    {
        // 判断左岸是否6人到达，若是则胜利
        if (HumansOnLeft.Count == 3 && EvilsOnLeft.Count == 3)
        {
            my.state = GameState.Win;
            return;
        }

        // 计算船上人、鬼数量
        int BoatH = 0, BoatE = 0;
        for (int i = 0; i < 2; i++)
        {
            if (OnBoat[i] != null && OnBoat[i].tag == "Human") BoatH++;
            if (OnBoat[i] != null && OnBoat[i].tag == "Evil") BoatE++;
        }

        // 计算两岸上人鬼数量对比
        int numH = 0, numE = 0;
        if (my.state == GameState.OnTheLeft)
        {
            numH = HumansOnLeft.Count + BoatH;
            numE = EvilsOnLeft.Count + BoatE;
            if (HumansOnRight.Count < EvilsOnRight.Count && HumansOnRight.Count > 0)
                my.state = GameState.Lose;
        }
        else if (my.state == GameState.OnTheRight)
        {
            numH = HumansOnRight.Count + BoatH;
            numE = EvilsOnRight.Count + BoatE;
            if (HumansOnLeft.Count < EvilsOnLeft.Count && HumansOnLeft.Count > 0)
                my.state = GameState.Lose;
        }

        if (numH > 0 && numH < numE)
            my.state = GameState.Lose;
    }

    // 判断人类上船状态
    public void HumanGetOnBoat()
    {
        if (my.state == GameState.OnTheLeft || my.state == GameState.ToRight)
        {
            if (HumansOnLeft.Count != 0 && Boater < 2)
                GetOn(HumansOnLeft.Pop());
        }
        else if (my.state == GameState.OnTheRight || my.state == GameState.ToLeft)
        {
            if (HumansOnRight.Count != 0 && Boater < 2)
                GetOn(HumansOnRight.Pop());
        }
    }

    // 判断恶魔上船状态
    public void EvilsGetOnBoat()
    {
        if (my.state == GameState.OnTheLeft)
        {
            if (EvilsOnLeft.Count != 0 && Boater < 2)
                GetOn(EvilsOnLeft.Pop());
        }
        else if (my.state == GameState.OnTheRight)
        {
            if (EvilsOnRight.Count != 0 && Boater < 2)
                GetOn(EvilsOnRight.Pop());
        }
    }

    // 游戏更新函数
    private void Update()
    {
        SetPosition(HumansOnRight, new Vector3(12, 1, 0));
        SetPosition(EvilsOnRight, new Vector3(18, 1, 0));
        SetPosition(HumansOnLeft, new Vector3(-16, 1, 0));
        SetPosition(EvilsOnLeft, new Vector3(-22, 1, 0));

        // 若船在移动，则完成移动动作
        if (my.state == GameState.ToLeft)
        {
            boat.transform.position = Vector3.MoveTowards(boat.transform.position, BoatLeftPos, speed * Time.deltaTime);
            if (boat.transform.position == BoatLeftPos)
                my.state = GameState.OnTheLeft;
        }
        else if (my.state == GameState.ToRight)
        {
            boat.transform.position = Vector3.MoveTowards(boat.transform.position, BoatRightPos, speed * Time.deltaTime);
            if (boat.transform.position == BoatRightPos)
                my.state = GameState.OnTheRight;
        }
        // 若船不在移动，则判断游戏状态
        else Check();
    }


}
