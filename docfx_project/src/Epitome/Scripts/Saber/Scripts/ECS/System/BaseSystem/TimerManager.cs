using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class Timer
    {

        //计时完成的委托
        private Action onFinished;
        //每一帧执行的委托
        private Action onUpdate;
        //完成时间
        private float finishTime;
        //延迟时间
        private float delayTime;
        //已经经过的时间
        private float continueTime;
        //是否循环
        private bool isLoop;
        //是否完成
        private bool isFinish;
        public bool IsFinish => isFinish;

        public float ContinueTime { get => continueTime; }

        public void AddFinishAction(Action action)
        {
            if (!isFinish)
                onFinished += action;
        }

        public void Start(Action onFinished, float delayTime, bool isLoop, Action onUpdate = null)
        {
            this.isFinish = false;
            this.onFinished = onFinished;
            this.onUpdate = onUpdate;
            this.finishTime = Time.time + delayTime;
            this.delayTime = delayTime;
            this.isLoop = isLoop;
            this.continueTime = 0;
        }

        public void Update()
        {
            // Debug.Log("剩余时间");
            // Debug.Log(finishTime-Time.time);
            if (isFinish) return;
            continueTime += Time.deltaTime;
            if (onUpdate != null) onUpdate();
            if (Time.time < finishTime) return;
            if (!isLoop) Stop();
            else finishTime = Time.time + delayTime;
            //结束触发计时完成事件
            try
            {
                onFinished?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        //停止方法
        public void Stop()
        {
            isFinish = true;
            isLoop = false;
            continueTime = 0;
            //解除事件，以免内存泄露
            onFinished += () => onFinished = null;
            onUpdate = null;
        }
    }

    public class TimerManagerSystem : SingletonSystemBase<TimerSystemModel>
    {



        //public Timer AddSecondTimer(Action secAction, float laestDestoryTime,float cycleSpeed=1)
        //{
        //    //Timer timer=new Timer
        //    Timer timer = instance.AvailableQueue.Count == 0 ? new Timer() : instance.AvailableQueue.Dequeue();
        //    Timer timer2= instance.AvailableQueue.Count == 0 ? new Timer() : instance.AvailableQueue.Dequeue();
        //    timer.Start(() => { secAction?.Invoke(); timer2.Stop(); }, cycleSpeed, true);

        //    timer2.Start(()=> { timer.Stop(); }, laestDestoryTime,false);
        //    //timer.Start(onFinished, delayTime, isLoop, onUpdate);
        //    instance.UpdateList.Add(timer);
        //    instance.UpdateList.Add(timer2);

        //    return timer;
        //}
        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="onFinished">完成后执行的委托</param>
        /// <param name="delayTime">持续时间</param>
        /// <param name="isLoop">是否循环</param>
        /// <returns></returns>
        public Timer AddTimer(Action onFinished, float delayTime, bool isLoop = false, Action onUpdate = null)
        {
            Timer timer = instance.AvailableQueue.Count == 0 ? new Timer() : instance.AvailableQueue.Dequeue();
            timer.Start(onFinished, delayTime, isLoop, onUpdate);
            instance.UpdateList.Add(timer);
            return timer;
        }
        public override void Reset()
        {
            base.Reset();
            instance.UpdateList.Clear();
            instance.AvailableQueue.Clear();
        }
        public override void Awake(WorldBase world)
        {
            base.Awake(world);
        }
        public override void Update()
        {
            base.Update();
            if (instance.UpdateList.Count == 0) return;
            for (int i = 0; i < instance.UpdateList.Count; i++)
            {
                if (instance.UpdateList.Count == 0) return;
                if (instance.UpdateList[i].IsFinish)
                {
                    instance.AvailableQueue.Enqueue(instance.UpdateList[i]);
                    instance.UpdateList.RemoveAt(i);
                    continue;
                }
                instance.UpdateList[i].Update();
            }

        }



    }
}

