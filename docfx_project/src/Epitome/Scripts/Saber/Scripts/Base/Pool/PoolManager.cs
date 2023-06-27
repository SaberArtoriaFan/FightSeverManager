using System;
using System.Collections.Generic;
using UnityEngine;
//脚本作者:Saber

namespace Saber.Base
{
    public class PoolManager : AutoSingleton<PoolManager>
    {
        Dictionary<string, ObjectPoolBase> poolDict;

        public T GetObjectInPool<T>(string poolName="") 
        {
            if(poolName=="")poolName = typeof(T).ToString();
            ObjectPool<T> pool= poolDict[poolName] as ObjectPool<T>;
            if (poolDict.ContainsKey(poolName)&&  pool!=null) return pool.GetObjectInPool();
            else
            {
                Debug.LogError($"不存在该对象池" + poolName);
                return default;
            }
        }
        public ObjectPool<T> GetPool<T>(string name)
        {
            if (!poolDict.ContainsKey(name)) return null;
            else if (poolDict[name] is ObjectPool<T> res) return res;
            else return null;
        }
        public void RecycleToPool<T>(T obj, string poolName = "")
        {

            if (poolName == "") poolName = typeof(T).ToString();
            if (!poolDict.ContainsKey(poolName))
            {Debug.LogError($"不存在该对象池" + poolName);return;}
            ObjectPool<T> pool = poolDict[poolName] as ObjectPool<T>;
            if(pool==null) { Debug.LogError($"不存在该对象池" + poolName); return; }
            if (poolDict.ContainsKey(poolName) && pool!=null) pool.RecycleToPool(obj);
            else
            {
                Debug.LogError($"不存在该对象池" + poolName);
            }
        }
        public ObjectPool<T> AddPool<T>(Func<T> spawn,Action<T> recycle,Action<T> init, string poolName = "",int initialCapacity=0, T[] initialObjects=null,bool isAlwaysReserve=false)
        {
            ObjectPool<T> ccPool = new ObjectPool<T>(spawn,recycle,init,initialCapacity,initialObjects,isAlwaysReserve);
            if (poolName == "") poolName = typeof(T).ToString();
            if (!poolDict.ContainsKey(poolName))
            {
                poolDict.Add(poolName, ccPool);
                return ccPool;
            }
            else { Debug.LogError($"已经存在该对象池" + poolName);return null; }
        }
        public bool IsPoolAlive(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        protected override void Init()
        {
            base.Init();
            poolDict = new Dictionary<string, ObjectPoolBase>();
            //AddPool<CoroutineCtrl>(() => { return new CoroutineCtrl(); }, (c) => { c.Stop(); });
            //AddPool<CoroutineItem>(() => { return new GameObject().AddComponent<CoroutineItem>(); }, (c) => { c.gameObject.SetActive(false); });
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach(var p in poolDict.Values)
            {
                if (p != null)
                    p.Destory();
            }

        }
    }
}
