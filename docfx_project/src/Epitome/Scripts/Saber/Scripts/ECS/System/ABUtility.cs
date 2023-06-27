using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Saber.ECS
{
    public abstract class ABUtility : AutoSingleton<ABUtility> 
    {

        public const string ABPackageDataPath = "Assets/HotUpdateResources";
        public const string EffectMainName = "/Main/Common/Prefab/FightServer/Effect/";
        public const string ProjectMainName = "/Main/Common/Prefab/FightServer/Projectile/";
        public const string UIMainName = "/Main/Common/Prefab/FightServer/UI/";
        public const string UnitMainName = "/Main/Common/Prefab/FightServer/Unit/";
        public const string SummonMainName = "/Main/Common/Prefab/FightServer/Summon/";
        public const string ShiftShapeMainName = "/Main/Common/Prefab/FightServer/ShiftShape/";
        public const string ScriptableObjectMainName = "/Main/Common/ScriptableObject/";

        public const string InitMainName = "/Main/Common/Prefab/FightServer/Init/";
        public const string LubanMainName = "/ExcelJson/FightServer/";
        public const string MainPackageName = "Main";
        public const string JsonPackageName = "ExcelJson";
        public static T1 Load<T1>(string path,string packageName=MainPackageName)where T1: Object
        {
            path=$"{ABPackageDataPath}{path}";
            return Instance.LoadAssest<T1>(path,packageName);
        }
        public static Object Load(string path, System.Type type,string packageName = MainPackageName)
        {
            path = ABPackageDataPath + path;
            return Instance.LoadAssest(path, packageName, type);
        }
        public static void LoadAsyncScene(string path, string packageName=MainPackageName)
        {
            path = $"{ABPackageDataPath}{path}";
            Instance._LoadAsyncScene(path, packageName);

        }
        protected abstract void _LoadAsyncScene(string path, string packageName );
        protected abstract T1 LoadAssest<T1>(string path,string packageName) where T1 : Object;

        protected abstract Object LoadAssest(string path, string packageName,System.Type type);
    }
}
