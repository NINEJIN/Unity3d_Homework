using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.mygame;

public class ActionExpection : System.Exception {}

public class SSAction: MonoBehaviour
{
    public void Free() { Destroy(this); }
}

//-----------------------------------------

public interface ActionCallback
{
    void OnActionDone(SSAction action);
}

public class ActionManager: System.Object
{
    private static ActionManager instance;

    public static ActionManager GetInstance()
    {
        if (instance == null)
            instance = new ActionManager();
        return instance;
    }

    // 简单动作
    public class MoveToAction : SSAction
    {
        public Vector3 target;
        public float speed;

        private ActionCallback monitor = null;

        public void setting(Vector3 target, float speed, ActionCallback monitor)
        {
            this.target = target;
            this.speed = speed;
            this.monitor = monitor;
            GameSceneController.GetInstance().setMoving(true);
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // Destroy after done
            if (transform.position == target)
            {
                GameSceneController.GetInstance().setMoving(false);
                if (monitor != null)
                    monitor.OnActionDone(this);
                Destroy(this);
            }
        }
    }

    public SSAction ApplyMoveToAction(GameObject obj, Vector3 target, float speed, ActionCallback callback)
    {
        MoveToAction act = obj.AddComponent<MoveToAction>();
        act.setting(target, speed, callback);
        return act;
    }

    public SSAction ApplyMoveToAction(GameObject obj, Vector3 target, float speed)
    {
        return ApplyMoveToAction(obj, target, speed, null);
    }

    // 组合动作
    public class MoveToMulAction : SSAction
    {
        public GameObject obj;
        public Vector3 target;
        public float speed;

        private ActionCallback monitor = null;

        public void setting(GameObject obj, Vector3 target, float speed, ActionCallback monitor)
        {
            this.obj = obj;
            this.target = target;
            this.speed = speed;
            this.monitor = monitor;
            GameSceneController.GetInstance().setMoving(true);

            if (target.y < obj.transform.position.y)
            {
                // 下船
                Vector3 DirecX = new Vector3(target.x, obj.transform.position.y, target.z);
                //ActionManager.GetInstance().ApplyMoveToAction(obj, DirecX, speed, this.monitor);
                ActionManager.GetInstance().ApplyMoveToAction(obj, target, speed, this.monitor);
            }
            else
            {
                Vector3 DirecY = new Vector3(obj.transform.position.x, target.y, target.z);
                //ActionManager.GetInstance().ApplyMoveToAction(obj, DirecY, speed, this.monitor);
                ActionManager.GetInstance().ApplyMoveToAction(obj, target, speed, this.monitor);
            }
        }

        public void OnActionDone (SSAction action)
        {
            ActionManager.GetInstance().ApplyMoveToAction(obj, target, speed, null);
        }

        private void Update()
        {
            if (obj.transform.position == target)
            {
                // 完成动作后destroy
                GameSceneController.GetInstance().setMoving(false);
                if (monitor != null)
                    monitor.OnActionDone(this);
                Destroy(this);
            }
        }
    }

    public SSAction ApplyMoveToMulAction(GameObject obj, Vector3 target, float speed, ActionCallback callback)
    {
        MoveToMulAction act = obj.AddComponent<MoveToMulAction>();
        act.setting(obj, target, speed, callback);
        return act;
    }
    public SSAction ApplyMoveToMulAction(GameObject obj, Vector3 target, float speed)
    {
        return ApplyMoveToMulAction(obj, target, speed, null);
    }
}
