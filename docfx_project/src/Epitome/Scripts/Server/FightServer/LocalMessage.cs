using Google.Protobuf;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

internal class LocalMessage
{

    
        private byte[] buffer = new byte[1024];
        private int startIndex = 0;

        public byte[] Buffer
        {
            get { return buffer; }
        }
        public int StartIndex
        {
            get { return startIndex; }
        }
        /// <summary>
        /// 消息剩余长度
        /// </summary>
        public int Remsize
        {
            get { return buffer.Length - startIndex; }
        }
        public static byte[] ArrayConcat(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            int l = 0;
            int r = a.Length;
            while (l < a.Length && r < c.Length)
            {
                while (l < a.Length)
                {
                    c[l] = a[l];
                    l++;
                }
                while (r < c.Length)
                {
                    c[r] = b[r - a.Length];
                    r++;
                }
            }
            return c;
        }
        public void ReadBuffer(int len, byte code, Action<MainPack> HandleRequest)
        {
            //byte[] reveset= new byte[len];
            //Array.Copy(buffer, startIndex, reveset, 0, len);
            //reveset.Reverse();
            //Array.Copy(reveset,0,buffer, startIndex, len);

            startIndex += len;
            while (true)
            {
                //只有包头或者包头也没有
                if (startIndex <= 5) return;
                //消息解析
                //解析包体
                //byte[] bytes = new byte[4];
                //Array.Copy(buffer,0, bytes, 0, 4);
                //bytes.Reverse();
                //for (int i = 0; i < 4; i++)
                //{
                //    //if (buffer[i] == 0) return;
                //    Debug.Log(bytes[i]);
                //}
                //Array.Reverse(buffer, 1, 4);
                if (code != buffer[0]) { Debug.LogError("似乎接收到了不该接收的消息！"); return; }
                int count = BitConverter.ToInt32(buffer, 1);
                //for(int i = 0; i < startIndex; i++)
                //{
                //    //if (buffer[i] == 0) return;
                //    Debug.Log(buffer[i]);
                //}
                //Debug.Log(count);
                //typeof(MainPack)
                //if()
                if (startIndex >= (count + 5))
                {
                    MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 5, count);
                    //处理消息
                    HandleRequest(pack);
                    Array.Copy(buffer, count + 5, buffer, 0, startIndex - count);
                    startIndex -= (count + 5);
                }
                else
                    break;
            }

        }

        public static byte[] PackData(MainPack pack)
        {
            byte[] data = pack.ToByteArray();//包体
            byte[] head = BitConverter.GetBytes(data.Length);//包头
                                                             //定义游戏客户端与总服务器之间的通讯使用0代表
            return new byte[1] { 1 }.Concat(head).Concat(data).ToArray();
        }

    
}
