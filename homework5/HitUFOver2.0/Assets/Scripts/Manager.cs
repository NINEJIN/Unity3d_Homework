using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFO;

public class Manager : MonoBehaviour
{
    private SceneController scene;
    private int oneScore = 10;
    private int winUfo = 4;
    
    private bool shooting;
    public bool isShooting() { return shooting; }

    private List<GameObject> ufos = new List<GameObject>(); // 发射的飞碟对象列表  
    private List<int> ids = new List<int>();                // 发射的飞碟id列表  
    private List<int> ufosSpeed = new List<int>();          // 运动学中 飞碟的速度
    private int scale;                                      // 飞碟大小  
    private Color color;                                    // 飞碟颜色  
    private Vector3 pos;                                    // 发射位置  
    private Vector3 dir;                                    // 发射方向  
    private float x, y, z;                                    // 发射向量参数
    private float speed;                                    // 发射速度  
    private int ufoNum;                                     // 发射数量  
    private int throwNum = 0;
    private float timer = 0;
    private float cd = 0.4f;
    private bool canThrow;                                  // 允许新的发射事件 

    private bool gameMode = true;

    void Awake()
    {
        scene = SceneController.getInstance();
        scene.setManager(this);
    }

    public void setting(int _scale, Color _color, Vector3 _pos, float _speed, int _num)
    {
        scale = _scale;
        color = _color;
        pos = _pos;
        speed = _speed;
        ufoNum = _num;
    }

    public void setMode(bool mode)
    {
        gameMode = mode;
    }

    public void Ready()
    {
        // 不在射击的话，即可以发射ufo
        if (!shooting)
           canThrow = true;
    }

    void Throw(int i)
    {        
        x = Random.Range(-50f, 50f);
        y = Random.Range(40f, 80f);
        z = Random.Range(40f, 60f);
        dir = new Vector3(x, y, z);

        if (i >= ufosSpeed.Count)
            ufosSpeed.Add(5);

        ids.Add(UfoFactory.getInstance().getUfoId());
        ufos.Add(UfoFactory.getInstance().getUfo(ids[i]));  
        ufos[i].transform.localScale *= scale;
        ufos[i].GetComponent<Renderer>().material.color = color;
        ufos[i].transform.position = new Vector3(pos.x, pos.y, pos.z + i * 2);
        ufos[i].SetActive(true);

        // 若是刚体模式, 则直接加一个力 （否则则在update上更新运动学动作）
        if (gameMode == true)
        {
            if (ufos[i].GetComponent<Rigidbody>() == null)
                ufos[i].AddComponent<Rigidbody>();

            ufos[i].GetComponent<Rigidbody>().mass = 0.1f;
            ufos[i].GetComponent<Rigidbody>().drag = 0.1f;
            ufos[i].GetComponent<Rigidbody>().angularDrag = 0.05f;
            //ufos[i].GetComponent<Rigidbody>().useGravity = true;
            ufos[i].GetComponent<Rigidbody>().AddForce(dir * Random.Range(speed * 5, speed * 10) / 10, ForceMode.Impulse);
        }
    }

    void Recycle(int i)
    {
        UfoFactory.getInstance().recycleUfo(ids[i]);
        ufos.RemoveAt(i);
        ids.RemoveAt(i);
    }

    // 使用FixedUpdate代替Update处理Rigidbody的刚体运动， 与Update的帧长不同
    void FixedUpdate()
    {
        if (canThrow)
        {
            if (throwNum < ufoNum)
            {
                if (timer == 0)
                    Throw(throwNum++);

                timer += Time.deltaTime;
                if (timer > cd) timer = 0;
            }                
            else
            {
                canThrow = false;
                shooting = true;
                throwNum = 0;
                timer = 0;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < ufos.Count; i++)
        {
            // 若是运动学模式, 则改变位置
            if (gameMode == false)
            {
                if (ufos[i].GetComponent<Rigidbody>() != null)
                    Destroy(ufos[i].GetComponent<Rigidbody>());

                Vector3 dir2 = new Vector3(x, 0, z);

                ufos[i].transform.position += dir2 * Time.deltaTime / 10;
                ufos[i].transform.position += Vector3.up * Time.deltaTime * (100 - ufosSpeed[i]++) / 10;
            }

            if (!ufos[i].activeInHierarchy)
            {
                scene.setScore(scene.getScore() + oneScore);

                if (ufos[i].GetComponent<Rigidbody>() == null)
                    ufos[i].AddComponent<Rigidbody>();

                Recycle(i);
                ufosSpeed[i] = 5;
            }
            else if (ufos[i].transform.position.y < 0)
            {
                if (scene.getScore() - oneScore < 0) scene.setScore(0);
                else scene.setScore(scene.getScore() - oneScore);

                if (ufos[i].GetComponent<Rigidbody>() == null)
                    ufos[i].AddComponent<Rigidbody>();

                Recycle(i);
                ufosSpeed[i] = 5;
            }
        }
        if (ufos.Count == 0)
        {
            shooting = false;
            if (scene.getScore() >= oneScore * winUfo)
                scene.toNextRound();
        }
    }
}
