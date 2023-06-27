using DG.Tweening;
using Saber.Base;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace XianXia
{

    //public class ProjectileSystem : SingletonSystemBase<ProjectileSystemModel>
    //{
    //    const string origin = "ProjectileOrigin";
    //    GameObject spawnOrigin = null;
    //    Saber.ECS.ObjectPool<NetworkObject> originObjectPool = null;
    //    public class ProjectileHelper
    //    {
    //        public Transform owner;
    //        public  Transform target;
    //        public  Tween tween;
    //        public  Vector3 targetPos;
    //        public string name;
    //        public float flySpeed;
    //        public float angel;
    //        public AnimationCurve animationCurve;
    //        public Action arriveAction;
    //        public Func<bool> endFunc;
    //        public Timer timer;
    //        public float countTime = 0;
    //        public bool IsHasTarget { get; private set; }
    //        public bool AutoRecycle { get; private set; }

    //        internal ProjectileHelper()
    //        {
    //        }

    //        public void Init(Transform owner, Transform target,Vector3 targetPos, string name, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction, Func<bool> endFunc,bool isAutoRecycle=true)
    //        {
    //            this.owner = owner;
    //            this.target = target;
    //            this.targetPos = target!=null? target.position:targetPos;
    //            this.name = name;
    //            this.flySpeed = flySpeed;
    //            this.angel = angel;
    //            this.animationCurve = animationCurve;
    //            this.arriveAction = arriveAction;
    //            this.endFunc = endFunc;
    //            if (this.endFunc == null) this.endFunc = EndFunc;
    //            IsHasTarget = target != null;
    //            AutoRecycle = isAutoRecycle;
    //        }
    //        private bool EndFunc()
    //        {
    //            return false;
    //        }
    //        public void Reset()
    //        {
    //            owner = null;
    //            target = null;
    //            tween = null;
    //            targetPos = Vector3.zero;
    //            name = null;
    //            flySpeed = 0f;
    //            angel = 0f;
    //            animationCurve = null;
    //            arriveAction = null;
    //            timer = null;
    //            countTime = 0;
    //            endFunc = null;
    //            IsHasTarget = false;
    //            AutoRecycle = false;
    //        }
    //    }
    //    ObjectPoolSystem poolSystem;
    //    Saber.ECS.ObjectPool<ProjectileHelper> helperPool;
    //    HashSet<ProjectileHelper> projectileHelpers = new HashSet<ProjectileHelper>();
    //    TimerManagerSystem timerManagerSystem;
    //    ServerManager serverManager;

    //    float ProjectileDetectionTime = Time.deltaTime;
    //    Transform parent;
    //    public override void Start()
    //    {
    //        base.Start();
    //        serverManager = InstanceFinder.GetInstance<ServerManager>();
    //        spawnOrigin = ABUtility.Load<GameObject>(ABUtility.ProjectMainName + origin);
    //        originObjectPool = poolSystem.AddPool<NetworkObject>(() => { return GameObject.Instantiate(spawnOrigin).GetComponent<NetworkObject>(); }, (u) => { serverManager.Despawn(u, DespawnType.Pool); u.transform.SetParent(parent);u.gameObject.SetActive(false); }, (u)=> { serverManager.Spawn(u); });
    //        poolSystem=world.FindSystem<ObjectPoolSystem>();
    //        timerManagerSystem = world.FindSystem<TimerManagerSystem>();
    //        helperPool= poolSystem.AddPool<ProjectileHelper>(() => { return new ProjectileHelper(); }, (u) => { u.Reset(); },null);
    //        parent = new GameObject().transform;
    //        parent.name = "Projectiles";
    //        parent.SetParent(world.transform);
    //    }
    //    public void InitPool(GameObject model, string name)
    //    {
    //        if (!instance.PoolHashSet.Contains(name))
    //        {
    //            instance.PoolHashSet.Add(name);
    //            poolSystem.AddPool<GameObject>(() => {GameObject go= GameObject.Instantiate(model);go.transform.SetParent(parent);return go; }, (u) => { u.SetActive(false);  }, (u) => { u.SetActive(true); }, name);
    //        }
    //    }

    //    public ProjectileHelper InitProjectile(Transform owner, Transform target,Vector3 targetPos, string name, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction, Func<bool> endFunc,bool autoRecycle)
    //    {
    //        ProjectileHelper helper = helperPool.GetObjectInPool();
    //        projectileHelpers.Add(helper);
    //        helper.Init(owner, target, targetPos, name, flySpeed, angel, animationCurve, arriveAction, endFunc,autoRecycle);
    //        return helper;
    //    }
    //    //生产投射物
    //    //给定出发点，抵达点，速度，弧度，抵达事件
    //    public Transform CreateProjectile(string name, Vector3 birthPos, float scale,Transform target, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction,Func<bool> endFunc)
    //    {
    //        //if (!instance.PoolHashSet.Contains(name))
    //        //{
    //        //    Debug.LogError("未首先初始化目标对象池");
    //        //    //return null;
    //        //    instance.PoolHashSet.Add(name);
    //        //    poolSystem.AddPool<GameObject>(() => { return GameObject.Instantiate(ABUtility.Load<GameObject>(ABUtility.ProjectMainName+name)); }, (u) => { u.SetActive(false); }, (u) => { u.SetActive(true);  }, name);
    //        //}

    //        Transform projectile = originObjectPool.GetObjectInPool().transform;
    //        //InstanceFinder.ServerManager.Spawn(projectile.gameObject);
    //        //InstanceFinder.ServerManager.Despawn
    //        projectile.transform.position = birthPos;
    //        projectile.transform.localScale = Vector3.one * scale;

    //        InstanceFinder.GetInstance<NormalUtility>()

    //        if (animationCurve == null) animationCurve = instance.DefaultAnimationCurve;

    //        ProjectileHelper helper= InitProjectile(projectile, target,Vector3.zero, name, flySpeed, angel, animationCurve, arriveAction,endFunc,true);
    //        helper.timer= timerManagerSystem.AddTimer(() => { SetProjectilePathWayFollow(helper); }, ProjectileDetectionTime, true);
    //        //InitProjectile(helper,name,flySpeed,angel,animationCurve,arriveAction);
    //        return projectile;

    //    }
    //    public void SetProjectilePathWay(ProjectileHelper p)
    //    {
    //        if (p == null || p.owner.gameObject == null) return;
    //        Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos + Vector3.up * 0.7f, p.angel);
    //        float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

    //        p.tween = p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
    //                    //.SetLookAt(0)
    //                    //.SetEase(p.animationCurve)
    //                    .OnComplete(() => {  p.arriveAction?.Invoke(); FinishProjectile(p); });
    //    }
    //    private void SetProjectilePathWayFollow(ProjectileHelper p)
    //    {
    //        if (p == null || p.owner.gameObject == null) return;
    //        if ((p.IsHasTarget&&p.target==null)||p.endFunc())
    //        {
    //            FinishProjectile(p);
    //            return;
    //        }
    //        p.countTime += ProjectileDetectionTime;
    //        if (p.countTime > 10) 
    //        {
    //            FinishProjectile(p);
    //            return;
    //        } 
    //        //检测目标位置是否发生变化
    //        if (p.IsHasTarget)
    //        {
    //            if (p.tween != null && Vector3.Distance(p.targetPos, p.target.position) < 0.1f) return;
    //            p.targetPos = p.target.position;
    //        }
    //        Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos+Vector3.up*0.7f, p.angel);
    //        float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

    //        if (p.tween != null)
    //            p.tween.Kill();
    //        p.tween= p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
    //                    //.SetLookAt(0)
    //                    //.SetEase(p.animationCurve)
    //                    .OnComplete(() => { p.arriveAction?.Invoke();FinishProjectile(p); });
    //    }
    //    void FinishProjectile(ProjectileHelper p)
    //    {
    //        if (p == null) return;

    //        if (p.timer != null)
    //            p.timer.Stop();
    //        if (p.tween != null) p.tween.Kill();
    //        if (p.AutoRecycle)
    //            poolSystem.RecycleToPool<GameObject>(p.owner.gameObject, p.name);
    //        projectileHelpers.Remove(p);

    //        helperPool.RecycleToPool(p);
    //    }
    //}
}
