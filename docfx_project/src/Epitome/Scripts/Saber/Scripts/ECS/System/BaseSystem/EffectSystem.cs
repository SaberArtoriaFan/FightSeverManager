using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Saber.ECS
{
    public class EffectSystem : SingletonSystemBase<EffectSystemModel>
    {
        string sortLayer="Effect";
        ObjectPoolSystem objectPoolSystem;
        //ABManagerSystem aBManagerSystem;
        TimerManagerSystem timerManagerSystem;
        float lastestDestroyTime;
        Transform parent;
        public override void Start()
        {
            base.Start();
            objectPoolSystem= world.FindSystem<ObjectPoolSystem>();
            //aBManagerSystem = world.FindSystem<ABManagerSystem>();
            timerManagerSystem = world.FindSystem<TimerManagerSystem>();
            parent = new GameObject("Effect").transform;
            parent.parent = world.transform;
        }
        
        /// <summary>
        /// 拿出来Active是False，需要手动激活以及Play
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="isAutoRecycle"></param>
        /// <returns></returns>
        public ParticleSystem GetEffectInPool_Main(string effectName,bool isAutoRecycle)
        {
            if (!objectPoolSystem.IsPoolAlive(effectName))
            {
                GameObject go = ABUtility.Load<GameObject>(ABUtility.EffectMainName + effectName);
                objectPoolSystem.AddPool<ParticleSystem>(() => {
                    SortingGroup sortingGroup = GameObject.Instantiate(go).AddComponent<SortingGroup>();
                    sortingGroup.transform.SetParent(parent);
                    sortingGroup.sortingLayerName = sortLayer;
                    return sortingGroup.GetComponent<ParticleSystem>();
                },
    (u) => { u.gameObject.SetActive(false);
                u.transform.SetParent(parent);   }, 
    (u) => { u.gameObject.SetActive(false); }, effectName);
            }

            ParticleSystem effect = objectPoolSystem.GetObjectInPool<ParticleSystem>(effectName);
            if (isAutoRecycle)
            {
                Timer timer = null;
                float t = 0;
                timer = timerManagerSystem.AddTimer(() => { t += Time.deltaTime;if (t >= lastestDestroyTime) { ReadyRecycle(effect, effectName); timer.Stop(); } }, 1);

            }

            return effect;
        }
        //public ParticleSystem GetEffectInPool(string effectName)
        //{
        //    if (!objectPoolSystem.IsPoolAlive(effectName))
        //        objectPoolSystem.AddPool<ParticleSystem>(() => { return     GameObject.Instantiate(aBManagerSystem.LoadResource<GameObject>(effectABPackageName, effectName)).GetComponent<ParticleSystem>(); }, (u) => { u.gameObject.SetActive(false); u.transform.SetParent(world.transform); }, (u) => { u.gameObject.SetActive(false); }, effectName);
        //    return objectPoolSystem.GetObjectInPool<ParticleSystem>(effectName);
        //}
        public void RecycleEffectToPool(ParticleSystem effect, string effectName)
        {
            if (!objectPoolSystem.IsPoolAlive(effectName)) return;
            objectPoolSystem.RecycleToPool(effect, effectName);
        }
        protected void ReadyRecycle(ParticleSystem particleSystem,string name)
        {
            if(!particleSystem.IsAlive(true))RecycleEffectToPool(particleSystem,name);
        }
        //IEnumerator IE_RecyclePart
    }
}
