using Proto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Log;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Local
{
    public class LocalClient
    {
        Socket socket;
        LocalServer localServer;
        //消息存储优化
        LocalMessage message;
        ushort port = 0;
        int processId = 0;
        string playerID;
        string[] fightInfo;//战斗开始时传给的关卡信息之类的

        const string poolName = "LocalClient";
        

        public ushort Port { get => port; set => port = value; }

        public bool IsActive => socket != null && socket.Connected;

        public int ProcessId { get => processId; set => processId = value; }
        public string PlayerID { get => playerID; set => playerID = value; }
        public string[] FightInfo { get => fightInfo; set => fightInfo = value; }

        private LocalClient()
        {
            this.message = new LocalMessage();
        }

        private LocalClient(Socket socket, LocalServer localServer)
        {
            this.socket = socket;
            this.localServer = localServer;
            this.message = new LocalMessage();
            StartReceive();
            //Send(LocalClientRequest.LocalClientIsLogin());
        }

        public static LocalClient Build(Socket socket, LocalServer localServer)
        {
            //PoolManager poolManager = InstanceFinder.GetInstance<PoolManager>();
            //if (!poolManager.IsPoolAlive(poolName))
            //    _ = poolManager.AddPool(() => { return new LocalClient(); }, (t) => { t.Clear(); }, null, poolName);
            //LocalClient localClient = poolManager.GetPool<LocalClient>(poolName).GetObjectInPool();
            //lock (localClient)
            //{
            //    localClient.socket = socket;
            //    localClient.localServer = localServer;
            //    localClient.StartReceive();
            //    return localClient;

            //}
            return new LocalClient(socket, localServer);
            //LocalClient localClient=new LocalClient()
        }
        public static void Recycle(LocalClient localClient)
        {
            localClient.message.Clear();
            //localClient.CloseConnect();
            //PoolManager poolManager = InstanceFinder.GetInstance<PoolManager>();

            //if (!poolManager.IsPoolAlive(poolName))
            //{
            //    PoolManager.LogRecycleError(poolName);
            //    return;
            //}
            //lock (poolManager)
            //{
            //    poolManager.RecycleToPool(localClient, poolName);

            //}
        }

        private void StartReceive()
        {
            //if(socket!=null)
            socket?.BeginReceive(message.Buffer, message.StartIndex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (socket == null || socket.Connected == false) return;

                int len = socket.EndReceive(ar);

                if (len == 0)
                {
                    Saber.SaberDebug.Log($"进程ID{processId}端口:{port}通讯关闭，可能是对方关闭了通道");
                    Close();
                    return;
                }

                message.ReadBuffer(len, 1,HandleRequset);

                StartReceive();
            }
            catch (Exception e)
            {
                Saber.SaberDebug.LogError($"进程ID{processId}端口:{port}通信出现错误:{e.Message}");
                //WriteLineUtility.WriteLine(e.Message);
                Close();
            }

        }

        public void Send(MainPack pack)
        {
            if (socket != null)
            {
                Saber.SaberDebug.Log($"[LocalSend][ID:{processId}][端口:{port}] |ActionCode: {pack.ActionCode} ");
                socket.Send(Message.PackData(pack));
            }
            else
            {
                Saber.SaberDebug.LogError($"[LocalSend][ID:{processId}][端口:{port}] Socket为空，请检查异常！",ConsoleColor.DarkRed);

            }
        }
        private void HandleRequset(MainPack pack)
        {
            Saber.SaberDebug.Log($"[ReceiveLocal][ID:{processId}][端口:{port}]  |ActionCode:{pack.ActionCode}|ReturnCode:{pack.ReturnCode}");
            localServer.HandleRequest(pack, this);
        }
        ///// <summary>
        ///// 注册
        ///// </summary>
        //public bool Logon(MainPack pack)
        //{
        //    return GetUserData.Logon(pack, sqlConnection);
        //}

        private void Clear()
        {
            message.Clear();
            if (IsActive)Close();
            socket = null;
            localServer = null;
            port = 0;
            processId = 0;
            playerID = string.Empty;
        }
        public void CloseConnect()
        {
            Close();
        }
        private void Close()
        {
            if (socket != null && socket.Connected)
            {
                Saber.SaberDebug.Log($"进程ID：{processId}端口：{port} +断开连接");
                socket?.Close();
                localServer?.RemoveClient(this);
                Clear();
            }

        }
    }
}
