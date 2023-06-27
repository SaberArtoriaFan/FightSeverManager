using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using XianXia.Terrain;
using cfg.skillActive;
using cfg.skillPassive;

namespace XianXia.Unit
{
    public interface IRealName
    {
        string RealName { get; }
    }
    public enum SpellTiggerType
    {
        immediate,
        point,
        unit
    }
    [Flags]
    public enum TargetType
    {
        none=0,
        enemy=2<<1,
        friend=2<<2,
        self=2 << 3,
        selfUnit=2 << 4
    }
    [Serializable]
    public abstract class SkillBase : IComponentBase, IAllowOwnNum, IRealName
    {
        private bool enable = true;
        ISkillSystem skillSystem;
        public string RealName { get; set; }
        public string SkillDescription { get; }
        public uint SkillId => skillId; 
        public Sprite SkillIcon { get; }

        public ComponentType ComponentType => ComponentType.none;

        public bool Enable { get => enable; set => enable = value; }
        public IContainerEntity Owner { get => skillOrgan; set => skillOrgan = value; }

        public int AllowOwnMaxNum => 1;

        public ISkillSystem SkillSystem { get => skillSystem; internal set => skillSystem = value; }

        protected IContainerEntity skillOrgan;
        protected uint skillId;

        //public bool IsShowInBar { get => isShowInBar; set => isShowInBar = value; }

        //public bool Enable => isEnable;

        public SkillBase()
        {
            //OnCreate();
        }


        public virtual void Init(IContainerEntity owner)
        {
            Owner = owner;
        }



        public virtual void AcquireSkill()
        {

        }
        public virtual void LostSkill()
        {

        }

        public virtual void Destory()
        {
        }

        void IComponentBase.ClearEnable()
        {
            
        }
    }
    internal interface ISelectTargetSkill
    {
        Node FindTarget();

    }
    public  class ActiveSkill : SkillBase
    {
        protected MagicOrgan ownerMagicOrgan;

        Node target;
        #region Excel����Ƶ��ֶ�
        protected int magicPoint_Attack = 10;
        protected int magicPoint_Damaged = 10;

        protected int startMagicPoint = 10;
        /// <summary>
        /// �ͷ�����Ҫ��ħ��ֵ
        /// </summary>
        protected int needMagicPointMax = 100;

        /// <summary>
        /// һ����Ϸ�����ͷŵĴ���
        /// </summary>
        protected int lifeTimeCount = -1;
        /// <summary>
        /// ������ȴʱ��
        /// </summary>
        protected float cDTime = 0;
        bool isCD = true;
        /// <summary>
        /// ����ȡ��������
        /// </summary>
        protected int effectNum=0;

        #endregion
        private Func<Node> findTargetFunc;

        protected Func<bool> isCanSpellFunc;

        //private SpellTiggerType spellTiggerType;
        //public virtual float spellTime
        //protected MagicOrgan magicOrgan=>()
        public virtual SpellTiggerType SpellTiggerType { get ; internal set ; }
        public Node Target { get => target; internal set => target = value; }
        public virtual bool NeedSpellAction { get => true; }
        public virtual TargetType TargetType { get ;private set; }
        public float SpellRange { get ;private set ; }
        public int StartMagicPoint { get => startMagicPoint; set => startMagicPoint = value; }
        public int NeedMagicPointMax { get => needMagicPointMax; set => needMagicPointMax = value; }
        public int MagicPoint_Attack { get => magicPoint_Attack; set => magicPoint_Attack = value; }
        public int MagicPoint_Damaged { get => magicPoint_Damaged; set => magicPoint_Damaged = value; }
        public float CDTime { get => cDTime; set => cDTime = value; }
        public bool IsCD { get => isCD; set => isCD = value; }
        protected Func<Node> FindTargetFunc { get => findTargetFunc;  }
        public Func<bool> IsCanSpellFunc { get => isCanSpellFunc;  }

        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            //Debug.Log((MagicOrgan)magicOrgan + "///88");
            this.ownerMagicOrgan = (MagicOrgan)magicOrgan;

        }
        public virtual void InitData(SkillActive skillActive)
        {
            skillId =(uint) skillActive.SkillId;
            needMagicPointMax = skillActive.SkillMp;
            RealName = skillActive.SkillId.ToString();
            TargetType = SkillUtility.CalculateTargetObject(skillActive.SkillObject);
            lifeTimeCount = SkillUtility.CalculateLifeTime(skillActive.SkillCount);
            cDTime = skillActive.SkillCd;
            effectNum = skillActive.ObjectNum;
            SpellRange = (float)skillActive.SkillSpellRange/100;
            FightLog.Record($"��������:{RealName}��ʼ����� ���{UnitUtility.GetUnitBelongPlayerEnum(ownerMagicOrgan.OwnerUnit)} ��λ:{ownerMagicOrgan.OwnerUnit.gameObject.name}");
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            FightLog.Record($"��������:{RealName}����ӵ� ���{UnitUtility.GetUnitBelongPlayerEnum(ownerMagicOrgan.OwnerUnit)} ��λ:{ownerMagicOrgan.OwnerUnit.gameObject.name}");

        }
        public override void LostSkill()
        {
            base.LostSkill();
            FightLog.Record($"��������:{RealName}��ʧȥ�� ���{UnitUtility.GetUnitBelongPlayerEnum(ownerMagicOrgan.OwnerUnit)} ��λ:{ownerMagicOrgan.OwnerUnit.gameObject.name}");

        }
        public void OnSpellSkill()
        {
            FightLog.Record($"����:{RealName}���ͷţ�����{needMagicPointMax},Cd{cDTime}");
            //ʹ�ô�������֮�󲻿��ͷ�
            OnSpell();
            //SkillUtility.ShowRisingSpace(RealName, ownerMagicOrgan.OwnerUnit.transform.position + 0.6f * Vector3.up, Vector3.up + Vector3.left, Color.blue);
            if (lifeTimeCount > 0) lifeTimeCount--;
            if (lifeTimeCount == 0) this.Enable = false;

        }
        protected virtual void OnSpell()
        {

        }
        /// <summary>
        /// �ͷŷ�ʽΪ��ʱ�ŻᱻѰ��Ŀ�꺯������
        /// </summary>
        /// <returns></returns>

    }
    public  class PassiveSkill : SkillBase
    {
        protected TalentOrgan ownerTalentOrgan;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            ownerTalentOrgan=(TalentOrgan)owner;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            FightLog.Record($"��������:{RealName}����ӵ� ���{UnitUtility.GetUnitBelongPlayerEnum(ownerTalentOrgan.OwnerUnit)} ��λ:{ownerTalentOrgan.OwnerUnit.gameObject.name}");

        }
        public override void LostSkill()
        {
            base.LostSkill();
            FightLog.Record($"��������:{RealName}��ʧȥ�� ���{UnitUtility.GetUnitBelongPlayerEnum(ownerTalentOrgan.OwnerUnit)} ��λ:{ownerTalentOrgan.OwnerUnit.gameObject.name}");
        }
        public virtual void InitData(SkillPassive sp)
        {
            skillId = (uint)sp.SkillId;
            RealName = sp.SkillId.ToString();
        }

    }
}
