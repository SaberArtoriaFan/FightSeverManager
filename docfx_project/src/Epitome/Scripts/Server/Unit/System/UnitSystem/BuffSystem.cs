using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Saber.ECS;
using cfg.buff;

namespace XianXia.Unit
{
    public class BuffSystem : SlightSystemBase<Buff,StatusOrgan>
    {
        Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)> buffEventDictionary = new Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)>();
        Dictionary<string, Type> inherentBuffDict = new Dictionary<string, Type>();
        SaberEvent<StatusOrgan, Buff> AddBuffEvent;
        int id_AddBuffEvent;
        EventSystem eventSystem;
        internal Dictionary<Buff, (Action<StatusOrgan>, Action<StatusOrgan>)> BuffEventDictionary { get => buffEventDictionary; }

        
        //创建Buff
        //移除Buff
        //update里面出发buff的每秒事件
        //查询Buff
        /// <summary>
        /// 把BuFF的一些属性值重置一下
        /// </summary>
        /// <param name="t"></param>
        protected override void InitAfterSpawn(Buff t)
        {
            base.InitAfterSpawn(t);

        }
        /// <summary>
        /// 把BuFF的一些属性值重置一下
        /// </summary>
        /// <param name="t"></param>
        protected override void InitializeBeforeRecycle(Buff buff)
        {
            UnitUtility.StatusOrganRemove(buff);
            if (buffEventDictionary.ContainsKey(buff))
                buffEventDictionary.Remove(buff);
            //清理buff参数
            base.InitializeBeforeRecycle(buff);
        }

        public Buff AddBuff(string buffName,StatusOrgan target,UnitBase source,Action<StatusOrgan> enterAction,Action<StatusOrgan> updateAction,Action<StatusOrgan> endAction,int continueLong,bool isDebuff)
        {
            if (target == null) return null;
            if (!BuffUtility.IsCanAddBuff(isDebuff, target)) return null;

            Buff buff = SpawnComponent(target);
            buff.Source = source;
            buff.RealName = buffName;
            buff.IsDeBuff = isDebuff;
            buff.Enable = true;
            if (continueLong <= 0)
                buff.IsForever = true;
            else
                buff.Curr_ContinueRoundNum = continueLong;
            //InitBuff(buffName, buff);
            buffEventDictionary.Add(buff, (updateAction, endAction));
            AddBuffEvent.Trigger(id_AddBuffEvent, target, buff);
            if (buff.Enable)
            {
                UnitUtility.StatusOrganAdd(target, buffName, buff, RemoveBuff);
                enterAction?.Invoke(target);
            }
            return buff;
        }
        public InherentBuff AddInherentBuff(int id, StatusOrgan target, UnitBase source,float val,int continueLong)
        {
            BuffData buffData = LubanMgr.GetBuffData(id);
            if (buffData == null)
            {
                FightServerManager.ConsoleWrite_Saber($"目标BUFF{id}资料不存在");
                return null;
            }
            if (!inherentBuffDict.ContainsKey(buffData.BuffName))
            {
                FightServerManager.ConsoleWrite_Saber($"脚本内没有该buff类型{buffData.BuffName}");
                return null;

            }
            InherentBuff buff = Activator.CreateInstance(inherentBuffDict[buffData.BuffName]) as InherentBuff;
            if (buff != null)
            {
                buff.InitParameter(val, target, source);
                buff.InitData(buffData);
                Debug.Log($"wwww{target},{source},{val},{buff.IsDeBuff}");
                if (!BuffUtility.IsCanAddBuff(buff.IsDeBuff, target)) return null;
                if (continueLong <= 0)
                    buff.IsForever = true;
                else
                    buff.Curr_ContinueRoundNum = continueLong;
                buff.Enable = true;
                AddBuffEvent.Trigger(id_AddBuffEvent, target, buff);
                if (buff.Enable)
                {
                    UnitUtility.StatusOrganAdd(target, buffData.BuffName, buff, RemoveBuff);
                    FightServerManager.ConsoleWrite_Saber($"{target.OwnerUnit.gameObject.name}获得了BUFF{buffData.BuffName},生效数值为{val},持续时间{continueLong}");
                    buff.OnEnterEvent();
                }
                return buff;
            }
            else
                FightServerManager.ConsoleWrite_Saber($"在创建Buff{buffData.BuffName}时发生意外");


            //初始化一个内有的buff
            //传入四个参数
            //触发事件
            return null;
        }
        //private void RemoveBuff(string buffName)
        //{
        //    throw new NotImplementedException();
        //}
        public override void Start()
        {
            base.Start();
            InitBuff();
            eventSystem = world.FindSystem<EventSystem>();
            AddBuffEvent = eventSystem.RegisterEvent<StatusOrgan, Buff>(EventSystem.EventParameter.UnitAddBuffEvent,out id_AddBuffEvent);
        }
        public void RemoveBuff(Buff buff)
        {
            if (buff == null) return;
            if (buff is InherentBuff inherentBuff) {
                inherentBuff.OnExitEvent();
                UnitUtility.StatusOrganRemove<Buff>(buff);
            }
            else
            {
                if (buffEventDictionary.TryGetValue(buff, out var actions) && actions != default)
                    actions.Item2?.Invoke((StatusOrgan)buff.Owner);
                DestoryComponent(buff);
            }
        }
        /// <summary>
        /// 用来删除未成功加入的buff
        /// </summary>
        /// <param name="buff"></param>
        public void DeleteBuff(Buff buff)
        {
            if(buff is InherentBuff)
            {
                buff.Destory();
            }
            else
            {
                if (buffEventDictionary.TryGetValue(buff, out var actions) && actions != default)
                    actions.Item2?.Invoke((StatusOrgan)buff.Owner);
                DestoryComponent(buff);
            }
        }

        protected void InitBuff()
        {
            //通过名字查表得到buff数据，并配置
            Type type = typeof(BuffDictionary);
            foreach(var v in type.GetNestedTypes())
            {
                Debug.Log("固有BUFF初始化:" + v.Name);
                inherentBuffDict.Add(v.Name, v);
            }
        }
        internal bool UpdateBuff(Buff buff)
        {
            if (!buff.IsForever)
            {
                if (buff.Curr_ContinueRoundNum == 0)
                {
                    RemoveBuff(buff);
                    return true;
                }
                buff.Curr_ContinueRoundNum--;
            }
            if(buff is InherentBuff inherentBuff)
                inherentBuff.OnUpdateEvent();
            else
                {
                    if (buffEventDictionary.TryGetValue(buff, out var actions) && actions != default)
                        actions.Item1?.Invoke((StatusOrgan)buff.Owner);
                }
            return false;

               
        }
    }
}
