。# 1、简答题
---
* **解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系。**

**游戏对象（GameObjects）**  
游戏对象是所有其他组件的容器。游戏对象出现在游戏的场景中，在游戏中的所有物体都是游戏对象。  

**资源（Assets）**  
Assets是可以在游戏或项目中使用的任何项目的表示。  
Assets可能来自Unity之外创建的文件，例如3D模型，音频文件，图像或Unity支持的任何其他类型的文件。还可以在Unity中创建一些资源类型，例如Animator Controller，Audio Mixer或Render Texture。

资源可以被多个对象使用，有些可作为模板被实例化成游戏中具体的对象。


---

* **下载几个游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构）**

![资源的目录组织结构](https://images-cdn.shimo.im/AUy2zEFdEG8vAJMc/image.png!thumbnail)
![对象组织结构](https://images-cdn.shimo.im/Bu5OlhJa7WoBkyyz/image.png!thumbnail)

如图所示是一个吃豆人的游戏。  
**资源的目录组织结构**包括【小精灵blinky的动画设计】、【吃豆人Pacman的动画设计】、【Prefab预设】、【Scripts脚本代码文件】、【Sprites图片精灵】。  
**游戏对象**包括【maze迷宫】、【pacman吃豆人】、【Dot食物】、【blinky小精灵】、【Points小精灵移动路径】。

---


* **编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件**
  - **基本行为包括 Awake() Start() Update() FixedUpdate() LateUpdate()** 
  - **常用事件包括 OnGUI() OnDisable() OnEnable()** 

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour {

    void Awake()
    {
        Debug.Log("Init Awake");
    }

    // Use this for initialization
    void Start () {
        Debug.Log("Init Start");
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Init Update");
	}

    void FixedUpdate()
    {
        Debug.Log("Init Fixed Update");
    }

    void LateUpdate()
    {
        Debug.Log("Init Late Update");
    }

    void OnGUI()
    {
        Debug.Log("On GUI");    
    }

    void OnDisable()
    {
        Debug.Log("On Disable");
    }

    void OnEnable()
    {
        Debug.Log("On Enable");
    }
}
```
  
---

* **查找脚本手册，了解 GameObject，Transform，Component 对象**  
  - **分别翻译官方对三个对象的描述（Description）**  
  
  **GameObject**  
  游戏对象是Unity中代表人物、道具、场景等的基本对象。它们本身不能发挥作用，但它们作为组件的容器能够实现真正的功能。  
  > GameObjects are the fundamental objects in Unity that represent characters, props and scenery. They do not accomplish much in themselves but they act as containers for Components, which implement the real functionality.
  
  **Transform**   
  Transform组件决定了场景中每一个对象的位置、旋转角度、尺寸。每一个游戏对象都有其transform组件。  
  > The Transform component determines the Position, Rotation, and Scale of each object in the scene. Every GameObject has a Transform.  
  
  **Component**  
  组件是游戏中对象的螺母和螺栓。它们是每一个游戏对象的功能块。  
  > Components are the nuts & bolts of objects and behaviors in a game. They are the functional pieces of every GameObject.
  
  - **描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件**    
  本题目要求是把可视化图形编程界面与 Unity API 对应起来，当你在 Inspector 面板上每一个内容，应该知道对应 API。  
  例如：table 的对象是 GameObject，第一个选择框是 activeSelf 属性。  

  ![table](https://pmlpml.github.io/unity3d-learning/images/ch02/ch02-homework.png)
  table对象是GameObject，由数个部件组成，六个选择框依次是 activeSelf属性、transform属性、Mesh Filter属性、Box Collider属性、Mesh Rederer属性、Default-Material属性；  
  其中，table的Transform部件描述了position、rotation以及scale三个属性：  
  position：x为0，y为0，z为0；  
  rotation：x为0，y为0，z为0；  
  scale：x为1，y为1，z为1。  
 
  - **用 UML 图描述 三者的关系（请使用 UMLet 14.1.1 stand-alone版本出图）** 
  ![relation](https://images-cdn.shimo.im/T68bcCEUYDICfxuS/image.png!thumbnail)

---

* **整理相关学习资料，编写简单代码验证以下技术的实现：**
  - 查找对象
  ```c#
  public static GameObject Find(string name)//通过名字查找对象
  public static GameObject FindWithTag(string tagname)//通过标签查找单个对象
  public static GameObject[] FindGameObjectsWithTag(string tagname)//通过标签查找多个对象
  ```
  - 添加子对象
  ```c#
  public static GameObject CreatePrimitive(PrimitiveType p)
  ```
  
  - 遍历对象树
  ```c#
  foreach (Transformchild in transform)
  {
    Debug.Log(child.gameObject.name);
  }
  ```
  
  - 清除所有子对象
  ```c#
  foreach (Transformchild in transform)
  {
    Destroy(child.gameObject);
  }
  ```
---

* **资源预设（Prefabs）与 对象克隆 (clone)**
  * **预设（Prefabs）有什么好处？**  
  预设（prefabs）是一种可以被重复使用的游戏对象，可以通过它创建多个具有相同属性的游戏对象，方便简单。只要prefabs的组件发生改变，以它为模板的对象都会跟着一起改变。
  * **预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？**    
  预设创建出来的对象由预设的组件决定，根据预设组件的变化而变化；    
  而对象克隆出来的对象与本体一样，但是克隆之后两者独立存在，不会彼此影响。    
  * **制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象**
  ```c#
  public GameObject table;

  void Start()
  {
      GameObject instance = (GameObject)Instantiate(table.gameObject, transform.position, transform.rotation); 
  }
  ```
---

* **尝试解释组合模式（Composite Pattern / 一种设计模式）。使用 BroadcastMessage() 方法向子对象发送消息**  
  组合模式，将对象组合成树形结构以表示“部分-整体”的层次结构，让游戏对象之间可以被当成子对象或设置为父对象的方式来连接两个对象。组合模式使得用户对单个对象和组合对象的使用具有一致性。
  ```c#
  //父类
  public class father : MonoBehaviour{
      void Start()
      {
        BroadcastMessage("message");
      }
  }

  //子类
  public class son : MonoBehaviour{
      void message()
      {
        Debug.log("hello!");
      }
  }
  ```
  
  ---
