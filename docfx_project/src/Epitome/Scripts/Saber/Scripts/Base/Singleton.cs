using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = (T)this;
        else if(instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Init();
    }

    public static bool IsInitialized { get { return instance != null; } }
    protected virtual void Init()
    {

    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}