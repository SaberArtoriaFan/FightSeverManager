using Saber.Camp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Saber.ECS;
using XianXia.Terrain;
using UnityEngine.Rendering;

namespace XianXia.Unit
{
    public class UnitModel
    {
        private string modelName;
        private string prefabName;
        [Header("玩家")]
        private PlayerEnum player;
        [Header("身体")]
        private float hight;
        private int health_Max;
        private int health_Curr;
        private int def;
        private float moveSpeed;
        private float evade;
        [Space]
        [Header("攻击")]
        private float attackTime;
        private float attackRange;
        private int attackVal;
        private float attackSpeed;
        private float warningRange;
        private Projectile projectile;
        private float attackHitrate;
        private float attackCriticalchance;
        private float attackCriticaldamage;
        [Header("技能")]
        private int mp_Max;
        private int mp_curr;
        private int attacked_mp;
        private int attack_mp;
        private int activeSkill;
        private int[] passiveSkills;



        public string PrefabName { get => prefabName; }
        public PlayerEnum Player { get => player; set => player = value; }
        public float Hight { get => hight;  }
        public int Health_Max { get => health_Max; }
        public int Health_Curr { get => health_Curr; }
        public int Def { get => def; }
        public float MoveSpeed { get => moveSpeed; }
        public float Evade { get => evade;  }
        public float AttackTime { get => attackTime;  }
        public float AttackRange { get => attackRange; }
        public int AttackVal { get => attackVal;  }
        public float AttackSpeed { get => attackSpeed;  }
        public float WarningRange { get => warningRange;  }
        public Projectile Projectile { get => projectile;  }
        public float AttackHitrate { get => attackHitrate;  }
        public float AttackCriticalchance { get => attackCriticalchance;  }
        public float AttackCriticaldamage { get => attackCriticaldamage;}
        public int Mp_Max { get => mp_Max; }
        public int Mp_curr { get => mp_curr;  }
        public int Attacked_mp { get => attacked_mp;  }
        public int Attack_mp { get => attack_mp;  }
        public int ActiveSkill { get => activeSkill;  }
        public int[] PassiveSkills { get => passiveSkills;  }
        public string ModelName { get => modelName; }

        public UnitModel(string modelName,string prefabName, float hight, int health_Max, int health_Curr, int def, float moveSpeed, float attackTime, float attackRange, int attackVal, float attackSpeed, float warningRange, Projectile projectile, int activeSkill, int[] passiveSkills, int mp_Max, int mp_curr, int attacked_mp, int attack_mp, float attackHitrate, float attackCriticalchance, float attackCriticaldamage, float evade)
        {
            this.prefabName = prefabName;
            this.player = Player;
            this.hight = hight;
            this.health_Max = health_Max;
            this.health_Curr = health_Curr;
            this.def = def;
            this.moveSpeed = moveSpeed;
            this.attackTime = attackTime;
            this.attackRange = attackRange;
            this.attackVal = attackVal;
            this.attackSpeed = attackSpeed;
            this.warningRange = warningRange;
            this.projectile = projectile;
            this.activeSkill = activeSkill;
            this.passiveSkills = passiveSkills;
            this.mp_Max = mp_Max;
            this.mp_curr = mp_curr;
            this.attacked_mp = attacked_mp;
            this.attack_mp = attack_mp;
            this.attackHitrate = attackHitrate;
            this.attackCriticalchance = attackCriticalchance;
            this.attackCriticaldamage = attackCriticaldamage;
            this.evade = evade;
            this.modelName = modelName;
            //Debug.Log("被动技能QQQ");
            //foreach (var v in passiveSkills)
            //{
            //    Debug.Log("被动技能" + v);
            //}
        }
    }
    [RequireComponent(typeof(UnitBase))]
    public class TestUnitModel : MonoBehaviour
    {
        public bool ShowInTime = false;
        [Header("仅做显示窗口")]
        public UnitBase follower;
        public UnitBase AttackTarget;
        [Header("玩家")]
        public PlayerEnum player;
        [Header("身体")]

        public float hight = 2.5f;
        public int health_Max = 100;

        public int health_Curr = 100;
        public int def = 0;
        public float evade = 0.05f;
        public float moveSpeed = 200f;
        [Space]
        [Header("攻击")]
        public float attackHitrate = 0.95f;
        public float attackCriticalchance = 0.08f;
        public float attackCriticaldamage = 1.5f;
        public float attackAnimationLong = 1f;
        public float attackTime = 0.3f;
        public float attackRange = 1.6f;
        public int attackVal = 5;
        public float attackSpeed = 1f;
        public float warningRange = 3.5f;
        public Projectile projectile = null;
        //public int moveSpeed = 200;
        [Space]
        [Header("技能")]
        public float magicMax = 0;
        public float magicCurr = 0;
        public float attackMagic = 0;
        public float attacedMagic = 0;
        public float spellTime = 1f;
        public string activeSkill;
        public string[] passiveSkills;

        public int unitEnable;
        public bool legEnable;
        public bool attackEnale;
        public bool magicEnable;
        public bool bodyEnable;
        private void Start()
        {
            Init();
        }
        public void  Init()
        {
            unit= GetComponent<UnitBase>();
            var world = FindObjectOfType<WorldBase>();
            body = unit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            attack = unit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack);
            legOrgan = unit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg);
            this.worldBase = world;
            attackSystem = world.FindSystem<UnitAttackSystem>();
            map = FindObjectOfType<AStarPathfinding2D>();
            showOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            magicOrgan = unit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            talentOrgan = unit.FindOrganInBody<TalentOrgan>(ComponentType.talent);
        }
        UnitBase unit = null;
        BodyOrgan body=null;
        AttackOrgan attack=null;
        [SerializeField]
        LegOrgan legOrgan = null;
        MagicOrgan magicOrgan = null;
        TalentOrgan talentOrgan = null;
        UnitAttackSystem attackSystem;
        UIShowOrgan showOrgan = null;
        WorldBase worldBase;
        AStarPathfinding2D map;
        //R40442
        private void Update()
        {
            //if (unit == null) unit = GetComponent<UnitBase>().FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            if (ShowInTime&& body != null){
                health_Curr = body.Health_Curr;
                health_Max = body.Health_Max;
                evade = body.Evade;

                def = body.Def;
                player = CampManager.GetPlayerEnum(body.OwnerPlayer);
                hight = showOrgan.UnitHight;
                bodyEnable = body.Enable;
                unitEnable = unit.EnaleShow;
                if (legOrgan != null)
                {
                    legEnable = legOrgan.Enable;
                    follower = legOrgan.Follower;
                }
                if (attack != null)
                {
                    attackVal = attack.AttackVal;
                    attackSpeed = attack.AttackSpeed;
                    AttackTarget = attackSystem.FindAttackTarget(attack);
                    attackEnale = attack.Enable;
                    attackHitrate = attack.AttackHitrate;
                    attackCriticalchance = attack.AttackCriticalChance;
                    attackCriticaldamage = attack.AttackCriticalDamage;
                    attackAnimationLong = attack.AttackAnimationLong;
                    attackTime = attack.AttackTime;
                    attackRange = attack.AttackRange;
                    //attackVal = 5;
                    //attackSpeed = 1f;
                    warningRange = attack.WarningRange;
                    projectile = attack.Projectile;
                }
                if (magicOrgan != null)
                {
                    activeSkill = magicOrgan.GetActiveSkillName();
                    magicMax = magicOrgan.MagicPoint_Max;
                    magicCurr = magicOrgan.MagicPoint_Curr;
                    attackMagic = magicOrgan.MagicPoint_Attack;
                    attacedMagic = magicOrgan.MagicPoint_Damaged;
                    magicEnable = magicOrgan.Enable;
                }
                if (talentOrgan != null)
                    passiveSkills = talentOrgan.GetPassiveSkillsName();

        //SortingGroup
    }

    //Debug.Log(gameObject.name + unit.Health_Curr + "///" + unit.Health_Max);
}
        private void OnDrawGizmosSelected()
        {
            if (attack != null && attack.Owner != null && attack.OwnerUnit.transform != null)
            {
                Gizmos.color = Color.red;
                GLUtility.DrawWireDisc(attack.OwnerUnit.transform.position, Vector3.forward, attack.AttackRange * map.NodeSize);
                Gizmos.color = Color.yellow;
                Vector2 mapY = new Vector2(map.startPos.y, map.startPos.y + map.MapHeight);
                GLUtility.DrawLine(new Vector3(attack.OwnerUnit.transform.position.x + attack.WarningRange * map.NodeSize, mapY.x), new Vector3(attack.OwnerUnit.transform.position.x + attack.WarningRange * map.NodeSize, mapY.y));
                GLUtility.DrawLine(new Vector3(attack.OwnerUnit.transform.position.x - attack.WarningRange * map.NodeSize, mapY.x), new Vector3(attack.OwnerUnit.transform.position.x - attack.WarningRange * map.NodeSize, mapY.y));

            }
        }
    }
}
