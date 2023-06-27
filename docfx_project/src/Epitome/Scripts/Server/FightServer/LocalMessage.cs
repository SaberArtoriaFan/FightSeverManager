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
        /// ��Ϣʣ�೤��
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
                //ֻ�а�ͷ���߰�ͷҲû��
                if (startIndex <= 5) return;
                //��Ϣ����
                //��������
                //byte[] bytes = new byte[4];
                //Array.Copy(buffer,0, bytes, 0, 4);
                //bytes.Reverse();
                //for (int i = 0; i < 4; i++)
                //{
                //    //if (buffer[i] == 0) return;
                //    Debug.Log(bytes[i]);
                //}
                //Array.Reverse(buffer, 1, 4);
                if (code != buffer[0]) { Debug.LogError("�ƺ����յ��˲��ý��յ���Ϣ��"); return; }
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
                    //������Ϣ
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
            byte[] data = pack.ToByteArray();//����
            byte[] head = BitConverter.GetBytes(data.Length);//��ͷ
                                                             //������Ϸ�ͻ������ܷ�����֮���ͨѶʹ��0����
            return new byte[1] { 1 }.Concat(head).Concat(data).ToArray();
        }

    
}
