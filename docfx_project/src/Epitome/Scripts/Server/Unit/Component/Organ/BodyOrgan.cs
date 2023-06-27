using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
using Saber.Camp;
using Saber.ECS;
using System.Linq;
using FishNet;

namespace XianXia.Unit
{
    public interface IBodyOrgan : IComponentBase
    {
        int Health_Curr { get; }
        int Def { get; }
    }

    public class Spine_Death : FSM.Death
    {
        public Spine_Death(Animator animator, float animationTime) : base(animator, animationTime)
        {
        }
        public override void OnExit()
        {
            //死亡结束后不执行动作，以防动画bug
        }
    }
    public class BodyOrgan : OrganBase, IBodyOrgan
    {
        public BodyOrgan() : base()
        {
        }

        IntAttributeContainer origin_health_Max = new IntAttributeContainer();
        //int ex_health_Max = 0;
        int health_Curr = 100;

        IntAttributeContainer origin_def = new IntAttributeContainer();
        //nt ex_def = 0;
        float deadTime = 1.5f;

        FloatAttributeContainer evade = new FloatAttributeContainer();
        protected PlayerMemeber ownerPlayer;
        private string modelName;
        Transform modelTR;

        Dictionary<int, Transform> partBodyDict;

        public int ClothesSpineID { get => clothesSpineID; }

        private int clothesSpineID;
        private byte levelID;
        public int Health_Curr
        {
            get => health_Curr; set
            {
                health_Curr = value;
                if (health_Curr > Health_Max) health_Curr = Health_Max;
                if (health_Curr <= 0) health_Curr = 0;
            }
        }
        public int Health_Max
        {
            get => origin_health_Max.SumValue; set
            {
                origin_health_Max.OriginValue = value;
                if (origin_health_Max.OriginValue <= 0) origin_health_Max.OriginValue = 1;
            }
        }

        public int Def { get => origin_def.SumValue; set => origin_def.OriginValue = value; }

        protected override ComponentType componentType => ComponentType.body;

        public PlayerMemeber OwnerPlayer { get => ownerPlayer; internal set => ownerPlayer = value; }
        public bool UnitAlive => Health_Curr > 0;

        public CharacterFSM CharacterFSM { get => characterFSM; internal set => characterFSM = value; }
        public float DeadTime { get => deadTime;  }
        public Transform ModelTR { get => modelTR; set => modelTR = value; }
        public int Ex_health_Max { get => origin_health_Max.ExValue; set => origin_health_Max.ExValue = value; }
        public int Origin_health_Max { get => origin_health_Max.OriginValue; set => origin_health_Max.OriginValue = value; }
        public int Ex_def { get => origin_def.ExValue; set => origin_def.ExValue = value; }
        public int Origin_def { get => origin_def.OriginValue; set => origin_def.OriginValue = value; }
        public float Evade { get => evade.SumValue; set => evade.OriginValue = value; }
        public float Ex_Evade { get => evade.ExValue; set => evade.ExValue = value; }
        public float Or_Evade { get => evade.OriginValue; set => evade.OriginValue = value; }
        public string ModelName { get => modelName; set => modelName = value; }
        public Dictionary<int, Transform> PartBodyDict { get => partBodyDict; }
        public byte PosID { get => levelID; set => levelID = value; }

        private CharacterFSM characterFSM;
        public void SetClothSpineID(int id)
        {
            clothesSpineID = id;
            float timer = InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().GetAnimationLong(clothesSpineID, FSM.AnimatorParameters.Death);
            Debug.Log("设置死亡时间为" + timer);
            if (timer > 0) deadTime = timer;
            var d = characterFSM.FindFSMState(FSM_State.death);
            if (d == null)
            {
                characterFSM.AddState(new Spine_Death(characterFSM.Animator, deadTime));
            }

        }

         void InitPart(int i)
        {
            GameObject go = new GameObject($"XXPart{i}");
            go.transform.parent = OwnerUnit.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localPosition = BuffUtility.CalculateEffectOffestPos(OwnerUnit, i);
            go.name = i.ToString();
            partBodyDict.Add(i, go.transform);
        }
        protected override void InitComponent(EntityBase unit)
        {
            base.InitComponent(unit);
            modelTR = unit.GetComponentInChildren<Animator>().transform;
            characterFSM = unit.GetComponent<CharacterFSM>();
            partBodyDict = partBodyDict == null ? new Dictionary<int, Transform>() : partBodyDict;
            partBodyDict.Clear();
            TimerManager.Instance.AddTimer(() =>
            {
                Transform[] transforms = unit.GetComponentsInChildren<Transform>();
                foreach(var v in transforms)
                {
                    if (v.name.StartsWith("XXPart"))
                        GameObject.Destroy(v.gameObject);
                }

                for (int i = 1; i < 3; i++)
                    InitPart(i);
                partBodyDict.Add(3, OwnerUnit.transform);
            }, Time.deltaTime);

        }
        public override void Destory()
        {
            base.Destory();
            origin_health_Max.Clear();
            //int ex_health_Max = 0;
            health_Curr = 0;

            origin_def.Clear();
            //nt ex_def = 0;
            deadTime = 1.5f;

            evade.Clear();
            ownerPlayer = null;
            modelName = string.Empty;
            modelTR = null;
            characterFSM = null;
            Transform[] transforms = partBodyDict.Values.ToArray();
            Debug.Log("销毁Body" + transforms.Length);
            foreach(var v in partBodyDict)
            {
                if (v.Key != 3)
                    GameObject.Destroy(v.Value?.gameObject);
            }
            partBodyDict.Clear();
            clothesSpineID = 0;
        }
    }
}
