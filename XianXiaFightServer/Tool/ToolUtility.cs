using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Tool
{
    public class ToolUtility
    {
        public static string ArrayToLog(string[] arr)
        {
            if (arr == null || arr.Length <= 0) return string.Empty;
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string str in arr)
                {
                    stringBuilder.Append($"{str}\n");
                }
                return stringBuilder.ToString();
            }
        }

        public static void InitAppEixtEvent()
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += (ctx) =>
            {
                
                InstanceFinder.GetInstance<LocalServer>().OnDestroy();
                Saber.SaberDebug.Log("退出成功！");
                HandleOn();
            };
        }
        public static async void HandleOn()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
            Console.CancelKeyPress += (s, e) => cancellationTokenSource.Cancel();
            await Task.Delay(-1, cancellationTokenSource.Token).ContinueWith(t =>
            {

            });
        }

    }
    
}
