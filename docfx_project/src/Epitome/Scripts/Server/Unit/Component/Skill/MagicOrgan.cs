using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using FSM;
using FishNet;

namespace XianXia.Unit
{
    internal interface IMagicPointRecover 
    {
        int MagicPoint_Max { get; set; }
        int MagicPoint_Curr { get;  set; }
        int MagicPoint_Attack { get ; set; }
        int MagicPoint_Damaged { get; set; }
        int NextMagicPoint { get; set; }
        bool CanRecordMagicPoint { get; set; }
    }
    public class Spine_Spell : Spell
    {
        public int hashID;
        Client_UnitProperty unitProperty;

        public Spine_Spell(Animator animator,int hashID) : base(animator)
        {
            unitProperty = animator.GetComponentInParent<Client_UnitProperty>();
            this.hashID = hashID;
        }
        public override void SetFinishTime(float v)
        {
            float res = InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().GetAnimationLong(hashID, FSM.AnimatorParameters.Spell);
            //Debug.Log(hashID+"读取到的动画时长为"+ FSM.AnimatorParameters.Attack+"_" + res);
            if (res <= 0) { res = v; Debug.LogError("没读取到这个动画的结束时长"); }
            Debug.Log("施法时间更改为" + v);
            base.SetFinishTime(res);
        }
        protected override void SetAnimatorParameter(bool isEnter)
        {
            animator.SetBool(FSM.AnimatorParameters.Spell, isEnter);
            //Debug.Log("设值动画参数"+animator.GetBool(FSM.AnimatorParameters.Attack));
            //InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Bool(unitProperty, FSM.AnimatorParameters.Attack, isEnter);
            //unitProperty.ORPC_AnimatorParameter_Bool(FSM.AnimatorParameters.Attack, isEnter);
        }
    }
    public class MagicOrgan : StatusOrganBase<ActiveSkill>,IMagicPointRecover
    {
        protected override ComponentType componentType =>ComponentType.magic;
        int magicPoint_Max=0;
        int magicPoint_Curr=0;
        int magicPoint_Attack=0;
        int magicPoint_Damaged=0;
        int nextMagicPoint = 0;
        bool canRecordMagicPoint = true;
        CharacterFSM characterFSM;
        float animationLong = 2.0f;
        public int MagicPoint_Max { get => magicPoint_Max;  }
        public int MagicPoint_Curr { get => magicPoint_Curr; }
        public int MagicPoint_Attack { get => magicPoint_Attack; }
        public int MagicPoint_Damaged { get => magicPoint_Damaged; }
        public bool ReadyToSpell=>HasSkill&&magicPoint_Curr>=MagicPoint_Max;

        public bool HasSkill => StatusList.Count > 0;

        int IMagicPointRecover.MagicPoint_Max { get => magicPoint_Max; set => magicPoint_Max=value; }
        int IMagicPointRecover.MagicPoint_Curr { get => magicPoint_Curr; set { if (value==0||canRecordMagicPoint) magicPoint_Curr = value; else return; } }
        int IMagicPointRecover.MagicPoint_Attack { get => magicPoint_Attack; set => magicPoint_Attack=value; }
        int IMagicPointRecover.MagicPoint_Damaged { get => magicPoint_Damaged; set => magicPoint_Damaged=value; }
        public CharacterFSM CharacterFSM { get=>characterFSM; internal set=>characterFSM=value; }
        public int NextMagicPoint { get => nextMagicPoint; }
        int IMagicPointRecover.NextMagicPoint { get => nextMagicPoint; set => nextMagicPoint=value; }
        bool IMagicPointRecover.CanRecordMagicPoint { get => canRecordMagicPoint; set {
                canRecordMagicPoint = value;
            }  }
        
        public float AnimationLong { get => animationLong; }

        public string GetActiveSkillName()
        {
            if (StatusList.Count > 0)
                return StatusList[0].SkillId.ToString();
            else
                return string.Empty;
        }
        public void SetAnimationFinishTime(int hash)
        {
            Spine_Spell spell = (characterFSM.FindFSMState(FSM_State.spell) as Spine_Spell);
            if (spell != null)
            {
                spell.hashID =hash;
                spell.SetFinishTime(spell.FinishTime);
            }

        }
        protected override void InitComponent(EntityBase owner)
        {
            base.InitComponent(owner);
            characterFSM = owner.GetComponent<CharacterFSM>();
   

        }
        public override void Destory()
        {
            base.Destory();
            magicPoint_Max = 0;
            magicPoint_Curr = 0;
            magicPoint_Attack = 0;
            magicPoint_Damaged = 0;
            nextMagicPoint = 0;
            canRecordMagicPoint = false;
            characterFSM=null;
            animationLong = 0;
        }
    }
}
