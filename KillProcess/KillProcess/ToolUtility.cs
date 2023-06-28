using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Tool;

namespace KillProcess
{
    [DataContractAttribute]
    internal class KillParameter
    {
        [DataMemberAttribute]
        private string perentProcessname;
        [DataMemberAttribute]
        private string sonProcessName;

        public KillParameter(string perentProcessname, string sonProcessName)
        {
            this.perentProcessname = perentProcessname;
            this.sonProcessName = sonProcessName;
        }

        public string SonProcessName { get => sonProcessName; set => sonProcessName = value; }
        public string PerentProcessname => perentProcessname;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Config\n");
            stringBuilder.Append($"子进程名字:{sonProcessName}\n");
            stringBuilder.Append($"父进程名字:{perentProcessname}\n");
            stringBuilder.Append("\n");
            return stringBuilder.ToString();
        }
    
    }

    internal static class ToolUtility
    {
        public static string ApplicationPath { get; private set; }

        public static string ConfigurationPath { get; private set; }
        public static void GetApplicationPathAndConfigurationPath()
        {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            Console.WriteLine($"当前程序运行路径 {path} ", ConsoleColor.Green);

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

        public static void CheckConfigurationPath()
        {
            if (!File.Exists(ConfigurationPath))
            {
                Console.WriteLine($"似乎没有配置文件，已自动生成，请到{ConfigurationPath}目录下填写正确", ConsoleColor.Red);
                //GetFightServerInstancePath();

                //FightServerParameter = new FightServerParameter();
                //CreateDirectoryOrFile(ServerInstancePath, true);
                JsonUtility.CreateDirectoryOrFile(ConfigurationPath, true);
                JsonUtility.WriteJsonFile(ConfigurationPath, JsonUtility.ToJson(new KillParameter("XianXiaServer", "XianXiaServer.x86_64")));
            }
            Console.WriteLine($"配置文件目录：{ConfigurationPath}");
        }
        internal static KillParameter ReadParamter()
        {
            GetApplicationPathAndConfigurationPath();
            CheckConfigurationPath();
            KillParameter res = JsonUtility.GetJsonFileToObject<KillParameter>(ConfigurationPath);
            if (res != null)
            {
               Console.WriteLine($"配置文件:{res.ToString()}", ConsoleColor.Green);
            }
            return res;
        }
        internal static int KillProcess(string processName)
        {
            int res = 0;
            foreach (var process in Process.GetProcessesByName(processName))
            {
                if (process != null)
                {
                    int id = process.Id;
                    try
                    {
                        // 杀掉这个进程。
                        process.Kill();

                        // 等待进程被杀掉。你也可以在这里加上一个超时时间（毫秒整数）。
                        process.WaitForExit();
                        Console.WriteLine($"{processName}:关闭了{id}号进程");
                        res ++;
                    }
                    catch (Exception ex)
                    {
                        // 无法结束进程，可能有很多原因。
                        // 建议记录这个异常，如果你的程序能够处理这里的某种特定异常了，那么就需要在这里补充处理。
                        // Log.Error(ex);
                        Console.WriteLine($"{processName}:无法关闭进程{id}，错误原因：{ex.ToString()}");
                    }
                }

            }
            return res;
        }
    }
}
