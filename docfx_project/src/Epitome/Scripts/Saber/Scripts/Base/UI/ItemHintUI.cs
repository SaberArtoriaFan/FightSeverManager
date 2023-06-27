using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Saber.Base
{

    public class TipsShowUIHelper<T> 
    {
        protected readonly UISubManager[] needShowUIs;
        readonly Dictionary<T, UISubManager> dict;
        readonly Func<T, string> showFunc;
        readonly Action<UISubManager,UISubManager> dragEvent;
        readonly Dictionary<UISubManager, Vector3> uiOriginPos;
        readonly GameObject parent;
        UISubManager tipsShowUI = null;
        ItemHintUI itemHintUI = null;
        private float width = 300;
        private bool isUp = true;
        //bool isCanDrag = false;
        float timer = 0;
        Timer show_timer=null;
        float showTime = 0.3f;
        bool isShowTips = false;
        public float Width { get => width; set => width = value; }
        public bool IsUp { get => isUp; set => isUp = value; }

        protected void FinishShowTips()
        {
            //Debug.Log("0105b");

            isShowTips = false;
            if (show_timer != null)
                show_timer.Stop();
            show_timer = null;
            timer = 0;
            tipsShowUI = null;
            itemHintUI.FinishShowTips();
        }
        void ShowTipsDelay(UISubManager uISubManager)
        {
            if (uISubManager == tipsShowUI) return;
            if (show_timer != null)
                show_timer.Stop();
            show_timer = null;
            show_timer = TimerManager.Instance.AddTimer(() => ShowTips(uISubManager), showTime);
        }
        protected  void ShowTips(UISubManager uISubManager)
        {
            //if(tipsShowUI !=)
            //Debug.Log("0105a");
            //if (!isShowTips) { timer += Time.deltaTime;
            //    if (timer < showTime) return;}
            if (uISubManager == tipsShowUI) return;
            T item = default;
            bool isFind = false;
            foreach (var v in dict)
            {
                if (v.Value == uISubManager)
                { item = v.Key; isFind = true; break; }
            }
            if (isFind&&item!=null)
            {
                isShowTips=true;
                tipsShowUI = uISubManager;
                itemHintUI.StartShowTips(showFunc(item), width, isUp);
            }
        }
        //protected virtual void BeginDrag(UISubManager uISub)
        //{
        //    if (uISub == null) return;
        //    CanvasGroup canvasGroup = uISub.GetComponent<CanvasGroup>();
        //    if(canvasGroup==null)canvasGroup= uISub.gameObject.AddComponent<CanvasGroup>();
        //    canvasGroup.blocksRaycasts = false;
        //    uiOriginPos.Add(uISub, uISub.transform.position);
        //    uISub.transform.SetParent(UIManager.Instance.MainCanvas.transform);
        //    uISub.transform.SetAsLastSibling();
        //}
        //void OnDrag(UISubManager uISub)
        //{
        //    if (uISub == null) return;
        //    uISub.transform.position = Input.mousePosition;
        //}
        //protected virtual void EndDrag(BaseEventData baseEvent,UISubManager uISub)
        //{
        //    PointerEventData pointerEventData=baseEvent as PointerEventData;
        //    //Debug.Log(pointerEventData.pointerCurrentRaycast.gameObject.name);
        //    if (uISub == null||!uiOriginPos.ContainsKey(uISub)) return;
        //    if (pointerEventData.pointerCurrentRaycast.gameObject!= null)
        //    {
        //        //Debug.Log(baseEvent.currentInputModule.gameObject.name + "QQ");
        //        IFindUISub uISub2= pointerEventData.pointerCurrentRaycast.gameObject.GetComponent<IFindUISub>();
        //        //Debug.Log(uISub2.MyUISubManager.name + "QQ");

        //        if (uISub2 != null&&uISub2.MyUISubManager!=null&&uISub2.MyUISubManager.BelongPanelUIBase==uISub.BelongPanelUIBase)
        //        {
        //            //Debug.Log(111 + "QQ");
        //            dragEvent?.Invoke(uISub, uISub2.MyUISubManager);
        //        }
        //    }

        //    uISub.transform.SetParent(parent.transform);
        //    uISub.transform.position = uiOriginPos[uISub];
        //    uiOriginPos.Remove(uISub);
        //    CanvasGroup canvasGroup = uISub.GetComponent<CanvasGroup>();
        //    if (canvasGroup != null) canvasGroup.blocksRaycasts = true;


        //}
        public TipsShowUIHelper(UISubManager[] needShowUIs, Dictionary<T, UISubManager> dict, Func<T, string> showFunc,float showTime=0.3f)
        {
            this.showTime = showTime;
            this.needShowUIs = needShowUIs;
            this.dict = dict;
            this.showFunc = showFunc;
            itemHintUI = UIManager.GetPanel<ItemHintUI>();
            //this.isCanDrag = isCanDrag;
            //this.dragEvent = dragEvent;
            //this.parent = parent;
            foreach (var u in needShowUIs)
            {
                u.Self_AddEventTriggerEvent(EventTriggerType.PointerEnter, (b) => { ShowTipsDelay(u); });
                u.Self_AddEventTriggerEvent(EventTriggerType.PointerExit, (b) => { FinishShowTips(); });

            }


                //}
            }
        public virtual void Destory()
        {
            foreach (var u in needShowUIs)
            {
                u.Self_ClearAllEventTriggerEvent();


            }
            dict.Clear();
        }
    }
    public class ItemHintUI :UIBase<ItemHintUI>
    {
        public GameObject bg;
        protected override GameObject BackGround => bg;
        string content = "";
        float width=300;
        bool isUp = true;
        Vector3 positon=default;
        bool isWorking = false;

        public void StartShowTips(string content, float width, bool isUp,Vector3 pos=default)
        {
            //Debug.Log("StartShow");
            this.content = content;
            this.width = width;
            this.isUp = isUp;
            isWorking = true;
            if (pos != default)
                positon = pos;
            LateUpdate();
            SetSelfActive("BackGround_N", true);
        }
        public void FinishShowTips()
        {
            SetSelfActive("BackGround_N", false);
            this.content = "";
            this.width = 300;
            this.isUp = true;
            isWorking = false;
            positon = default;
        }
        [Button]
        protected void ShowTips(Vector3 pos,string content,float width,bool isUp)
        {
            int y = isUp ? 0 : 1;
            SetPivot("BackGround_N", 0, y);
            ChangePosition("BackGround_N", pos);
            //ChangeText("BackGround_N", content);
            ChangeText("HintText_N", content);
            ChangeRectSize("BackGround_N", width);
        }
        string abond = "rqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflibrqflibfqfqfqfrqfbfqfqfqfrqflib";
        private void LateUpdate()
        {
            if (isWorking)
            {
                if (positon == default) ShowTips(Input.mousePosition, content, width, isUp);
                else ShowTips(positon, content, width, isUp);
            }
        }
    }
}
