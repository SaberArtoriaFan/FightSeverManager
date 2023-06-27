using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
//Saber阿尔托莉雅

    public class AutoSingleton<T> : Singleton<T>where T:AutoSingleton<T>
    {
        public new static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else
                {
                    string name = typeof(T).FullName;
                    GameObject gameObject = new GameObject(name);
                    instance=gameObject.AddComponent<T>();
                    return instance;
                }
            }
        }
    }
