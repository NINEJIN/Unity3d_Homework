using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HitUFO;

public class UserInterface : MonoBehaviour {

    // Text for Game Information
    public Text Score;
    public Text Countdown;
    public Text Round;

    private int roundHint = 0;

    // Bullet's porperties
    public GameObject bullet;
    //public ParticleSystem explosion;  
    public float speed = 1000f;

    // About Fire Time Interval
    public float interval = 0.25f;
    public float nextTime;

    // instance
    private SceneController scene;
    private UI user;
    private GameState state;

    void Start()
    {
        bullet = Instantiate(Resources.Load("Prefabs/bullet")) as GameObject;

        scene = SceneController.getInstance();
        user = SceneController.getInstance() as UI;
        state = SceneController.getInstance() as GameState;

        scene.toNextRound();

        Debug.Log("UserInterface start");
    }
    
	// Update is called once per frame
	void Update () {
        Round.text = "Round : " + state.getRound().ToString();
        Score.text = "Score : " + state.getScore().ToString();

        // 进入下一回合的时候显示提示
        if (roundHint != state.getRound())
        {
            roundHint = state.getRound();
            Countdown.text = "ROUND " + roundHint.ToString() + " !";
        }

        // 按下空格键开始发射飞盘
        if (Input.GetKeyDown("space"))
            user.throwUfo();
        
            
        if (state.isShooting())
        {
            Countdown.text = "";
            if (Input.GetMouseButtonDown(0) && Time.time > nextTime)
            {
                nextTime = Time.time + interval;

                // 射线与碰撞
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // 设置子弹的位置、发射
                bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                bullet.transform.position = this.transform.position;
                bullet.GetComponent<Rigidbody>().AddForce(ray.direction * speed, ForceMode.Impulse);

                // 光线投射，投射一条射线并返回所有碰撞，返回一个RaycastHit[]结构体out， 若out中包括"ufo"
                // 若击中ufo，则回收它
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "ufo")
                    hit.collider.gameObject.SetActive(false);
                          
            }
        }  
	}
}
