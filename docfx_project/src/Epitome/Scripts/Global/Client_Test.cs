using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    public class Client_Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
#if !UNITY_SERVER
            Debug.Log("���¼�");
            XianXia.Client.Client_ConnectToFightServer.AddGameSuccAndFailEvent((r) =>
            {
                if (r == Saber.ECS.GameResult.Success)
                    Debug.Log("��Ϸ�ɹ�");
                else
                    Debug.Log("��Ϸʧ��");
            }, true);
#endif
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
