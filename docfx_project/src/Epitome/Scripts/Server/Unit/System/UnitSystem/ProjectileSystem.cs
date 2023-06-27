using DG.Tweening;
using FishNet;
using FishNet.Managing.Server;
using FishNet.Object;
using Saber.Base;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace XianXia
{

    public class ProjectileSystem : SingletonSystemBase<ProjectileSystemModel>
    {
        const string origin = "ProjectileOrigin";
        GameObject spawnOrigin = null;
        Saber.Base.ObjectPool<GameObject> originObjectPool = null;
        public class ProjectileHelper
        {
            public Transform owner;
            public  Transform target;
            public  Tween tween;
            public  Vector3 targetPos;
            public string name;
            public float flySpeed;
            public float angel;
            public AnimationCurve animationCurve;
            public Action arriveAction;
            public Func<bool> endFunc;
            public Saber.ECS.Timer timer;
            public float countTime = 0;
            public bool IsHasTarget => target != null;
            public bool AutoRecycle { get; private set; }

            internal ProjectileHelper()
            {
            }

            public void Init(Transform owner, Transform target,Vector3 targetPos, string name, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction, Func<bool> endFunc,bool isAutoRecycle=true)
            {
                this.owner = owner;
                this.target = target;
                this.targetPos = target!=null? target.position:targetPos;
                this.name = name;
                this.flySpeed = flySpeed;
                this.angel = angel;
                this.animationCurve = animationCurve;
                this.arriveAction = arriveAction;
                this.endFunc = endFunc;
                if (this.endFunc == null) this.endFunc = EndFunc;
                AutoRecycle = isAutoRecycle;
            }
            private bool EndFunc()
            {
                return false;
            }
            public void Reset()
            {
                owner = null;
                target = null;
                tween = null;
                targetPos = Vector3.zero;
                name = null;
                flySpeed = 0f;
                angel = 0f;
                animationCurve = null;
                arriveAction = null;
                timer = null;
                countTime = 0;
                endFunc = null;
                AutoRecycle = false;
            }
        }
        //ObjectPoolSystem poolSystem;
        Saber.Base.ObjectPool<ProjectileHelper> helperPool;
        HashSet<ProjectileHelper> projectileHelpers = new HashSet<ProjectileHelper>();
        TimerManagerSystem timerManagerSystem;
        //ServerManager serverManager;
        //NormalUtility normalUtility;
        float ProjectileDetectionTime = Time.deltaTime;
        Transform parent;
        NormalUtility normalUtility;
        public override void Start()
        {
            base.Start();
            //world.StartCoroutine(IE_Find());
            //world
            //originObjectPool = poolSystem.AddPool(() => { return GameObject.Instantiate(spawnOrigin); }, (u) => { InstanceFinder.GetInstance<NormalUtility>().ORPC_RecycleProjectile(u.gameObject); InstanceFinder.GetInstance<ServerManager>().Despawn(u, DespawnType.Pool); u.transform.SetParent(parent); u.gameObject.SetActive(false); }, (u) => { InstanceFinder.GetInstance<ServerManager>().Spawn(u); });
            spawnOrigin = ABUtility.Load<GameObject>(ABUtility.ProjectMainName + origin);
            //Debug.Log(spawnOrigin.name + "QQQ");
            //poolSystem=world.FindSystem<ObjectPoolSystem>();
            timerManagerSystem = world.FindSystem<TimerManagerSystem>();
            helperPool= PoolManager.Instance.AddPool<ProjectileHelper>(() => { return new ProjectileHelper(); }, (u) => { u.Reset(); },null);
            parent = new GameObject().transform;
            parent.name = "Projectiles";
            parent.SetParent(world.transform);
        }
        public override void StartAfterNetwork()
        {
            normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            //originObjectPool = poolSystem.AddPool(() => { return GameObject.Instantiate(spawnOrigin); }, (u) => { InstanceFinder.GetInstance<NormalUtility>().ORPC_RecycleProjectile(u.gameObject); InstanceFinder.GetInstance<NormalUtility>().Despawn(u, DespawnType.Pool); /*u.transform.SetParent(parent);*/ u.gameObject.SetActive(false); }, (u) => { InstanceFinder.GetInstance<NormalUtility>().Spawn(u); });
            originObjectPool = normalUtility.Server_InitSpawnPool($"{ABUtility.ProjectMainName}{origin}", origin, parent, 5, (u) => { normalUtility.ORPC_RecycleProjectile(u); });
            //Debug.Log(1111 +originObjectPool.ToString());
            base.StartAfterNetwork();
        }
        //IEnumerator IE_Find()
        //{
        //    WaitUntil waitUntil = new WaitUntil(() => { if (serverManager==null) serverManager= InstanceFinder.GetInstance<ServerManager>();
        //    if(normalUtility==null) normalUtility= InstanceFinder.GetInstance<NormalUtility>();
        //        return serverManager != null && normalUtility != null; });
        //    yield return new WaitForSeconds(1f);
        //    yield return waitUntil;
        //    Debug.Log(originObjectPool + "QQQ");

        //}
        //public void InitPool(GameObject model, string name)
        //{
        //    if (!instance.PoolHashSet.Contains(name))
        //    {
        //        instance.PoolHashSet.Add(name);
        //        poolSystem.AddPool<GameObject>(() => {GameObject go= GameObject.Instantiate(model);go.transform.SetParent(parent);return go; }, (u) => { u.SetActive(false);  }, (u) => { u.SetActive(true); }, name);
        //    }
        //}

        private ProjectileHelper InitProjectile( Transform owner, Transform target,Vector3 targetPos, string name, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction, Func<bool> endFunc,bool autoRecycle)
        {
            ProjectileHelper helper = helperPool.GetObjectInPool();
            projectileHelpers.Add(helper);
            helper.Init(owner, target, targetPos, name, flySpeed, angel, animationCurve, arriveAction, endFunc,autoRecycle);
            return helper;
        }
        //生产投射物
        //给定出发点，抵达点，速度，弧度，抵达事件
        public ProjectileHelper CreateProjectile(string name, Vector3 birthPos, float scale,Transform target,Vector3 targetPos, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction,Func<bool> endFunc,bool autoCreateModel=true)
        {
            //if (!instance.PoolHashSet.Contains(name))
            //{
            //    Debug.LogError("未首先初始化目标对象池");
            //    //return null;
            //    instance.PoolHashSet.Add(name);
            //    poolSystem.AddPool<GameObject>(() => { return GameObject.Instantiate(ABUtility.Load<GameObject>(ABUtility.ProjectMainName+name)); }, (u) => { u.SetActive(false); }, (u) => { u.SetActive(true);  }, name);
            //}
           
            Transform projectile =   originObjectPool.GetObjectInPool().transform;
            //InstanceFinder.ServerManager.Spawn(projectile.gameObject);
            //InstanceFinder.ServerManager.Despawn
            //projectile.transform.position = birthPos;
            projectile.SetParent(null);
            projectile.transform.position = birthPos;
            //先同步点的位置，再加特效
            projectile.localScale = Vector3.one * scale;
            if(autoCreateModel&&!string.IsNullOrEmpty(name))
                InstanceFinder.GetInstance<NormalUtility>().ORPC_CreateProjectile(projectile.gameObject,birthPos, name);
            if (animationCurve == null) animationCurve = instance.DefaultAnimationCurve;

            ProjectileHelper helper= InitProjectile(projectile, target,targetPos, name, flySpeed, angel, animationCurve, arriveAction,endFunc,true);
            if (target != null)
                helper.timer = timerManagerSystem.AddTimer(() => { SetProjectilePathWayFollow(helper); }, ProjectileDetectionTime, true);
            else
                SetProjectilePathWay(helper);
            //InitProjectile(helper,name,flySpeed,angel,animationCurve,arriveAction);
            return helper;

        }
        private void SetProjectilePathWay(ProjectileHelper p)
        {
            if (p == null || p.owner.gameObject == null) return;
            Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos + Vector3.up * 0.7f, p.angel);
            float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

            p.tween = p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
                        //.SetLookAt(0)
                        //.SetEase(p.animationCurve)
                        .OnComplete(() => {  p.arriveAction?.Invoke(); FinishProjectile(p); });
        }
        private void SetProjectilePathWayFollow(ProjectileHelper p)
        {
            if (p == null || p.owner.gameObject == null) return;
            if (!p.IsHasTarget||p.endFunc())
            {
                FinishProjectile(p);
                return;
            }
            p.countTime += ProjectileDetectionTime;
            if (p.countTime > 10) 
            {
                FinishProjectile(p);
                return;
            } 
            //检测目标位置是否发生变化
            if (p.IsHasTarget)
            {
                if (p.tween != null && Vector3.Distance(p.targetPos, p.target.position) < 0.1f) return;
                p.targetPos = p.target.position;
            }
            Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos+Vector3.up*0.7f, p.angel);
            float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

            if (p.tween != null)
                p.tween.Kill();
            p.tween= p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
                        .SetLookAt(0)
                        //.SetEase(p.animationCurve)
                        .OnComplete(() => { p.arriveAction?.Invoke();FinishProjectile(p); });
        }
        void FinishProjectile(ProjectileHelper p)
        {
            if (p == null) return;

            if (p.timer != null)
                p.timer.Stop();
            if (p.tween != null) p.tween.Kill();
            if (p.AutoRecycle)
                originObjectPool.RecycleToPool(p.owner.gameObject);
                //poolSystem.RecycleToPool<GameObject>(p.owner.gameObject, p.name);
            projectileHelpers.Remove(p);

            helperPool.RecycleToPool(p);
        }
    }
}
