using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using FishNet;
using Sirenix.OdinInspector;
using Saber.ECS;
using System.Text;
//using YooAsset.Editor;

namespace XianXia.Unit
{
    public class Server_GetAnimationTimeLong : MonoBehaviour,IClothes
    {
        //[ShowInInspector]
        [SerializeField]
        string ABPackage;
        [SerializeField]
        //[ShowInInspector]
        string path;
        [SerializeField]
        [AssetsOnly]
        SkeletonDataAsset skeletonDataAsset;
        float timer = 0;
        int id;
        void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (skeletonDataAsset != null)
                {
                    path = UnityEditor.AssetDatabase.GetAssetPath(skeletonDataAsset);
                    int len = path.Length;
                    path = path.Replace(ABUtility.ABPackageDataPath, "");
                    if (len == path.Length)
                    {
                        Debug.LogError(path + "所选资产似乎并不是位于合法路径");
                        path = "";
                        skeletonDataAsset = null;
                        ABPackage = "";
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 1; i < path.Length; i++)
                        {

                            if (path[i] == '/' || path[i] == '\\')
                            {
                                ABPackage = stringBuilder.ToString();
                                return;
                            }
                            stringBuilder.Append(path[i]);
                        }
                    }
                }
                else
                {
                    path = "";
                    ABPackage = "";
                }
            }

#endif
        }
        void Awake()
        {

#if UNITY_SERVER
            if (skeletonDataAsset == null)
                skeletonDataAsset = ABUtility.Load<SkeletonDataAsset>(path, ABPackage);
            id = Animator.StringToHash(skeletonDataAsset.name);
            if (!InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().IsContains(id))
            {
                Debug.Log("设置动画时长" + skeletonDataAsset.name);
                Dictionary<int, float> animationDict = new Dictionary<int, float>();
                foreach (var v in skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations)
                {
                    Debug.Log(gameObject.name + "ID_" + id + "Spine添加了" + v.Name + "_" + Animator.StringToHash(v.Name) + "时长_" + v.Duration);
                    _ = animationDict.TryAdd(Animator.StringToHash(v.Name), v.Duration);

                }
                InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().Register(id, animationDict);
            }

#endif
        }


        public int Init(string modelName)
        {
            if (!string.IsNullOrEmpty(modelName) && !InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().heroAnimationDict.ContainsKey(modelName))
                InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().heroAnimationDict.Add(modelName, id);

            TimerManager.Instance.AddTimer(() =>
            {
                GameObject.Destroy(this);
            }, 1);
            return id;
    
        }
    }
}
