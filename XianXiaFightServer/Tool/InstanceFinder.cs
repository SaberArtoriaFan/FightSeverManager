using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XianXiaFightGameServer.Tool
{
    internal class InstanceFinder
    {
        static InstanceFinder instance;
        Dictionary<string, object> dict;
        public InstanceFinder()
        {
            if (instance == null) { instance = this; }
            else { Saber.SaberDebug.LogWarning("Cant Create InstanceFinder Twice!!!", ConsoleColor.Red);return; }
            dict = new Dictionary<string, object>();

        }

        public static void StartAfterNetwork()
        {
            foreach (var v in instance.dict.Values)
            {
                MethodInfo method = v.GetType().GetMethod("StartAfterNetwork", new Type[0]);
                method?.Invoke(v, new object[0]);
            }
        }
        public static bool Register<T>(T t)
        {
            if (instance == null) return false;
            return instance._Register<T>(t);
        }
        public static void Logout<T>()
        {
            if (instance == null) return;
            instance._Logout<T>();
        }
        public static T GetInstance<T>()
        {
            if (instance == null) return default(T);
            return instance._GetInstance<T>();
        }
        private bool _Register<T>(T t)
        {
            if (t == null) return false;
            string name = typeof(T).FullName;
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
        private T _GetInstance<T>()
        {
            string name = typeof(T).FullName;
            if (!dict.ContainsKey(name)) { /*Debug.LogError($"CantFindClientInstance{name}"); */return default(T); }
            if (dict[name] is T res) return res;
            return default(T);
        }
        ~InstanceFinder()
        {
            if (instance != null)
                instance = null;
            if (dict != null)
                dict.Clear();
        }
    }
}
