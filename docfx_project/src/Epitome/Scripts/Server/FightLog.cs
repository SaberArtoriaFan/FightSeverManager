using FishNet;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XianXia;


    public class FightLog :MonoBehaviour
    {
        private void Awake()
        {
            if (InstanceFinder.GetInstance<FightLog>() != null)
            {
                Debug.LogError("Cannt Create Two Instance");
                GameObject.Destroy(this);
                return;
            }
            InstanceFinder.RegisterInstance<FightLog>(this);
            logManager = this;
        }
        static FightLog logManager;

        StringBuilder s=new StringBuilder();

        public static void Record(string s)
        {
            logManager.s.Append($"{s}\n");
            Debug.Log($"Record:{s}");
        }
        public static void LogError(string s)
        {
            logManager.s.Append($"[Error]:{s}\n");
             Debug.LogError($"Error:{s}");
        }
    public static string OutPut()
        {
            return logManager.s.ToString();
        }
    }

