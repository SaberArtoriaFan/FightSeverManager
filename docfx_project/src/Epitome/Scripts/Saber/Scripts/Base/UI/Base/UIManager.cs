using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
public class UIManager : AutoSingleton<UIManager>
{
    Dictionary<string, Dictionary<string, GameObject>> AllWedgate;
    Dictionary<string, IUIBase> allUIPanel;
    Canvas mainCanvas;
    //public readonly string UIABPackageName = "ui";
    public Canvas MainCanvas { get => mainCanvas; }

    public void ResetSelf()
    {
        AllWedgate.Clear();
    }
    //注册物体与Base，用字典管理
    #region GetWithRegister
    public GameObject GetWedgateGameObject(string PanelName, string WedgateName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            GameObject WedgateGameObject = AllWedgate[PanelName][WedgateName];
            return WedgateGameObject;
        }
        return null;
    }
    public void RegisterWedgate(string PanelName, string WedgateName, GameObject gameObject)
    {
        if (!AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName] = new Dictionary<string, GameObject>();
        }
        AllWedgate[PanelName].Add(WedgateName, gameObject);
    }
    #endregion
    #region Destroy
    public void DestroyWedgate(string PanelName, string WedgateName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName].Remove(WedgateName);
        }
    }
    public  void DestroyPanel(string PanelName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName].Clear();
            AllWedgate[PanelName] = null;
        }
    }
    #endregion
    public static void RegisterPanel(IUIBase uIBase)
    {
        if (Instance.allUIPanel.ContainsKey(uIBase.Name)) { BaseUtility.DebugError("层级重复出现！！"+uIBase.Name);return; }
        else Instance.allUIPanel.Add(uIBase.Name, uIBase);
    }
    public static T GetPanel<T>()where T : UIBase<T>
    {
        string s = typeof(T).Name;
        if (Instance.allUIPanel.ContainsKey(s))
            return Instance.allUIPanel[s] as T;
        else { BaseUtility.DebugError("该层级未找到！！");return null; }
    }
    public static void DestroyPanel<T>()
    {
        string s = typeof(T).Name;
        if (Instance.allUIPanel.ContainsKey(s))
            Instance.allUIPanel.Remove(s);
    }
    protected override void Init()
    {
        base.Init();
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        AllWedgate = new Dictionary<string, Dictionary<string, GameObject>>();
        allUIPanel = new Dictionary<string, IUIBase>();
    }
    protected override void Awake()
    {
        base.Awake();
    }
}