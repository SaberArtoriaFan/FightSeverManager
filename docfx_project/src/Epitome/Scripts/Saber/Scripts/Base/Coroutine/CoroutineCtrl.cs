using System;
using System.Collections;
using UnityEngine;
//脚本作者:Saber

namespace Saber.Base
{
    public class CoroutineCtrl
    {
        private static int _id=0;
        public int ID { get; private set; }
        private CoroutineItem _item;
        private MonoBehaviour _mono;
        private IEnumerator _routine;
        private Coroutine _coroutine;
        bool isCanPaused = true;
        //public CoroutineCtrl(MonoBehaviour mono, IEnumerator routine,Action finishAction)
        //{
        //    Init(mono, routine, finishAction);
        //}
        public void Init(CoroutineMgr mono, IEnumerator routine, Action finishAction,string iecoroutineName, bool isCanPaused = true)
        {
            _item = mono.CiPool.GetObjectInPool();
            _item.coroutineName = iecoroutineName;
            _item.gameObject.SetActive(true);
            _item.StopAllCoroutines();
            _item.FinishEvent += ()=>mono.CiPool.RecycleToPool(_item);
            _item.FinishEvent += finishAction;
            _item.State = CoroutineState.Running;
            _mono = mono;
            _routine = routine;
            this.isCanPaused = isCanPaused;
            ResetData();
        }

        public CoroutineState State => _item.State;

        public bool IsCanPaused { get => isCanPaused; }

        public void Start()
        {
            _coroutine = _mono.StartCoroutine(_item.Body(_routine));
        }

        public void Pause()
        {
            _item.State =CoroutineState.Pasued;
        }

        public void Stop()
        {
            _item.State = CoroutineState.Stop;
        }

        public void Continue()
        {
            _item.State = CoroutineState.Running;
        }

        public void ResetStart()
        {
            if (_coroutine != null)
            {
                _mono.StopCoroutine(_coroutine);
            }
            Start();
        }

        private void ResetData()
        {
            ID = _id++;
        }
    }
}
