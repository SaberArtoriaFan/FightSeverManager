using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer{

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

    public void Start(Action onFinished,float delayTime,bool isLoop,Action onUpdate=null) {
        this.isFinish = false;
        this.onFinished=onFinished;
        this.onUpdate=onUpdate;
        this.finishTime=Time.time+delayTime;
        this.delayTime=delayTime;
        this.isLoop=isLoop;
        this.continueTime=0;
    }

    public void Update() {
        // Debug.Log("剩余时间");
        // Debug.Log(finishTime-Time.time);
        if(isFinish) return;
        continueTime+=Time.deltaTime;
        if(onUpdate!=null) onUpdate();
        if(Time.time<finishTime) return;
        if(!isLoop) Stop();
        else finishTime=Time.time+delayTime;
   
#if UNITY_EDITOR
        onFinished?.Invoke();
#else
     //结束触发计时完成事件
        try
        {
                onFinished?.Invoke();
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
#endif
    }

    //停止方法
    public void Stop(){
        isFinish=true;
        continueTime=0;
        //解除事件，以免内存泄露
        onFinished += () => onFinished = null;
        onUpdate = null;
    }
}

public class TimerManager : AutoSingleton<TimerManager> {

    private readonly List<Timer> updateList=new List<Timer>();
    private readonly Queue<Timer> availableQueue=new Queue<Timer>();


    /// <summary>
    /// 添加计时器
    /// </summary>
    /// <param name="onFinished">完成后执行的委托</param>
    /// <param name="delayTime">持续时间</param>
    /// <param name="isLoop">是否循环</param>
    /// <returns></returns>
    public Timer AddTimer(Action onFinished,float delayTime,bool isLoop= false,Action onUpdate= null)
    {
        Timer timer=availableQueue.Count==0?new Timer():availableQueue.Dequeue();
        timer.Start(onFinished,delayTime,isLoop,onUpdate);
        updateList.Add(timer);
        return timer;
    }
    public void ResetSelf()
    {
        this.enabled = false;
        updateList.Clear();
        availableQueue.Clear();
        this.enabled=true;
    }
    protected override void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        //SceneManager.activeSceneChanged+=(a,b)=>ResetSelf();
        base.Awake();
    }
    /// <summary>
    /// 更新计时器
    /// </summary>
    private void Update(){
        if(updateList.Count==0) return;
        for(int i=0;i<updateList.Count;i++){
            if(updateList.Count==0) return;
            if(updateList[i].IsFinish){
                availableQueue.Enqueue(updateList[i]);
                updateList.RemoveAt(i);
                continue;
            }
            updateList[i].Update();
        }
    }

}
