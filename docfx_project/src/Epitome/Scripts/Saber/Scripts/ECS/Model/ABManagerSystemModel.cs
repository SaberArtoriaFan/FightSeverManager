using Saber.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
//    public class ABManagerSystemModel : SystemModelBase
//    {
//        //AB包缓存---解决AB包无法重复加载的问题 也有利于提高效率。
//        private Dictionary<string, AssetBundle> abCache;

//        private AssetBundle mainAB = null; //主包

//        private AssetBundleManifest mainManifest = null; //主包中配置文件---用以获取依赖包

//        internal Dictionary<string, AssetBundle> AbCache { get => abCache; set => abCache = value; }
//        internal AssetBundle MainAB { get => mainAB; set => mainAB = value; }
//        internal AssetBundleManifest MainManifest { get => mainManifest; set => mainManifest = value; }

//        //各个平台下的基础路径 --- 利用宏判断当前平台下的streamingAssets路径
//        internal string BasePath
//        {
//            get
//            {
//                //使用StreamingAssets路径注意AB包打包时 勾选copy to streamingAssets
//#if UNITY_EDITOR || UNITY_STANDALONE
//                return Application.dataPath + "/StreamingAssets/";
//#elif UNITY_IPHONE
//                return Application.dataPath + "/Raw/";
//#elif UNITY_ANDROID
//                return Application.dataPath + "!/assets/"                
//#endif

//            }
//        }
//        //各个平台下的主包名称 --- 用以加载主包获取依赖信息
//        internal string MainABName
//        {
//            get
//            {
//#if UNITY_EDITOR || UNITY_STANDALONE
//                return "StandaloneWindows";
//#elif UNITY_IPHONE
//                return "IOS";
//#elif UNITY_ANDROID
//                return "Android";
//#endif
//            }
//        }
//    }
}
