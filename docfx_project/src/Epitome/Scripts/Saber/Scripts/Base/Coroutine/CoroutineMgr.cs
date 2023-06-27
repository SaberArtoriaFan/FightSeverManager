using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
using System;
//脚本作者:Saber

namespace Saber.Base
{
    public class CoroutineMgr : AutoSingleton<CoroutineMgr>
    {
        private Dictionary<int, CoroutineCtrl> _ctrlDic;
        Dictionary<CoroutineCtrl, CoroutineState> _ctrlStateDic;
        ObjectPool<CoroutineCtrl> ccPool;
        ObjectPool<CoroutineItem> ciPool;
        public bool isPaused=false;
        public ObjectPool<CoroutineItem> CiPool { get => ciPool; }

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
            _ctrlDic = new Dictionary<int, CoroutineCtrl>();
            _ctrlStateDic = new Dictionary<CoroutineCtrl, CoroutineState>();    
            ccPool = new ObjectPool<CoroutineCtrl>(()=>{return new CoroutineCtrl();}, (c) => { c.Stop(); },null);
            ciPool = new ObjectPool<CoroutineItem>(() => {
                GameObject go = new GameObject();
                go.transform.SetParent(transform);
                return go.AddComponent<CoroutineItem>(); }, (c) => { c.gameObject.SetActive(false); },null);
        }

        public void PauseAllCoroutine()
        {
            //if (isPaused) return;
            ContinueAllCoroutine();
            foreach (var c in _ctrlDic.Values)
            {
                if (c.IsCanPaused&&c.State != CoroutineState.Stop)
                {
                    _ctrlStateDic.Add(c, c.State);
                    c.Pause();
                }
            }
            isPaused = true;
        }
        public void ContinueAllCoroutine()
        {
            foreach(var c in _ctrlStateDic)
            {
                if (c.Key!=null&&c.Value == CoroutineState.Running)
                    c.Key.Continue();
            }
            _ctrlStateDic.Clear();
            isPaused = false;
        }
        public int Execute(IEnumerator routine, Action finishAction,string name,bool autoStart = true,bool isCanPaused=true)
        {
            CoroutineCtrl ctrl = ccPool.GetObjectInPool();
            ctrl.Init(this, routine, ()=> { finishAction?.Invoke(); Finish(ctrl.ID); },name,isCanPaused);
            _ctrlDic.Add(ctrl.ID, ctrl);
            if (!autoStart)
            {
                PauseExecute(ctrl.ID);
            }else if (isPaused)
            {
                if (!_ctrlStateDic.ContainsKey(ctrl))
                {
                    _ctrlStateDic.Add(ctrl, CoroutineState.Running);
                }
                PauseExecute(ctrl.ID);
            }
            StartExecute(ctrl.ID);

            return ctrl.ID;
        }
        //public void ExecuteOnce(IEnumerator routine, Action finishAction)
        //{
        //    CoroutineCtrl ctrl =ccPool.GetObjectInPool();
        //    ctrl.Init(this, routine, () => { finishAction(); Finish(ctrl); });
        //    ctrl.Start();
        //}

        public void StartExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                ctrl.Start();
            }

        }
        public void ContinueExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                ctrl.Continue();
            }
        }
        public void PauseExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                ctrl.Pause();
            }
        }
        public void StopExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                ctrl.Stop();
            }
        }
        public void RestartExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                ctrl.ResetStart();
            }
        }
        public CoroutineState GetStateExecute(int id)
        {
            var ctrl = GetCtrl(id);
            if (ctrl != null)
            {
                return ctrl.State;
            }else
                return CoroutineState.Stop;
        }
        private void Finish(int id)
        {
            if (_ctrlDic.ContainsKey(id))
            {
                ccPool.RecycleToPool(_ctrlDic[id]);
                _ctrlDic.Remove(id);
            }
        }
        private void Finish(CoroutineCtrl cc)
        {
            ccPool.RecycleToPool(cc);
        }
        private CoroutineCtrl GetCtrl(int id)
        {
            if (_ctrlDic.ContainsKey(id))
            {
                return _ctrlDic[id];
            }
            else
            {
                Debug.LogWarning($"当前id不存在,id:{id}");
                return null;
            }
        }
    }
}
