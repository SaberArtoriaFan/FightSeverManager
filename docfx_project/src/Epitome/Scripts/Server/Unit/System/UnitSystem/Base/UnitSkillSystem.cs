using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using System.Reflection;
using System;
using cfg.skillActive;

namespace XianXia.Unit
{

    public abstract class UnitSkillSystem<T,T1> : NormalSystemBase<T> where T:StatusOrganBase<T1>,new() where T1:SkillBase,new()
    {
        protected SkillSystemBase<T1> skillSystem;


        public override void Start()
        {
            base.Start();
            skillSystem=world.FindSystem<SkillSystemBase<T1>>();
            //Debug.Log((skillSystem == null) + "//88");
        }
        protected override void InitializeBeforeRecycle(T t)
        {
            UnitUtility.ClearStatusOrgan(t);
            base.InitializeBeforeRecycle(t);
        }
        public void GainSkill(T t,T1 skill)
        {
            skillSystem.GainSkill(t, skill);
            GainSkillAfter(skill, t);

        }
        protected abstract string GainSkillBefore(int id, out object data);
        public T1 GainSkill(T t,int id)
        {
            T1 s= skillSystem.GainSkill(t, GainSkillBefore(id, out object data));
            if (s == null) return null;
            GainSkillAfter(s, t,data);
            return s;
        }

        public void SortFristSkill(T t,T1 skill)
        {
            if (t == null || skill == null||t.StatusList.Contains(skill)==false) return;
            t.StatusList.Remove(skill);
            t.StatusList.Insert(0, skill);
        }
        protected virtual void GainSkillAfter(T1 s,T organ,object data=null)
        {

        }
        public  void GainSkill<T2>(T t)where T2:T1,new ()
        {
            T1 s= skillSystem.GainSkill<T2>(t);
            GainSkillAfter(s, t);
        }
        public virtual void LostSkill(T1 skill)
        {
            skillSystem.LostSkill(skill);

        }
        public virtual void LostSkill<T2>(string skill,StatusOrganBase<T2> bar)where T2:SkillBase
        {
            if (string.IsNullOrEmpty(skill)) return;
            skillSystem.LostSkill(skill,bar);

        }
    }
}
