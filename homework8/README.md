# Unity3d - Quest Log 公告牌

---

先看看预览图吧~
![预览图][1]

## 第一步 建立场景
首先，新建canvas，在canvas下新建一个scoll view(滚动视图)。然后在Scroll View -> Viewport -> Content下添加垂直列表组件vertical layout group，再添加三个Button和Text用于显示公告。  
![新建canvas][2]

然后调一调垂直列表Vertical Layout Group中的间距还有自适应宽高等的属性。  
![调整Vertical Layout Group][3]

再调一调公告牌Scroll View的大小。  
![调整Scroll View][4]

给canvas添加image组件，用来显示背景图片。注意的是，添加图片时要将其Texture Type变为Sprite(2D and UI)，不然等下无法添加图片。此外，也可以给公告牌Scroll View添加背景图片。  
![添加image][5]

接下来界面设计就差不多了，可以按照个人喜好调整一下button和text的文字颜色大小、button的图片等等，由于这次时间比较紧张就=_=随意一点了。另外如果文本没有完整显示的话可以调整一下text的高度，还有显示位置不够的话也可以调整Vertical Layout Group的高度。调整完后效果是这样的：
![效果图][6]


## 第二步 实现公告内容的显示和隐藏
这一部分的目的是实现公告内容的隐藏（收缩）和显示（展开）。从师兄的博客中，我了解到了C#一个很强大而且很实用的接口IEnumerator——协程。简单来说，协程就是：你可以写一段顺序的代码，然后标明哪里需要暂停，然后在下一帧或者一段时间后，系统会继续执行这段代码。具体使用方法如下：
```c#
IEnumerator LongComputation()
{
    while(someCondition)
    {
        /* 做一系列的工作 */
        // 在这里暂停然后在下一帧继续执行
        yield return null;
    }
}
```
IEnumerator迭代器和yield关键字需要配合使用，有兴趣的可以多了解一下：[Unity3d IEnumerator 协程的理解][7]  
那么具体到这里的使用，我们可以通过逐一帧改变公告内容的高度来实现显示隐藏功能。废话不多说，直接上代码：
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    // 分别对应点击的按钮和缩放的文本内容
    private Button button;
    public Text text;

    // 公告文本的高度
    private float height;

    // 公告内容变化帧数
    private int frame = 20;
    

    void Start()
    {
        // 获取button，添加监听事件
        button = this.gameObject.GetComponent<Button>();
        button.onClick.AddListener(showOrHide);

        // 并获取公告文本的高度，以便还原
        height = text.rectTransform.sizeDelta.y;
    }

    void showOrHide()
    {
        // 判断公告文本是否激活，分别选择隐藏或显示
        if (text.gameObject.activeSelf)
            StartCoroutine(hide());
        else
            StartCoroutine(show());

    }

    IEnumerator hide()
    {
        float h = height;

        for (int i = 0; i < frame; i++)
        {
            h -= height / frame;
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, h);
            // 在这停顿，下一帧再执行
            yield return null;
        }

        text.gameObject.SetActive(false);
        // 禁用文本
    }

    IEnumerator show()
    {
        float h = 0;
        text.gameObject.SetActive(true);
        // 激活文本

        for (int i = 0; i < frame; i++)
        {
            h += height / frame;            
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, h);
            // 在这停顿，下一帧再执行
            yield return null;
        }

    }
}
```
这一部分的代码不多，而且比较容易理解。脚本完成之后挂在各个按钮上面，就大功告成了！  

---

一些废话：公告牌的工作量相比其他几个可选任务其实少很多，本来想做背包系统的，但是发现留给毛概的时间不多了，六个学分的课还是不敢浪，还是赶紧做完去看书吧TAT。


  [1]: https://images-cdn.shimo.im/PKDvt6jdXy00Tz5H/GIF.gif
  [2]: https://images-cdn.shimo.im/SdbrThx7tmMd0SH6/image.png!thumbnail
  [3]: https://images-cdn.shimo.im/0sK4IPPGzsYiikSN/image.png!thumbnail
  [4]: https://images-cdn.shimo.im/tZz8qIrE808e6AwY/image.png!thumbnail
  [5]: https://images-cdn.shimo.im/3KE36P0yPl8SMEak/image.png!thumbnail
  [6]: https://images-cdn.shimo.im/RqEwadR4IW4nBfRH/image.png!thumbnail
  [7]: https://blog.csdn.net/jasonwang18/article/details/55519165
