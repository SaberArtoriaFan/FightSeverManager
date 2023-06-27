using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public interface IFindUISub
{
    UISubManager MyUISubManager { get; }
}
public class UISubManager : MonoBehaviour,IFindUISub
{
    readonly Dictionary<string, UIBehavior> allChildren=new Dictionary<string, UIBehavior>();
    IUIBase belongPanelUIBase;

    public IUIBase BelongPanelUIBase { get => belongPanelUIBase; }

    public UISubManager MyUISubManager => this;

    public void ChangeRectSize(string WedgateName, float width = -1, float hight = -1)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetRectSize(width, hight);
        }
        else
        {
            Debug.Log("Findnot " + WedgateName);
        }
    }
    public void RegisterWedgate(string Wegate,UIBehavior uIBehavior)
    {
        if (!allChildren.ContainsKey(Wegate))
        {
            allChildren.Add(Wegate, uIBehavior);
        }
    }
    public UIBehavior GetUIBehavior(string Wedgate)
    {
        if (allChildren[Wedgate] == null)
        {
            Debug.LogWarning("找不到组件");
        }
        return allChildren[Wedgate];
    }
    #region 提供修改自身的接口
    public void Self_AddComponentToUIBehavior<T>() where T : MonoBehaviour
    {
        gameObject.AddComponent<T>();
    }
    public void Self_ClearEventTriggerEvent(EventTriggerType eventTriggerType)
    {
       
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            for (int i = 0; i < eventTrigger.triggers.Count; i++)
            {
                Entry entry = eventTrigger.triggers[i];
                if (entry.eventID == eventTriggerType)
                {
                    entry.callback.RemoveAllListeners();
                    return;
                }
            }
        }
        else
        {
            Debug.LogWarning("未找到组件");
            return;
        }
    }
    public void Self_ClearAllEventTriggerEvent()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.triggers.Clear();
        }
        else
        {
            Debug.LogWarning("未找到组件");
            return;
        }
    }
    public void Self_AddEventTriggerEvent(EventTriggerType eventTriggerType, UnityAction<BaseEventData> triggerEvent)
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            bool isContain = false;
            foreach (var v in eventTrigger.triggers)
            {
                if (v.eventID == eventTriggerType)
                {
                    v.callback.AddListener(triggerEvent);
                    isContain = true;
                    break;
                }
            }
            if (!isContain)
            {
                Entry entry = new Entry();
                entry.eventID = eventTriggerType;
                entry.callback.AddListener(triggerEvent);
                eventTrigger.triggers.Add(entry);
            }
        }
        else
        {
            Debug.LogWarning("未找到组件");
            return;
        }
    }

    //public void AddButtonLister(string Wedgate, UnityAction action)
    //{
    //    Transform temtransform = GetUIBehavior(Wedgate);
    //    Button button = temtransform.GetComponent<Button>();
    //    button.onClick.AddListener(action);
    //}
    //public void AddInputFieldonEndEditLister(string Wedgate, UnityAction<string> action)
    //{
    //    Transform temtransform = GetUIBehavior(Wedgate);
    //    InputField inputField = temtransform.GetComponent<InputField>();
    //    if (inputField != null)
    //    {
    //        inputField.onEndEdit.AddListener(action);
    //    }
    //}
    #endregion
    #region 通过再写子控件中的修改方法来达到管理子控件效果
    /// <summary>
    /// 设置子控件活性
    /// </summary>
    /// <param name="WedgateName">控件名称</param>
    /// <param name="IsActive"></param>
    public void SetSelfActive(string WedgateName, bool IsActive)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetSelfActive(IsActive);
        }
        else
        {
            Debug.Log("Findnot " + WedgateName);
        }
    }
    public void SetRaycastTarget(string WedgateName, bool IsActive)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetRaycastTarget(IsActive);
        }
        else
        {
            Debug.Log("Findnot " + WedgateName);
        }
    }
    public void SetImageEnabled(string WedgateName, bool active)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetImageEnabled(active);
        }
    }

    public void AddComponentToUIBehavior<T>(string WedgateName) where T : MonoBehaviour
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddComponentToUIBehavior<T>();
        }
        else
        {
            Debug.LogError("Findnot " + WedgateName);
        }
    }
    public void ClearEventTriggerEvent(string WedgateName, EventTriggerType eventTriggerType)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.ClearEventTriggerEvent(eventTriggerType);
        }
        else
        {
            Debug.LogError("Findnot " + WedgateName);
        }
    }
    public void AddEventTriggerEvent(string WedgateName, EventTriggerType eventTriggerType, UnityAction<BaseEventData> triggerEvent)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddEventTriggerEvent(eventTriggerType,triggerEvent);
        }
        else
        {
            Debug.LogError("Findnot " + WedgateName);
        }
    }

    //public GameObject AddExtraGrid(string WedgateName)
    //{
    //    UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
    //    if (WedgateUIBe != null)
    //    {
    //        return WedgateUIBe.AddExtraGrid();
    //    }
    //    else
    //    {
    //        Debug.Log("Findnot " + WedgateName);
    //        return null;
    //    }
    //}
    /// <summary>
    ///目标按钮控件是否激活
    /// </summary>
    /// <param name="WedgateName">目标控件</param>
    /// <returns></returns>
    public bool IsButtonActive(string WedgateName)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            return WedgateUIBe.IsButtonActive();
        }
        else
        {
            Debug.Log("Findnot UIB");
            return false;
        }
    }
    /// <summary>
    /// 清除目标按钮所有监听事件
    /// </summary>
    /// <param name="WedgateName">目标控件</param>
    public void ClearButtonAllListeners(string WedgateName)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.ClearButtonAllListeners();
        }
    }
    /// <summary>
    /// 设置目标空间Image
    /// </summary>
    /// <param name="WedgateName"></param>
    /// <param name="sprite"></param>
    public void SetImageSprite(string WedgateName, Sprite sprite)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetImageSprite(sprite);
        }
    }
    public void SetImageColor(string WedgateName,Color color)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetImageColor(color);
        }
    }
    public Sprite GetImageOriginSprite(string WedgateName)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            return WedgateUIBe.GetImageSprite();
        }
        return null;
    }
    public void SetButtonActive(string WedgateName, bool IsActive)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetButtonActive(IsActive);
        }
    }
    public void AddButtonLister(string WedgateName, UnityAction action)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddButtonLister(action);
        }
    }
    public void AddDragInterface(string WedgateName, UnityAction<BaseEventData> action)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddDragInterface(action);
        }
    }
    public void AddEndDragInterface(string WedgateName, UnityAction<BaseEventData> action)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddEndDragInterface(action);
        }
    }
    public void AddInputFieldonEndEditLister(string WedgateName, UnityAction<string> action)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddInputFieldonEndEditLister(action);
        }
    }
    public void ChangeText(string WedgateName, string newword)
    {
        UIBehavior WedgateUIB = GetUIBehavior(WedgateName);
        if (WedgateUIB != null)
        {

            WedgateUIB.ChangeText(newword);
        }
    }
    public void ChangeTextMeshProColor(string WedgateName,Color color, float duration)
    {
        UIBehavior WedgateUIB = GetUIBehavior(WedgateName);
        if (WedgateUIB != null)
        {

            WedgateUIB.ChangeTextMeshProColor(color,duration);
        }
    }
    public void ChangeImageWidth(string wedgateName, float Per)
    {
        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
        if (WedgateUIB != null)
        {

            WedgateUIB.ChangeImageWidth(Per);
        }
    }
    public void ChangeImageFillAmount(string wedgateName, float per)
    {
        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
        if (WedgateUIB != null)
        {
            WedgateUIB.ChangeImageFillAmount(per);
        }
    }
    public void AddImageFillAmount(string wedgateName, float add_Per)
    {
        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
        if (WedgateUIB != null)
        {
            WedgateUIB.AddImageFillAmount(add_Per);
        }
    }
    #endregion
    void Awake()
    {
        //将子控件存在字典中

        //向上注册
        belongPanelUIBase = transform.GetComponentInParent<IUIBase>();
        if(belongPanelUIBase != null)
            UIManager.Instance.RegisterWedgate(belongPanelUIBase.Name, transform.name, this.gameObject);
        Transform[] AllChildren = transform.GetComponentsInChildren<Transform>();
        //Debug.Log(AllChildren.Length);
        for (int i = 0; i < AllChildren.Length; i++)
        {
            if (AllChildren[i].name.EndsWith("_N"))
            {
                if (AllChildren[i].GetComponent<UIBehavior>() == null)
                {
                    RegisterWedgate(AllChildren[i].transform.name, AllChildren[i].gameObject.AddComponent<UIBehavior>());
                }
            }
        }
    }
    private void Start()
    {

    }
    //销毁
    void OnDestroy()
    {
        if (allChildren != null)
        {
            allChildren.Clear();
        }
    }
}


