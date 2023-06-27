using FishNet;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Transporting.Tugboat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XianXia
{
    public class NetworkIPAndPortUI : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField ip;
        [SerializeField]
        TMP_InputField port;
        [SerializeField]
        TMP_Text ipText;
        [SerializeField]
        TMP_Text portText;

        Tugboat tugboat;

        //public string ipAd;
        //public ushort portint;

        private void Start()
        {
            //Console.WriteLine("Enter Offline Scene");
            tugboat = InstanceFinder.NetworkManager.TransportManager.Transport as Tugboat;


#if UNITY_SERVER
            Console.WriteLine("Enter Offline Scene");
            //ServerManager networkManager = GetComponentInParent<ServerManager>();
            //tugboat.SetServerBindAddress(ipAd, FishNet.Transporting.IPAddressType.IPv4);
            //tugboat.SetPort(portint);
            Console.WriteLine("ip:" + tugboat.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4) + ";port:" + tugboat.GetPort()) ;

            //networkManager.StartConnection();
#else
            ip.onSubmit.AddListener(IpUpdate);
            port.onSubmit.AddListener(PortUpdate);
#endif
        }

        private void IpUpdate(string s)
        {
            tugboat.SetClientAddress(s);
            tugboat.SetServerBindAddress(s, FishNet.Transporting.IPAddressType.IPv4);
            //ipText.text = s;
        }

        private void PortUpdate(string s)
        {
            tugboat.SetPort(Convert.ToUInt16(s));
            //port.text = s;
        }
        public void Update()
        {

#if UNITY_SERVER && !UNITY_EDITOR

#elif UNITY_EDITOR
            ipText.text =InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4);
            portText.text = InstanceFinder.NetworkManager.TransportManager.Transport.GetPort().ToString();
#elif Server
            ipText.text = tugboat.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4);
            portText.text = tugboat.GetPort().ToString() ;
#else
            ipText.text = tugboat.GetClientAddress();
            portText.text = tugboat.GetPort().ToString();
#endif
        }
    }
}
