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

        ///// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        ///// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        ///// 
        //HomologousImageMode mode = HomologousImageMode.SuperSet;
        //foreach (var aotDllName in aotMetaAssemblyFiles)
        //{
        //    byte[] dllBytes = ReadBytesFromAB(aotDllName + ".bytes");
        //    // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
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
