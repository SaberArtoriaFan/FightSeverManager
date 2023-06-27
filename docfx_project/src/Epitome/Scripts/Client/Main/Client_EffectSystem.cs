using Saber.Base;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace XianXia
{
    public class Client_EffectSystem : Client_SingletonBase<Client_EffectSystem>
    {
        string sortLayer = "Effect";
        //ObjectPoolSystem objectPoolSystem;
        //ABManagerSystem aBManagerSystem;
        //TimerManagerSystem timerManagerSystem;
        float lastestDestroyTime;
        Transform parent;
        Dictionary<GameObject, GameObject> parentAndSonDict;
        Dictionary<string, GameObject> nameAndSonDict;

        protected override void Start()
        {
            base.Start();
            //objectPoolSystem = world.FindSystem<ObjectPoolSystem>();
            //aBManagerSystem = world.FindSystem<ABManagerSystem>();
            //timerManagerSystem = world.FindSystem<TimerManagerSystem>();
            parent = new GameObject("Effect").transform;
            parent.parent = this.transform;
            nameAndSonDict = new Dictionary<string, GameObject>();
            parentAndSonDict = new Dictionary<GameObject, GameObject>();
        }

        public void CreateEffectInPool_Main(string effectName, string key,GameObject parent, Vector3 offestPos, bool isAutoRecycle)
        {
            if (nameAndSonDict.ContainsKey(key)) { Debug.LogError("创建特效出错！！！"); nameAndSonDict.Remove(key); }
            GameObject particleSystem = GetEffectInPool_Main(effectName, isAutoRecycle);
            if (particleSystem == null) { Debug.LogError("CantFindPOOL"); return; }
            particleSystem.transform.SetParent(parent.transform);
            particleSystem.transform.localPosition = offestPos;
            //particleSystem.transform.localRotation = Quaternion.identity;
            particleSystem.transform.localScale = Vector3.one;
            particleSystem.gameObject.SetActive(true);
            ParticleSystem p = particleSystem.GetComponentInChildren<ParticleSystem>();
            if (p != null)
                p.Play();
            nameAndSonDict.Add(key, particleSystem);
        }
        public void CreateEffectInPool_Main(string effectName, GameObject parent,Vector3 offestPos, bool isAutoRecycle)
        {
            if (parentAndSonDict.ContainsKey(parent)) { Debug.LogError("创建特效出错！！！"); parentAndSonDict.Remove(parent); }
            GameObject particleSystem = GetEffectInPool_Main(effectName, isAutoRecycle);
            if (particleSystem == null) { Debug.LogError("CantFindPOOL"); return; }
            particleSystem.transform.SetParent(parent.transform);
            particleSystem.transform.localPosition = offestPos;
            particleSystem.transform.localRotation = Quaternion.identity;
            particleSystem.transform.localScale = Vector3.one;
            particleSystem.gameObject.SetActive(true);
            ParticleSystem p = particleSystem.GetComponentInChildren<ParticleSystem>();
            if (p != null)
                p.Play(); parentAndSonDict.Add(parent, particleSystem);
        }
        public void CreateEffectInPool_Main(string effectName, string key,Vector3 pos,Vector3 rotate=default,Vector3 scale=default, bool isAutoRecycle=false)
        {
            GameObject particleSystem = GetEffectInPool_Main(effectName, isAutoRecycle);
            particleSystem.transform.position = pos;
            if(rotate!=default)
                particleSystem.transform.rotation = Quaternion.Euler(rotate);
            if(scale!=default)
                particleSystem.transform.localScale = scale;
            particleSystem.gameObject.SetActive(true);
            ParticleSystem p = particleSystem.GetComponentInChildren<ParticleSystem>();
            if (p != null)
                p.Play();
            if (!string.IsNullOrEmpty(key)&&nameAndSonDict.ContainsKey(key)==false)
                nameAndSonDict.Add(key,particleSystem);
        }
        public void RecycleEffect(string key)
        {
            if (!nameAndSonDict.ContainsKey(key)) return;
            GameObject project = null;
            project = nameAndSonDict[key];
            nameAndSonDict.Remove(key);
            if (project == null) { Debug.LogError("CantFindProjectModel"); return; }
            RecycleEffectToPool(project, project.gameObject.name);
        }
        public void RecycleEffect(GameObject parent)
        {
            if (!parentAndSonDict.ContainsKey(parent)) return;
            GameObject project = null;
            project = parentAndSonDict[parent];
            parentAndSonDict.Remove(parent);
            if (project == null) { Debug.LogError("CantFindProjectModel"); return; }
            RecycleEffectToPool(project,project.gameObject.name);
        }
        /// <summary>
        /// 拿出来Active是False，需要手动激活以及Play
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="isAutoRecycle"></param>
        /// <returns></returns>
        public GameObject GetEffectInPool_Main(string effectName, bool isAutoRecycle)
        {
            if (!PoolManager.Instance.IsPoolAlive(effectName))
            {
                GameObject go = ABUtility.Load<GameObject>(ABUtility.EffectMainName + effectName);
                PoolManager.Instance.AddPool<GameObject>(() => {
                    SortingGroup sortingGroup = GameObject.Instantiate(go).AddComponent<SortingGroup>();
                    sortingGroup.gameObject.name = effectName;
                    sortingGroup.transform.SetParent(parent);
                    sortingGroup.sortingLayerName = sortLayer;
                    return sortingGroup.gameObject;
                },
                (u) => { u.gameObject.transform.SetParent(null); u.gameObject.SetActive(false); }, 
                (u) => { u.gameObject.SetActive(false); }, effectName);
            }

            GameObject effect = PoolManager.Instance.GetObjectInPool<GameObject>(effectName);
            if (isAutoRecycle)
            {
                ParticleSystem p = null;
                XianXia.Client.XianXiaSkeletonAnimationHandle xx = null;
                if ((p= effect.GetComponent<ParticleSystem>()) != null)
                {
                    Timer timer = null;
                    float t = 0;
                    timer = TimerManager.Instance.AddTimer(() => { t += Time.deltaTime; ReadyRecycleEffect(p, effectName, timer); if (t >= lastestDestroyTime) { timer.Stop(); RecycleEffectToPool(effect, effectName); } }, 1, true);
                }else if ((xx= effect.GetComponentInChildren<XianXia.Client.XianXiaSkeletonAnimationHandle>()) != null)
                {
                    Timer timer = null;
                    float t = 0;
                    timer = TimerManager.Instance.AddTimer(() => { t += Time.deltaTime; ReadyRecycleSpine(xx, effectName, timer); if (t >= lastestDestroyTime) { timer.Stop(); RecycleEffectToPool(effect, effectName); } }, 1, true);
                }


            }

            return effect;
        }


        private void ReadyRecycleSpine(XianXia.Client.XianXiaSkeletonAnimationHandle effect, string effectName, Timer timer)
        {
            if (effect==null|| effect.gameObject.activeSelf == false||effect.IsActiveAnimation) return;
            RecycleEffectToPool(effect.gameObject, name);
            timer.Stop();
        }


        //public ParticleSystem GetEffectInPool(string effectName)
        //{
        //    if (!objectPoolSystem.IsPoolAlive(effectName))
        //        objectPoolSystem.AddPool<ParticleSystem>(() => { return     GameObject.Instantiate(aBManagerSystem.LoadResource<GameObject>(effectABPackageName, effectName)).GetComponent<ParticleSystem>(); }, (u) => { u.gameObject.SetActive(false); u.transform.SetParent(world.transform); }, (u) => { u.gameObject.SetActive(false); }, effectName);
        //    return objectPoolSystem.GetObjectInPool<ParticleSystem>(effectName);
        //}
        private void RecycleEffectToPool(GameObject effect, string effectName)
        {
            if (!PoolManager.Instance.IsPoolAlive(effectName)) return;
            PoolManager.Instance.RecycleToPool(effect, effectName);
        }
        protected void ReadyRecycleEffect(ParticleSystem particleSystem, string name,Timer timer)
        {
            if (particleSystem.gameObject.activeSelf == false) return;
            if (particleSystem != null && particleSystem.IsAlive(true)) return;
            RecycleEffectToPool(particleSystem.gameObject, name); 
            timer.Stop(); 
        }

        public void Clear()
        {
            var parents = parentAndSonDict.Keys.ToArray();
            var names = nameAndSonDict.Keys.ToArray();
            foreach(var v in parents)
            {
                if (v != null)
                    RecycleEffect(v);
            }
            foreach(var v in names)
            {
                if (!string.IsNullOrEmpty(v))
                    RecycleEffect(v);
            }
        }
    }
}
