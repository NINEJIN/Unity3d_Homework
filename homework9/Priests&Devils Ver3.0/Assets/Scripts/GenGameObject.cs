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
    //Vector3 RiverPos = new Vector3(0, -3.2f, 0);
    Vector3 BoatRightPos = new Vector3(7, -3, 0);
    Vector3 BoatLeftPos = new Vector3(-7, -3, 0);
    Vector3 OnBoatRight = new Vector3(1, 0, 0);
    Vector3 OnBoatLeft = new Vector3(-1, 0, 0);

    // 设关于船的各种变量，包括船对象、船上对象、船上人数、船的速度、船的行驶距离
    GameObject boat;
    GameObject[] OnBoat = new GameObject[2];    
    public int Boater = 0;    
    public float speed = 25f;
    public float distance = 2f;
    int side = 1; // 1 for right, 0 for left

    // 场景控制my
    GameSceneController my;

    // AI次状态表，以枚举型矩阵形式存储
    private enum act { LH, LE, LHE, RHE, RHH, REE , x};
    private act[,] matLeft = new act[4, 4] {{ act.x, act.LE, act.LE, act.x },
                                    { act.x, act.LHE, act.x, act.x },
                                    { act.x, act.x, act.LH, act.x },
                                    { act.LE, act.LE, act.LE, act.x }};
    private act[,] matRight = new act[4, 4] {{ act.x, act.x, act.REE, act.REE },
                                    { act.x, act.x, act.x, act.x },
                                    { act.x, act.x, act.RHH, act.x },
                                    { act.x, act.RHH, act.REE, act.RHE }};

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
       // Instantiate(Resources.Load("Prefabs/River"), RiverPos, Quaternion.identity);

        // 加载船
        boat = Instantiate(Resources.Load("Prefabs/Boat"), BoatRightPos, Quaternion.identity) as GameObject;

        // 加载人与恶魔
        for (int i = 0; i < 3; i++)
        {
            GameObject temp;
            temp = Instantiate(Resources.Load("Prefabs/human"), new Vector3(12 + 2 * i, 1, 0), Quaternion.Euler(0, -90, 0)) as GameObject;
            temp.tag = "Human";
            HumansOnRight.Push(temp);

            temp = Instantiate(Resources.Load("Prefabs/evil"), new Vector3(18 + 2 * i, 1, 0), Quaternion.Euler(0, -90, 0)) as GameObject;
            temp.tag = "Evil";
            EvilsOnRight.Push(temp);
        }

        // 加载灯光
        Instantiate(Resources.Load("Prefabs/Light"));

    }

    public void moveBoat()
    {
        if (Boater > 0)
        {
            if (side == 1)
            {
                ActionManager.GetInstance().ApplyMoveToAction(boat, BoatLeftPos, speed);
                side = 0;
            }
            else if (side == 0)
            {
                ActionManager.GetInstance().ApplyMoveToAction(boat, BoatRightPos, speed);
                side = 1;
            }
        }
    }

    // 指定一个对象上船
    void GetOn(GameObject obj)
    {
        if (Boater == 2) return;    // 若船上满人则拒绝操作

        Vector3 target = new Vector3();
        obj.transform.parent = boat.transform; //将船设置为上船对象的父体

        if (OnBoat[0] == null)  //左侧上船
        {
            OnBoat[0] = obj;
            //obj.transform.localPosition = new Vector3(-0.2f, 5, 0);
            target = boat.transform.position + new Vector3(-1f, 1, 0);
        }
        else    // 右侧下船
        {
            OnBoat[1] = obj;
            //obj.transform.localPosition = new Vector3(0.2f, 5, 0);
            target = boat.transform.position + new Vector3(1f, 1, 0);
        }
        Boater++;
        ActionManager.GetInstance().ApplyMoveToMulAction(obj, target, speed);
        //print(target);
    }
    
    // 指定船上一个对象下船
    public void GetOff(int BoatNum)
    {
        if (Boater == 0) return;

        OnBoat[BoatNum].transform.parent = null;    // 取消父系关系
        Vector3 target = new Vector3();

        if (side == 1)       // 在右岸下船
        {
            if (OnBoat[BoatNum].tag == "Evil")
            {
                EvilsOnRight.Push(OnBoat[BoatNum]);
                target = new Vector3(18 + 2 * (EvilsOnRight.Count - 1), 1, 0);
            }                
            else if (OnBoat[BoatNum].tag == "Human")
            {
                HumansOnRight.Push(OnBoat[BoatNum]);
                target = new Vector3(12 + 2 * (HumansOnRight.Count - 1), 1, 0);
            }                            
        }
        else if (side == 0)   // 在左岸下船
        {
            if (OnBoat[BoatNum].tag == "Evil")
            {
                EvilsOnLeft.Push(OnBoat[BoatNum]);
                target = new Vector3(-18 - 2 * (EvilsOnLeft.Count - 1), 1, 0);
            }
                
            else if (OnBoat[BoatNum].tag == "Human")
            {
                HumansOnLeft.Push(OnBoat[BoatNum]);
                target = new Vector3(-12 - 2 * (HumansOnLeft.Count - 1), 1, 0);
            }                     
        }

        ActionManager.GetInstance().ApplyMoveToMulAction(OnBoat[BoatNum], target, speed);
        OnBoat[BoatNum] = null;
        Boater--;
    }

    // 判断人类上船状态
    public void HumanGetOnBoat()
    {
        if (side == 0)
        {
            if (HumansOnLeft.Count != 0 && Boater < 2)
                GetOn(HumansOnLeft.Pop());
        }
        else if (side == 1)
        {
            if (HumansOnRight.Count != 0 && Boater < 2)
                GetOn(HumansOnRight.Pop());
        }
    }

    // 判断恶魔上船状态
    public void EvilsGetOnBoat()
    {
        if (side == 0)
        {
            if (EvilsOnLeft.Count != 0 && Boater < 2)
                GetOn(EvilsOnLeft.Pop());
        }
        else if (side == 1)
        {
            if (EvilsOnRight.Count != 0 && Boater < 2)
                GetOn(EvilsOnRight.Pop());
        }
    }

    // 自动执行最优解步骤
    public void AutoAct()
    {
        int h = HumansOnRight.Count;
        int e = EvilsOnRight.Count;
        if (side == 0)
        {
            // 船在左侧，右岸增员
            act nextMove = matLeft[h, e];
            switch (nextMove)
            {
                case act.LE:
                    GetOn(EvilsOnLeft.Pop());
                    break;
                case act.LH:
                    GetOn(HumansOnLeft.Pop());
                    break;
                case act.LHE:
                    GetOn(HumansOnLeft.Pop());
                    GetOn(EvilsOnLeft.Pop());
                    break;
                default:
                    break;
            }
            /*
            if ( (h == 3 && e < 3) || (h == 0 && e < 3))
            {
                GetOn(EvilsOnLeft.Pop());
            }
            if ((h == 2 && e == 2) )
            {
                GetOn(HumansOnLeft.Pop());
            }
            if ((h == 1 && e == 1))
            {
                GetOn(HumansOnLeft.Pop());
                GetOn(EvilsOnLeft.Pop());
            }
            */

        }
        else if (side == 1)
        {
            // 船在右侧，右岸减员
            act nextMove = matRight[h, e];
            switch (nextMove)
            {
                case act.REE:
                    GetOn(EvilsOnRight.Pop());
                    GetOn(EvilsOnRight.Pop());
                    break;
                case act.RHH:
                    GetOn(HumansOnRight.Pop());
                    GetOn(HumansOnRight.Pop());
                    break;
                case act.RHE:
                    GetOn(EvilsOnRight.Pop());
                    GetOn(HumansOnRight.Pop());
                    break;
                default:
                    break;
            }
            /*
            if (h == 3 && e == 3)
            {
                GetOn(EvilsOnRight.Pop());
                GetOn(HumansOnRight.Pop());
            }
            if ((h == 3 && e == 2) || (h == 0 && e > 1))
            {
                GetOn(EvilsOnRight.Pop());
                GetOn(EvilsOnRight.Pop());
            }
            if ((h == 3 && e == 1) || (h == 2 && e == 2))
            {
                GetOn(HumansOnRight.Pop());
                GetOn(HumansOnRight.Pop());
            }
            */
        }
    }

    void Check()
    {
        // 判断左岸是否6人到达，若是则胜利
        if (HumansOnLeft.Count == 3 && EvilsOnLeft.Count == 3)
        {
            //my.state = GameState.Win;
            my.setMessage("You Win !");
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
        if (side == 0)
        {
            numH = HumansOnLeft.Count + BoatH;
            numE = EvilsOnLeft.Count + BoatE;
            if (HumansOnRight.Count < EvilsOnRight.Count && HumansOnRight.Count > 0)
                my.setMessage("You Lose !");
        }
        else if (side == 1)
        {
            numH = HumansOnRight.Count + BoatH;
            numE = EvilsOnRight.Count + BoatE;
            if (HumansOnLeft.Count < EvilsOnLeft.Count && HumansOnLeft.Count > 0)
                my.setMessage("You Lose !");
        }

        if (numH > 0 && numH < numE)
            my.setMessage("You Lose !");
    }

    // 游戏更新函数
    private void Update()
    {
        Check();
    }


}
