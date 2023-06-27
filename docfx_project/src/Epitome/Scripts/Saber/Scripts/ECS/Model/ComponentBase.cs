using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public enum ComponentType
    {
        none,
        system,
        body,
        attack,
        leg,
        magic,
        talent,
        statusHeart,
        uIShow,
        equip,
        max
    }
    public interface IComponentBase
    {
        ComponentType ComponentType { get; }
        bool Enable { get;  }
        void ClearEnable();
        IContainerEntity Owner { get; set; }
        void Init(IContainerEntity owner);
        void Destory();
    }
    public class BoolAttributeContainer
    {
        int originValue = 0;
        bool origin;

        /// <summary>
        /// 该值代表它不受影响时的布尔值
        /// </summary>
        public bool Origin { get => origin; }

        public int intValue => originValue;
        //T sumValue;


        public void ChangeValue(int value)
        {
            Debug.Log("现在时" + originValue);
            originValue += value;
        }
        public bool GetValue()
        {
            if (originValue >= 0) return origin;
            else return !origin;
        }
        public BoolAttributeContainer(bool origin)
        {
            this.origin = origin;
        }
        /// <summary>
        /// 回到不受外界影响时
        /// </summary>
        public void Clear()
        {
            originValue = 0;
        }
    }
    public class ComponentBase:IComponentBase
    {
        
        internal IContainerEntity owner;
        internal BoolAttributeContainer enable=new BoolAttributeContainer(true);
        //internal bool enable;
        internal bool isCantFindWhenDisable = false;
        public virtual ComponentType ComponentType { get; }

        public IContainerEntity Owner { get => owner;  }
        public bool Enable { get => enable.GetValue(); }
        public bool Alive => enable.GetValue() && owner != null;

        ComponentType IComponentBase.ComponentType { get =>ComponentType;  }
        bool IComponentBase.Enable { get=>enable.GetValue();  }
        IContainerEntity IComponentBase.Owner { get => owner; set => owner=value; }

        public ComponentBase()
        {

        }
        //internal void Init(EntityBase owner)
        //{

        //    InitComponent(owner);
        //}
        /// <summary>
        /// 被添加到Entity时调用
        /// 用来重设一些参数
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void InitComponent(IContainerEntity owner)
        {
            this.owner = owner;
            enable.Clear();
        }
        /// <summary>
        /// 只有异常状态设置才需要用这个
        /// </summary>
        /// <param name="value"></param>
        public void SetEnable(bool value)
        {
            SetComponentEnable(value);
        }
        protected virtual void SetComponentEnable(bool value)
        {
            if (value == enable.Origin) enable.ChangeValue(1);
            else enable.ChangeValue(-1);

        }
        public void ClearEnable()
        {
            enable.Clear();
        }

        public void Init(IContainerEntity owner)
        {
            InitComponent(owner);
        }

        public virtual void Destory()
        {
            this.owner = null;
            enable.Clear();
        }
    }
    public class SystemModelBase : IComponentBase
    {
        public ComponentType ComponentType => ComponentType.system;
        protected bool enable;
        public bool Enable => enable;

        public IContainerEntity Owner => null;

        bool IComponentBase.Enable { get; }
        IContainerEntity IComponentBase.Owner { get; set; }

        public virtual void Init(IContainerEntity owner)
        {
            //throw new System.NotImplementedException();
        }

        public void Destory()
        {
        }

        public void ClearEnable()
        {
            
        }
    }
}
