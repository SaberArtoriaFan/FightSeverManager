using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using XianXia.Terrain;
using FSM;

namespace XianXia.Unit
{
    public class Spine_Walk : FSM.Walk
    {
        Client_UnitProperty unitProperty;
        public Spine_Walk(Animator animator, IWalk walk) : base(animator, walk)
        {
            unitProperty = animator.GetComponentInParent<Client_UnitProperty>();
        }
        protected override void SetAnimatorParameter(bool isEnter)
        {
            animator.SetBool(FSM.AnimatorParameters.Walk, isEnter);
            //unitProperty.ORPC_AnimatorParameter_Bool(FSM.AnimatorParameters.Walk, isEnter);
        }
    }
    [System.Serializable]
    public class LegOrgan : OrganBase,IWalk
    {
        private float moveSpeed = 200;

        [SerializeField]
        bool canFindPath = true;
        [SerializeField]
        bool isFinding = false;
        CharacterFSM characterFSM = null;
        [SerializeField]
        List<Vector3> path = new List<Vector3>();
        [SerializeField]
        public Queue<float> pathG = new Queue<float>();
        [SerializeField]
        int currentPathIndex = 0;
        UnitBase follower;
        [SerializeField]
        bool isStand = true;
        [SerializeField]
        bool isFollowEachOther = false;
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public bool IsCanMove => characterFSM != null && characterFSM.isCanMove;

        protected override ComponentType componentType => ComponentType.leg;


        public CharacterFSM CharacterFSM { get => characterFSM; }
        public List<Vector3> Path { get => path; set => path = value; }
        public int LastPathLength { get; set; } = 0;
        public int CurrentPathIndex { get => currentPathIndex; set => currentPathIndex = value; }
        public UnitBase Follower { get => follower; set => follower = value; }
        public bool IsStand { get => isStand; set => isStand = value; }

        public bool HasFollower { get => follower != null; }
        public Vector3 Pos { get; set; }
        public Vector3 FollowerPos { get;set; }
        public bool CanFindPath { get => canFindPath; set => canFindPath = value; }
        public bool IsFollowEachOther { get => isFollowEachOther; set => isFollowEachOther = value; }

        float IWalk.WalkSpeed => moveSpeed/200;

        public bool IsFinding { get => isFinding; set => isFinding = value; }

        // public List<float> PathG { get => pathG; set => pathG = value; }


        protected override void InitComponent(EntityBase owner)
        {
            base.InitComponent(owner);
            characterFSM = owner.GetComponent<CharacterFSM>();
            characterFSM.AddState(new Spine_Walk(owner.GetComponentInChildren<Animator>(), this));
        }
        public override void Destory()
        {
            base.Destory();
            lock (this)
            {
                moveSpeed = 200;
                canFindPath = true;
                isFinding = false;
                characterFSM = null;
                path.Clear();
                pathG.Clear();
                currentPathIndex = 0;
                follower = null;
                isStand = true;
                isFollowEachOther = false;
                //Debug.Log("œ˙ªŸ“ªÃıÕ»");
            }
        }
    }
}
