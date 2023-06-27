using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


namespace XianXia
{
    public class FightServerClient 
    {
        public event Action OnCloseSocketEvent;

        // Start is called before the first frame update
        Socket socket;
        
        LocalMessage message;
        public ControllerManager controllerManager;

        
        public bool IsActive => socket != null && socket.Connected;
        public  void Init()
        {
            message = new LocalMessage();
        }
        public void InitSocket(string ip, ushort port)
        {
            Init();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ip, port);
                StartReceive();
                //this.controllerManager = controllerManager;
                FightServerManager.ConsoleWrite_Saber("Connect to FightAllServer");

                //XianXiaControllerInit.Excess_LoginRequest();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
        public  void Destory()
        {
            CloseSocket();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void CloseSocket()
        {
            if (socket != null && socket.Connected == true)
            {
                FightServerManager.ConsoleWrite_Saber("Close this Server");

                try 
                {
                    XianXiaControllerInit.Request_CloseApplication(ReturnCode.Zero,new string[] { "通讯异常" });
                }
                catch(Exception ex)
                {
                    FightServerManager.ConsoleWrite_Saber($"申请关闭自己发生错误{ex.Message}");
                }

                socket.Close();
                OnCloseSocketEvent?.Invoke();
                OnCloseSocketEvent = null;
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
                    CloseSocket();
                    return;
                }
                message.ReadBuffer(len,1,HandleResponse);
                StartReceive();
            }
            catch
            {
                CloseSocket();
            }
        }

        private void HandleResponse(MainPack pack)
        {
            FightServerManager.ConsoleWrite_Saber("Receive Message,Action:" + pack.ActionCode.ToString()+pack.ReturnCode);
            controllerManager.AddPackToDeal(pack);
            //TcpManager.Instance.Response(pack);
        }

        public void Send(MainPack pack)
        {
            if (pack != null)
            {
                FightServerManager.ConsoleWrite_Saber($"Send {pack.ActionCode}Action,ReturnResult=");

                socket.Send(LocalMessage.PackData(pack));
            }


        }
    }
}
