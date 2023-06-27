using System;
using System.Collections.Generic;
//脚本作者:Saber

namespace XianXiaFightGameServer.Tool
{
    public class ObjectPoolBase
    {
        public virtual void Destory() { }
    }
    public class ObjectPool<T>:ObjectPoolBase
    {
        private bool isAlwaysReserve = false;
        private  Func<T> SpawnEvent;
        private  Action<T> RecycleEvent;
        private  Action<T> InitEvent;
        protected Stack<T> pool = new Stack<T>();
        public ObjectPool(Func<T> spawn,Action<T> recycle, Action<T> init, int initialcapacity = 0, T[] initialObjects = null,bool isAlwaysReserve=false)
        {
            if (spawn == null) { Saber.SaberDebug.LogError("PoolSpawn cant be null");return; }
            SpawnEvent = spawn;
            RecycleEvent = recycle;
            InitEvent = init;
            for (int i = 0; i < initialcapacity; i++)
            {
                pool.Push(SpawnEvent());
            }
            if (initialObjects != null)
            {
                for (int i = 0; i < initialObjects.Length; i++)
                {
                    pool.Push(initialObjects[i]);
                }
            }
            this.isAlwaysReserve = isAlwaysReserve;
            if (isAlwaysReserve&&pool.Count==0)
                pool.Push(SpawnEvent());
        }
        public T GetObjectInPool()
        {
            T res;
            if (pool.Count > 0)
            {
                res = pool.Pop();
            }
            else//Ab包中实例化
            {
                res = SpawnEvent();
                //gridProjectorsPool.Push(res);
            }
            if (isAlwaysReserve&&pool.Count==0)
                pool.Push(SpawnEvent());
            InitEvent?.Invoke(res);

            return res;
        }
        public T GetObjectInPool(Action<T> initAction)
        {
            T res;
            if (pool.Count > 0)
            {
                res = pool.Pop();
            }
            else//Ab包中实例化
            {
                res = SpawnEvent();
                //gridProjectorsPool.Push(res);
            }
            if (isAlwaysReserve && pool.Count == 0)
                pool.Push(SpawnEvent());

            initAction?.Invoke(res);
            InitEvent?.Invoke(res);

            return res;
        }
        public void RecycleToPool(T obj)
        {
            if (obj!=null&&!pool.Contains(obj))
            {
                RecycleEvent?.Invoke(obj);
                pool.Push(obj);
            }
        }

        public override void Destory()
        {
            base.Destory();
            SpawnEvent = null;
            RecycleEvent = null;
            InitEvent = null;
        }

    }
}
