using FishNet;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XianXia.Unit
{
    public class UnitUIShowSystem : NormalSystemBase<UIShowOrgan>
    {
        //ObjectPoolSystem poolSystem;
        ////ABManagerSystem aBManagerSystem;
        //ObjectPool<HealthMagicPointShowUI> uiObjectPool;
        //const string HealthMagicPointUIName= "HealthMagicPointUI";
        ////const string ABpackageName = "fight";
        //const string HealthMagicPointShowUIPanel = "HealthMagicPointShowUI";
        //Transform healthMgiacPanel;
        //GameObject model;
        public override void Start()
        {
            //Debug.Log("999" + IsSpwanSystem);
            base.Start();
            //aBManagerSystem=world.FindSystem<ABManagerSystem>();
            //poolSystem=world.FindSystem<ObjectPoolSystem>();
            //Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            //Canvas canvas = null;
            //foreach (var v in canvases)
            //{
            //    if (v.name == "MainCanvas")
            //    { canvas = v; break; }
            //}
            ////healthMgiacPanel= canvas.transform.Find(HealthMagicPointShowUIPanel);
            //foreach (var v in canvas.transform.GetComponentsInChildren<RectTransform>())
            //{
            //    if(v.name==HealthMagicPointShowUIPanel)
            //    {
            //        healthMgiacPanel = v;
            //        break;
            //    }
            //}
            ////Debug.Log(healthMgiacPanel.name+"111");
            //model = ABUtility.Load<GameObject>(ABUtility.UIMainName+HealthMagicPointUIName);
            //if (model == null) model = new GameObject();
            //uiObjectPool = poolSystem.AddPool<HealthMagicPointShowUI>(() =>
            //{
            //    GameObject GO = GameObject.Instantiate(model);
            //    GO.transform.SetParent(healthMgiacPanel);
            //    return GO.AddComponent<HealthMagicPointShowUI>();
            //}, (u) =>
            //{
            //    u.gameObject.SetActive(false);

            //}, (u) =>
            //{
            //    u.gameObject.SetActive(true);

            //});
        }
        protected override void InitAfterSpawn(UIShowOrgan t)
        {
            base.InitAfterSpawn(t);
            //Client_UnitProperty client_UnitProperty = InstanceFinder.GetInstance<NormalUtility>().Server_AddUnitProperty(t.OwnerUnit.gameObject);
            t.UnitProperty = InstanceFinder.GetInstance<NormalUtility>().Server_AddUnitProperty(t.OwnerUnit.gameObject);
            //t.UnitProperty.HealthMagicPointShowUI= uiObjectPool.GetObjectInPool();
            InitShow(t);
            //Debug.Log("11" + t.BodyOrgan + "22" + t.MagicOrgan);
            //t.HealthMagicPointShowUI.SetHealthPer(t.HealthPer,false);
            //t.HealthMagicPointShowUI.SetMagicPer(t.MagicPer,false);
        }
        protected override void InitializeBeforeRecycle(UIShowOrgan t)
        {
            //Debug.Log(t.HealthMagicPointShowUI + "333");
            InstanceFinder.GetInstance<NormalUtility>().Server_RemoveUnitProperty(t.OwnerUnit.gameObject);
            //t.BodyOrgan = null;
            //t.MagicOrgan = null;
            t.UnitProperty = null;
            base.InitializeBeforeRecycle(t);
        }
        protected void InitShow(UIShowOrgan uIShowOrgan)
        {
            //uIShowOrgan.UnitProperty.HealthMagicPointShowUI.SetHealthPer(uIShowOrgan.HealthPer, false);
            //uIShowOrgan.UnitProperty.HealthMagicPointShowUI.SetMagicPer(uIShowOrgan.MagicPer, false);
            //Debug.Log(uIShowOrgan.OwnerUnit+"的技能为"+uIShowOrgan.MagicOrgan.StatusList
            //Debug.Log(uIShowOrgan.OwnerUnit + "的魔法为" + uIShowOrgan.MagicOrgan.MagicPoint_Max);

        }
        public override void LateUpdate()
        {
            base.LateUpdate();
            //HealthMagicPointShowUI ui;
            foreach(var v in allComponents)
            {
                if (v != null&&v.OwnerUnit!=null&&v.UnitProperty!=null)
                {
                    v.UnitProperty.HealthPointPer = v.HealthPer;
                    v.UnitProperty.MagicPointPer = v.MagicPer;
                    v.UnitProperty.UnitHigh = v.UnitHight;
                }
            }
        }
    }
}
