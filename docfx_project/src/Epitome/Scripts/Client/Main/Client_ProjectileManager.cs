using Saber.Base;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia
{
    public class Client_ProjectileManager : Client_SingletonBase<Client_ProjectileManager>
    {

        //float ProjectileDetectionTime = Time.deltaTime;
        Transform parent;
        [SerializeField]
        AnimationCurve defaultAnimationCurve;
        private HashSet<string> poolHashSet = new HashSet<string>();
        Dictionary<GameObject, GameObject> parentAndSonDict = new Dictionary<GameObject, GameObject>();
        public AnimationCurve DefaultAnimationCurve { get => defaultAnimationCurve; }
        public HashSet<string> PoolHashSet { get => poolHashSet; }
        protected override void Start()
        {
            base.Start();
            parent = new GameObject().transform;
            parent.name = "ProjectilesMoel";
            parent.SetParent(this.transform);
        }
        //public override void Start()
        //{
        //    base.Start();
        //    poolSystem = world.FindSystem<ObjectPoolSystem>();
        //    timerManagerSystem = world.FindSystem<TimerManagerSystem>();
        //    helperPool = poolSystem.AddPool<ProjectileHelper>(() => { return new ProjectileHelper(); }, (u) => { u.Reset(); }, null);

        //}
        public void InitPool(GameObject model, string name)
        {
            if (!string.IsNullOrEmpty(name)&&!PoolHashSet.Contains(name))
            {
                PoolHashSet.Add(name);
                if(model!=null)
                    PoolManager.Instance.AddPool<GameObject>(() => { GameObject go = GameObject.Instantiate(model);go.name = name; go.transform.SetParent(parent); return go; }, (u) => { u.SetActive(false); u.transform.SetParent(parent); }, (u) => { u.SetActive(false); }, name);
                else
                {
                    GameObject gob = ABUtility.Load<GameObject>(ABUtility.ProjectMainName + name);
                    if (gob == null) return;
                    PoolManager.Instance.AddPool<GameObject>(() => { GameObject go = GameObject.Instantiate(gob); go.name = name; go.transform.SetParent(parent); return go; }, (u) => { u.SetActive(false); u.transform.SetParent(parent); }, (u) => { u.SetActive(false); }, name);

                }

            }
        }
        public void InitProjectile(GameObject parent, string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            InitPool(null, name);
            if (parentAndSonDict.ContainsKey(parent)) { Debug.LogError("创建投射物出错！！！");parentAndSonDict.Remove(parent); }
            GameObject model = PoolManager.Instance.GetObjectInPool<GameObject>(name);
            if (model == null) {Debug.LogError("CantFindPOOL"); return; }
            model.transform.SetParent(parent.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            model.SetActive(true);
            parentAndSonDict.Add(parent, model);

        }
        public void RecycleProjectile(GameObject parent)
        {
            
            if (!parentAndSonDict.ContainsKey(parent)) return;
            GameObject project = null;
            project = parentAndSonDict[parent];
            parentAndSonDict.Remove(parent);
            if (project == null) { Debug.LogError("CantFindProjectModel");return; }
            PoolManager.Instance.RecycleToPool<GameObject>(project, project.name);
        }
        public void Clear()
        {
            var v = parentAndSonDict.Keys.ToArray();
            foreach(var n in v)
            {
                if (n != null)
                    RecycleProjectile(n);
            }
        }
        ////生产投射物
        ////给定出发点，抵达点，速度，弧度，抵达事件
        //public Transform CreateProjectile(string name, Vector3 birthPos, float scale, Transform target, float flySpeed, float angel, AnimationCurve animationCurve, Action arriveAction, Func<bool> endFunc)
        //{
        //    if (!instance.PoolHashSet.Contains(name))
        //    {
        //        Debug.LogError("未首先初始化目标对象池");
        //        //return null;
        //        instance.PoolHashSet.Add(name);
        //        poolSystem.AddPool<GameObject>(() => { return GameObject.Instantiate(ABUtility.Load<GameObject>(ABUtility.ProjectMainName + name)); }, (u) => { u.SetActive(false); }, (u) => { u.SetActive(true); }, name);
        //    }

        //    Transform projectile = poolSystem.GetObjectInPool<GameObject>(name).transform;
        //    projectile.transform.position = birthPos;
        //    if (animationCurve == null) animationCurve = instance.DefaultAnimationCurve;

        //    ProjectileHelper helper = InitProjectile(projectile, target, Vector3.zero, name, flySpeed, angel, animationCurve, arriveAction, endFunc, true);
        //    helper.timer = timerManagerSystem.AddTimer(() => { SetProjectilePathWayFollow(helper); }, ProjectileDetectionTime, true);
        //    //InitProjectile(helper,name,flySpeed,angel,animationCurve,arriveAction);
        //    return projectile;

        //}
        //public void SetProjectilePathWay(ProjectileHelper p)
        //{
        //    if (p == null || p.owner.gameObject == null) return;
        //    Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos + Vector3.up * 0.7f, p.angel);
        //    float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

        //    p.tween = p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
        //                //.SetLookAt(0)
        //                //.SetEase(p.animationCurve)
        //                .OnComplete(() => { p.arriveAction?.Invoke(); FinishProjectile(p); });
        //}
        //private void SetProjectilePathWayFollow(ProjectileHelper p)
        //{
        //    if (p == null || p.owner.gameObject == null) return;
        //    if ((p.IsHasTarget && p.target == null) || p.endFunc())
        //    {
        //        FinishProjectile(p);
        //        return;
        //    }
        //    p.countTime += ProjectileDetectionTime;
        //    if (p.countTime > 10)
        //    {
        //        FinishProjectile(p);
        //        return;
        //    }
        //    //检测目标位置是否发生变化
        //    if (p.IsHasTarget)
        //    {
        //        if (p.tween != null && Vector3.Distance(p.targetPos, p.target.position) < 0.1f) return;
        //        p.targetPos = p.target.position;
        //    }
        //    Vector3[] path = Saber.Base.BaseUtility.CalculatePathCrossTwoPoints2D(p.owner.transform.position, p.targetPos + Vector3.up * 0.7f, p.angel);
        //    float timer = Vector3.Distance(path[0], path[path.Length - 1]) / p.flySpeed;

        //    if (p.tween != null)
        //        p.tween.Kill();
        //    p.tween = p.owner.transform.DOPath(path, timer, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
        //                //.SetLookAt(0)
        //                //.SetEase(p.animationCurve)
        //                .OnComplete(() => { p.arriveAction?.Invoke(); FinishProjectile(p); });
        //}
        //void FinishProjectile(ProjectileHelper p)
        //{
        //    if (p == null) return;

        //    if (p.timer != null)
        //        p.timer.Stop();
        //    if (p.tween != null) p.tween.Kill();
        //    if (p.AutoRecycle)
        //        poolSystem.RecycleToPool<GameObject>(p.owner.gameObject, p.name);
        //    projectileHelpers.Remove(p);

        //    helperPool.RecycleToPool(p);
        //}
    }
}
