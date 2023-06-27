using FishNet;
using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XianXia.Unit
{
    public class Client_TimeScaleUI : Client_SingletonBase<Client_TimeScaleUI>
    {
        Button button;
        TMP_Text text;
        float speed;
        [SerializeField]
        float toBeSpeed = 2;

        public event Action<float> OnSetTimeScale;
        protected override void Start()
        {
            base.Start();
            Canvas canvas = BaseUtility.GetMainCanvas();
            Transform[] trs =  canvas.GetComponentsInChildren<RectTransform>();
            foreach(var v in trs)
            {
                if (button == null && v.name == "TimeScaleButton")
                    button = v.gameObject.GetComponent<Button>();
                if (text == null && v.name == "TimeScaleText")
                    text = v.gameObject.GetComponent<TMP_Text>();
            }
            speed = 1;
            button.onClick.AddListener(SetTimeScale);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetTimeScale = null;
        }
        void SetTimeScale()
        {
            if (speed!=1)
            {
                speed = 1;
                //Time.timeScale = speed;
                text.text = "二倍速";
            }
            else
            {
                speed = toBeSpeed;
                //Time.timeScale = toBeSpeed;
                text.text = "正常速";
            }
            OnSetTimeScale?.Invoke(speed);

        }
    }
}
