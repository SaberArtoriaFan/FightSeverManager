using DG.Tweening;
using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.Base
{
    public class RisingSpacePanelUI : UIBase<RisingSpacePanelUI>
    {
        readonly string itemName = "RisingSpaceItem_S";
        readonly string textName = "RisingSpaceText_N";
        int id = 0;
        [SerializeField]
        AnimationCurve animationCurve;
        protected override void Init()
        {
            base.Init();
            PoolManager.Instance.AddPool<UISubManager>(
                () => { GameObject go = Instantiate(ABManager.Instance.LoadResource<GameObject>("ui", itemName));
                    go.transform.SetParent(this.transform);
                    go.name = go.name.Replace("_S(Clone)", $"{id++}_S");
                    go.transform.position = Vector3.up * 10000;
                    return go.AddComponent<UISubManager>();
                },
                (u) => { u.gameObject.SetActive(false); },
                null,
                itemName, 0,null,true
            );
            //rectTransform=GetComponent<RectTransform>();
        }
        public void ShowRisingSpace(string s,Vector3 worldPos,Vector3 dir,float speed=1,float continueTime=1.5f)
        {
            UISubManager showUi = PoolManager.Instance.GetObjectInPool<UISubManager>(itemName);
            showUi.ChangeText(textName, s);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            showUi.transform.position = screenPos;
            showUi.gameObject.SetActive(true);
            Vector3 offest=Vector3.zero;
            float timer = 0;
            float per;
            float curvePer;
            TimerManager.Instance.AddTimer(() => PoolManager.Instance.RecycleToPool(showUi, itemName), continueTime, false,
                () => {
                    timer += Time.deltaTime;
                    per = timer / continueTime;
                    curvePer=animationCurve.Evaluate(per);
                    //Debug.Log("Cur" + curvePer);
                    screenPos = Camera.main.WorldToScreenPoint(worldPos);
                    screenPos += offest;
                    offest += dir.normalized * speed*60*Time.deltaTime*curvePer;
                    //var realPos = Vector3.zero;
                    //switch (MainCanvas.renderMode)
                    //{
                    //    case RenderMode.ScreenSpaceOverlay:
                    //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    //            rectTransform,
                    //            screenPos,
                    //            null,
                    //            out realPos);
                    //        break;
                    //    case RenderMode.ScreenSpaceCamera:
                    //    case RenderMode.WorldSpace:
                    //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    //            rectTransform,
                    //            screenPos,
                    //            MainCanvas.worldCamera,
                    //            out realPos);
                    //        break;
                    //}
                    showUi.transform.position = screenPos;
                });
        }
    }
}
