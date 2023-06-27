using cfg.skillActive;
using FishNet;
using Saber.Camp;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;
using cfg.summoned;

namespace XianXia.Unit
{
    public class SummonSkill : ActiveSkill
    {
        public class Summon
        {
            public  string Name=>summoned.HeroPrefab;
            readonly int summonCount;
            public readonly float liveTime;
            public int AlivedMaxCount=>summoned.SummonedMaxnum;
            public readonly Saber.Base.ObjectPool<GameObject> pool;
            public int CurrAlivedCount=>summonedList.Count;

            public readonly UnitModel summonModel;
            public readonly Summoned summoned;
            readonly List<UnitBase> summonedList;
            public int SummonCount { get {
                    return summonCount;
                } }

            public void AddSummon(UnitBase unitBase)
            {
                if (summonedList.Contains(unitBase)) return;
                if(CurrAlivedCount==AlivedMaxCount&&AlivedMaxCount>0)
                {
                    UnitBase u = summonedList[0];
                    SkillUtility.KillSummon(this, u);
                    summonedList.RemoveAt(0);
                }
                summonedList.Add(unitBase);
            }
            public void RemoveSummon(UnitBase unitBase)
            {
                if (summonedList.Contains(unitBase))
                    summonedList.Remove(unitBase);

            }
            public bool Contains(UnitBase unitBase)
            {
                return summonedList.Contains(unitBase);
            }
            public Summon(int summonCount, float liveTime, Summoned summoned, Saber.Base.ObjectPool<GameObject> pool)
            {
                this.summonCount = summonCount;
                this.liveTime = liveTime;
                this.pool = pool;
                this.summoned = summoned;
                summonedList = new List<UnitBase>(summoned.SummonedMaxnum);
                this.summonModel = SkillUtility.GetSummonedModel(summoned);
                Debug.Log("�����ٻ�����Ϣ��" + summonModel.ModelName);
            }
        }

        public override bool NeedSpellAction => true;
        public override TargetType TargetType { get => base.TargetType;  }
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;

        UnitBodySystem bodySystem;
        protected UnitMainSystem mainSystem;
        //ObjectPoolSystem poolSystem;
        BuffSystem buffSystem;
        GameObject model;
        //GameObject[] usedModels;
        Summon[] summons;

        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            CDTime = 20f;
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            buffSystem = SkillSystem.World.FindSystem<BuffSystem>();
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
        }


        public override void InitData(SkillActive skillActive)
        {
            base.InitData(skillActive);
            summons = new Summon[skillActive.Summoned.Count];
            if (summons.Length == 0)
            {
                FightServerManager.ConsoleWrite_Saber($"{skillActive.SkillResourcename} SummonSkill no summoned����������",ConsoleColor.Red);
                return;
            }
            int count = 0;
            foreach(var v in skillActive.Summoned)
            {
                summons[count] = SkillUtility.GetSummonData(v.Key, v.Value, skillActive.LiveTime);
                count++;
            }

        }
        protected override void OnSpell()
        {
            base.OnSpell();
            //if (Target == null) return;
            GameObject model;
            //if (model == null) return;
            //�õ���ģ�ͣ�������û�м����
            //�ŵ�Ŀ�����
            //����Ҫȡ�ĸ���
            int summonNum = 0;
            foreach (var v in summons)
                summonNum += v.SummonCount;
            //ȡ��Ӧ�ĸ�������
            var res = GetSummonedPosition(summonNum);
            //��ȡ���
            PlayerEnum player = CampManager.GetPlayerEnum(this.ownerMagicOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).OwnerPlayer);

            Debug.Log("�ٻ�������������" + player);
            int i = 0;
            //�����ٻ�
            foreach (var v in summons)
            {
                if (v.Equals(null)) continue;
                int num = v.SummonCount;
                //�ж�����
                for (int j = 0; j < num; j++)
                {
                    if (i >= res.Count) break;
                    Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(res[i].Position, SkillSystem.Map);

                    model = v.pool.GetObjectInPool((u) =>
                    {
                        u.gameObject.transform.position = pos;
                    });
                    //model.SetActive(true);
                    Debug.Log("�ٻ��ٻ���" + model.name);
                    FightLog.Record($"���:{player} ��λ:{ownerMagicOrgan.OwnerUnit.gameObject.name} �ٻ���{model.name}��");
                    //v.summonModel.Player= CampManager.GetPlayerEnum(player); 

                    //�����ٻ�buff
                    Action<StatusOrgan> buffEndAction = v.liveTime > 0 ? (u) => { bodySystem.UnitDead(u.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body), null); v.RemoveSummon(u.OwnerUnit); } : (u) => { v.RemoveSummon(u.OwnerUnit); };
                    Action<UnitBase> action = (UnitBase unit) =>
                    {
                        v.AddSummon(unit);
                        UpdateSummonProperty(unit, v.summonModel, ownerMagicOrgan.OwnerUnit);
                        buffSystem.AddBuff("Summon", unit.FindOrganInBody<StatusOrgan>(ComponentType.statusHeart),ownerMagicOrgan.OwnerUnit,
                          null,
                          null,
                          buffEndAction,
                          (int)v.liveTime,
                          false);
                    };
                    //˳��ǿ������
                    //action = null;
                    mainSystem.SwapnUnitInPos(v.summonModel,model,player , res[i].Position, action);
                    i++;
                }
            }
        }
        protected virtual List<Node> GetSummonedPosition(int summonNum)
        {
            //�����Լ�����ĵط�
            var res = AStarPathfinding2D.FindNearestNode(mainSystem.GetGridItemByUnit(ownerMagicOrgan.OwnerUnit), 100, summonNum, (a, b) =>
            {
                if (mainSystem.GetUnitByGridItem(b) == null) return true;
                else return false;
            }, SkillSystem.Map, true, true, true);
            return res;
        }
        protected void UpdateSummonProperty(UnitBase summoned,UnitModel unitModel,UnitBase owner)
        {
            Debug.Log("�����ٻ�������");
            AttackOrgan attackOrgan = summoned.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            AttackOrgan ownerAttackOrgan= owner.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            if (attackOrgan != null && ownerAttackOrgan != null)
            {
                attackOrgan.OriginAttackVal = (int)(attackOrgan.OriginAttackVal * ((float)ownerAttackOrgan.AttackVal / 100f));
                attackOrgan.WarningRange = 1000;
            }
            BodyOrgan bodyOrgan= summoned.FindOrganInBody<BodyOrgan>(ComponentType.body);
            BodyOrgan ownerbodyOrgan = owner.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (bodyOrgan != null && ownerbodyOrgan != null)
            {
                bodyOrgan.Origin_health_Max = (int)(bodyOrgan.Origin_health_Max * ((float)ownerbodyOrgan.Origin_health_Max / 100f));
                bodyOrgan.Origin_def= (int)(bodyOrgan.Origin_def * ((float)ownerbodyOrgan.Origin_def / 100f));
                bodyOrgan.Health_Curr = bodyOrgan.Health_Max;
            }
        }
    }
}
