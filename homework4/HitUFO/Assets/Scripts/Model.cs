using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFO;

public class Model : MonoBehaviour
{

    private SceneController scene;
    private int oneScore = 10;
    private int winUfo = 3;

    public float cd = 3f;
    public float timer;
    private bool counting;
    private bool shooting;
    public bool isCounting() { return counting; }
    public bool isShooting() { return shooting; }

    private List<GameObject> ufos = new List<GameObject>();    // 发射的飞碟对象列表  
    private List<int> ids = new List<int>();                // 发射的飞碟id列表  
    private int scale;                  // 飞碟大小  
    private Color color;                // 飞碟颜色  
    private Vector3 pos;           // 发射位置  
    private Vector3 dir;          // 发射方向  
    private float speed;                // 发射速度  
    private int ufoNum;                 // 发射数量  
    private bool canThrow;                // 允许新的发射事件 

    void Awake()
    {
        scene = SceneController.getInstance();
        scene.setModel(this);
    }

    public void setting(int _scale, Color _color, Vector3 _pos/*, Vector3 _dir*/, float _speed, int _num)
    {
        scale = _scale;
        color = _color;
        pos = _pos;
        //dir = _dir;
        speed = _speed;
        ufoNum = _num;
    }

    public void Ready()
    {
        // 不在计时也不在射击的话，即可以发射ufo
        if (!counting && !shooting)
        {
            timer = cd;
            canThrow = true;
        }
    }

    void Throw()
    {
        //UfoFactory it = UfoFactory.getInstance();
        for (int i = 0; i < ufoNum; i++)
        {

            float x = Random.Range(-50f, 50f);
            float y = Random.Range(40f, 80f);
            float z = Random.Range(40f, 60f);
            dir = new Vector3(x, y, z);

            ids.Add(UfoFactory.getInstance().getUfoId());
            ufos.Add(UfoFactory.getInstance().getUfo(ids[i]));
            ufos[i].transform.localScale *= scale;
            ufos[i].GetComponent<Renderer>().material.color = color;
            ufos[i].transform.position = new Vector3(pos.x, pos.y, pos.z + i * 2);
            ufos[i].SetActive(true);
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
        if (timer > 0)
        {
            counting = true;
            timer -= Time.deltaTime;
        }
        else
        {
            counting = false;
            if (canThrow)
            {
                Throw();
                canThrow = false;
                shooting = true;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < ufos.Count; i++)
        {
            if (!ufos[i].activeInHierarchy)
            {
                scene.setScore(scene.getScore() + oneScore);
                Recycle(i);
            }
            else if (ufos[i].transform.position.y < 0)
            {
                if (scene.getScore() - oneScore < 0) scene.setScore(0);
                else scene.setScore(scene.getScore() - oneScore);
                Recycle(i);
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
