using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

public enum FSM_State { idle, walk,run, attack, damaged,spell,interaction, tied, death, max }
public interface IFSM
{
    FSM_State CurrentState { get; }
    void SetCurrentState(FSM_State state);
    Animator Animator { get; }
    void AddState(FSMBase fSMBase);
    void RemoveState(FSM_State fSM_State);
    FSMBase FindFSMState(FSM_State fsmState);
}
//[RequireComponent(typeof(ChessBuilder))]
public class CharacterFSM :MonoBehaviour,IFSM
{
    public bool isCanMove { get
        {
            return currentState == FSM_State.idle || currentState == FSM_State.walk || currentState == FSM_State.run;
        } }
    private bool isInit=false;

    //UnitBase unit;
    Animator animator;
    FSMManager FSM;
    Idle idle;
    Dictionary<FSM_State, AnimationClip> animationClipDict;
    Action<float> SendXAction;
   
    //Walk walk;
    //Run run;
    ////Tied tied;
    //Spell spell;
    //private Attack attack;
    //private Damaged damaged;
    ////private Interaction interaction;
    //private Death death;

    ////根据每个不同单位设置他们的攻击时间

    //public Attack Attack { get => attack;}
    //public Damaged Damaged { get => damaged; }
    //public Death Death { get => death; }
    ////public Interaction Interaction { get => interaction;}
    //public Spell Spell { get => spell; }


    // Start is called before the first frame update
    #region 供外界调用获得输入的X与Y
    public void SendXY(float _x,float _y)
    {
        animator.SetFloat("InputX", _x);
        animator.SetFloat("InputY", _y);
    }
    public void SendX(float _x)
    {
        //_x *= -1;
        //SendXAction(_x);
        //animator.SetFloat("InputX", _x);
        if (_x > 0)
            SendXAction(0);
        //transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        else
            SendXAction(180);
        //transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

        //if (_x == 1) _x = 0;
        //else _x = 1;
        //int y = _x == 1 ?  -1 : 1;
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
    //[Command]
    //private void CmdSetCurrentState(FSM_State state)
    //{
    //    if (!isInit)
    //    {
    //        return;
    //    }
    //    FSM.ChangeState((sbyte)state);
    //    //Debug.Log(state + "0425");
    //    if ((sbyte)currentState != FSM.CurrentState)
    //    {
    //        //Debug.Log(currentState + "0425");
    //        currentState = (FSM_State)FSM.CurrentState;

    //        FSM_StateChange?.Invoke(unit);
    //    }
    //}
    //[TargetRpc]
    //private void TRSetCurrentState(NetworkConnection conn, sbyte state)
    //{
    //    FSM.ChangeState(state);
    //}
    protected virtual void Awake()
    {

        //unit=GetComponent<UnitBase>();
        Init();

    }
    public void SetSendXAction(Action<float> action)
    {
        this.SendXAction = action;
    }
    public void ChangeAnimator(Animator animator)
    {
        if (animator == null) return;
        this.animator = animator;


    }
    public void ChangeClothes()
    {
        animationClipDict.Clear();
        foreach (var v in animator.runtimeAnimatorController.animationClips)
        {
            if (v.name.Contains("Attack"))
                animationClipDict.Add(FSM_State.attack, v);
            else if (v.name.Contains("Spell"))
                animationClipDict.Add(FSM_State.spell, v);
            else if (v.name.Contains("Dead"))
                animationClipDict.Add(FSM_State.death, v);

            //Debug.Log(v.name+"animator,"+v.length);

        }
        foreach (var v in FSM.AllState.Values)
        {
            if (v != null)
            {
                v.Animator = animator;
                if (animationClipDict.ContainsKey(v.State) && v is TriggerFSM t)
                {
                    t.SetFinishTime(animationClipDict[v.State].length - 0.05f);
                }
            }
        }
    }
    public void ChangeTriggerFSMContinue(FSM_State fSM_State,float timelong)
    {
        if (timelong > 0 && FSM.AllState.TryGetValue(fSM_State, out var value) && value is TriggerFSM t)
            t.SetFinishTime(timelong + 0.05f);
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
        animationClipDict = new Dictionary<FSM_State, AnimationClip>();
        //animator=GetComponentInChildren<Animator>();

        FSM = new FSMManager();
        idle = new Idle(animator);

        ChangeAnimator(GetComponentInChildren<Animator>());


        FSM.AddState(idle);
        FSM.ChangeState(FSM_State.idle);

        /*
        damaged = new Damaged(animator, Damaged.const_damagedFinishTime, Damaged.const_damagedTime,()=> PlayerActionManager.DamagedChess(chess));
        FSM.AddState(damaged);
        death = new Death(animator, 0.6f);
        FSM.AddState(Death);

        if (GetComponent<IMoveChess>() != null)
        {
            walk = new Walk(animator);
            run = new Run(animator);
            FSM.AddState(walk);
            FSM.AddState(run);
        }
        IAttackChess attackable = GetComponent<IAttackChess>();
        if (attackable != null)
        {
            attack = new Attack(animator, attackable.AttackAnimationLong, attackable.AttackAniamtionTime, () => PlayerActionManager.HurtChess(attackable));
            FSM.AddState(Attack);
        }
        //damaged = new Damaged(animator,0.2f);
        ISpellChess spellChess = GetComponent<ISpellChess>();
        if (spellChess != null)
        {
            spell = new Spell(animator, 2f);
            FSM.AddState(spell);
        }

        //interaction= new Interaction(animator);
        //tied = new Tied(animator);
        //deathTime = transform.GetComponent<UnitControlBase>().data.deathTime;

        //FSM.AddState(Damaged);
        //FSM.AddState(Interaction);
        //FSM.AddState(tied);
        */
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
            if(FSM.CurrentFSM_State==null)
            {
                SetCurrentState(FSM_State.idle);
                return;
            }
            if (FSM.CurrentFSM_State.IsTrigger)
            {
                timer += Time.deltaTime;
                if (timer > ((TriggerFSM)FSM.CurrentFSM_State).FinishTime)
                {
                    //Debug.Log(((TriggerFSM)FSM.CurrentFSM_State).Finish_Time);
                    //Debug.Log(FSM.CurrentFSM_State.GetType().Name + "状态结束");
                    ((TriggerFSM)FSM.CurrentFSM_State).isFinish = true;
                    SetCurrentState(FSM_State.idle);
                    timer = 0;
                }
            }
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
        this.SendXAction = null;
    }

    public void AddState(FSMBase fSMBase)
    {

        //if (animationClipDict.ContainsKey(fSMBase.State) && fSMBase is TriggerFSM t)
        //    t.SetFinishTime(animationClipDict[fSMBase.State].length - 0.05f);
    
        FSM.AddState(fSMBase);
    }

    public void RemoveState(FSM_State fSM_State)
    {
        if (FSM == null) return;
        FSM.RemoveState(fSM_State);
    }
}

