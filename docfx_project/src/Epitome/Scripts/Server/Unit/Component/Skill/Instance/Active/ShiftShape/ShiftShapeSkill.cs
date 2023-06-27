using cfg.shiftShape;
using cfg.skillActive;
using FishNet;
using Saber.ECS;
using UnityEngine;

namespace XianXia.Unit
{
    //public class ShapeShiftData 
    //{
    //    public float heath=1;
    //    public float attack=1;
    //    public float defence=1;
    //    public float attackRange;
    //    public float warningRange;
    //    public float attackSpeed=1;
    //    public float evade=1;
    //    public float hitrate=1;
    //    public float criticalchance=1;
    //    public float criticaldamage=1;
    //    public float movementspeed=1;
    //    public string[] passiveSkills=new string[] { "HealSkill" };
    //    public string activeSkill= "CriticalStrikeSkill";
    //    internal string prefabName;
    //    internal float attackTime=0.5f;
    //    internal float attackAnimationLong=0.7f;
    //    internal Projectile projectile;
    //    internal float spellTime=1f;
    //}
    public class ShiftShapeSkill : ActiveSkill
    {


        //这里应该是有一个记录了英雄数据的类，因为战斗服务器不需要查表
        UnitModel originModel;
        ShiftShape shapeModel;
        Projectile shapeProjectile = null;
        int originHash;
        int targetHash;
        //篮量
        public override bool NeedSpellAction => true;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;
        public override TargetType TargetType => TargetType.self;
        //
        float continueTime = 5;


        //internal void InitSkill(UnitModel unitName,ShapeShiftData unitModel)
        //{
        //    //this.shapeShiftUnitModel = unitName;
        //    this.unitModel = unitModel;
        //}
        void GetOriginModelAndShapeModel(int shiftShapeId)
        {
            UnitMainSystem unitMainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            BodyOrgan bodyOrgan = ownerMagicOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (!unitMainSystem.TryGetUnitModel(bodyOrgan.ModelName, out originModel))
            {
                FightLog.LogError($"无法找到{bodyOrgan.ModelName}的单位模型数据，但明明已经用来初始化过了！");
                return;
            }
            this.shapeModel = SkillUtility.GetShiftShapeModel(shiftShapeId);
            shapeProjectile = Projectile.GetProjectile(shapeModel.ProjectilePrefab);
            //this.shapeModel = new ShapeShiftData();
            //shapeModel.heath =1.8f;
            //shapeModel.attack = 2;
            //shapeModel.defence = 1.5f;
            //shapeModel.attackRange = 6;
            //shapeModel.attackSpeed = 1.2f;
            //shapeModel.warningRange = 8;
            //shapeModel.movementspeed = 2;
            //shapeModel.prefabName = "TestShape";
            //shapeModel.evade = 3;
            //shapeModel.hitrate = 1;
            //shapeModel.projectile = new Projectile("Cyclone", 5, 0);
        }
        public override void InitData(SkillActive skillActive)
        {
            base.InitData(skillActive);
            GetOriginModelAndShapeModel(skillActive.SkillShapeshift);
            continueTime = skillActive.ShapeshiftTime;

        }
        protected override void OnSpell()
        {
            base.OnSpell();
            //测试用的
            //GetOriginModelAndShapeModel();

            this.Enable = false;
            //变装这里获取动画时间好像还有点小Bug
            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            UIShowOrgan uIShowOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(ComponentType.body);
            //先换模型的ID，方便后面换手之类的更改动画时间
            originHash = bodyOrgan.ClothesSpineID;
            IClothes clothes = unit.GetComponentInChildren<IClothes>();
            if (clothes!=null)
                bodyOrgan?.SetClothSpineID(clothes.Init(""));
            targetHash = bodyOrgan.ClothesSpineID;


            bodyOrgan.Ex_health_Max += (int)(bodyOrgan.Health_Max * (shapeModel.ShapeshiftHealth - 100)/100);
            bodyOrgan.Health_Curr += (int)(bodyOrgan.Health_Curr * (shapeModel.ShapeshiftHealth - 100)/100);
            bodyOrgan.Ex_def += (int)(bodyOrgan.Origin_def * (shapeModel.ShapeshiftDefence - 100)/100);
            bodyOrgan.Ex_Evade += bodyOrgan.Or_Evade * ((float)shapeModel.ShapeshiftEvade - 100)/100;
            AttackOrgan attackOrgan = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);

            attackOrgan.Origin_AttackRange = (float)shapeModel.ShapeshiftAttackrange/100;
            attackOrgan.WarningRange = (float)shapeModel.HeroAlertdistance/100;
            attackOrgan.ExtraAttackVal += (int)(attackOrgan.OriginAttackVal * (shapeModel.ShapeshiftAttack - 100)/100);
            attackOrgan.Ex_attackSpeed+= (attackOrgan.Origin_AttackSpeed * (shapeModel.ShapeshiftAttackspeed - 100)/100);
            attackOrgan.Ex_AttackHitrate+= ((float)shapeModel.ShapeshiftHitrate-100)/100*(attackOrgan.Or_AttackHitrate);
            //Debug.Log("命中率" + attackOrgan.AttackHitrate);
            attackOrgan.Ex_AttackCriticalChance  += ((float)shapeModel.ShapeshiftCriticalchance- 100)/100 * attackOrgan.Or_AttackCriticalChance;
            attackOrgan.Ex_AttackCriticalDamage += ((float)shapeModel.ShapeshiftCriticaldamage - 100)/100 * attackOrgan.Or_AttackCriticalDamage;
            UnitAttackSystem attackSystem = SkillSystem.World.FindSystem<UnitAttackSystem>();
            attackSystem.ChangeWeapon(attackOrgan, shapeProjectile);
            attackSystem.ChangeFsm(attackOrgan);

            UnitSpellSystem unitSpellSystem = SkillSystem.World.FindSystem<UnitSpellSystem>();
            //unitSpellSystem.SetSpellTime(attackOrgan.CharacterFSM, shapeModel.spellTime);

            UnitTalentSystem unitTalentSystem = SkillSystem.World.FindSystem<UnitTalentSystem>();
            //添加技能
            //
            if (shapeModel.ShapeshiftSkill != null && shapeModel.ShapeshiftSkill.Length > 0)
            {
                TalentOrgan talentOrgan = unit.FindOrganInBody<TalentOrgan>(ComponentType.talent);
                foreach (var v in shapeModel.ShapeshiftSkill)
                {
                    unitTalentSystem.GainSkill(talentOrgan, v);
                }
            }
            MagicOrgan magicOrgan = unit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            magicOrgan?.SetAnimationFinishTime(bodyOrgan.ClothesSpineID);
            //return;
            if (shapeModel.ShapeshiftStrongskill!=0)
            {
                //MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
                //magicOrgan.AnimationLong = ;
                //unitSpellSystem.LostSkill(this);
                
                ActiveSkill skill= unitSpellSystem.GainSkill(magicOrgan, shapeModel.ShapeshiftStrongskill);
                unitSpellSystem.SortFristSkill(ownerMagicOrgan,skill);
                SkillUtility.SetSkillMagicPoint(shapeModel.HeroMp, 0, shapeModel.AttackMp, shapeModel.AttackedMp, magicOrgan);

            }
            //unitSpellSystem.GainSkill( magicOrgan,this);
            else
            {
                //没有主动技能就关闭蓝条
                IMagicPointRecover m = (IMagicPointRecover)magicOrgan;
                m.MagicPoint_Max = 0;
                m.MagicPoint_Curr = 0;

            }

            //更换模型
            //加载模型
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShapeShift(unit.gameObject, ABUtility.ShiftShapeMainName, shapeModel.HeroPrefab);

            //判断是不是到时间需要变回来
            if (continueTime > 0)
            {
                TimerManagerSystem timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
                timerManagerSystem.AddTimer(CancelShapeShift, continueTime);
            }


            //警戒范围也需要更改
            //动画状态机里的  Attack的攻击时间和结束时间,Spell的结束时间也要
            //attackOrgan.
        }

        private void SetSomeParameter(Transform tr)
        {
            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            //UIShowOrgan uIShowOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            unit.GetComponent<CharacterFSM>().ChangeAnimator(tr.GetComponentInChildren<Animator>());
            //uIShowOrgan.OriginScale = tr.localScale;
            //uIShowOrgan.UnitScale = uIShowOrgan.UnitScale;
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            //originTR = bodyOrgan.ModelTR;
            //originTR.gameObject.SetActive(false);
            bodyOrgan.ModelTR = tr;
            tr.localRotation = Quaternion.identity;

        }
        private void CancelShapeShift()
        {
            //人死了就不用变回来了
            if (this.ownerMagicOrgan == null || ownerMagicOrgan.OwnerUnit == null) return;


            ////模型变回来
            //shapeShiftTR.gameObject.SetActive(false);
            //originTR.gameObject.SetActive(true);



            UnitBase unit = ownerMagicOrgan.OwnerUnit;
            BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            bodyOrgan.SetClothSpineID(originHash);

            bodyOrgan.Ex_health_Max -= (int)(bodyOrgan.Origin_health_Max * (shapeModel.ShapeshiftHealth - 100)/100);
            bodyOrgan.Health_Curr = bodyOrgan.Health_Curr;
            //bodyOrgan.Health_Curr -= (int)(bodyOrgan.Health_Curr * (unitModel.heath - 1));
            bodyOrgan.Ex_def-= (int)(bodyOrgan.Origin_def * (shapeModel.ShapeshiftDefence - 100)/100);
            bodyOrgan.Ex_Evade -= bodyOrgan.Or_Evade * ((float)shapeModel.ShapeshiftEvade - 100)/100;


            AttackOrgan attackOrgan = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            attackOrgan.Origin_AttackRange = originModel.AttackRange;
            attackOrgan.WarningRange = originModel.WarningRange;

            attackOrgan.ExtraAttackVal -= (int)(attackOrgan.OriginAttackVal * (shapeModel.ShapeshiftAttack - 100)/100);
            attackOrgan.Ex_attackSpeed -=(attackOrgan.Origin_AttackSpeed * (shapeModel.ShapeshiftAttackspeed - 100)/100);
            attackOrgan.Ex_AttackHitrate -= ((float)shapeModel.ShapeshiftHitrate - 100)/100 * (attackOrgan.Or_AttackHitrate);
            attackOrgan.Ex_AttackCriticalChance -= ((float)shapeModel.ShapeshiftCriticalchance - 100)/100 * attackOrgan.Or_AttackCriticalChance;
            attackOrgan.Ex_AttackCriticalDamage -= ((float)shapeModel.ShapeshiftCriticaldamage - 100)/100 * attackOrgan.Or_AttackCriticalDamage;

            UnitAttackSystem attackSystem = SkillSystem.World.FindSystem<UnitAttackSystem>();
            attackSystem.ChangeWeapon(attackOrgan, originModel.Projectile);
            attackSystem.ChangeFsm(attackOrgan);

            UnitSpellSystem unitSpellSystem = SkillSystem.World.FindSystem<UnitSpellSystem>();
            //unitSpellSystem.SetSpellTime(attackOrgan.CharacterFSM, shapeModel.spellTime);

            UnitTalentSystem unitTalentSystem = SkillSystem.World.FindSystem<UnitTalentSystem>();
            //
            if (shapeModel.ShapeshiftSkill != null && shapeModel.ShapeshiftSkill.Length > 0)
            {
                TalentOrgan talentOrgan = unit.FindOrganInBody<TalentOrgan>(ComponentType.talent);
                foreach (var v in shapeModel.ShapeshiftSkill)
                {
                    unitTalentSystem.LostSkill(SkillUtility.GetSkillResouceNameById(v,false), talentOrgan);
                }
            }
            MagicOrgan magicOrgan = unit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            magicOrgan?.SetAnimationFinishTime(bodyOrgan.ClothesSpineID);
            if (shapeModel.ShapeshiftStrongskill!=0)
            {
                //MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
                //magicOrgan.AnimationLong = ;
                //magicOrgan.StatusList.Clear();
                //Debug.Log(this);
                               
                unitSpellSystem.LostSkill(SkillUtility.GetSkillResouceNameById(shapeModel.ShapeshiftStrongskill, true),magicOrgan);
            }
            //unitSpellSystem.GainSkill(magicOrgan, this);
            //Transform temp = originTR;
            //originTR = shapeShiftTR;
            //shapeShiftTR = temp;
            SkillUtility.SetSkillMagicPoint(originModel.Mp_Max, 0, originModel.Attack_mp, originModel.Attacked_mp, magicOrgan);
            this.Enable = true;

            ////模型已经更新完毕
            ////更新数据
            //SetSomeParameter(originTR);
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShapeShift(this.ownerMagicOrgan.OwnerUnit.gameObject, null, null);
        }
    }
}
