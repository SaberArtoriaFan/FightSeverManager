using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public interface IMono
    {
        bool IsSpwanSystem { get; }
        public Type ModelType { get; }

        bool Enable { get; }
        void Start ();
        void Update ();
        void Awake(WorldBase world);
        void OnDestory();
        void LateUpdate ();
        void OnDrawGizmos();
        void OnDrawGizmosSelected();
        void FixedUpdate();
        void StartAfterNetwork();
        void Reset();

    }
    internal interface IDestroyComponentSystem
    {
        //public T SpawnComponent();
        //public void DestoryComponent<T>(T component)where T:IComponentBase;


    }

    public abstract class SingletonSystemBase<T> : SystemBase<T> where T : SystemModelBase, new() 
    {
        protected T instance;
        public SingletonSystemBase()
        {
            instance=new T();
        }
    }
    public abstract class SlightSystemBase<T,T2> : SystemBase<T>, IDestroyComponentSystem where T : IComponentBase, new()where T2 : IContainerEntity
    {
        protected List<T> allComponents;
        protected ObjectPool<T> objectPool;
        public override bool IsSpwanSystem => true;
        #region Pool
        protected T Spawn()
        {
            T t = new T();
            return t;
        }
        protected virtual void InitAfterSpawn(T t)
        {

        }
        protected virtual void InitializeBeforeRecycle(T t)
        {
            
        }
        public virtual T SpawnComponent(T2 entity)
        {
            T t = new T();
            allComponents.Add(t);
            t.ClearEnable();
            t.Init(entity);
            InitAfterSpawn(t);
            return t;


        }
        public virtual void DestoryComponent(T t)
        {
            if (t == null) return;
            //Debug.Log("Ä¿±êÏú»Ù");
            t.Destory();
            InitializeBeforeRecycle(t);
            t.ClearEnable();
            t.Owner = null;
            if (allComponents.Contains(t))
                allComponents.Remove(t);
            //objectPool.RecycleToPool(t);
        }
        #endregion
        public SlightSystemBase()
        {
            allComponents = new List<T>();
        }
        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            objectPool = world.FindSystem<ObjectPoolSystem>().AddPool<T>(Spawn,null,null);
        }

        //public  void DestoryComponent(ComponentBase component)
        //{

        //}

        //public void DestoryComponent<T1>(T1 component) where T1 : ComponentBase,new()
        //{
        //    if (component is T)
        //    {
        //        DestoryComponent((T)component);

        //    }
        //}
    }

    public abstract class NormalSystemBase<T> : SlightSystemBase<T,EntityBase> where T : ComponentBase, new()
    {
    }
    public abstract class SystemStone:IMono
    {
        protected WorldBase world;
        public virtual bool IsSpwanSystem => false;
        public virtual Type ModelType { get; }
        protected bool enable = true;

        public WorldBase World { get => world; }

        public bool Enable => enable;

        public virtual void SetEnable(bool value)
        {
            enable = value;
           
        }

        public virtual void Awake(WorldBase world)
        {
            this.world = world;
        }

        public virtual void OnDestory()
        {
            this.world = null;
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }
        public virtual void FixedUpdate()
        {

        }
        public virtual void OnDrawGizmos()
        {
        }

        public virtual void OnDrawGizmosSelected()
        {
        }

        public virtual void StartAfterNetwork()
        {

        }
        public virtual void Reset()
        {

        }
    }
    public abstract class SystemBase<T> : SystemStone where T : IComponentBase,new ()
    {
        private Type modelType= typeof(T);

        public override Type ModelType => modelType;
    }
}
