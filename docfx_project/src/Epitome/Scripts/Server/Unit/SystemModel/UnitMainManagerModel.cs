using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
using Saber.ECS;
using XianXia.Terrain;
using System;
using Object = UnityEngine.Object;
using Saber.Camp;

namespace XianXia.Unit
{
    public class UnitMainManagerModel :SystemModelBase
    {
        Dictionary<UnitBase, Node> unitPosDict;
        Dictionary<Node, UnitBase> posUnitDict;
        Dictionary<UnitBase,(FSM_State, Coroutine)> unitCoroutinesDict;
        List<Coroutine> coroutines = new List<Coroutine>();
        Queue<Object> unitDeadQeueue;
        Queue<(GameObject, PlayerEnum, UnitModel)> unitCreateQueue=new Queue<(GameObject, PlayerEnum, UnitModel)>();
        Dictionary<GameObject, Action<UnitBase>> unitCreateActionDict = new Dictionary<GameObject, Action<UnitBase>>();
        AStarPathfinding2D gridMap;
        UnitMoveSystem moveSystem;
        UnitAttackSystem attackSystem;
        TimerManagerSystem timerManagerSystem;
        HashSet<UnitBase> unitTimerHash=new HashSet<UnitBase>();
        Dictionary<PlayerMemeber, List<UnitBase>> playersUnitsDict = new Dictionary<PlayerMemeber, List<UnitBase>>();
        Dictionary<string, UnitModel> unitModelDict = new Dictionary<string, UnitModel>();
        public UnitMainManagerModel()
        {
            unitPosDict=new Dictionary<UnitBase, Node>();
            posUnitDict = new Dictionary<Node, UnitBase>();
            unitCoroutinesDict = new Dictionary<UnitBase, (FSM_State, Coroutine)>();
            unitDeadQeueue = new Queue<Object>();
            gridMap =Object.FindObjectOfType<AStarPathfinding2D>(); 
        }


        //public override ComponentType ComponentType => throw new System.NotImplementedException();
        //public List<Coroutine> Coroutines { get => coroutines;  }
        //public Queue<Object> UnitDeadQeueue { get => unitDeadQeueue; }
        public UnitMoveSystem MoveSystem { get => moveSystem; set => moveSystem = value; }
        public UnitAttackSystem AttackSystem { get => attackSystem; set => attackSystem = value; }
        public TimerManagerSystem TimerManagerSystem { get => timerManagerSystem; set => timerManagerSystem = value; }
        //public HashSet<UnitBase> UnitTimerHash { get => unitTimerHash; set => unitTimerHash = value; }
        public Queue<(GameObject,PlayerEnum,UnitModel)> UnitCreateQueue { get => unitCreateQueue; }
        public Dictionary<GameObject, Action<UnitBase>> UnitCreateActionDict { get => unitCreateActionDict;  }
        public Dictionary<PlayerMemeber, List<UnitBase>> PlayersUnitsDict { get => playersUnitsDict; }
        public Dictionary<string, UnitModel> UnitModelDict { get => unitModelDict; }
        internal AStarPathfinding2D GridMap { get => gridMap; set => gridMap = value; }
        internal Dictionary<UnitBase, (FSM_State, Coroutine)> UnitCoroutinesDict { get => unitCoroutinesDict; }
        internal Dictionary<UnitBase, Node> UnitPosDict { get => unitPosDict; }
        internal Dictionary<Node, UnitBase> PosUnitDict { get => posUnitDict; }
    }
}
