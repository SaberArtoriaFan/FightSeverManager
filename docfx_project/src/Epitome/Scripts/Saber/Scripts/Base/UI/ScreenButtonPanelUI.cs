using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.Base
{
    public class ScreenButtonPanelUI : UIBase<ScreenButtonPanelUI>
    {
        protected class ScreenButtonArgs
        {
            public readonly Timer timer;
            public readonly UISubManager uI;

            public ScreenButtonArgs( Timer timer, UISubManager uI)
            {
                this.timer = timer;
                this.uI = uI;
            }
        }
        readonly string itemName = "ScreenButton_S";
        readonly string buttonName = "ScreenButton_N";
        readonly string textName = "ScreenText_N";
        Dictionary<int, ScreenButtonArgs> screenButtonDic;
        int id = 0;
        int argsId = 0;
        protected override void Init()
        {
            base.Init();
            PoolManager.Instance.AddPool<UISubManager>(
                () => {
                    GameObject go = Instantiate(ABManager.Instance.LoadResource<GameObject>("ui", itemName));
                    go.transform.SetParent(this.transform);
                    go.name = go.name.Replace("_S(Clone)", $"{id++}_S");
                    go.transform.position = Vector3.up * 10000;
                    return go.AddComponent<UISubManager>();
                },
                (u) => { u.gameObject.SetActive(false); },
                null,
                itemName, 0, null, true
            );
            screenButtonDic = new Dictionary<int, ScreenButtonArgs>();
            //rectTransform=GetComponent<RectTransform>();
        }
        public int ShowScreenButton(string s, Action buttonAction,Vector3 worldPos,float width=80,bool isOnce=true)
        {
            UISubManager showUi = PoolManager.Instance.GetObjectInPool<UISubManager>(itemName);
            showUi.ChangeText(textName, s);
            showUi.ChangeRectSize(buttonName, width);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            showUi.transform.position = screenPos;
            showUi.gameObject.SetActive(true);
            showUi.SetButtonActive(buttonName, true);
            showUi.ClearButtonAllListeners(buttonName);
            showUi.AddButtonLister(buttonName, () => { buttonAction?.Invoke(); });
            Timer timer = TimerManager.Instance.AddTimer(null, 0, true,
                () => {
                    screenPos = Camera.main.WorldToScreenPoint(worldPos);
                    showUi.transform.position = screenPos;
                    });

            int id=argsId++;
            if (isOnce)
                showUi.AddButtonLister(buttonName, ()=>FinishShowScreenButton(id));
            screenButtonDic.Add(id, new ScreenButtonArgs(timer, showUi));
            return id;
        }
        public void FinishShowScreenButton(int id)
        {
            if (screenButtonDic.ContainsKey(id) == false) return;
            ScreenButtonArgs arg = screenButtonDic[id];
            if(arg == null) return;
            arg.timer.Stop();
            arg.uI.SetButtonActive(buttonName, false);
            arg.uI.ClearButtonAllListeners(buttonName);
            TimerManager.Instance.AddTimer(() =>
            {
                arg.uI.gameObject.SetActive(false);
            }, Time.deltaTime);
            screenButtonDic.Remove(id);
        }
        public void FinishAll()
        {
            foreach(var v in screenButtonDic)
            {
                FinishShowScreenButton(v.Key);
            }
        }
    }
}
