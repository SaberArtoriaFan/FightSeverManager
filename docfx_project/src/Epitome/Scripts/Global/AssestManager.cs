using JEngine.Core;
using System.Collections;
using System.Collections.Generic;
using Saber.ECS;
using Saber.Base;
using UnityEngine;
using System;
using YooAsset;

public class AssestManager : ABUtility
{
    protected override T LoadAssest<T>(string path,string packgeName)
    {
        //Singleton
       return AssetMgr.Load<T>(path,packgeName);
    }
    protected override UnityEngine.Object LoadAssest(string path, string packageName,Type type) => AssetMgr.Load(path, packageName, type);
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //TimerManager.Instance.AddTimer(() =>
        //{
        //    ResourcePackage package = YooAssets.GetPackage(JsonPackageName);
        //    foreach (var v in package.GetAssetInfos("EJ"))
        //    {
        //        Debug.Log("SSS" + v.Address + v.AssetPath);
        //    }
        //}, 5);



    }

    protected override async void _LoadAsyncScene(string path, string packageName)
    {
       await AssetMgr.LoadSceneAsync(path, false, packageName);
    }
}
