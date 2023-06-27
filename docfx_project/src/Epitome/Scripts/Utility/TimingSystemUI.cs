using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Saber.Base;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static FishNet.Object.Synchronizing.SyncTimer;

namespace XianXia.Unit
{
    public class TimingSystemUI : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SyncObject]
        private readonly SyncTimer syncTimer=new SyncTimer();
        [SyncVar]
        [ShowInInspector]
        protected float pastedTime;
        [SyncVar]
        [ShowInInspector]
        bool time_switch = false;

        
        public event Action OnGameOver;
        public float Remaining => syncTimer.Duration - pastedTime;

        [SerializeField]
        RectTransform root;
        const string RootName = "TimingSystemPanel";
        int minute;
        int second;
        [ShowInInspector]
        int urgentTime = 0;
        TMP_Text text;
        GameObject bg;
        StringBuilder stringBuilder;
        [ShowInInspector]
        //float timer = 0;
        int lastTimer=0;

        private void Awake()
        {
            if (InstanceFinder.GetInstance<TimingSystemUI>() != null) { Debug.LogError("Cant Find"); return; }
            InstanceFinder.RegisterInstance(this, true);
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (root == null)
            {
                Canvas mianCanvas = BaseUtility.GetMainCanvas();
                RectTransform[] rectTransforms = mianCanvas.GetComponentsInChildren<RectTransform>();
                foreach(var v in rectTransforms)
                {
                    if (v.name == RootName)
                    {
                        root = v;
                        break;
                    }
                }

            }

            stringBuilder = new StringBuilder();
            Transform[] transforms = root.GetComponentsInChildren<Transform>();
            foreach (var v in transforms)
            {
                if (v.name == "TimingSystemText")
                {
                    text = v.GetComponent<TMP_Text>();
                    //break;
                }
                if (v.name == "TimingSystemBG")
                    bg = v.gameObject;
            }
            if (bg != null) bg.SetActive(false);
        }
        [Button]
        [Server]
        public void StartTimer(int time,int urgentTime)
        {
            syncTimer.StartTimer(time);
            syncTimer.OnChange += GameOver;
            ORPC_StartTimer(time, urgentTime);
            time_switch = true;
            Debug.Log("开始计时！！");

        }
        [ObserversRpc]
        private void ORPC_StartTimer(int time, int urgentTime)
        {
            second = time % 60;
            minute = (time - second) / 60;
            this.urgentTime = time - urgentTime;
            lastTimer = 0;
            text.color = Color.black;
            //Update();
            stringBuilder.Clear();
            stringBuilder.Append(minute.ToString().PadLeft(2, '0'));
            stringBuilder.Append(":");
            stringBuilder.Append(second.ToString().PadLeft(2, '0'));
            text.text = stringBuilder.ToString();
            stringBuilder.Clear();
            bg.SetActive(true);
            Debug.Log("开始计时！！");
        }
        [Server]
        public void StopTimer()
        {
            time_switch = false;
            if (syncTimer.Remaining > 0)
                syncTimer.OnChange -= GameOver;
            syncTimer.StopTimer();
        }
        [Server]
        private void GameOver(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            //Debug.Log("next" + next);
            if (next == 0)
            {
                Debug.Log("计时器计时结束导致的游戏结束");
                OnGameOver?.Invoke();
                OnGameOver = null;
                syncTimer.OnChange -= GameOver;
            }


        }
        [Server]
        public void AddTimeChangeEvent(SyncTypeChanged action,bool isAdd)
        {
            if (isAdd)
                syncTimer.OnChange += action;
            else
                syncTimer.OnChange -= action;
        }
        private void Update()
        {
            if (IsServer)
            {
                syncTimer.Update(Time.deltaTime);
                pastedTime = syncTimer.Elapsed;
                //Debug.Log(syncTimer.Elapsed + "TT");
            }
            if (IsClient)
            {
                if (time_switch)
                {
                    float timer = pastedTime;
                    //Debug.Log(syncTimer.Elapsed+"TT");
                    if (timer >= urgentTime && urgentTime > 0)
                    {
                        urgentTime = -1;
                        text.color = Color.red;
                    }else
                        text.color = Color.black;


                    if (timer >= lastTimer)
                        lastTimer++;
                    else
                        return;
                    stringBuilder.Clear();
                    stringBuilder.Append(minute.ToString().PadLeft(2, '0'));
                    stringBuilder.Append(":");
                    stringBuilder.Append(second.ToString().PadLeft(2, '0'));
                    text.text = stringBuilder.ToString();
                    stringBuilder.Clear();
                    second--;
                    if (second < 0)
                    {
                        second = 59;
                        minute--;
                        if (minute < 0)
                            EndTimer();
                    }
                }
            }


        }


        private void EndTimer()
        {
            time_switch = false;
            //syncTimer.StopTimer();
            urgentTime = 0;
        }

        public  float _TimeRemaining => syncTimer.Remaining;
        public  float _TimePast => syncTimer.Elapsed;
        /// <summary>
        /// 服务器使用的
        /// </summary>
        public static float TimeRemaining => InstanceFinder.GetInstance<TimingSystemUI>().syncTimer.Remaining;
        public static float TimePast => InstanceFinder.GetInstance<TimingSystemUI>().syncTimer.Elapsed;
    }
}
