using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public interface ISaberEvent
    {
        public void Destory();

    }
    public abstract class SaberEventBase : ISaberEvent
    {
        protected int id;

        protected SaberEventBase(int id)
        {
            this.id = id;
        }
        protected bool Verify(int id)
        {
            return id == this.id;
        }

        public abstract void Destory();
    }
    public class SaberEvent<T> : SaberEventBase
    {

        protected event Action<T> GridAction;
        internal SaberEvent(int id):base(id)
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T> action)
        {
            GridAction -= action;
        }
        public void Trigger(int id,T grid)
        {
            if(Verify(id))
                GridAction?.Invoke(grid);
        }
    
        public override void Destory()
        {
            GridAction = null;
        }
    }
    public class SaberEvent<T1, T2> : SaberEventBase
    {
        protected event Action<T1, T2> GridAction;
        internal SaberEvent(int id) : base(id)
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T1, T2> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T1, T2> action)
        {
            GridAction -= action;
        }
        public void Trigger(int id,T1 grid, T2 t2)
        {
            if (Verify(id))
                GridAction?.Invoke(grid, t2);
        }
        public override void Destory()
        {
            GridAction = null;
        }
    }
    public class SaberEvent<T1, T2,T3> : SaberEventBase
    {
        protected event Action<T1, T2,T3> GridAction;
        internal SaberEvent(int id) : base(id)
        {
        }
        ~SaberEvent()
        {
            Destory();
        }
        public void AddAction(Action<T1, T2,T3> action)
        {
            GridAction += action;
        }
        public void RemoveAction(Action<T1, T2,T3> action)
        {
            GridAction -= action;
        }
        public void Trigger(int id,T1 grid, T2 t2,T3 T3)
        {
            if (Verify(id))
                GridAction?.Invoke(grid, t2,T3);
        }
        public override void Destory()
        {
            GridAction = null;
        }
    }
    public class EventSystemModel :SystemModelBase
    {

    }
}
