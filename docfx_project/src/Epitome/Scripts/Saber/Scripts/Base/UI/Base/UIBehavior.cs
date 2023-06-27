using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using TMPro;

public class UIBehavior : MonoBehaviour,IFindUISub
{
    UISubManager belongUISub=null;
    public UISubManager MyUISubManager => belongUISub;

    //通过写子控件修改自身属性的方法
    #region ChangeSelf
    public void SetSelfActive(bool IsActive)
    {
        this.gameObject.SetActive(IsActive);
    }
    public void SetRectSize(float width,float hight)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            if (width == -1) width = rectTransform.sizeDelta.x;
            if (hight == -1) hight = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(width, hight);
        }
        else
            Debug.Log("Findnot ");
    }
    public void SetRaycastTarget(bool value)
    {
        MaskableGraphic maskableGraphic=GetComponent<MaskableGraphic>();
        if (maskableGraphic != null)
        {
            maskableGraphic.raycastTarget = value;
        }
        else
            Debug.Log("Findnot ");
    }
    public void SetPivot(int x,int y)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.pivot = new Vector2(x, y);

        }
        else
            Debug.Log("Findnot ");
    }
    //public void SetImageTransparency(bool value)
    //{
    //    Image image = GetComponent<Image>();
    //    if (value)
    //    {
    //        image.
    //    }
    //}
    public void ChangePosition(Vector3 pos,bool isLoacl=true)
    {
        if (isLoacl)
            transform.localPosition = pos;
        else
            transform.position = pos;
    }
    public bool IsButtonActive()
    {
        Button button = transform.GetComponent<Button>();
        if (button != null)
        {
            return button.interactable;
        }
        else
        {
            Debug.Log("Findnot Button");
            return false;
        }
    }
    public void SetImageSprite(Sprite sprite)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.overrideSprite = sprite;
        }
    }
    public void SetImageColor(Color color)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color =color;
        }
    }
    public void SetImageEnabled(bool active)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.enabled = active;
        }
    }
    public Sprite GetImageSprite()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            return image.overrideSprite;
        }
        return null;
    }
    public void ClearButtonAllListeners()
    {
        Button button = transform.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    public void SetButtonActive(bool IsActive)
    {
        Button button = transform.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = IsActive;
        }
    }
    public void AddButtonLister(UnityAction action)
    {
        Button button = transform.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }
    public void AddScrollBarValueChanged(UnityAction<float> action){
        Scrollbar scrollbar=transform.GetComponent<Scrollbar>();
        if(scrollbar!=null){
            scrollbar.onValueChanged.AddListener(action);
        }
    }
    public void AddInputFieldonEndEditLister(UnityAction<string> action)
    {
        InputField inputField = transform.GetComponent<InputField>();
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(action);
        }
    }
    public void ChangeInputFieldText(string str)
    {
        InputField inputField = transform.GetComponent<InputField>();
        if (inputField != null)
        {
            inputField.text = str;
        }
    }
    public string GetInputFieldText()
    {
        InputField inputField = transform.GetComponent<InputField>();
        if (inputField != null)
        {
            return inputField.text;
        }
        else
        {
            Debug.LogWarning("未找到组件");
            return "";
        }
    }
    public void AddComponentToUIBehavior<T>()where T:MonoBehaviour
    {
        gameObject.AddComponent<T>();
    }
    public void ClearEventTriggerEvent(EventTriggerType eventTriggerType)
    {
        EventTrigger eventTrigger=GetComponent<EventTrigger>();
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
    public void AddEventTriggerEvent(EventTriggerType eventTriggerType, UnityAction<BaseEventData> triggerEvent)
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
    public void ChangeText(string newword)
    {
        Text text = GetComponent<Text>();
        if(text != null)
            text.text = newword;
        else
        {
            TMP_Text tMP_Text=GetComponent<TMP_Text>();
            if(tMP_Text != null)
                tMP_Text.text = newword;
        }
        //text.text.Replace(oldword, newword);
    }
    public void ChangeTextMeshProColor(Color color, float duration)
    {
        TMP_Text tMP_Text = GetComponent<TMP_Text>();
        if (tMP_Text != null)
            tMP_Text.color = color;
    }
    public void ChangeText(string newword, float alpha, float duration)
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = newword;
            text.CrossFadeAlpha(alpha, duration, false);
        }
        else
        {
            TMP_Text tMP_Text = GetComponent<TMP_Text>();
            if (tMP_Text != null)
            {
                tMP_Text.text = newword;
                tMP_Text.CrossFadeAlpha(alpha, duration, false);
            }
        }

        //text.text.Replace(oldword, newword);
    }
    public void ChangeImageCrossFade(float alpha, float duration)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.CrossFadeAlpha(alpha, duration, false);
        }
    }
    public void ChangeImageWidth(float Per)
    {
        RectTransform rect = GetComponent<RectTransform>();
        float WidthMax = 600;
        float CurrentWidth = WidthMax * Per;
        rect.sizeDelta = new Vector2(CurrentWidth, rect.sizeDelta.y);
    }
    public void ChangeImageFillAmount(float per)
    {
        Image image = GetComponent<Image>();
        if (per >= 0 && per <= 1)
            image.fillAmount = per;
        else
        {
            image.fillAmount = 0;

            Debug.LogWarning("传入参数错误");
        }

    }
    public void ChangeScrollBarValue(float value){
        Scrollbar scrollbar=GetComponent<Scrollbar>();
        if(scrollbar!=null){
            scrollbar.value=value;
        }
    }
    /// <summary>
    /// 与另一个函数最大不同是直接加上这个Float
    /// </summary>
    public void AddImageFillAmount(float add_Per)
    {
        Image image = GetComponent<Image>();
        if (add_Per >= -1 && add_Per <= 1)
            image.fillAmount += add_Per;
        else
            Debug.LogError("传入参数错误");
    }

    #endregion
    #region ChangeSelfWithJoggle
    public void AddDragInterface(UnityAction<BaseEventData> action)
    {
        //事件
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件实体ID(Drag拖拽，，点击等事件)
        entry.eventID = EventTriggerType.Drag;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }
    public void AddEndDragInterface(UnityAction<BaseEventData> action)
    {
        //事件
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        //事件实体
        EventTrigger.Entry entry = new EventTrigger.Entry();
        //事件实体ID(Drag拖拽，，点击等事件)
        entry.eventID = EventTriggerType.EndDrag;
        //事件回调
        entry.callback = new EventTrigger.TriggerEvent();
        //添加回调函数
        entry.callback.AddListener(action);
        //监听事件
        trigger.triggers.Add(entry);
    }


    #endregion
    #region AddExtraGrid
    //public GameObject AddExtraGrid()
    //{
    //    GameObject ExtraGrid = Utility.LoadGameObject("UI/ExtraGrid/BuildMenu");
    //    ExtraGrid.transform.SetParent(this.transform);
    //    ExtraGrid.AddComponent<BuildMenuShow>();
    //    ExtraGrid.transform.localPosition = Vector3.zero;
    //    ExtraGrid.transform.localRotation = Quaternion.identity;
    //    ExtraGrid.transform.localScale = Vector3.one;
    //    //Image ExtraGrid = Tem.GetComponent<Image>();
    //    //ExtraGrid.rectTransform.localPosition = Vector3.zero;
    //    //ExtraGrid.rectTransform.localRotation = Quaternion.identity;
    //    //ExtraGrid.rectTransform.localScale = Vector3.one;
    //    return ExtraGrid;
    //}
    #endregion
    protected virtual void Awake()
    {
        UISubManager uISub=transform.GetComponentInParent<UISubManager>();
        IUIBase uIBase = transform.GetComponentInParent<IUIBase>();
        //Debug.Log(uIBase.name);
        if (uISub != null)
        {
            belongUISub = uISub;
            uISub.RegisterWedgate(transform.name, this);
        }
        else if(uIBase!= null)
            UIManager.Instance.RegisterWedgate(uIBase.Name, transform.name, transform.gameObject);
    }
}


