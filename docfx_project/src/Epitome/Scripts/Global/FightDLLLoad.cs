using HybridCLR;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class FightDLLLoad : MonoBehaviour
{
    public const string hotAssemblyABPath= "/Main/FightDll";
    private const string BytesExtension=".bytes";
    Dictionary<string, Assembly> hotAssemblyDict = new Dictionary<string, Assembly>();
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public async Task InitHotUpdateDLL()
    {
        LoadMetadataForAOTAssemblies();

    }
    private static void LoadMetadataForAOTAssemblies()
    {
        //List<string> aotMetaAssemblyFiles = HybridCLR.Editor.SettingsUtil.AOTAssemblyNames;

        ///// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        ///// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        ///// 
        //HomologousImageMode mode = HomologousImageMode.SuperSet;
        //foreach (var aotDllName in aotMetaAssemblyFiles)
        //{
        //    byte[] dllBytes = ReadBytesFromAB(aotDllName + ".bytes");
        //    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
        //    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
        //    Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        //}
    }

    private static byte[] ReadBytesFromAB(string v)
    {
        return ABUtility.Load<TextAsset>(GetDllInRuntimePath(v)).bytes;
    }
    public static string GetDllInRuntimePath(string name)
    {
        JEngine.Core.Tools.EnsureEndWith(ref name, BytesExtension);
        return Path.Combine(hotAssemblyABPath, name);
    }
}
