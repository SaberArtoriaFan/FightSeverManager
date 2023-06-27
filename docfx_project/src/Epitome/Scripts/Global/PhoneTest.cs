using JEngine.Core;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using XianXia;

public class PhoneTest : XianXia.Server.XianXiaUpdater
{
    JEngine.Core.Updater updater;
    [SerializeField]
    Text text;
    [SerializeField]
    Button button;
    [Header("[��Editorģʽ����]�����Ƿ�ֱ�ӽ���OfflieScene,�費��Ҫ����ս��������")]
    public bool isTest = true;
    private void Start()
    {
        updater = GetComponent<JEngine.Core.Updater>();
        button.onClick.AddListener(ChangeMode);
        if (updater != null)
        {

#if !UNITY_EDITOR&&UNITY_SERVER
            Console.WriteLine("StartServer Succ");
            updater.mode = JEngine.Core.Updater.UpdateMode.Standalone;
#elif UNITY_EDITOR&&UNITY_SERVER
            updater.mode = JEngine.Core.Updater.UpdateMode.Simulate;
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            updater.mode = JEngine.Core.Updater.UpdateMode.Remote;
#else
            updater.mode = JEngine.Core.Updater.UpdateMode.Simulate;
#endif
#if UNITY_SERVER
            FightServerManager.ConsoleWrite_Saber($"�ȸ���ģʽ:{updater.mode.ToString()}");
#endif
#if UNITY_EDITOR
            if(isTest)
                updater.StartUpdate();
#endif

        }

    }

    private void ChangeMode()
    {
        switch (updater.mode)
        {
            case JEngine.Core.Updater.UpdateMode.Simulate:
                updater.mode = JEngine.Core.Updater.UpdateMode.Standalone;
                break;
            case JEngine.Core.Updater.UpdateMode.Standalone:
                updater.mode = JEngine.Core.Updater.UpdateMode.Remote;
                break;
            case JEngine.Core.Updater.UpdateMode.Remote:
                updater.mode = JEngine.Core.Updater.UpdateMode.Simulate;
                break;
        }
    }

    [Button]
    void Text()
    {
        updater.StartUpdate();
    }
    
    private void Update()
    {
#if UNITY_EDITOR
        if(text!=null)
            text.text = updater.mode.ToString();
#endif
    }

    //public void Load()
    //{
    //    JEngine.Core.AssetMgr.
    //}
    public override void EnterTargetModel(int id,Action<bool> updateResourceFinishEvent)
    {
        if(updater==null) updater = GetComponent<JEngine.Core.Updater>();
        switch (id)
        {
            case 0:
                updater.mode = Updater.UpdateMode.Standalone;
                break;
            case 1:
                updater.mode = Updater.UpdateMode.Remote;
                //����ɹ���ʧ�ܸ�����Դ�¼�
                updater.AddUpdateResourceFinishEvent(updateResourceFinishEvent);
                break;
        }
        updater.StartUpdate();
    }
}
