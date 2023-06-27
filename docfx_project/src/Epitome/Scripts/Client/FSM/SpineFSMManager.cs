using FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public class SpineFSMManager : MonoBehaviour
    {
        public bool isCanMove
        {
            get
            {
                return currentState == FSM_State.idle || currentState == FSM_State.walk || currentState == FSM_State.run;
            }
        }
        private bool isInit = false;

        //UnitBase unit;
        Animator animator;
        XianXiaSkeletonAnimationHandle animationHandle;
        FSMManager FSM;
        SpineIdle idle;

        #region 供外界调用获得输入的X与Y
        public void SendXY(float _x, float _y)
        {
            animator.SetFloat("InputX", _x);
            animator.SetFloat("InputY", _y);
        }
        public void SendX(float _x)
        {
            _x *= -1;
            animationHandle.SetFlip(_x);
        }
        public void SendY(float _y)
        {
            animator.SetFloat("InputY", _y);
        }
        #endregion
        public event Action FSM_StateChange;

        [SerializeField]
        private FSM_State currentState;
        public FSM_State CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public Animator Animator => animator;

        //public void SetCurrentState(FSM_State state)

        //{
        //    if(NetworkServer.)
        //}
        //public void ClientSetCurrentState(FSM_State state)
        //{
        //    if (!isInit)
        //    {
        //        return;
        //    }
        //    FSM.ChangeState((sbyte)state);
        //    //CmdSetCurrentState(state);
        //}
        public FSMBase FindFSMState(FSM_State fSM_State)
        {
            return FSM.FindFSMState(fSM_State);
        }
        public virtual void SetCurrentState(FSM_State state)
        {
            if (!isInit)
            {
                return;
            }
            //TRSetCurrentState(connectionToClient, (sbyte)state);
            FSM.ChangeState(state);
            //Debug.Log(state + "0425");
            if (currentState != FSM.CurrentState)
            {
                //Debug.Log(currentState + "0425");
                currentState = FSM.CurrentState;

                FSM_StateChange?.Invoke();
            }
        }
        protected virtual void Awake()
        {

            //unit=GetComponent<UnitBase>();
            Init();

        }
        public void ChangeAnimator(Animator animator)
        {
            if (animator == null) return;
            this.animator = animator;
            this.animationHandle = animator.GetComponent<XianXiaSkeletonAnimationHandle>();

        }

        /// <summary>
        /// 初始化，之后为不同的行动初始化不同的状态机
        /// </summary>
        //[Server]
        public virtual void Init()
        {
            //if (isClientOnly) return;
            //ChessBuilder chess = GetComponent<ChessBuilder>();
            //if (chess == null) { Debug.LogError("未挂载IChess");return; };
            isInit = true;
            //animator=GetComponentInChildren<Animator>();

            FSM = new FSMManager();

            ChangeAnimator(GetComponentInChildren<Animator>());
            idle = new SpineIdle(animator, animationHandle);


            FSM.AddState(idle);
            FSM.ChangeState(FSM_State.idle);
        }
        float timer = 0;
        //[ServerCallback]
        protected virtual void Update()
        {
            UpdateFsm();
        }
        public virtual void UpdateFsm()
        {
            if (isInit)
            {
                FSM.StayState();
            }
        }
        //private void FixedUpdate()
        //{
        //    if(isInit)
        //        FSM.CheckTriggerStateFinish();
        //}
        //[ServerCallback]
        public virtual void OnDestroy()
        {
            FSM.Destory();
            FSM_StateChange = null;
        }

        public void AddState(FSMBase fSMBase)
        {
            FSM.AddState(fSMBase);
        }

        public void RemoveState(FSM_State fSM_State)
        {
            if (FSM == null) return;
            FSM.RemoveState(fSM_State);
        }
    }
}
