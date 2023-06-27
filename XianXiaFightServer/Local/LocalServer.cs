using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Proto;
using XianXiaFightGameServer.Controller;
using XianXiaFightGameServer.Tool;
using System.Diagnostics;
using XianXiaFightServer.Tool;
using XianXiaFightGameServer.Log;
using Saber;

namespace XianXiaFightGameServer.Local
{
    public class LocalServer
    {
        public class UpdateResourceHelper 
        {
            //更新资源的程序的地址
            string path;
            bool isUpdateResourceing = false;
            bool isMainProcessUpdated =true;
            int updatingProcessID = 0;
            LocalServer localServer;
            public UpdateResourceHelper(LocalServer localServer,string path)
            {
                this.localServer = localServer;
                this.path = path;
            }

            public bool IsUpdateResourceing { get => isUpdateResourceing;}
            public bool IsMainProcessUpdated { get => isMainProcessUpdated; }
            public int UpdatingProcessID { get => updatingProcessID; set => updatingProcessID = value; }
            public string Path { get => path;  }
            public void BreakUpdateResource()
            {
                isUpdateResourceing = false;
                isMainProcessUpdated = true;
                updatingProcessID = 0;
                Saber.SaberDebug.LogWarning("[资源热更新]:失败！！");
            }
            public void StartResourceUpdate()
            {
                //开启线程，并把ID记录下来
                isUpdateResourceing = true;
                isMainProcessUpdated = true;
                //开启资源线程更新，主线程不更新
                Saber.SaberDebug.Log("[资源热更新]:开始！！");
                updatingProcessID = localServer.StartFightServerProcess(path,true);

            }
            private void SpareUpdateResourceFinish()
            {
                isUpdateResourceing = false;
                isMainProcessUpdated = false;
                //把所有在等待的线程关闭，再开启一个主要线程，进行更新
                //当前使用的线程地址更换为备用线程，这样后面进来的玩家就已经体验到更新内容了
                Saber.SaberDebug.Log("[资源热更新]:备用线程更新完毕，切换至主要线程更新。");
                updatingProcessID = localServer.StartFightServerProcess(JsonUtility.ServerInstancePath, true);
                int count=localServer.CloseAllWaitForFightProcess();
                localServer.serverInstancePath= path;
                localServer.StartPreparedProcess(count);
            }
            private void RealUpdateResourceFinish()
            {
                isUpdateResourceing = false;
                isMainProcessUpdated = true;
                int count = localServer.CloseAllWaitForFightProcess();
                localServer.serverInstancePath = JsonUtility.ServerInstancePath;
                //清空所有等待的线程
                updatingProcessID = 0;
                Saber.SaberDebug.Log("[资源热更新]:结束,切换回正常模式。");
            }
            public void FinishUpdatedResource(int id)
            {
                if (id != this.updatingProcessID)
                {
                    SaberDebug.LogError("出现错误，再更新资源时，线程ID对不上！！！");
                    return;
                }
                if(isUpdateResourceing)
                {
                    //说明只是备用的更新完毕
                    SpareUpdateResourceFinish();
                }
                else
                {
                    RealUpdateResourceFinish();
                }
            }
        }


        UpdateResourceHelper resourceHelper;
        string ip;
        ushort port;
        private Socket socket;
        private List<LocalClient> clientList = new List<LocalClient>();
        private ControllerManager controllerManager;
        public int startPorcessNum = 1;
        string serverInstancePath;
        
        //Message message;
        //private string path;

        List<ushort> portList=new List<ushort>();

        Queue<LocalClient> waitForFight = new Queue<LocalClient>();

        //public string Path { get => path;  }

        //public static string ApplicationPath;

        //private List<Room> roomList = new List<Room>();
        //private List<Room> waitServerRooms = new List<Room>();
        //private List<Room> playingRoomList = new List<Room>();
        //private int roomIndex = 0;

        //private ControllerManager controllerManager;
        //private PortManager portManager;
        //internal List<Room> GetRoomList { get => roomList; }

        //internal List<Room> WaitServerRooms { get => waitServerRooms; }

        //internal List<Room> PlayingRoomList { get => playingRoomList; }
        //internal PortManager PortManager { get => portManager; }
        //internal List<MatchClient> MatchClients { get => matchClients; }
        //internal ClientLoginManager ClientLoginManager { get => clientLoginManager; }
        private string GetIP()
        {
            //内网(局域网)IP
            //IPAddress LocalIP = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily.ToString().Equals("InterNetwork")).FirstOrDefault();

            //外网(公网)IP
            Stream stream = null;
            StreamReader streamReader = null;
            try
            {
                stream = WebRequest.Create("https://www.ipip5.com/").GetResponse().GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.UTF8);
                var str = streamReader.ReadToEnd();
                int first = str.IndexOf("<span class=\"c-ip\">") + 19;
                int last = str.IndexOf("</span>", first);
                var ip = str.Substring(first, last - first);
                //IPAddress PublicIP = IPAddress.Parse(ip);       //这里就得到了

                return ip;
            }
            catch (Exception ex)
            {
                Saber.SaberDebug.Log($"Get IP Error，{ex.Message}。获取失败");
            }
            finally
            {
                streamReader?.Dispose();
                stream?.Dispose();
            }
            return "";
        }
        // async Task<string> GetIPAsync()
        //{
        //    var result= await GetIPAsync();
        //    return result;

        //private string GetFightServerInstancePath()
        //{
        //    string path = Process.GetCurrentProcess().MainModule.FileName;
        //    WriteLineUtility.WriteLine($"{path} ", ConsoleColor.Green);

        //    string AppName = Process.GetCurrentProcess().ProcessName;
        //    StringBuilder report = new StringBuilder(AppName.Length * 6);
        //    for (int i = 0; i < path.Length; i++)
        //    {
        //        if (path[i] != JsonUtility.PathSlicer)
        //        {
        //            report.Append((char)path[i]);
        //            //Console.WriteLine(report);

        //            if (report.ToString() == AppName)
        //            {
        //                path = path.Substring(0, i + 1);
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            report.Clear();
        //        }
        //    }
        //    ApplicationPath = path;

        //    switch (OSPlatformUtility.MyPlatformTarget)
        //    {
        //        case OSPlatformUtility.PlatformTarget.Windows:
        //            path += @"\Windows\XianXia\XianXiaServer.exe";
        //            break;
        //        case OSPlatformUtility.PlatformTarget.Linux:
        //            path += @"/Linux/XianXia/XianXiaServer";
        //            break;
        //        default:
        //            break;
        //    }

        //    //path += @$"{JsonUtility.PathSlicer}FightServer{JsonUtility.PathSlicer}";


        //    bool res=File.Exists(path);
        //    //path += @"";
        //    if(res)
        //        WriteLineUtility.WriteLine($"{path} is true",ConsoleColor.Green);
        //    else
        //        WriteLineUtility.WriteLine($"please make sure :{path} is true", ConsoleColor.Red);

        //    return path;
        //}

        //}
        Dictionary<int, Process> allProcessNameDict = new Dictionary<int, Process>();

        public Dictionary<int, Process> AllProcessNameDict { get => allProcessNameDict;  }
        internal Queue<LocalClient> WaitForFightProcess { get => waitForFight; }
        public UpdateResourceHelper ResourceHelper { get => resourceHelper;  }

        internal void SetWaitForFightProcess(Queue<LocalClient> localClients)
        {
            waitForFight = localClients;
        }


        public int GetNeedProcessNum()
        {
            return waitForFight.Count - startPorcessNum;
        }
      private void StartPreparedProcess(int num)
        {
            startPorcessNum = num;
            Saber.SaberDebug.Log($"正在启动预备协程，数量为{startPorcessNum}");
            for(int i = 0; i < startPorcessNum; i++)
            {
                _=StartFightServerProcess();
            }
        }
        public int StartFightServerProcess(string path="",bool isUpdateResource=false)
        {
            try
            {
                //string path = Process.GetCurrentProcess().MainModule.FileName;
                //Process.Start(JsonUtility.ServerInstancePath);
                if(string.IsNullOrEmpty(path))
                    path = serverInstancePath;
                ProcessStartInfo startInfo = new ProcessStartInfo(path);
                //startInfo.WindowStyle= ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                //startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                Process process= Process.Start(startInfo);
                LogInfo logManager = LogInfo.Build();

                int id = process.Id;
                string name = process.ProcessName;
                //WriteLineUtility.WriteLine(id + "assa");
                lock (allProcessNameDict)
                {
                    allProcessNameDict.Add(id, process);
                }
                if (!isUpdateResource)
                {
 
                    Saber.SaberDebug.Log($"成功开启,名称:{name} id:{id},路径:{path}", ConsoleColor.Blue);
                    process.Exited += (s, d) => { Saber.SaberDebug.Log($"{id}实例退出了"); allProcessNameDict.Remove(id); logManager.OutPutJson(); LogInfo.Recycle(logManager); };
                }
                else
                {
                    process.Exited += (s, d) => { Saber.SaberDebug.Log($"{id}资源更新完毕");  logManager.OutPutJson(); LogInfo.Recycle(logManager); };
                    Saber.SaberDebug.Log($"成功开启热更新资源,名称:{name} id:{id},路径:{path}", ConsoleColor.Blue);

                }


                process.OutputDataReceived += (s, d) => { logManager.Reocrd(d.Data); };
                process.ErrorDataReceived += (s, d) => { logManager.Reocrd($"{d.Data}:Error"); };
                process.EnableRaisingEvents = true;



                process.BeginOutputReadLine();

                process.BeginErrorReadLine();
                //process.WaitForExit();
                return process.Id;
                //startInfo.CreateNoWindow = false;
                //Process process = new Process();
                //process.StartInfo = startInfo;
                //process.Start();
                //return true;
            }
            catch (Exception ex)
            {
                //WriteLineUtility.WriteLine(ex.Message, ConsoleColor.Red);
                Saber.SaberDebug.LogError($"StartFightGameProcess fail,please check path:{JsonUtility.ServerInstancePath}!!!\n{ex.Message}",ConsoleColor.Red);
                return 0;
                //return false;
            }
        }

        public int CloseAllWaitForFightProcess()
        {
            lock (waitForFight)
            {
                int res = waitForFight.Count;
                while (waitForFight.Count > 0)
                {
                    LocalClient localClient = waitForFight.Dequeue();
                    localClient.CloseConnect();
                }
                return res;
            }
        }
        public LocalServer(ushort port,ControllerManager controllerManager,int num,string processPath,string updateResourcePath)
        {
            try
            {
                this.port = port;
                //message = new Message();
                this.controllerManager = controllerManager;
                this.serverInstancePath=processPath;
                this.resourceHelper = new UpdateResourceHelper(this, updateResourcePath);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //限制在本地，不能和其他交互
                IPAddress ips = IPAddress.Parse("127.0.0.1");
                //IPEndPoint ipNode = new IPEndPoint(ips, 80);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(1024);
                StartAccept();


                StartPreparedProcess(num);
                //controllerManager = new ControllerManager(this);
                //portManager = new PortManager();
                //clientLoginManager = new ClientLoginManager();

                //new Task(() => { ip = GetIPAsync().Result;
                //    Console.WriteLine($"ip地址为:{ip},port为{port}");
                //}).Start();
                JsonUtility.ChechFightServerInstanceAndUpdateResourcePath();
                //path = GetFightServerInstancePath();
                Saber.SaberDebug.Log($"开启本地战斗分发服务器:端口号为{port}");


            }
            catch (Exception ex)
            {
                Saber.SaberDebug.LogError("开启服务器失败"+ex.Message);
            }


        }
        /// <summary>
        /// 与客户端建立连接
        /// </summary>
        private void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
            //IAsyncResult ar=null;
            //AcceptCallback(ar, Blick);
        }
        private void AcceptCallback(IAsyncResult ar)
        {


            Socket client = socket.EndAccept(ar);

            Saber.SaberDebug.Log($"本地战斗服务器客户端连接成功");
            clientList.Add(LocalClient.Build(client, this));

            try
            {
            }
            catch (Exception ex)
            {
                Saber.SaberDebug.Log($"本地战斗服务器连接失败{ex.Message.ToString()}");
            }
            //Console.Read();
            StartAccept();
        }

        public void HandleRequest(MainPack pack, LocalClient client)
        {
            controllerManager.Local_HandleRequest(pack, this,client);
        }
        public void RemoveClient(LocalClient client)
        {

            if (clientList.Contains(client))
            {
                GiveBackPort(client.Port);
                clientList.Remove(client);
                if (allProcessNameDict.ContainsKey(client.ProcessId))
                {
                    if (allProcessNameDict[client.ProcessId].HasExited == false)
                    {
                        Saber.SaberDebug.Log($"正在关闭{client.ProcessId}服务器实例");
                        allProcessNameDict[client.ProcessId].Kill();
                    }
                    allProcessNameDict.Remove(client.ProcessId);

                }else
                    Saber.SaberDebug.LogWarning($"并不存在{client.ProcessId}服务器实例", ConsoleColor.Red);

            }

        }
        internal void OnDestroy()
        {
            foreach(var client in allProcessNameDict.Values)
            {
                if (client!=null&&client.HasExited==false)
                {
                     Saber.SaberDebug.Log($"正在关闭{client.Id}服务器实例");
                    allProcessNameDict[client.Id].Kill();
                }
                else
                    Saber.SaberDebug.LogWarning($"并不存在{client.Id}服务器实例", ConsoleColor.Red);
            }
            allProcessNameDict.Clear();
        }

        internal void AddPort(ushort form,ushort to)
        {
            for(ushort i = form; i<= to; i++)
            {
                portList.Add(i);
            }
        }
        internal void AddPort(ushort[] port)
        {
            portList.AddRange(port);
        }
        internal ushort AllotPort()
        {
            if (portList.Count <= 0)
            {
                Saber.SaberDebug.LogWarning($"Has not port can allot to fight!!!", ConsoleColor.Red);
                return 0;
            }
            ushort port = portList[0];
            portList.RemoveAt(0);
            return port;
        }
        internal void GiveBackPort(ushort port)
        {
            if (port == 0) return;
            if (portList.Contains(port))
            {
                Saber.SaberDebug.LogWarning($"Cant give back one port{port} twice!!!", ConsoleColor.Red);
                return;
            }
            Saber.SaberDebug.Log($"Give Back {port} port!", ConsoleColor.Green);

            portList.Add(port);
        }
        public void BroadcastAll(MainPack mainPack)
        {
            foreach (var v in clientList) 
            {
                v.Send(mainPack);
            }
        }
    }
}
