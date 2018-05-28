#粒子流黑洞

---
当知道要做一个粒子光环的时候，第一反应是最近见到的这张黑洞图，觉得实在太好看了（百度黑洞有惊喜），于是就想照着黑洞的样子大概弄一个出来看看效果如何。话不多说，直接开始。
![黑洞][1]  

**Step 1. 创建对象**  
首先创建一个空对象Halo，为它添加一个ParticleSystem的组件
![添加粒子系统组件][2]

**Step 2. 添加素材**  
为了弄出星空中黑洞的效果，我们先把背景变黑，打开Window-Lighting-Settings，在Scene窗口下将Skybox Material设置为Default-Material即可。另外，我们还要将粒子效果设置为白色发光粒子，在粒子系统组件下的Renderer部分可以设置素材为Default-Material。  
![背景设置][3]

**Step 3. 新建脚本**  
接下来我们就进入正题了——即是粒子流的创建与控制。先创建一个脚本文件Halo.cs，并挂在对象Halo上面。

**Step 4. 设置变量**  
为Halo类设置私有变量和Position的类（用于记录粒子位置信息）。parSys是要用到的粒子系统，它由parArr数组里的所有粒子构成，另外parPos数组记录每一个粒子的位置。
```c#
public class Position  
{  
    public float radius = 0f, angle = 0f;  
    public Position(float r, float a)  
    {  
        radius = r;                         // 半径
        angle = a;                          // 角度
    }  
}  
  
private ParticleSystem parSys;              // 粒子系统  
private ParticleSystem.Particle[] parArr;   // 粒子数组  
private Position[] parPos;                  // 粒子位置数组  
  
public int num = 10000;         // 粒子数量  
public float size = 0.03f;      // 粒子大小  
public float r = 5f, R = 10f;   // 内半径及外半径  
``` 
**Step 5. 粒子初始化**  
先对每一个粒子进行初始化，初始化粒子的起始速度、大小、最大数量等等属性。然后粒子系统就可以发射粒子了！对于每一个粒子的位置，我使用了一个SetRandom()的函数，使每一个粒子的起始位置都不一样，构成一个圆环。
```c#
    void Start () {
        parArr = new ParticleSystem.Particle[num];
        parPos = new Position[num];

        parSys = this.GetComponent<ParticleSystem>();
        var main = parSys.main;
        main.startSpeed = 0;
        main.startSize = size;
        main.loop = false;
        main.maxParticles = num;
        parSys.Emit(num);
        parSys.GetParticles(parArr);

        SetRandom();
	}
```
接下来我们要解决粒子随机分布在圆环上的问题。圆环有内半径和外半径，我们可以适当地调整随机范围来使大部分粒子落在圆环中心。
```c#
    void SetRandom()
    {
        for (int i = 0; i < num; i++)
        {
            float min = Random.Range(1f, (r + R) / (2 * r)) * r;
            float max = Random.Range((r + R) / (2 * R), 1.2f) * R;
            float radius = Random.Range(min, max);
            // 生成随机半径，主要集中在环中间

            float angle = Random.Range(0f, 360f);
            float radian = angle * 180 / Mathf.PI;
            // 生成随机角度
            
            parPos[i] = new Position(radius, angle);
            parArr[i].position =
                new Vector3(parPos[i].radius * Mathf.Cos(radian),
                            0f,
                            parPos[i].radius * Mathf.Sin(radian));
            // 设定粒子的随机位置
        }
        parSys.SetParticles(parArr, parArr.Length);
        // 将所有生成的随机粒子放入粒子系统里
    }
```
运行！看看出来的运行结果如何~结果还算是符合预期。
![初始光环][4]

**Step 6. 粒子流控制**  
然后就是通过Update来更新粒子的状态，也是关键的控制粒子流步骤。我是先调整角度，然后直接使用角度的数据修改每个粒子的position来达到移动的效果。每个粒子的速度每个时刻均是随机产生，各不相等。
```c#
void Update () {
    // 更新每个粒子位置和角度
    for (int i = 0; i < num; i++)
    {
        // 顺时针旋转  
        parPos[i].angle = (parPos[i].angle - Random.Range(0.5f,3f)) % 360f;           
        float radian = parPos[i].angle / 180 * Mathf.PI;
        parArr[i].position = new Vector3(parPos[i].radius * Mathf.Cos(radian), 
                                        0f, 
                                        parPos[i].radius * Mathf.Sin(radian));
    }
    parSys.SetParticles(parArr, parArr.Length);
}
```
这下我们来看下运行效果，感觉还可以。
![光环1][5]

**Step 7. 添加第二个光环**  
再新建一个空对象Halo2，并添加ParticleSystem的组件。设置跟Halo对象一样，只是挂的脚本不一样。在Halo2.cs的脚本中，对Halo.cs略作修改，具体修改地方有：
```c#
    // 修改小环的内外半径
    public float r = 4f, R = 5f;   // 内半径及外半径
    
    //......
    // 初始化粒子的位置，为竖立的圆环
    parArr[i].position =
                new Vector3(parPos[i].radius * Mathf.Cos(radian),
                            parPos[i].radius * Mathf.Sin(radian),
                            0f);
                            
    //......
    // 同理，更新未知的时候亦是
    parArr[i].position = new Vector3(parPos[i].radius * Mathf.Cos(radian), 
                                    parPos[i].radius * Mathf.Sin(radian), 
                                    0f);
```
这时的运行效果是这样的。  
![光环2][6]

**Step 8. 调整光环效果（加特效）**
上面的效果图中，粒子效果还没达到预想中的酷炫。为了加强粒子效果，这里我使用了Glow11插件（如要使用此插件，自行上网下载，或在我github里也可以找到Glow11.unitypackage）。点击Assets-Import Package-Custom Package，选择Glow11即可导入。  
然后给Main Camera添加Glow11组件（Glow11的效果是通过摄像机实现的），粒子的素材依然为Default-Material。这时粒子只是变得明亮了，要想获得更好的效果，调整摄像机的Glow11组件，选择高精度High Percision，boost strength调成3，增强粒子效果。  
![调整Glow11][7]
这时再看运行效果，大功告成！！（好好看啊哈哈哈）  
![最终Halo][8]


  [1]: https://images-cdn.shimo.im/z9PX38ZXQX8NGMsx/image.png!thumbnail
  [2]: https://images-cdn.shimo.im/tKTCR5fzcZkbQ7KI/image.png!thumbnail
  [3]: https://images-cdn.shimo.im/0cA5dG6mfLUSmnm6/image.png!thumbnail
  [4]: https://images-cdn.shimo.im/LUsupjXDbn8vIzoP/image.png!thumbnail
  [5]: https://images-cdn.shimo.im/mYCclF9iECkR2viG/Halo1.gif
  [6]: https://images-cdn.shimo.im/lLI8cUHaoXMIa32K/Halo2.gif
  [7]: https://images-cdn.shimo.im/HNYy511NyFwdCTLq/image.png!thumbnail
  [8]: https://images-cdn.shimo.im/hbR64aaTid8kmXVt/Halo.gif
