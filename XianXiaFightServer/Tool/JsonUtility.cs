using Newtonsoft.Json;
using Saber;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightServer.Tool;

namespace XianXiaFightGameServer.Tool
{
   public static class JsonUtility
    {
        public static FightServerParameter FightServerParameter { get;private set; }
        public static string ApplicationPath { get; private set; }

        public static string ServerInstancePath { get; private set; }
        public static string UpdateResourcePath { get; private set; }

        public static string ConfigurationPath { get; private set; }


        public static char PathSlicer { get
            {
                if (OSPlatformUtility.MyPlatformTarget==OSPlatformUtility.PlatformTarget.Windows) return '\\';
                else if (OSPlatformUtility.MyPlatformTarget == OSPlatformUtility.PlatformTarget.Linux) return '/';
                else
                    return ' ';
            } }

        public static void InitConfiguration()
        {
            GetApplicationPathAndConfigurationPath();
            Saber.SaberDebug.Log($"配置文件路径:{JsonUtility.ConfigurationPath}", ConsoleColor.Yellow);

            if (!File.Exists(ConfigurationPath))
            {
                Saber.SaberDebug.LogWarning($"似乎没有配置文件，已自动生成，请到{ConfigurationPath}目录下填写正确", ConsoleColor.Red);
                GetFightServerInstancePath();

                //FightServerParameter = new FightServerParameter();
                CreateDirectoryOrFile(ServerInstancePath,true);
                CreateDirectoryOrFile(UpdateResourcePath, true);

                JsonUtility.CreateDirectoryOrFile(JsonUtility.ConfigurationPath,true);
                JsonUtility.WriteJsonFile(JsonUtility.ConfigurationPath,JsonUtility.ToJson(new FightServerParameter("192.168.2.20",8888, 5000, new ushort[] { 6001,6002 }, ServerInstancePath,"",1,new (string, string)[]{ ("535889004@qq.com", "dhyantjmyqyjcaea")},new string[] {"203622332@qq.com"},UpdateResourcePath)));
                throw new NotImplementedException();
            }
            else
            {
                FightServerParameter = JsonUtility.GetJsonFileToObject<FightServerParameter>(JsonUtility.ConfigurationPath);
                Saber.SaberDebug.Log($"配置文件:{FightServerParameter.ToString()}", ConsoleColor.Green);
                JsonUtility.WriteJsonFile(JsonUtility.ConfigurationPath, JsonUtility.ToJson(FightServerParameter));
                ServerInstancePath = FightServerParameter.FightServerPath;
                UpdateResourcePath = FightServerParameter.UpdateResourcePath;
            }
        }

        public static void GetApplicationPathAndConfigurationPath()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            Saber.SaberDebug.Log($"当前程序运行路径 {path} ");

            string AppName = Process.GetCurrentProcess().ProcessName;
            StringBuilder report = new StringBuilder(AppName.Length * 6);
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != JsonUtility.PathSlicer)
                {
                    report.Append((char)path[i]);
                    //Console.WriteLine(report);

                    if (report.ToString() == AppName)
                    {
                        path = path.Substring(0, i + 1);
                        break;
                    }
                }
                else
                {
                    report.Clear();
                }
            }
            path += "Resources";
            ApplicationPath = path;
            switch (OSPlatformUtility.MyPlatformTarget)
            {
                case OSPlatformUtility.PlatformTarget.Windows:
                    ConfigurationPath = @$"{path}\Windows\Configuration.txt";
                    break;
                case OSPlatformUtility.PlatformTarget.Linux:
                    ConfigurationPath = @$"{path}/Linux/Configuration.txt";
                    break;
                default:
                    break;
            }
        }
        public static void ChechFightServerInstanceAndUpdateResourcePath()
        {
            bool res = File.Exists(ServerInstancePath);
            //path += @"";
            if (res)
                Saber.SaberDebug.Log($"{ServerInstancePath} is true");
            else
                Saber.SaberDebug.LogError($"please make sure [FightServer] :{ServerInstancePath} is true", ConsoleColor.Red);
            res= File.Exists(UpdateResourcePath);
            if(!res)
                Saber.SaberDebug.LogError($"please make sure [UpdateResource] :{UpdateResourcePath} is true", ConsoleColor.Red);

        }
        private static void GetFightServerInstancePath()
        {

            string path = ApplicationPath;
            string updateResourcePath = ApplicationPath;
            switch (OSPlatformUtility.MyPlatformTarget)
            {
                case OSPlatformUtility.PlatformTarget.Windows:
                    path += @"\Windows\XianXia\XianXia.exe";
                    updateResourcePath += @"\Windows\UpdateResource\XianXia.exe";
                    break;
                case OSPlatformUtility.PlatformTarget.Linux:
                    path += @"/Linux/XianXia/XianXiaServer.x86_64";
                    updateResourcePath += @"/Linux/UpdateResource/XianXiaServer.x86_64";
                    break;
                default:
                    break;
            }

            //path += @$"{JsonUtility.PathSlicer}FightServer{JsonUtility.PathSlicer}";



            UpdateResourcePath = updateResourcePath;
            ServerInstancePath = path;
        }

        public static void CreateDirectoryOrFile(string path,bool isFile=false)
        {
            //string path = "C:\\Users\\Confidence\\Testing\\";
            int i = path.LastIndexOf(PathSlicer);
            if (isFile&&i>0)
                path = path.Substring(0,path.LastIndexOf(PathSlicer));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// 将序列化的json字符串内容写入Json文件，并且保存
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="jsonConents">Json内容</param>
        public static void WriteJsonFile(string path, string jsonConents)
        {
            //string directory=path.Substring(0,path.LastIndexOf(PathSlicer));
            CreateDirectoryOrFile(path,true);
            File.WriteAllText(path, jsonConents, System.Text.Encoding.UTF8);
            SaberDebug.Log($"Succ OutPut Json To {path}");
        }

        /// <summary>
        /// 获取到本地的Json文件并且解析返回对应的json字符串
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        private static string GetJsonFile(string filepath)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return json;
        }
        public static T GetJsonFileToObject<T>(string filepath)
        {
            string s = GetJsonFile(filepath);
            return JsonConvert.DeserializeObject<T>(s);

        }
        /// <summary>
        /// 对象 转换为Json字符串
        /// </summary>
        /// <param name="tablelList">需要转换成Json的</param>
        /// <returns></returns>
        public static string ToJson(object tablelList)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(tablelList.GetType());
            string finJson = "";
            //序列化
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, tablelList);
                finJson = Encoding.UTF8.GetString(stream.ToArray());

            }
            //(tablelList + "JSON数据为:" + finJson);
            return finJson;
        }
    }
}
