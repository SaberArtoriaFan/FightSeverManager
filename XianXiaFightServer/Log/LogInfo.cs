using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Log
{
   public class LogInfo
    {
//        #region 监听退出事件

//        public delegate bool ControlCtrlDelegate(int CtrlType);
//        [DllImport("kernel32.dll")]
//        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
//        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

//        public static bool HandlerRoutine(int CtrlType)
//        {
//            switch (CtrlType)
//            {
//                case 0:
//                    //Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
//                    break;
//                case 2:
//                    //Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭  
//                    break;
//            }
//            InstanceFinder.GetInstance<LogManager>().OutPutJson();
//            //Console.ReadLine();
//            return false;
//        }
//#endregion
        string outPath= @$"{JsonUtility.PathSlicer}LogInfo{JsonUtility.PathSlicer}";
        StringBuilder logInfo = new StringBuilder();
        const string poolName = "LogInfo";
        static ulong _id = 0;
        ulong GetID()
        {
            if( _id == ulong.MaxValue)
                _id = 0;
            return _id++;
        }
        public static LogInfo Build()
        {
            PoolManager poolManager = InstanceFinder.GetInstance<PoolManager>();
            lock (poolManager)
            {
                if (!poolManager.IsPoolAlive(poolName))
                    _ = poolManager.AddPool(() => { return new LogInfo(); }, (t) => { t.logInfo.Clear(); }, null, poolName);
            }
            ObjectPool<LogInfo> p = poolManager.GetPool<LogInfo>(poolName);
            lock (p)
            {
                return p.GetObjectInPool();
            }
        }
        public static void Recycle(LogInfo logInfo)
        {
            PoolManager poolManager = InstanceFinder.GetInstance<PoolManager>();
            lock (poolManager)
            {
                if (!poolManager.IsPoolAlive(poolName))
                {
                    PoolManager.LogRecycleError(poolName);
                    return;
                }
                else
                    poolManager.RecycleToPool(logInfo, poolName);
            }
        }
        private LogInfo()
        {
            //SetConsoleCtrlHandler(cancelHandler, true);
        }
        public void Reocrd(string message)
        {
            logInfo.Append($"{message}\n");
        }
        public void OutPutJson()
        {
            string today= System.DateTime.Now.ToString("d");
            today=today.Replace("/", "").Replace(" ", "").Replace(":", "");
            today = $"{JsonUtility.ApplicationPath}{outPath}{today}";
            JsonUtility.CreateDirectoryOrFile(today);
            string realPath = System.DateTime.Now.ToString("G");
            realPath=realPath.Replace("/","").Replace(" ","").Replace(":","");
            realPath = $"{GetID()}_{realPath}";
            realPath= today + JsonUtility.PathSlicer +realPath;
            //JsonUtility.CreateDirectoryOrFile
            Saber.SaberDebug.Log($"Is Output To {realPath}......");
            JsonUtility.WriteJsonFile(realPath, logInfo.ToString());
        }

    }
}
