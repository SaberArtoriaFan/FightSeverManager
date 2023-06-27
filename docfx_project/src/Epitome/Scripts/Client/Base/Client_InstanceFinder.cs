using FishNet;
using FishNet.Managing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XianXia
{
    public sealed class Client_InstanceFinder : Singleton<Client_InstanceFinder>
    {
        Dictionary<string, object> dict;
        protected override void Awake()
        {
            if (InstanceFinder.IsClient == false) { Destroy(this);return; }
            base.Awake();
            dict = new Dictionary<string, object>();

        }

        public static void StartAfterNetwork()
        {
            foreach(var v in Instance.dict.Values)
            {
                MethodInfo method = v.GetType().GetMethod("StartAfterNetwork", new Type[0]);
                method?.Invoke(v,new object[0]);
            }
        }
        public static bool Register<T>(T t,string name="") 
        {
            if (Instance == null) return false;
           return Instance._Register<T>(t,name);
        }
        public static void Logout<T>() 
        {
            if (Instance == null) return ;
            Instance._Logout<T>();
        }
        public static T GetInstance<T>(string name="") 
        {
            if (Instance == null) return default(T);
            return Instance._GetInstance<T>(name);
        }
        private bool _Register<T>(T t,string name="")
        {
            if (t == null) return false;
            if(string.IsNullOrEmpty(name))
                name= typeof(T).FullName;
            if (dict.ContainsKey(name)) { /*Debug.LogError($"CantExistTwoClientInstance{name}");*/return false; }
            dict.Add(name, t);
            return true;
        }

       private void _Logout<T>()
        {
            string name = typeof(T).FullName;
            if (!dict.ContainsKey(name)) { /*Debug.LogError($"CantFindClientInstance{name}");*/ return; }
            dict.Remove(name);
        }
        private T _GetInstance<T>(string name="")
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).FullName;
            if (!dict.ContainsKey(name)) { /*Debug.LogError($"CantFindClientInstance{name}"); */return default(T); }
            if (dict[name] is T res) return res;
            return default(T);
        }
        protected override void OnDestroy()
        {
            if(dict!=null)
                dict.Clear();
            base.OnDestroy();
        }
    }
}
