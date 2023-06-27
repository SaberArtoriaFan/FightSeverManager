using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    [Serializable]
    public class FSMManager
    {
        //初始化一下所有状态，记录下来
        Dictionary<FSM_State, FSMBase> allStates = new Dictionary<FSM_State, FSMBase>();
        sbyte stateCount = -1;
        FSM_State currentState = FSM_State.max;
        //动画延迟(后摇)
        public float triggerLag = 1;
        //Animator animator;
        float timer = 0;

        public FSM_State CurrentState { get => currentState; private set => currentState = value; }
        public Dictionary<FSM_State, FSMBase> AllState { get => allStates; }
        public FSMBase CurrentFSM_State
        {
            get
            {
               allStates.TryGetValue(currentState, out var v);
                return v;
            }
        }
        public FSMBase FindFSMState(FSM_State fSM_State)
        {
            if (allStates.ContainsKey(fSM_State))
                return allStates[fSM_State];
            else
                return null;
        }

        //public FSMManager(sbyte _count)
        //{
        //    InitializeFSM(_count);
        //}

        //public bool IsTriggerCurrentState()
        //{
        //    if(currentState != -1)
        //        return IsTriggerThisState(currentState);
        //    else
        //        return false;
        //}

        //public bool IsTriggerThisState(sbyte _state_sbyte)
        //{
        //    if(_state_sbyte>=0&&_state_sbyte<stateCount)
        //        return allState[currentState].IsTrigger;
        //    else
        //    {
        //        Debug.LogError("传入参数错误");
        //        return false;
        //    }
        //}

        //private void InitializeFSM(sbyte Count)
        //{
        //    allState = new FSMBase[Count];

        //}
        public void AddState(FSMBase State)
        {
            if (allStates.ContainsKey(State.State))
                return;

            allStates.Add(State.State, State);
        }
        public void RemoveState(FSM_State fSM_State)
        {
            if (!allStates.ContainsKey(fSM_State))
                return;
            allStates.Remove(fSM_State);
        }
        public void ChangeState(FSM_State state)
        {
            //bool isReEnter = CurrentState == state && allStates[state].IsCanReEnter;
            //防止数字越界
            if (allStates.ContainsKey(state)==false)
                return;
            else if (CurrentState == state /*&& !allStates[state].IsCanReEnter*/)
                return;

            if (CurrentState != FSM_State.max && allStates.TryGetValue(CurrentState, out var v))
            {
                v.OnExit();
            }
            CurrentState = state;
            if (allStates.TryGetValue(CurrentState, out var c))
                c.OnEnter();
            else
                Debug.LogError("没找到这个状态" + currentState);
            //if (AllState[CurrentState].IsTrigger)
            //{
                //ChangeState((sbyte)FSM_State.Idle);
            //}
        }
        public void Destory()
        {
            foreach(var f in allStates.Values)
            {
                if(f!=null)
                    f.Destory();
            }
        }
        public void StayState()
        {
            if (CurrentState != FSM_State.max)
            {
                if (allStates.ContainsKey(CurrentState))
                    allStates[CurrentState].OnStay();
                //回到Idle状态

            }
        }

    }

}
