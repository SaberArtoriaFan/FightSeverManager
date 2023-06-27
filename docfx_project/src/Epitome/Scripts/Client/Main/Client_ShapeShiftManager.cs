using FishNet;
using FishNet.Component.Animating;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    //public class Client_ShapeShiftManager : Client_SingletonBase<Client_ShapeShiftManager>
    //{
    //    Dictionary<GameObject, GameObject> originDict=new Dictionary<GameObject, GameObject>();
    //    Dictionary<GameObject, GameObject> shapeShiftDict=new Dictionary<GameObject, GameObject>();

    //    protected override void StartAfterNetwork()
    //    {
    //        base.StartAfterNetwork();
    //        InstanceFinder.GetInstance<NormalUtility>().UnitDeadClientAction += UnitDeadAction;
    //    }
    //    /// <summary>
    //    /// 原模型旋转和缩放必须为Reset
    //    /// </summary>
    //    /// <param name="go"></param>
    //    /// <param name="modelPath"></param>
    //    /// <param name="modelName"></param>
    //    public void ShapeShift(GameObject go,string modelPath,string modelName)
    //    {
    //        if (go == false) return;
    //        GameObject toModel;
    //        GameObject recycleModel;
    //        //第一次进来
    //        if (originDict.ContainsKey(go) == false && shapeShiftDict.ContainsKey(go))
    //        {
    //            originDict.Add(go, go.GetComponentInChildren<Animator>().gameObject);

    //        }
    //        //路径为空,则说明是变回原型
    //        if (string.IsNullOrEmpty(modelPath))
    //        {
    //            _=originDict.TryGetValue(go, out toModel);
    //            _ = shapeShiftDict.TryGetValue(go, out recycleModel);
    //        }
    //        else
    //        {
    //            if(!shapeShiftDict.TryGetValue(go, out toModel))
    //            {
    //                toModel = GameObject.Instantiate(ABUtility.Load<GameObject>(modelPath + modelName));
    //                if (toModel != null)
    //                {
    //                    toModel.transform.SetParent(go.transform);
    //                    toModel.transform.localPosition = Vector3.zero;
    //                    toModel.transform.localRotation = Quaternion.identity;
    //                    toModel.transform.localScale = Vector3.one;
    //                    toModel.SetActive(false);
    //                    shapeShiftDict.Add(go, toModel);
    //                }
    //            }

    //            _ = shapeShiftDict.TryGetValue(go, out toModel);
    //            _ = originDict.TryGetValue(go, out recycleModel);
    //        }
    //        if (recycleModel == null || toModel == null) { Debug.LogError("模型找不到了，哪里搞错了");return; }

    //        recycleModel.SetActive(false);
    //        toModel.SetActive(true);
    //        Animator animator = toModel.GetComponentInChildren<Animator>();
    //        if (InstanceFinder.IsClient)
    //        {
    //            go.GetComponent<Client_UnitProperty>()?.ChangeAnimator();
    //            go.GetComponent<NetworkAnimator>()?.SetAnimator(animator);
    //        }

    //        if (InstanceFinder.IsServer)
    //        {
    //            go.GetComponent<NetworkAnimator>()?.SetAnimator(animator);
    //            go.GetComponent<CharacterFSM>()?.ChangeAnimator(animator);
    //        }



    //    }

    //    public void UnitDeadAction(GameObject go)
    //    {
    //        if (go == null) return;
    //        if(originDict.ContainsKey(go))
    //            originDict.Remove(go);
    //        if (shapeShiftDict.ContainsKey(go))
    //            shapeShiftDict.Remove(go);
    //    }
    //    protected override void OnDestroy()
    //    {
    //        NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
    //        if (normalUtility != null)
    //            normalUtility.UnitDeadClientAction -= UnitDeadAction;
    //        base.OnDestroy();

    //    }
    //}
}
