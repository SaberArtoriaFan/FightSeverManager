using Saber.Base;
using UnityEngine;
using Saber.ECS;
using System;
using System.Reflection;
using UnityEngine.EventSystems;
using EventSystem = Saber.ECS.EventSystem;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public interface ISkillSystem
    {
        public void ChangeActionToEvent<T2, T3,T4>(string name, Action<T2, T3,T4> action, bool isAdd);

        public void ChangeActionToEvent<T2, T3>(string name, Action<T2, T3> action, bool isAdd);
        public void ChangeActionToEvent<T2>(string name, Action<T2> action, bool isAdd);

        WorldBase World { get; }
        AStarPathfinding2D Map { get; }
 

    }
    public class SkillSystemBase<T> : SlightSystemBase<T, StatusOrganBase<T>>,ISkillSystem where T:SkillBase,new ()
    {
        Assembly assembly;
        protected EventSystem eventSystem;
        public const string SkillNameSpace = "XianXia.Unit.";
        public AStarPathfinding2D Map { get; private set; }

        public override void Start()
        {
            base.Start();
            assembly = Assembly.GetExecutingAssembly();
            eventSystem=world.FindSystem<EventSystem>();
            Map=GameObject.FindObjectOfType<AStarPathfinding2D>();  
        }
        public void ChangeActionToEvent<T2, T3,T4>(string name, Action<T2, T3,T4> action, bool isAdd)
        {
            SaberEvent<T2, T3,T4> saberEvent = eventSystem.GetEvent<T2, T3,T4>(name);
            if (saberEvent != null)
            {
                if (isAdd) saberEvent.AddAction(action);
                else saberEvent.RemoveAction(action);
            }
            else
            {
                Debug.LogWarning("未找到该类型事件");
            }
        }
        public void ChangeActionToEvent<T2, T3>(string name, Action<T2, T3> action, bool isAdd)
        {
            SaberEvent<T2, T3> saberEvent = eventSystem.GetEvent<T2, T3>(name);
            if (saberEvent != null)
            {
                if (isAdd) saberEvent.AddAction(action);
                else saberEvent.RemoveAction(action);
            }
            else
            {
                Debug.LogWarning("未找到该类型事件");
            }
        }
        public void ChangeActionToEvent<T2>(string name, Action<T2> action, bool isAdd)
        {
            SaberEvent<T2> saberEvent = eventSystem.GetEvent<T2>(name);
            if (saberEvent != null)
            {
                if (isAdd) saberEvent.AddAction(action);
                else saberEvent.RemoveAction(action);
            }
        }

        internal T1 GainSkill<T1>(StatusOrganBase<T> magicOrgan )where T1 : T,new ()
        {
            if (magicOrgan == null) return null;
            T1 skill=new T1();
            return GainSkill(magicOrgan, skill) as T1;
        }
        internal T GainSkill(StatusOrganBase<T> magicOrgan,T skill) 
        {
            if (magicOrgan == null || skill == null) return null;
            InitSkill(skill);
            skill.Init(magicOrgan);
            //string skillName = skill.SkillRealName;
            Debug.Log(skill.GetType().ToString() + "SSS");
            UnitUtility.StatusOrganAdd(magicOrgan, skill.GetType().ToString(), skill, LostSkill);

            skill.AcquireSkill();
            return skill;
        }
        internal T GainSkill(StatusOrganBase<T> t, string skillName)
        {
            if (skillName == "") { Debug.LogError("SkillName is null,waht happened?"); return null; }
            string path = SkillNameSpace + skillName + "," + assembly.FullName;//命名空间.类型名,程序集
            //Debug.Log("QQQ" + assembly.Location);


            Type o = Type.GetType(path);//加载类型
            //Debug.Log("QQQ"+skillName +"wwww"+ o.ToString());
            if (o == null) { Debug.LogError($"CANT Find SkillType:{path},waht happened?"); return null; }
            T obj = Activator.CreateInstance(o) as T;//根据类型创建实例
            //Debug.Log("QQQ" + obj.ToString());
            if(obj==null) { Debug.LogError("CANT Instance SkillType,waht happened?"); return null; }
            //Debug.Log(t.OwnerUnit + "获得技能" + skillName);
            return GainSkill(t, obj);
            

        }
        internal void LostSkill(string name, object v)
        {
            if (!(v is StatusOrganBase<T> bar)) return;
            if (bar == null) return;
            name = SkillNameSpace + name;
            T[] skills= UnitUtility.StatusOrganRemove<T>(name,bar, out bool res);
            if (res)
            {
                Debug.Log(name + "失去技能");
                foreach (var b in skills)
                {
                    b.LostSkill();
                    //清理技能参数
                    ClearPara(b);
                }
            }

        }
        internal void LostSkill(T skill)
        {
            if(skill == null) return;
            UnitUtility.StatusOrganRemove(skill, out bool res);
            if (res)
                skill.LostSkill();
            //清理技能参数
            ClearPara(skill);
        }

        public  void InitSkill(T skill)
        {
            skill.SkillSystem = this;
        }
        private void ClearPara(T skill)
        {
            skill.Owner = null;
            skill.SkillSystem = null;
        }

        public override T SpawnComponent(StatusOrganBase<T> entity)
        {
            Debug.LogWarning("不该使用此函数");
            return null;
        }
        public override void DestoryComponent(T component)
        {
            Debug.LogWarning("不该使用此函数");
        }


    }
}
