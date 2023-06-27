using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface IUIBase
{
    string Name { get; }
}
//public abstract class NetworkUIBase<T> : NetworkSingleton<T>,IUIBase where T : NetworkUIBase<T>
//{

//    public string Name => this.name;
//    #region 通过Manager拿到自己的子控件们

//    public GameObject GetWedgateGameObject(string WedgateName)
//    {
//        GameObject gameObject = UIManager.Instance.GetWedgateGameObject(transform.name, WedgateName);
//        if (gameObject != null)
//        {
//            return gameObject;
//        }
//        return null;
//    }
//    public UIBehavior GetUIBehavior(string WedgateName)
//    {
//        GameObject gameObject = GetWedgateGameObject(WedgateName);
//        if (gameObject != null)
//        {
//            return gameObject.GetComponent<UIBehavior>();
//        }
//        return null;
//    }
//    public UISubManager GetUISubManager(string WedgateName)
//    {
//        GameObject gameObject = GetWedgateGameObject(WedgateName);
//        if (gameObject != null)
//        {
//            return gameObject.GetComponent<UISubManager>();
//        }
//        return null;
//    }
//    #endregion
//    #region 通过再写子控件中的修改方法来达到管理子控件效果
//    /// <summary>
//    /// 设置子控件活性
//    /// </summary>
//    /// <param name="WedgateName">控件名称</param>
//    /// <param name="IsActive"></param>
//    public void SetSelfActive(string WedgateName, bool IsActive)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.SetSelfActive(IsActive);
//        }
//        else
//        {
//            Debug.Log("Findnot " + WedgateName);
//        }
//    }
//    //public GameObject AddExtraGrid(string WedgateName)
//    //{
//    //    UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//    //    if (WedgateUIBe != null)
//    //    {
//    //        return WedgateUIBe.AddExtraGrid();
//    //    }
//    //    else
//    //    {
//    //        Debug.Log("Findnot " + WedgateName);
//    //        return null;
//    //    }
//    //}
//    /// <summary>
//    ///目标按钮控件是否激活
//    /// </summary>
//    /// <param name="WedgateName">目标控件</param>
//    /// <returns></returns>
//    public bool IsButtonActive(string WedgateName)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            return WedgateUIBe.IsButtonActive();
//        }
//        else
//        {
//            Debug.Log("Findnot UIB");
//            return false;
//        }
//    }
//    /// <summary>
//    /// 清除目标按钮所有监听事件
//    /// </summary>
//    /// <param name="WedgateName">目标控件</param>
//    public void ClearButtonAllListeners(string WedgateName)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.ClearButtonAllListeners();
//        }
//    }
//    /// <summary>
//    /// 设置目标空间Image
//    /// </summary>
//    /// <param name="WedgateName"></param>
//    /// <param name="sprite"></param>
//    public void SetImageSprite(string WedgateName, Sprite sprite)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.SetImageSprite(sprite);
//        }
//    }
//    public void SetImageEnabled(string WedgateName,bool active)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.SetImageEnabled(active);
//        }
//    }
//    public Sprite GetImageOriginSprite(string WedgateName)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//           return WedgateUIBe.GetImageSprite();
//        }
//        return null;
//    }
//    public void SetImageColor(string WedgateName,Color color)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.SetImageColor(color);
//        }
//    }
//    public void SetButtonActive(string WedgateName, bool IsActive)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.SetButtonActive(IsActive);
//        }
//    }
//    public void AddButtonLister(string WedgateName, UnityAction action)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.AddButtonLister(action);
//        }
//    }
//    public void AddButtonLister(string SubWedgateName, string WedgateName, UnityAction action)
//    {
//        UISubManager uISubManager = GetUISubManager(SubWedgateName);
//        if (uISubManager != null)
//        {
//            uISubManager.AddButtonLister(WedgateName, action);
//        }
//    }
//    public void AddDragInterface(string WedgateName, UnityAction<BaseEventData> action)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.AddDragInterface(action);
//        }
//    }
//    public void AddEndDragInterface(string WedgateName, UnityAction<BaseEventData> action)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.AddEndDragInterface(action);
//        }
//    }
//    public void AddInputFieldonEndEditLister(string WedgateName, UnityAction<string> action)
//    {
//        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
//        if (WedgateUIBe != null)
//        {
//            WedgateUIBe.AddInputFieldonEndEditLister(action);
//        }
//    }
//    public void AddInputFieldonEndEditLister(string SubWedgateName, string WedgateName, UnityAction<string> action)
//    {
//        UISubManager uISubManager = GetUISubManager(SubWedgateName);
//        if (uISubManager != null)
//        {
//            uISubManager.AddInputFieldonEndEditLister(WedgateName, action);
//        }
//    }
//    public void ChangeText(string WedgateName, string newword)
//    {
//        UIBehavior WedgateUIB = GetUIBehavior(WedgateName);
//        if (WedgateUIB != null)
//        {

//            WedgateUIB.ChangeText(newword);
//        }
//    }
//    public void ChangeImageWidth(string wedgateName, float Per)
//    {
//        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
//        if (WedgateUIB != null)
//        {

//            WedgateUIB.ChangeImageWidth(Per);
//        }
//    }
//    public void ChangeImageFillAmount(string wedgateName, float per)
//    {
//        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
//        if (WedgateUIB != null)
//        {
//            WedgateUIB.ChangeImageFillAmount(per);
//        }
//    }
//    public void AddImageFillAmount(string wedgateName, float add_Per)
//    {
//        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
//        if (WedgateUIB != null)
//        {
//            WedgateUIB.AddImageFillAmount(add_Per);
//        }
//    }
//    #endregion
//    protected virtual void Start()
//    { 
//        //找到C层下面的所以子控件，动态添加组件
//        Transform[] AllChildren = transform.GetComponentsInChildren<Transform>();
//        //Debug.Log(AllChildren.Length);
//        for (int i = 0; i < AllChildren.Length; i++)
//        {
//            if (AllChildren[i].name.EndsWith("_N"))
//            {
//                AllChildren[i].gameObject.AddComponent<UIBehavior>();
//            }
//            if (AllChildren[i].name.EndsWith("_S"))
//            {
//                AllChildren[i].gameObject.AddComponent<UISubManager>();
//            }
//        }
//        //this.gameObject.SetActive(false);
//    }

//    protected virtual void Destroy()
//    {
//        UIManager.Instance.DestroyPanel(transform.name);
//    }
//}

public abstract class UIBase<T> : MonoBehaviour, IUIBase where T : UIBase<T>
{
    private string uiName;
    public string Name => uiName;
    protected virtual GameObject BackGround => null;
    #region 通过Manager拿到自己的子控件们

    public GameObject GetWedgateGameObject(string WedgateName)
    {
        GameObject gameObject = UIManager.Instance.GetWedgateGameObject(uiName, WedgateName);
        if (gameObject != null)
        {
            return gameObject;
        }
        return null;
    }
    public UIBehavior GetUIBehavior(string WedgateName)
    {
        GameObject gameObject = GetWedgateGameObject(WedgateName);
        if (gameObject != null)
        {
            return gameObject.GetComponent<UIBehavior>();
        }
        return null;
    }
    public UISubManager GetUISubManager(string WedgateName)
    {
        GameObject gameObject = GetWedgateGameObject(WedgateName);
        if (gameObject != null)
        {
            return gameObject.GetComponent<UISubManager>();
        }
        return null;
    }
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
    public void SetPivot(string WedgateName,int x,int y)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetPivot(x, y);
        }
        else
        {
            Debug.Log("Findnot " + WedgateName);
        }
    }
    public void ChangePosition(string WedgateName, Vector3 pos,bool isLoacl=true)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.ChangePosition(pos, isLoacl);
        }
        else
        {
            Debug.Log("Findnot " + WedgateName);
        }
    }
    public void ChangeRectSize(string WedgateName,float width=-1,float hight=-1)
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
        Debug.Log(WedgateUIBe.name+"ada");
        if (WedgateUIBe != null)
        {
            WedgateUIBe.SetImageSprite(sprite);
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
    public void ChangeImageCrossFade(string WedgateName,float alpha, float duration)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.ChangeImageCrossFade(alpha, duration);
        }
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
    public void AddButtonLister(string SubWedgateName, string WedgateName, UnityAction action)
    {   
        UISubManager uISubManager = GetUISubManager(SubWedgateName);
        if (uISubManager != null)
        {
            uISubManager.AddButtonLister(WedgateName, action);
        }
    }
    public void AddScrollBarValueChanged(string WedgateName,UnityAction<float> action){
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.AddScrollBarValueChanged(action);
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
    public void ChangeInputFieldText(string WedgateName, string str)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            WedgateUIBe.ChangeInputFieldText(str);
        }
    }
    public string GetInputFieldText(string WedgateName)
    {
        UIBehavior WedgateUIBe = GetUIBehavior(WedgateName);
        if (WedgateUIBe != null)
        {
            return WedgateUIBe.GetInputFieldText();
        }
        else
        {
            return null;
        }
    }
    public void AddInputFieldonEndEditLister(string SubWedgateName, string WedgateName, UnityAction<string> action)
    {
        UISubManager uISubManager = GetUISubManager(SubWedgateName);
        if (uISubManager != null)
        {
            uISubManager.AddInputFieldonEndEditLister(WedgateName, action);
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
    public void ChangeText(string WedgateName, string newword, float alpha, float duration)
    {
        UIBehavior WedgateUIB = GetUIBehavior(WedgateName);
        if (WedgateUIB != null)
        {

            WedgateUIB.ChangeText(newword,alpha,duration);
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
    public void ChangeScrollBarValue(string wedgateName,float value){
        UIBehavior WedgateUIB = GetUIBehavior(wedgateName);
        if (WedgateUIB != null)
        {
            WedgateUIB.ChangeScrollBarValue(value);
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

    public virtual void Open()
    {

    }
    public virtual void Close()
    {

    }
    protected virtual void Start()
    {
        if (BackGround != null) BackGround.SetActive(true);
        uiName = this.GetType().Name;
        UIManager.RegisterPanel(this);
        //找到C层下面的所以子控件，动态添加组件
        Transform[] AllChildren = transform.GetComponentsInChildren<Transform>();
        //Debug.Log(AllChildren.Length);
        for (int i = 0; i < AllChildren.Length; i++)
        {
            if (AllChildren[i].name.EndsWith("_N"))
            {
                if (AllChildren[i].GetComponent<UIBehavior>()==null)
                    AllChildren[i].gameObject.AddComponent<UIBehavior>();
            }
            if (AllChildren[i].name.EndsWith("_S"))
            {
                if(AllChildren[i].GetComponent<UISubManager>()==null)
                    AllChildren[i].gameObject.AddComponent<UISubManager>();
            }
        }
        if (BackGround != null) BackGround.SetActive(false);
        //this.gameObject.SetActive(false);
        TimerManager.Instance.AddTimer(Init,Time.deltaTime*5);

    }
    protected virtual void Init()
    {
    }
    protected virtual void Destroy()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.DestroyPanel(transform.name);
            UIManager.DestroyPanel<T>();
        }
    }
}

