using System;
using UnityEngine;
namespace FSM
{
    public static class AnimatorParameters
    {
        public static readonly int Walk = Animator.StringToHash("walk");
        public static readonly int Attack = Animator.StringToHash("attack");
        public static readonly int Death = Animator.StringToHash("death");
        public static readonly int Spell = Animator.StringToHash("spell");
        public static readonly int Damaged = Animator.StringToHash("Damaged");
        public static readonly int Idle = Animator.StringToHash("idle");
        public static readonly int FinishAttack = Animator.StringToHash("FinishAttack");
        public static readonly int Reset = Animator.StringToHash("Reset");
        


    }
    public abstract class FSMBase
    {
        protected Animator animator;

        private bool isTrigger=false;
        public abstract FSM_State State { get; }
       public bool IsTrigger { get => isTrigger;protected set => isTrigger = value; }

        public virtual bool IsCanReEnter { get; protected set; }=false;
        public Animator Animator { get => animator; set => animator = value; }

        //float ActIndex;
        protected FSMBase(Animator animator)
        {
            this.animator = animator;
            //animator.
            //animator.runtimeAnimatorController.
        }
        protected virtual void SetAnimatorParameter(bool isEnter)
        {

        }
        public virtual void OnEnter()
        {
            SetAnimatorParameter(true);
        }
        public virtual void OnStay()
        {

        }
        public virtual void OnExit()
        {
            SetAnimatorParameter(false);
        }
        public virtual void Destory()
        {

        }
    }
    public abstract class TriggerFSM:FSMBase
    {
        protected float finish_Time;
        public float FinishTime { get => finish_Time;   }
        public bool isFinish = false;
        public virtual void SetFinishTime(float v)
        {
            finish_Time = v;
        }

        public TriggerFSM(Animator animator) : base(animator)
        {
            IsTrigger = true;
            finish_Time = 1;
        }

    }
    public class Idle : FSMBase
    {
        public override FSM_State State => FSM_State.idle;

        public Idle(Animator animator) : base(animator)
        {
        }

        public override void OnEnter()
        {
            //animator.SetInteger("Action", 0);
        }
        public override void OnStay()
        {

        }
        public override void OnExit()
        {
        }
    }
    public interface IWalk
    {
        float WalkSpeed { get; }
    }
    public class Walk : FSMBase
    {
        public override FSM_State State => FSM_State.walk;
        IWalk walk;

        //private float WalkSpeed => animator.GetComponentInParent<UnitBase>().SyncData.MoveSpeed;
        public Walk(Animator animator,IWalk walk) : base(animator)
        {
            this.walk = walk;
        }

        protected override void SetAnimatorParameter(bool isEnter)
        {
            animator.SetBool(AnimatorParameters.Walk, isEnter);
        }
        public override void OnEnter()
        {
            //animator.speed = WalkSpeed;
            animator.speed = 1;
            base.OnEnter();
            //rigid.bodyType = RigidbodyType2D.Dynamic;
        }
        public override void OnStay()
        {
            animator.speed = walk != null ? walk.WalkSpeed : 1;
        }
        public override void OnExit()
        {
            animator.speed = 1;
            base.OnExit();
            //rigid.bodyType = RigidbodyType2D.Static;
        }
    }
    public class Run : FSMBase
    {
        public override FSM_State State => FSM_State.walk;

        public Run(Animator animator) : base(animator)
        {
        }

        public override void OnEnter()
        {
            animator.SetInteger("Action", 2);
        }
        public override void OnStay()
        {
        }
        public override void OnExit()
        {
            animator.SetInteger("Action", 0);
        }
    }
    public class Attack : TriggerFSM
    {
        //AnimationState attackAnimation;
        float speed = 1;
        public Attack(Animator animator,float attackTime,Action attackEvent) : base(animator)
        {
            this.attackTime = attackTime;
            this.AttackEvent += attackEvent;
            //origin_FinishTime = _finish_Time;
            this.origin_AttackTime = this.attackTime;
            //attackAnimation=animator.
            IsCanReEnter = true;
        }
        public event Action AttackEvent;
        float attackTime;
        float origin_AttackTime;
        float origin_FinishTime;
        float timer = 0;
        bool isEnd = false;
        bool isSet = false;
        public override FSM_State State => FSM_State.attack;

        public bool IsEnd { get => isEnd; }
        public void SetSpeed(float value)
        {
            //animator.speed = value;
            speed = value;
        }
        public override void SetFinishTime(float v)
        {
            base.SetFinishTime(v);
            origin_FinishTime = v;
            if (origin_AttackTime >= v)
            {
                Debug.LogError($"设置原始攻击时间出错{animator.gameObject.name}，{origin_AttackTime}大于{v}");
                origin_AttackTime = Mathf.Abs(v - 0.05f);
            }
        }
        void SetAttackAnimationParameter()
        {
            this.attackTime = this.origin_AttackTime * (1 / speed);
            this.finish_Time = this.origin_FinishTime * (1 / speed);
        }
        //bool isFinish = false;
        //float attackLong;
        public override void OnEnter()
        {
            animator.speed = speed;
            timer = 0;
            isEnd = false;
            SetAttackAnimationParameter();
            
            isSet = false;
            SetAnimatorParameter(true);

            //animator.SetBool(AnimatorParameters.Attack, true) ;
            //animator.SetBool(AnimatorParameters.FinishAttack, true);
        }
        protected override void SetAnimatorParameter(bool isEnter)
        {
            if (isEnter)
            {
                animator.ResetTrigger(AnimatorParameters.Attack);
                animator.SetTrigger(AnimatorParameters.Attack);
            }
            else
            {

            }

        }
        public override void OnStay()
        {
            if (isEnd) return;
            //if ( !isSet && timer>Time.deltaTime) { isSet = true;}
            timer += Time.deltaTime;
            if (timer > attackTime)
            {
                //animator.SetBool(AnimatorParameters.FinishAttack, true);
                //animator.SetBool(AnimatorParameters.Attack, false);
                isEnd = true;
                AttackEvent?.Invoke();
            }
        }
 
        public override void OnExit()
        {
            //animator.SetBool(AnimatorParameters.Attack, false);
            SetAnimatorParameter(false);
            timer = 0;
            isEnd = false;
            isSet = true;
            animator.speed = 1;
            speed = 1;
            attackTime = origin_AttackTime;
            finish_Time = origin_FinishTime;
        }
        public override void Destory()
        {
            base.Destory();
            AttackEvent = null;
        }
    }
    public class Damaged : TriggerFSM
    {
        public const float const_damagedTime = 0.375f;
        public const float const_damagedFinishTime = 0.675f;
        public override FSM_State State => FSM_State.damaged;
        float damagedTime;
        float timer;
        public Damaged(Animator animator, Action damagedEvent, float _finish_Time=const_damagedFinishTime, float damagedTime=const_damagedTime) : base(animator)
        {
            if (damagedTime >= _finish_Time) Debug.LogError("受伤时间设置错误！！！");
            this.damagedTime = damagedTime;
            OnDamaged += damagedEvent;
        }

        public event Action OnDamaged;

        public override void OnEnter()
        {
            //Debug.Log("受伤了捏");
            animator.SetTrigger(AnimatorParameters.Damaged);
            timer = 0;
        }
        public override void OnStay()
        {
            if(timer>=0)
            {
                timer += Time.deltaTime;
                if(timer>=damagedTime)
                {
                    timer = -1;
                    OnDamaged?.Invoke();
                }
            }

        }
        public override void OnExit()
        {
            timer = 0;
            animator.ResetTrigger(AnimatorParameters.Damaged);

            //animator.SetBool("IsDamaging", false);
        }
        public override void Destory()
        {
            OnDamaged = null;
        }
    }
    //public class Death : TriggerFSM
    //{

    //    public Death(Animator animator, Action deathAction, float _finish_Time) : base(animator, _finish_Time)
    //    {
    //        OnDeath += deathAction;

    //    }

    //    public event Action OnDeath;

    //    public override FSM_State State => FSM_State.death;

    //    public override void OnEnter()
    //    {
    //        //Debug.Log("death");
    //        animator.SetBool(AnimatorParameters.Death, true);
    //    }
    //    public override void OnExit()
    //    {
    //        OnDeath?.Invoke();
    //        //animator.SetBool(AnimatorParameters.Death, false);
    //    }
    //    public override void Destory()
    //    {
    //        OnDeath = null;
    //    }
    //}
    public class Death : FSMBase
    {
        float animationTime;
        public Death(Animator animator,float animationTime) : base(animator)
        {
            this.animationTime = animationTime;
        }


        public override FSM_State State => FSM_State.death;

        public float AnimationTime { get => animationTime; }

        public override void OnEnter()
        {
            //Debug.Log("death");
            animator.speed = 1;
            animator.SetBool(AnimatorParameters.Death, true);
        }
        public override void OnExit()
        {
            animator.SetBool(AnimatorParameters.Death, false);
            //animator.SetBool(AnimatorParameters.Death, false);
        }
        public override void Destory()
        {

        }
    }
    //ʾ����ί�����¼��Ĺ淶д��
    //public class DelegateAndEvent
    //{
    //    ChessBuilder unit;
    //    public delegate void InteractEventHandler(object _sender, InteractEventArgs _e);
    //    public class InteractEventArgs : EventArgs
    //    {
    //        public readonly ChessBuilder unit;
    //        public InteractEventArgs(ChessBuilder _unit)
    //        {
    //            unit = _unit;
    //        }
    //    }

    //    public event InteractEventHandler Interact;

    //    private void OnInteract(InteractEventArgs e)
    //    {
    //        if (Interact != null)
    //        {
    //            Interact(this, e);
    //        }
    //    }
    //    public void Interacting()
    //    {
    //        InteractEventArgs e =new InteractEventArgs(unit);
    //        OnInteract(e);
    //    }
    //}



    //public delegate void InteractEventHandler(ChessBuilder _unit);

    //public class Interaction : FSMBase
    //{
    //    public bool isZazen=false;
    //    public event InteractEventHandler Interact;
    //    private ChessBuilder unit;
    //    private float timer=0;

    //    public override FSM_State State => FSM_State.interaction;

    //    public Interaction(Animator animator) : base(animator)
    //    {
    //        unit = animator.GetComponentInParent<ChessBuilder>();
    //    }
    //    public override void OnEnter()
    //    {
    //        base.OnEnter();

    //        //���Ž�������
    //        //Ӧ����Boolֵ���ƣ�������ѭ��
    //        //ע���Ⱥ�˳��
    //        animator.SetBool("IsZazen", isZazen);
    //        animator.SetBool("IsInteraction", true);
    //        if(isZazen)isZazen=false;

    //    }

    //    public override void OnStay()
    //    {
    //        base.OnStay();
    //        timer += Time.deltaTime;
    //        //ÿ��ִ��
    //        if (timer >= 1)
    //        {
    //            timer = 0;
    //            if (Interact != null)
    //                Interact(unit);
    //        }
    //        //����ǳ����Խ����Զ������Ϳ��԰����Ӱٷֱ�֮����¼�
    //    }

    //    public override void OnExit()
    //    {
    //        base.OnExit();
    //        //ÿ�ν������������¼����
    //        animator.SetBool("IsInteraction", false);
    //        Interact = null;
    //    }
    //    public override void Destory()
    //    {
    //        Interact = null;
    //    }

    //}
    //public class Tied : FSMBase
    //{
    //    public Tied(Animator animator) : base(animator)
    //    {
    //    }
    //    public override void OnEnter()
    //    {
    //        base.OnEnter();
    //        animator.SetBool("IsTied", true);
    //    }
    //    public override void OnStay()
    //    {
    //        base.OnStay();
    //    }
    //    public override void OnExit()
    //    {
    //        base.OnExit();
    //        animator.SetBool("IsTied", false);
    //    }
    //}
    public class Spell : TriggerFSM
    {
        //public const float SpellFinishTime= 1f;
        public override FSM_State State => FSM_State.spell;

        public event Action OnSpell;
        public Spell(Animator animator) : base(animator)
        {

        }
        protected override void SetAnimatorParameter(bool isEnter)
        {
            if(isEnter)
            {
                animator.ResetTrigger(AnimatorParameters.Spell);
                animator.SetTrigger(AnimatorParameters.Spell);
            }
        }
        public override void OnEnter()
        {
            animator.speed = 1;
            base.OnEnter();

            //animator.SetBool(AnimatorParameters.Spell,true);
        }
        public override void OnExit()
        {
            animator.speed = 1;
            base.OnExit();
            OnSpell?.Invoke();
            //animator.SetBool(AnimatorParameters.Spell, false);
        }
        public override void Destory()
        {
            OnSpell = null;
        }
    }
}
