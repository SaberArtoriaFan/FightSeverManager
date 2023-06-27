using Saber;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Tool
{
    public class TimerSystem
    {
        public class Timer
        {
            TimerSystem timerSystem;
            //计时完成的委托
            private Action onFinished;
            //每一帧执行的委托
            private Action onUpdate;
            //完成时间
            internal float finishTime;
            //延迟时间
            private float delayTime;
            //已经经过的时间
            private float continueTime;
            private int loopTime;
            //是否循环
            private bool isLoop;
            //是否完成
            private bool isFinish;
            public bool IsFinish => isFinish;

            public float ContinueTime { get => continueTime; }
            public int LoopTime { get => loopTime;}

            public void AddFinishAction(Action action)
            {
                if (!isFinish)
                    onFinished += action;
            }

            public void Start(Action onFinished, float delayTime, bool isLoop, int loopTime = -1, Action onUpdate = null)
            {
                timerSystem=InstanceFinder.GetInstance<TimerSystem>();
                this.isFinish = false;
                this.onFinished = onFinished;
                this.onUpdate = onUpdate;
                this.finishTime = timerSystem.CurrTime + delayTime;
                this.delayTime = delayTime;
                this.isLoop = isLoop;
                this.continueTime = 0;
                this.loopTime = loopTime;
            }

            public void Update()
            {
                // Debug.Log("剩余时间");
                // Debug.Log(finishTime-Time.time);
                if (isFinish) return;
                continueTime += timerSystem.updateInterval;
                if (onUpdate != null) onUpdate();
                if (timerSystem.CurrTime < finishTime) return;
                if (!isLoop|| --loopTime == 0) Stop();
                else if (isLoop)
                    finishTime = timerSystem.CurrTime + delayTime;
                //结束触发计时完成事件
                try
                {
                    onFinished?.Invoke();
                }
                catch (Exception e)
                {
                    SaberDebug.LogError("计时器内部错误"+ e.Message);
                }
            }

            //停止方法
            public void Stop(bool isBreak=false)
            {
                lock (this)
                {
                    isFinish = true;
                    if (isBreak)
                        onFinished = null;
                    else
                        onFinished += () => onFinished = null;
                    isLoop = false;
                    continueTime = 0;
                    //解除事件，以免内存泄露

                    onUpdate = null;
                }
            }
        }
        float currTime = 0;
        float CurrTime { get=>currTime; set
            {
                if (float.MaxValue <= value)
                    ResetCurrTime();
                else
                    currTime = value;
            } }

        float updateInterval=0.1f;
        Queue<Timer> waitPoolTimer = new Queue<Timer>();
        Queue<Timer> poolTimer = new Queue<Timer>();
        List<Timer> waitForEnterTimer=new List<Timer>();
        List<Timer>updateTimer=new List<Timer>();
        List<Timer> waitForRemoveList = new List<Timer>();

        public TimerSystem() 
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            //timer.SynchronizingObject=InstanceFinder.GetInstance<AutoProcessManager>(); 
            timer.Elapsed += (o, t) => { 
                Update();
                LateUpdate(); };
            timer.Interval = updateInterval * 1000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
            InitTimerPool(100);
        }
        private void ResetCurrTime()
        {
            foreach (var v in updateTimer)
            {
                v.finishTime -= currTime;
            }
            currTime = 0;
        }
        private void InitTimerPool(int timer)
        {
            while (timer-- > 0)
                poolTimer.Enqueue(new Timer());
        }
        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="onFinished">完成后执行的委托</param>
        /// <param name="delayTime">持续时间</param>
        /// <param name="isLoop">是否循环</param>
        /// <returns></returns>
        public Timer AddTimer(Action onFinished, float delayTime, bool isLoop = false,int loopTime=-1, Action onUpdate = null,bool isNew=false)
        {

                Timer timer = null;
            if(isNew)
                timer=new Timer();
            else
            {
                 timer = poolTimer.Count == 0 ? new Timer() : poolTimer.Dequeue();
            }

                //Timer timer = new Timer();
                timer.Start(onFinished, delayTime, isLoop, loopTime, onUpdate);
                waitForEnterTimer.Add(timer);
                return timer;
        }
        //int num;
        void Update()
        {
            //SaberDebug.Log("update"+num++);
            lock (waitForEnterTimer)
            {
                updateTimer.AddRange(waitForEnterTimer);
                waitForEnterTimer.Clear();
            }
            CurrTime += updateInterval;
            if (updateTimer.Count == 0) return;
            for (int i = 0; i < updateTimer.Count; i++)
            {
                if (updateTimer.Count == 0) return;
                if (updateTimer[i].IsFinish)
                {
                    //updateTimer.RemoveAt(i);
                    //poolTimer.Enqueue(updateTimer[i]);
                    waitForRemoveList.Add(updateTimer[i]);
                    continue;
                }
                updateTimer[i].Update();
            }
            foreach (var v in waitForRemoveList)
                updateTimer.Remove(v);
            //waitForRemoveList.Clear();
        }
        void LateUpdate()
        {
            foreach (var v in waitForRemoveList)
                poolTimer.Enqueue(v);
            waitForRemoveList.Clear();
        }
    }
}
