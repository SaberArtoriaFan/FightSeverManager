using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class ObjectPoolSystem : SystemStone
    {
        Dictionary<string, ObjectPoolBase> poolDict;

        public T GetObjectInPool<T>(string poolName = "")
        {
            if (poolName == "") poolName = typeof(T).ToString();
            ObjectPool<T> pool = poolDict[poolName] as ObjectPool<T>;
            if (poolDict.ContainsKey(poolName) && pool != null) return pool.GetObjectInPool();
            else
            {
                Debug.LogError($"不存在该对象池" + poolName);
                return default;
            }
        }
        public void RecycleToPool<T>(T obj, string poolName = "")
        {

            if (poolName == "") poolName = typeof(T).ToString();
            if (!poolDict.ContainsKey(poolName))
            { Debug.LogError($"不存在该对象池" + poolName); return; }
            ObjectPool<T> pool = poolDict[poolName] as ObjectPool<T>;
            if (pool == null) { Debug.LogError($"不存在该对象池" + poolName); return; }
            if (poolDict.ContainsKey(poolName) && pool != null) pool.RecycleToPool(obj);
            else
            {
                Debug.LogError($"不存在该对象池" + poolName);
            }
        }
        //public void AddPool<T>(Func<T> spawn, Action<T> recycle, Action<T> init, string poolName = "", int initialCapacity = 0, T[] initialObjects = null, bool isAlwaysReserve = false)
        //{
        //    ObjectPool<T> ccPool = new ObjectPool<T>(spawn, recycle, init, initialCapacity, initialObjects, isAlwaysReserve);
        //    if (poolName == "") poolName = typeof(T).ToString();
        //    if (!poolDict.ContainsKey(poolName))
        //        poolDict.Add(poolName, ccPool);
        //    else Debug.LogError($"已经存在该对象池" + poolName);
        //}
        public ObjectPool<T> AddPool<T>(Func<T> spawn, Action<T> recycle, Action<T> init, string poolName = "", int initialCapacity = 0, T[] initialObjects = null, bool isAlwaysReserve = false)
        {
            if (poolName==null|| poolName == "") poolName = typeof(T).ToString();
            if (!poolDict.ContainsKey(poolName))
            {
                ObjectPool<T> ccPool = new ObjectPool<T>(spawn, recycle, init, initialCapacity, initialObjects, isAlwaysReserve);
                poolDict.Add(poolName, ccPool);
                return ccPool;
            }
            else
            {
                Debug.LogWarning($"已经存在该对象池" + poolName);
                if (poolDict[poolName] is ObjectPool<T> pool)
                    return pool;
                else
                    return null;
            }
        }
        public bool IsPoolAlive(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            poolDict = new Dictionary<string, ObjectPoolBase>();
        }
        public override void OnDestory()
        {
            base.OnDestory();
            foreach (var p in poolDict.Values)
            {
                if (p != null)
                    p.Destory();
            }
        }
    }
}
