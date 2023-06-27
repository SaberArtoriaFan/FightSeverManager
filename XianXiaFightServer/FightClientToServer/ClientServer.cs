using Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Controller;
using XianXiaFightGameServer.Email;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;


namespace XianXiaFightGameServer.FightClientToServer
{
   public class ClientServer
    {
        class LoCK
        {
            public bool isloacking = false;
        }
        /// <summary>
        /// 记得写一个默认的地址
        /// </summary>

        string ip="192.168.2.20";
        ushort port=8888;

        ushort origin_ReConnectTime = 5;

        ushort reConnectTime = 5;

        const float reConnectInterval = 3f;

        const float heartInterval = 10f;

        LoCK loCK = new LoCK();
        bool IsReConnecting { get => loCK.isloacking; set => loCK.isloacking = value; }
        Socket socket;
        Message message;
        ControllerManager controllerManager;
        TimerSystem.Timer heartWaitRespondTimer;
        TimerSystem.Timer heartTimer;
        TimerSystem.Timer reConnectTimer;

        string self_IP;
        public bool IsActive => socket != null && socket.Connected;

        public string Self_IP { get => self_IP;  }
        public TimerSystem.Timer HeartTimer { get => heartWaitRespondTimer; set => heartWaitRespondTimer = value; }

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
                Console.WriteLine($"出错了，{ex.Message}。获取失败");
            }
            finally
            {
                streamReader?.Dispose();
                stream?.Dispose();
            }
            return "";
        }
        private async Task<string> GetIPAsync()
        {
            //内网(局域网)IP
            //IPAddress LocalIP = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily.ToString().Equals("InterNetwork")).FirstOrDefault();

            //外网(公网)IP
            Stream stream = null;
            StreamReader streamReader = null;
            try
            {
                WebResponse webRequest = await WebRequest.Create("https://www.ipip5.com/").GetResponseAsync();
                stream = webRequest.GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.UTF8);
                var str = streamReader.ReadToEnd();
                int first = str.IndexOf("<span class=\"c-ip\">") + 19;
                int last = str.IndexOf("</span>", first);
                var ip = str.Substring(first, last - first);
                //IPAddress PublicIP = IPAddress.Parse(ip);       //这里就得到了

                //找到IP后再登录
                //ClientServerRequest.Login();
                return ip;
            }
            catch (Exception ex)
            {
                Saber.SaberDebug.LogError($"获取公网IP错误:{ex.Message}。");
            }
            finally
            {
                streamReader?.Dispose();
                stream?.Dispose();
            }
            return "";
        }


        //private async void AsyncLogin()
        //{

        //}

        internal void SetSelfIP(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                new Task(() => {
                    this.self_IP = GetIPAsync().Result;
                    Saber.SaberDebug.Log($"本机公网IPv4地址为:{self_IP}");
                    JsonUtility.FightServerParameter.SelfIP = self_IP;
                    JsonUtility.WriteJsonFile(JsonUtility.ConfigurationPath, JsonUtility.ToJson(JsonUtility.FightServerParameter));
                    //this.self_IP = "127.0.0.1";
                }).Start();
            }else
            { this.self_IP = ip;
                Saber.SaberDebug.Log($"本机公网IPv4地址为:{self_IP}");
            }

        }
        List<MainPack> waitForStartGame = new List<MainPack>();
        HashSet<string> waitFotStartGamePlayerHash = new HashSet<string>();

        public ClientServer(ControllerManager controllerManager)
        {
            this.controllerManager = controllerManager;
           Saber.SaberDebug.Log($"使用 [-InitServer ipv4地址,端口号] 指令来指定连接的后端服务器");
        }
        ~ClientServer()
        {
            Destory();
        }
        public int GetFightDataNum()
        {
            return waitForStartGame.Count;
        }
        public int EnqueueFightData(MainPack mainPack) 
        {
            Saber.SaberDebug.Log(mainPack.Word + " player enter wait Queue");
            if (!waitFotStartGamePlayerHash.Contains(mainPack.Word))
            {
                mainPack.IpAndPortPack = new IPAndPortPack();
                mainPack.IpAndPortPack.Ip = self_IP;
                mainPack.IpAndPortPack.Port = InstanceFinder.GetInstance<LocalServer>().AllotPort();
                waitForStartGame.Add(mainPack);
                waitFotStartGamePlayerHash.Add(mainPack.Word);
                Saber.SaberDebug.Log($"正在为了 {mainPack.Word} 玩家开启服务器实例！" , ConsoleColor.DarkRed);
                InstanceFinder.GetInstance<LocalServer>().StartFightServerProcess();
                return mainPack.IpAndPortPack.Port;
            }
            else
                Saber.SaberDebug.LogWarning($"Player{mainPack.Word}was Waitting for a fight",ConsoleColor.Yellow);
            return 0;

        }
        /// <summary>
        /// pack里包含了：
        /// word:玩家ID
        /// HeroAndPosList 
        /// IP
        /// Port
        /// ActionCode=ReadyFightAction
        /// ReturnCode=null
        /// </summary>
        /// <returns></returns>
        public MainPack DequeueFightData()
        {
            if ( waitForStartGame.Count <= 0) { Saber.SaberDebug.LogError("Is Error,No Pack is Waitting For StartGame,if u open an extra fightServer?"); return null; }
            else
            {
                MainPack mainPack = waitForStartGame[0];
                waitForStartGame.RemoveAt(0);

                if (waitFotStartGamePlayerHash.Contains(mainPack.Word)) waitFotStartGamePlayerHash.Remove(mainPack.Word);
                else Saber.SaberDebug.LogWarning($"Player{mainPack.Word} maybe neednt has a fight");
                return mainPack;
            }
        }
        public void InitSocket(string ip=null, ushort port=0)
        {

            this.ip = ip!=null?ip:this.ip;
            this.port = port!=0?port:this.port;
            Saber.SaberDebug.Log($"开始连接后端服务器中，ip为{this.ip}，端口号为{this.port}");

            message = new Message();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(this.ip, this.port);
               
                StartReceive();
  
                //直接登录
                ClientServerRequest.Login();
                //发送心跳包
                Saber.SaberDebug.Log($"连接到后端服务器成功,后端服务器ip地址为{this.ip}，端口号为{this.port}");
                //表明身份
                StartSendHeartTimer();
                //ClientServerRequest.Login();
     
            }
            catch (Exception ex)
            {
                Saber.SaberDebug.LogError($"无法与后端服务器连接，请检查端口号与IP{ex.Message}");
                ReConnect();
            }
        }
        private void StartSendHeartTimer()
        {
            if (heartTimer != null)
                heartTimer.Stop(true);
            heartTimer = InstanceFinder.GetInstance<TimerSystem>().AddTimer(() => ClientServerRequest.SendHeartbeat(heartInterval / 2), heartInterval,true,-1,null,true);
        }
        public void Destory()
        {
            CloseSocket();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void CloseSocket(bool isReal=false)
        {
            if (socket != null &&(isReal || socket.Connected == true))
            {
                Saber.SaberDebug.Log($"{ip}:{port},MainServer Lost Connect");
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch
                {

                }

                socket = null;
                if (heartTimer != null)
                {
                    heartTimer.Stop(true);
                    heartTimer = null;
                }
                if(heartWaitRespondTimer!=null)
                {
                    heartWaitRespondTimer.Stop(true);
                    heartWaitRespondTimer = null;
                }
            }
        }
        public void ReConnect()
        {
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Close();
            if (IsReConnecting||reConnectTimer!=null&&!reConnectTimer.IsFinish) return;
            IsReConnecting = true;
            lock (loCK)
            {
                if (IsReConnecting == false) return;
                ReConnectTimer(origin_ReConnectTime);
            }
            MailUtility.SendToDefault("[战斗服务器-重连通知!!]", $"自己IP:{self_IP},目标总服务器IP{ip}端口{port}");
        }
        private void ReConnectTimer(int reConnectTime)
        {
            Saber.SaberDebug.LogWarning(IsReConnecting.ToString());

            //if (IsReConnecting == false) return;
            CloseSocket(reConnectTime == origin_ReConnectTime);
            if (reConnectTime == origin_ReConnectTime)
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            reConnectTime--;
            Saber.SaberDebug.Log($"第{origin_ReConnectTime - reConnectTime}次尝试连接后端服务器中......");
            try
            {
                socket.Connect(this.ip, this.port);

                StartReceive();

                //直接登录
                ClientServerRequest.Login();
                StartSendHeartTimer();
                Saber.SaberDebug.Log($"重新连接到后端服务器成功,后端服务器ip地址为{this.ip}，端口号为{this.port}");
                //表明身份
                //ClientServerRequest.Login();
                reConnectTimer = null;
                IsReConnecting = false;
            }
            catch (Exception ex)
            {
                if (reConnectTime == 0)
                {
                    reConnectTime = origin_ReConnectTime;
                    reConnectTimer = null;
                    IsReConnecting = false;
                    Saber.SaberDebug.LogError($"无法与后端服务器连接，请检查后端服务器，端口号与IP{ex.Message}");
                    _ =  InstanceFinder.GetInstance<TimerSystem>().AddTimer(() => ReConnect(), reConnectInterval*5, false, -1, null, true);

                    return;
                }
                else
                {
                    Saber.SaberDebug.LogWarning($"第{origin_ReConnectTime - reConnectTime}次尝试连接后端服务器失败，无法连接原因:{ex.Message}");
                    reConnectTimer = InstanceFinder.GetInstance<TimerSystem>().AddTimer(() => ReConnectTimer(reConnectTime), reConnectInterval,false,-1,null,true);
                    //InstanceFinder.GetInstance<TimerSystem>().AddTimer(ReConnect, reConnectInterval);
                    //TimerUtility.AddTimer_Repeated((o, e) => ReConnect(), reConnectInterval);
                }
            }
        }
        private void StartReceive()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
        }
        private void ReceiveCallback(IAsyncResult ia)
        {
            try
            {
                if (socket == null || socket.Connected == false) return;
                int len = socket.EndReceive(ia);
                if (len == 0)
                {
                    Saber.SaberDebug.LogError($"与后端服务器断开连接，原因:通讯似乎被切断了");
                    ReConnect();
                    return;
                }
                message.ReadBuffer(len,1, HandleResponse);
                StartReceive();
            }
            catch(Exception ex)
            {
                Saber.SaberDebug.LogError($"与后端服务器断开连接，原因:{ex.Message}");
                ReConnect();
            }
        }

        private void HandleResponse(MainPack pack)
        {
            if(pack.ActionCode!=ActionCode.HeartBeat)
                Saber.SaberDebug.Log($"[FromServer]: Receive |ActionCode:{pack.ActionCode}|ReturnCode:{pack.ReturnCode}");
            controllerManager.HandleRequest(pack,this);
            //controllerManager.
            //TcpManager.Instance.Response(pack);
        }
        public void SendHeartBeat(MainPack mainPack,TimerSystem.Timer timer)
        {
            SendNoOutPut(mainPack);
            if(heartWaitRespondTimer!= null)
                heartWaitRespondTimer.Stop(true);
            this.heartWaitRespondTimer = timer;
        }

        public void Send(MainPack pack)
        {
            if (IsActive)
            {
                socket.Send(Message.PackData(pack));
                Saber.SaberDebug.Log($"[ToServer]: Send |ActionCode:{pack.ActionCode}");
            }
            else
            {
                Saber.SaberDebug.LogWarning($"Cant Send To A NotActiveServer");
                ReConnect() ;
            }
        }
        public void SendNoOutPut(MainPack pack)
        {
            if (IsActive)
            {
                socket.Send(Message.PackData(pack));
            }
            else
            {
                Saber.SaberDebug.LogWarning($"Cant Send To A NotActiveServer");
                ReConnect();
            }
        }
    }
}
