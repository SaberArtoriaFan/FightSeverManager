using FishNet;
using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XianXia.Client
{
    public class Client_ExitFightUI : Client_SingletonBase<Client_ExitFightUI>
    {
        const string ExitFightUIName = "ExitButton";
        Button button;
        // Start is called before the first frame update
        public event Action OnExitEvent;
       protected override  void Start()
        {
            base.Start();
            Canvas canvas = BaseUtility.GetMainCanvas();
            foreach(var v in canvas.transform.GetComponentsInChildren<Button>())
            {
                if (v.name == ExitFightUIName)
                {
                    button = v;
                    button.onClick.AddListener(OnClick);
                    break;
                }
            }
        }
        void OnClick()
        {
            OnExitEvent?.Invoke();
            InstanceFinder.ClientManager?.StopConnection();
        }
        // Update is called once per frame
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnExitEvent = null;
        }
    }
}
