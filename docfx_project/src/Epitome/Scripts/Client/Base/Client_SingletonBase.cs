using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    public class Client_SingletonBase<T> : MonoBehaviour where T: Client_SingletonBase<T>
    {
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            //if (!InstanceFinder.IsClient) { Destroy(this);return; }
        }
        protected virtual void Start()
        {
           if(!Client_InstanceFinder.Register<T>(this as T))
            {
                Destroy(this);
                return;
            }

        }
        protected virtual void StartAfterNetwork()
        {

        }
        protected virtual void OnDestroy()
        {
            /*if (InstanceFinder.IsClient)*/ Client_InstanceFinder.Logout<T>();
        }

    }
}
