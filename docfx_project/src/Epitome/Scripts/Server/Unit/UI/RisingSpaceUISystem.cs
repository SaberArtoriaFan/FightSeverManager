using DG.Tweening;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace XianXia.Unit
{
    public class RisingSpaceUISystem :SystemStone
    {
        ObjectPool<TMP_Text> textObjectPool;
        //public string packageName = "fight";
        public string TextMeshProName = "RisingSpaceItem";
        GameObject model;
        Transform panel;
        const string panelName= "RisingSpaceItemPanelUI";

        
        public override void Start()
        {
            base.Start();
            model = ABUtility.Load<GameObject>(ABUtility.UIMainName+ TextMeshProName);
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            Canvas canvas = null ;
            foreach(var v in canvases)
            {
                if (v.name == "MainCanvas")
                { canvas = v; break; }
            }
            RectTransform[] rectTransforms=canvas.transform.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                Debug.Log("name" + rectTransforms[i].gameObject.name);
                if (rectTransforms[i].gameObject.name == panelName)
                {
                    panel = rectTransforms[i].transform;
                    break;
                }
            }
            if (panel == null) { Debug.LogError("CantFindRisingSpacePanel");panel = canvas.transform; }
            //Debug.Log("GGG"+panel.name);
            textObjectPool = world.FindSystem<ObjectPoolSystem>().AddPool(Spawn, RecycleBefore, InitText, "RisingSpaceItemPool", 20);

        }

        private void InitText(TMP_Text obj)
        {
            obj.gameObject.SetActive(true);
            obj.transform.SetAsFirstSibling();
            obj.gameObject.SetActive(false);
        }

        private void RecycleBefore(TMP_Text obj)
        {
            obj.gameObject.SetActive(false);
        }

        private TMP_Text Spawn()
        {
            GameObject go = GameObject.Instantiate(model);
            go.transform.SetParent(panel);
            TMP_Text textMeshPro = go.GetComponent<TMP_Text>();
            go.SetActive(false);
            return textMeshPro;
        }

        public void ShowRisingSpace(string s, Vector3 worldPos, Vector3 dir,Color color = default, int size = 24, FontStyles fontStyles = FontStyles.Normal ,float speed = 1, float continueTime = 1.5f)
        {
            TMP_Text textMeshPro = textObjectPool.GetObjectInPool();
            textMeshPro.text = s;
            if (color == default) color = Color.yellow;
            textMeshPro.color = color;
            textMeshPro.fontSize = size;
            textMeshPro.fontStyle=textMeshPro.fontStyle;
            //showUi.ChangeText(textName, s);
            Vector3 endPos = Camera.main.WorldToScreenPoint(worldPos + dir.normalized * speed);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            textMeshPro.transform.position = screenPos;
            textMeshPro.gameObject.SetActive(true);
            textMeshPro.transform.DOMove(endPos, continueTime).OnComplete(()=>textObjectPool.RecycleToPool(textMeshPro));
            //TimerManager.Instance.AddTimer(() => { textObjectPool.RecycleToPool(textMeshPro); }, continueTime, false,
            //    () =>
            //    {
            //        timer += Time.deltaTime;
            //        per = timer / continueTime;
            //        curvePer = animationCurve.Evaluate(per);
            //        //Debug.Log("Cur" + curvePer);
            //        screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //        screenPos += offest;
            //        offest += dir.normalized * speed * 60 * Time.deltaTime * curvePer;
            //        //var realPos = Vector3.zero;
            //        //switch (MainCanvas.renderMode)
            //        //{
            //        //    case RenderMode.ScreenSpaceOverlay:
            //        //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            //        //            rectTransform,
            //        //            screenPos,
            //        //            null,
            //        //            out realPos);
            //        //        break;
            //        //    case RenderMode.ScreenSpaceCamera:
            //        //    case RenderMode.WorldSpace:
            //        //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            //        //            rectTransform,
            //        //            screenPos,
            //        //            MainCanvas.worldCamera,
            //        //            out realPos);
            //        //        break;
            //        //}
            //        showUi.transform.position = screenPos;
            //    });
        }
    }
}
