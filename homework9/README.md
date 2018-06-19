
![往期作业](https://img-blog.csdn.net/2018061919444723?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0VkZGllX1Blbmc=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
  在之前的作业中，曾经实现过[牧师与魔鬼][1]这个益智小游戏，但是一些小朋友玩这个游戏的时候可能有些困难（比如十年前在qq空间玩这个游戏的我），因此，我们可以开发一个AutoNext的功能，给小朋友提示一下下一步该怎么操作。
![智能帮助](https://img-blog.csdn.net/20180619194219256?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0VkZGllX1Blbmc=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)

  由于这次的智能设计比较简单，只有三个牧师和三个魔鬼，因此我们可以使用状态图来帮助分析游戏当前的状态，然后根据当前状态执行下一步正确操作即可。以下为状态图的表示，每一个状态记录在起始岸（右岸）上的牧师与魔鬼数。
![状态图](https://img-blog.csdn.net/20180619194552771?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0VkZGllX1Blbmc=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
  需要注意的是，每一次箭头指出之后，船的位置会发生变化，也就是说同样的右岸上3人1鬼，船在左侧和船在右侧的操作是不一样的。即使是3人3鬼这样比较小的数据，其状态还是有很多种的（以至于我画流程图到2人2鬼的时候，实在受不了了，于是就把后面的其他过程省略了...）。然而这么多的状态，能成功的解法却很少，把游戏失败的状态以及多余的步骤去掉之后，我们就能得出这个游戏的正确解法流程。
  ![获胜解法](https://img-blog.csdn.net/20180619194644111?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0VkZGllX1Blbmc=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
如此一来，流程就变得简单多了，接下来就是用代码实现了。首先，当用户选择执行AutoNext操作时，需要判断当前游戏状态，然后执行最优解。举个栗子，如当前船在右岸，岸上3人2鬼，接下来的步骤可以是：

* 1人1鬼上船，岸上剩2人1鬼； 
* 2人上船，岸上剩1人2鬼； 
* 2鬼上船，岸上剩3人； 
* 1人上船，岸上剩2人2鬼； 
* 1鬼上船，岸上剩3人1鬼； 

从状态图中可以看出，无论岸上剩下1人2鬼还是2人1鬼（意味对岸是1人2鬼），游戏都会以失败结束，因此方法1和2是不可行的。其次，方法4和5虽然不会导致游戏失败，但从状态图中可看出方法4和5其实是一种回溯，也就是步骤重复，因此也不是最优解。那么只剩下方法3是可行的方案了。
具体实现如下：

**Step 1.** 在GenGameObject类（创建游戏对象和处理对象运动）中，定义枚举类型act，添加两个枚举型矩阵，分别记录船在左右两岸时不同的游戏状态下的下一步执行操作标记。使用矩阵（状态表）的目的是表达清晰，方便更改维护，且能减少逻辑判断复杂度。
act枚举说明：L（R）表示左岸（右岸）的人物进行上船操作，E表示魔鬼上船，H表示人类（牧师）上船，HE表示1人1鬼上船，HH表示2人上船，EE表示2鬼上船，x代表对应状态不存在或是该状态下游戏结束（失败或胜利）。
[贴一张状态表图片]
```C#
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
```

**Step 2. **同样在GenGameObject类中，设计自动执行最优解步骤的函数（仅上船操作）
```c#
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
        }
    }
```
**Step 3.** 更改UI界面以及AutoNext的相应操作
```c#
if (GUI.Button(new Rect(x + width + 5f, (height + 5f) * 2 + y, width, height), "AutoNext"))
                {
                    // 先使船上的任务上岸，便于判断当前游戏状态
                    action.LeftOff();
                    action.RightOff();
                    // 选择最优解，相应人物上船
                    action.Auto();
                    action.BoatMove();
                    action.LeftOff();
                    action.RightOff();
                }
```

**Step 4. **更新BaseCode类中的函数接口

以上，便是牧师与魔鬼小游戏的智能帮助实现，附上视频连接：[牧师与魔鬼游戏之智能帮助](https://www.bilibili.com/video/av25222176/)


  [1]: https://github.com/sysuxwh/Unity3d_Homework/tree/master/homework3