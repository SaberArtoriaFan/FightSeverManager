using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Saber.ECS
{
    public class BaseWorld : WorldBase
    {


        protected override void InitSystem()
        {
            base.InitSystem();
            //AddSystem<ABManagerSystem>();
            AddSystem<ObjectPoolSystem>();
            AddSystem<EventSystem>();
            //AddSystem<EffectSystem>();
            AddSystem<TimerManagerSystem>();

        }
    }
    public class WorldBase : MonoBehaviour
    {
        [ShowInInspector]
        List<string> systemName=new List<string>();
        Dictionary<object,IMono> systemDict = new Dictionary<object,IMono>();
        Dictionary<Type,IMono> componentSystemDict=new Dictionary<Type, IMono>();
        List<IMono> systemQueue=new List<IMono>();
        Queue<IMono> startQueue=new Queue<IMono>();

        bool isPaused = false;

        public bool IsPaused { get => isPaused; set => isPaused = value; }

        //[SerializeField]
        //Canvas mainCanvas;

        //public Canvas MainCanvas { get => mainCanvas; }
        public void AddSystem<T>()where T: IMono,new()
        {
            if (systemDict.ContainsKey(typeof(T))) { Debug.LogError("Cant add system repeat");return; }
            T t=new T();
            systemDict.Add(typeof(T),t);
            if (t.IsSpwanSystem)
            {
                componentSystemDict.Add(t.ModelType, t);
                //Debug.Log("¥ µ‰"+t.ModelType.ToString());
            }
            systemQueue.Add(t);
            t.Awake(this);
            systemName.Add(t.ToString());
            startQueue.Enqueue(t);
        }
        public void DestorySystem<T>() where T : IMono, new()
        {
            if (!systemDict.ContainsKey(typeof(T))) { Debug.LogWarning("Try Destory dont live system"); }
            IMono t = systemDict[typeof(T)];
            if (componentSystemDict.ContainsKey(t.ModelType))
                componentSystemDict.Remove(t.ModelType);
            systemDict.Remove(typeof(T));
            systemQueue.Remove(t);
            t.OnDestory();
        }
        public  T FindSystem<T>() where T : class,IMono, new()
        {
            Type type = typeof(T);
            if (systemDict.ContainsKey(type)&& systemDict[type] is T) { return (T)systemDict[type]; }
            else
            {
                foreach(var v in systemQueue)
                {
                    if(v is  T)return v as T;
                }
                return null;
            }
        }
        public NormalSystemBase<T> FindSystemByComponent<T>() where T : ComponentBase,new()
        {
            Type type = typeof(T);
            Debug.Log("—∞’“œµÕ≥" + type);
            if(componentSystemDict.ContainsKey(type) && componentSystemDict[type] is NormalSystemBase<T>) { return (NormalSystemBase<T>)componentSystemDict[type]; }
            else return null;
        }
        public NormalSystemBase<T> FindSystemByComponent<T>(T component) where T : ComponentBase, new()
        {
            if (component == null) return null;
            Type type = component.GetType();
            if (componentSystemDict.ContainsKey(type)&& componentSystemDict[type] is NormalSystemBase<T>) { Debug.Log(componentSystemDict[type] + "888"); return (NormalSystemBase<T>)componentSystemDict[type]; }
            else return null;
        }
        public void DesotryComponent<T>(T component) where T : ComponentBase, new()
        {
            if (component == null) return;
            Type type = component.GetType();
            if (componentSystemDict.ContainsKey(type)) { 
                //Debug.Log(componentSystemDict[type] + "888");
                MethodInfo method = componentSystemDict[type].GetType().GetMethod("DestoryComponent", new Type[] {type });
                method?.Invoke(componentSystemDict[type],new object[1] { component });
                //componentSystemDict[type].;
            }
            else return;
        }
        protected virtual void InitSystem()
        {

        }
        public void WorldReset()
        {
            foreach (var item in systemQueue)
            {
                if (item.Enable == true)
                    item.Reset();
            }
        }
        private void Awake()
        {
            InitSystem();
            while(startQueue.Count > 0)
            {
                startQueue.Dequeue().Start();
            }
        }
        protected void LateUpdate()
        {
            if (isPaused) return;
            foreach (var item in systemQueue)
            {
                if(item.Enable==true)
                    item.LateUpdate();
            }
        }
        protected void FixedUpdate()
        {
            if (isPaused) return;
            foreach (var item in systemQueue)
            {
                if (item.Enable == true)
                    item.FixedUpdate();
            }
        }
        public void StartAfterNetwork()
        {
            foreach(var item in systemQueue)
            {
                item.StartAfterNetwork();
            }
        }
        protected void Update()
        {
            if (isPaused) return;
            while (startQueue.Count > 0)
            {
                startQueue.Dequeue().Start();
            }
            foreach (var  item in systemQueue)
            {
                if (item.Enable == true)
                    item.Update();
            }
        }
        private void OnDestroy()
        {
            foreach(var item in systemQueue)
            {
                if (item != null)
                    item.OnDestory();
            }
            systemQueue.Clear();
            systemDict.Clear();
            startQueue.Clear();
            componentSystemDict.Clear();
        }
        private void OnDrawGizmos()
        {
            foreach (var item in systemQueue)
            {
                if (item.Enable == true)
                    item.OnDrawGizmos();
            }
        }
        private void OnDrawGizmosSelected()
        {
            foreach (var item in systemQueue)
            {
                if (item.Enable == true)
                    item.OnDrawGizmosSelected();
            }
        }
    }
}
