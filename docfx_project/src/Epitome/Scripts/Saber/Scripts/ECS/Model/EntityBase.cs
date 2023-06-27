using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Saber.ECS
{
    public interface IContainerEntity 
    {

    }

    public class EntityBase:MonoBehaviour,IContainerEntity
    {
        [ShowInInspector]
        List<string> componentShow=new List<string>();
        internal Dictionary<ComponentType,ComponentBase> organDict= new Dictionary<ComponentType, ComponentBase> ();
        //internal Dictionary<ComponentBase, IDestroyComponentSystem> componentSystemDict = new Dictionary<ComponentBase, IDestroyComponentSystem>();
        internal HashSet<ComponentType> limitOrgans = null;
        internal uint entityID;

        public MonoBehaviour Host => this;

        internal BoolAttributeContainer entity_Enable = new BoolAttributeContainer(true);
        public bool Enable { get => entity_Enable.GetValue(); }
        public uint EntityID { get => entityID; }
        public int EnaleShow => entity_Enable.intValue;
        public void SetEnable(bool value)
        {
            if (value == entity_Enable.Origin) entity_Enable.ChangeValue(1);
            else entity_Enable.ChangeValue(-1);
        }
        public void LimitOrgans(HashSet<ComponentType> limit)
        {
            if (limitOrgans == null)
                limitOrgans = new HashSet<ComponentType>(limit);
            else
            {
                foreach (var v in limit)
                {
                    if (!limitOrgans.Contains(v))
                        limitOrgans.Add(v);
                }
            }
            foreach (var v in limitOrgans)
            {
                if (organDict.ContainsKey(v))
                {
                    DestroyOrgan(organDict[v]);
                }
            }
        }
        public void RelieveLimitOrgans(HashSet<ComponentType> limit)
        {
            if (limitOrgans == null)
                return;
            else
            {
                foreach (var v in limit)
                {
                    if (limitOrgans.Contains(v))
                        limitOrgans.Remove(v);
                }
            }
        }
        public T ModuleGetInBody<T>() where T : ComponentBase
        {
            foreach (var v in organDict.Values)
            {
                if (v is T)
                    return v as T;
            }
            return null;
        }
        public T FindOrganInBody<T>(ComponentType componentType,bool isIgnoreEnable =true) where T : ComponentBase
        {
            if (organDict.ContainsKey(componentType) && organDict[componentType] is T)
            {
                T organ = organDict[componentType] as T;
                if (isIgnoreEnable) return organ;
                else
                {
                    if (!organ.Enable && !organ.isCantFindWhenDisable)
                        return null;
                    else
                        return organ;
                }
            }
            else
            {
                //Debug.LogWarning($"不存在该组织" + componentType);
                return null;
            }
        }
        public void DestroyOrgan<T>(T obj) where T : ComponentBase,new ()
        {
            if (obj != null && organDict.ContainsKey(obj.ComponentType))
            {
                organDict.Remove(obj.ComponentType);
                //Debug.Log(obj.GetType()+"888");
                GameManagerBase.DesotryComponent(obj);
                //NormalSystemBase<T> system = GameManagerBase.FindSystemByComponent(obj);
                //if (system != null)
                //    system.DestoryComponent(obj);
                if(componentShow.Contains(obj.ToString()))
                    componentShow.Remove(obj.ToString());
            }
            else
            {
                Debug.LogWarning($"不存在组织");
            }
        }
        //public void DestoryAllOrgans()
        //{
        //    int start = ((int)ComponentType.none) + 1;
        //    int end = ((int)ComponentType.max);
        //    for (int i = start; i < end; i++)
        //    {
        //        this.DestroyOrgan(FindOrganInBody<ComponentBase>((ComponentType)i));
        //    }
        //    organDict.Clear();
        //}
        public T AddOrgan<T>(ComponentType chessOrganType, bool takePlace = false) where T : ComponentBase, new()
        {
            if (limitOrgans != null && limitOrgans.Contains(chessOrganType)) return null;
            NormalSystemBase<T> system = GameManagerBase.FindSystemByComponent<T>();
            //Debug.Log(system.ToString());
            if (system == null) return null;
            if (!organDict.ContainsKey(chessOrganType))
            {
                T t = system.SpawnComponent(this);
                organDict.Add(t.ComponentType, t);
                componentShow.Add(t.ToString());
                return t;
            }
            else
            {
                if (!takePlace)
                {
                    if (organDict[chessOrganType] is T)
                        return (T)organDict[chessOrganType];
                    else
                        return null;
                }
                else
                {
                    DestroyOrgan(organDict[chessOrganType] as T);
                    return AddOrgan<T>(chessOrganType);
                }
            }


        }

        private void OnDestroy()
        {
            //DestoryAllOrgans();
        }
        //public void AddOrgan<T>(T t, bool takePlace = false) where T : ComponentBase, new()
        //{
        //    if (limitOrgans != null && limitOrgans.Contains(t.ComponentType)) return;
        //    if (organDict.ContainsKey(t.ComponentType))
        //    {
        //        if (takePlace)
        //        {
        //            DestroyOrgan(organDict[t.ComponentType]);
        //            AddOrgan<T>(t);
        //        }
        //    }
        //    else
        //    {
        //        organDict.Add(t.ComponentType, t);
        //        t.Init(this);
        //    }
        //}
    }
}
