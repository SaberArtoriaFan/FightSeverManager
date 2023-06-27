using System;
using System.Collections.Generic;
//脚本作者:Saber

namespace XianXiaFightGameServer.Tool
{
    public class PoolManager 
    {
        Dictionary<string, ObjectPoolBase> poolDict;
        public PoolManager() {
            Init();
        }
        public static void LogRecycleError(string poolName)
        {
           Saber.SaberDebug.LogError($"{poolName}pool is cant find,but u need recycle!!!");

        }
        public T GetObjectInPool<T>(string poolName="") 
        {
            if(poolName=="")poolName = typeof(T).ToString();
            ObjectPool<T> pool= poolDict[poolName] as ObjectPool<T>;
            if (poolDict.ContainsKey(poolName)&&  pool!=null) return pool.GetObjectInPool();
            else
            {
                Saber.SaberDebug.LogError($"不存在该对象池" + poolName);
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
            {
                Saber.SaberDebug.LogError($"不存在该对象池" + poolName);
                return;
            }
            ObjectPool<T> pool = poolDict[poolName] as ObjectPool<T>;
            if(pool==null)
            {
                Saber.SaberDebug.LogError($"不存在该对象池" + poolName);
                 return; }
            if (poolDict.ContainsKey(poolName) && pool!=null) pool.RecycleToPool(obj);
            else
            {
                Saber.SaberDebug.LogError($"不存在该对象池" + poolName);
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
            else
            {
                Saber.SaberDebug.LogError($"已经存在该对象池" + poolName);
                ; return null; }
        }
        public bool IsPoolAlive(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        protected  void Init()
        {
            poolDict = new Dictionary<string, ObjectPoolBase>();
            //AddPool<CoroutineCtrl>(() => { return new CoroutineCtrl(); }, (c) => { c.Stop(); });
            //AddPool<CoroutineItem>(() => { return new GameObject().AddComponent<CoroutineItem>(); }, (c) => { c.gameObject.SetActive(false); });
        }
        protected  void OnDestroy()
        {
            if (poolDict != null){
                foreach (var p in poolDict.Values)
                {
                    if (p != null)
                        p.Destory();
                }
            }


        }
        ~PoolManager() 
        {
            OnDestroy();
        }
    }
}
