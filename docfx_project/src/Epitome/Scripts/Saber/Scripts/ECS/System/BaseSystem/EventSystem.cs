using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class EventSystem : SingletonSystemBase<EventSystemModel>
    {
        public static class EventParameter 
        {
            public const string UnitDeadAfter = "UnitDeadAfter";
            public const string UnitDeadBefore = "UnitDeadBefore";
            public const string UnitDamagedAfter = "UnitDamagedAfter";
            public const string UnitDamagedBefore = "UnitDamagedBefore";
            public const string UnitMoveToNodeEvent = "UnitMoveToNodeEvent";
            public const string UnitFindNoPathEvent = "UnitFindNoPathEvent";
            public const string UnitSpellBefore = "UnitSpellBefore";
            public const string UnitSpellAfter = "UnitSpellAfter";
            public const string UnitAttackBefore = "UnitAttackBefore";
            public const string UnitAttackAfter = "UnitAttackAfter";
            public const string UnitAddBuffEvent = "UnitAddBuffEvent";
        }

        Dictionary<string, object> eventDict;
        //public DNDEvent<IProp> ChessGainPropEvent;
        //public DNDEvent<IProp> ChessLostPropEvent;

        public SaberEvent<T> RegisterEvent<T>(string name, out int id)
        {
            SaberEvent<T> res = null;
            if (name == "") name = typeof(T).Name;
            if (!eventDict.ContainsKey(name))
            {
                id = UnityEngine.Random.Range(0, 1000);
                res = new SaberEvent<T>(id);
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                id = -1;
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
        public SaberEvent<T1, T2> RegisterEvent<T1, T2>(string name, out int id)
        {
            SaberEvent<T1, T2> res = null;
            if (name == "") name = typeof(T1).Name;
            if (!eventDict.ContainsKey(name))
            {
                id = UnityEngine.Random.Range(0, 1000);
                res = new SaberEvent<T1, T2>(id);
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                id = -1;
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
        public SaberEvent<T> GetEvent<T>(string name = "")
        {
            if (name == "") name = typeof(T).Name;
            if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T>) return eventDict[name] as SaberEvent<T>;
            return null;
        }
        public SaberEvent<T1, T2> GetEvent<T1, T2>(string name = "")
        {
            if (name == "") name = typeof(T1).Name;
            if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T1, T2>) return eventDict[name] as SaberEvent<T1, T2>;
            return null;
        }

        public SaberEvent<T1, T2,T3> RegisterEvent<T1, T2,T3>(string name,out int id)
        {
            SaberEvent<T1, T2, T3> res = null;
            if (name == "") name = typeof(T1).Name;
            if (!eventDict.ContainsKey(name))
            {
                id = UnityEngine.Random.Range(0, 1000);
                res = new SaberEvent<T1, T2, T3>(id);
                eventDict.Add(name, res);
                return res;
            }
            else
            {
                id = -1;
                Debug.LogError("CantAddRepeatEvent");
                return null;
            }
        }
        public SaberEvent<T1, T2,T3> GetEvent<T1, T2,T3>(string name = "")
        {
            if (name == "") name = typeof(T1).Name;
            if (eventDict.ContainsKey(name) && eventDict[name] is SaberEvent<T1, T2, T3>) return eventDict[name] as SaberEvent<T1, T2, T3>;
            return null;
        }

        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            eventDict = new Dictionary<string, object>();
        }

        public override void OnDestory()
        {
            base.OnDestory();
            foreach (var v in eventDict.Values)
            {
                ((ISaberEvent)v).Destory();
            }
            eventDict.Clear();
            eventDict = null;
        }
    }
}
