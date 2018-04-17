using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFO;

namespace HitUFO {
    public class UfoFactory : System.Object
    {
        private static UfoFactory instance;
        private static List<GameObject> ufoList;    // ufo的队列
        public GameObject ufoTemplate;              // 预设的ufo, 设为public对象， 在游戏开始时加载预设

        public static UfoFactory getInstance()
        {
            if (instance == null)   {
                instance = new UfoFactory();
                ufoList = new List<GameObject>();
            }
            return instance;
        }

        // 获取一个可用的ufo序号
        public int getUfoId()
        {
            for (int i = 0; i < ufoList.Count; i++)
                if (!ufoList[i].activeInHierarchy)   // 另有activeInHierarchy
                    return i;

            // 若队列没有空闲的飞碟的话，新增一个ufo实例
            GameObject it = GameObject.Instantiate(ufoTemplate) as GameObject;
            it.tag = "ufo";
            ufoList.Add(it);
            return ufoList.Count - 1;
        }

        // 获取id对应的ufo对象
        public GameObject getUfo(int id)
        {
            if (id < 0 || id >= ufoList.Count) return null;
            return ufoList[id];
        }

        public void recycleUfo(int id)
        {
            if (id < 0 || id >= ufoList.Count) return;

            // ufo的刚体向量速度设为0, 大小变回原始，激活状态取消
            ufoList[id].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ufoList[id].transform.localScale = ufoTemplate.transform.localScale; // scale?
            ufoList[id].SetActive(false);
        }
    }
}

public class Factory : MonoBehaviour {

    public GameObject UFO;

	void Awake () {
        UfoFactory.getInstance().ufoTemplate = UFO;	
	}	
}
