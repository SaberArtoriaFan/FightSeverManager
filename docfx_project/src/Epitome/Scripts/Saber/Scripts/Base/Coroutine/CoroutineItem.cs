using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//脚本作者:Saber

namespace Saber.Base
{
    public enum CoroutineState
    {
        Waitting,
        Running,
        Pasued,
        Stop
    }
    public class CoroutineItem : MonoBehaviour
    {
        public event Action FinishEvent;
        public string coroutineName;
        [SerializeField]
        private CoroutineState state;
        public CoroutineState State { get=>state; set=>state=value; }
        public IEnumerator Body(IEnumerator routine)
        {
            while (State != CoroutineState.Stop)
            {
                while (State == CoroutineState.Waitting||State==CoroutineState.Pasued)
                {
                    yield return null;
                }
                while (State == CoroutineState.Running)
                {
                    if (routine != null && routine.MoveNext())
                    {
                        yield return routine.Current;
                    }
                    else
                    {
                        State = CoroutineState.Stop;
                        FinishEvent?.Invoke();
                    }
                }
            }

            //Debug.Log("辅助协程结束啦!");
        }
        private void OnDisable()
        {
            FinishEvent = null;
        }
    }
}
