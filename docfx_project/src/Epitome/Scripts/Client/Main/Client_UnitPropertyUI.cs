using FishNet;
using FishNet.Object;
using Saber.Base;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia;

namespace XianXia
{


    public class Client_UnitPropertyUI : Client_SingletonBase<Client_UnitPropertyUI>
    {
        Saber.Base.ObjectPool<HealthMagicPointShowUI> uiObjectPool;
        const string HealthMagicPointUIName = "HealthMagicPointUI";
        //const string ABpackageName = "fight";
        const string HealthMagicPointShowUIPanel = "HealthMagicPointShowUI";
        Transform healthMgiacPanel;
        GameObject model;
        protected override void Start()
        {
            
            base.Start();
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            Canvas canvas = null;
            foreach (var v in canvases)
            {
                if (v.name == "MainCanvas")
                { canvas = v; break; }
            }
            //healthMgiacPanel= canvas.transform.Find(HealthMagicPointShowUIPanel);
            foreach (var v in canvas.transform.GetComponentsInChildren<RectTransform>())
            {
                if (v.name == HealthMagicPointShowUIPanel)
                {
                    healthMgiacPanel = v;
                    break;
                }
            }
            //Debug.Log(healthMgiacPanel.name+"111");
            model = ABUtility.Load<GameObject>(ABUtility.UIMainName + HealthMagicPointUIName);
            if (model == null) model = new GameObject();
            uiObjectPool = PoolManager.Instance.AddPool<HealthMagicPointShowUI>(() =>
            {
                GameObject GO = GameObject.Instantiate(model);
                GO.transform.SetParent(healthMgiacPanel);
                GO.transform.localScale = Vector3.one;
                return GO.AddComponent<HealthMagicPointShowUI>();
            }, (u) =>
            {
                u.gameObject.SetActive(false);

            }, (u) =>
            {
                u.gameObject.SetActive(true);

            });
        }
        public void AddUnitProperty(NetworkObject networkObject)
        {

            if (networkObject == null) return;
            IClient_UnitProperty unitProperty = networkObject.GetComponent<IClient_UnitProperty>();
            //if(unitProperty==null)unitProperty = networkObject.AddAndSerialize<Client_UnitProperty>();
            if (unitProperty == null || unitProperty.HealthMagicPointShowUI != null) return;

            unitProperty.SetShowUI(uiObjectPool.GetObjectInPool());
        }
        public void RemoveUnitProperty(NetworkObject networkObject)
        {

            if (networkObject == null) return;
            RemoveUnitProperty(networkObject.gameObject);
        }
        public void RemoveUnitProperty(GameObject go)
        {

            if (go == null) return;
            IClient_UnitProperty unitProperty = go.GetComponent<IClient_UnitProperty>();
            if (unitProperty == null) return;
            if (unitProperty.HealthMagicPointShowUI != null)
                uiObjectPool.RecycleToPool(unitProperty.HealthMagicPointShowUI);
            unitProperty.SetShowUI(null);
        }
        public void SetUnitColor(NetworkObject networkObject ,Color color)
        {
            if (networkObject == null) return;
            IClient_UnitProperty unitProperty = networkObject.GetComponent<IClient_UnitProperty>();
            if (unitProperty == null) return;
            if (unitProperty.HealthMagicPointShowUI == null) AddUnitProperty(networkObject);
            unitProperty.HealthMagicPointShowUI.SetHandleColor(color);
        }


    }


}
