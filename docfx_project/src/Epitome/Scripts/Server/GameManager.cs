using FishNet;
using Proto;
using Saber.Camp;
using Saber.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;
using XianXia.Unit;

namespace XianXia
{
    public class GameManager :GameManagerBase, IStartAfterNetwork
    {
        
        public static GameManager NewInstance=>(GameManager)Instance;
        [SerializeField]
        AStarPathfinding2D gridMap;
        [SerializeField]
        PlayerEnum myPlayerEnum;
        PlayerMemeber neutralPlayer;
        PlayerMemeber hostilePlayer;
        PlayerMemeber player1;
        PlayerMemeber eastPlayer;
        Camp neutralCamp;
        Camp hostileCamp;
        Camp playerCamp1;
        Camp eastCamp;

        bool isPreapareFight = false;
        #region 波次敌人
        StartPosManager startPosManager;
        Queue<HeroAndPosPack> roundEnemies;
        Dictionary<byte, int> myUnitStartPos;
        #endregion

        float startPos = 0;
        public PlayerMemeber NeutralPlayer { get => neutralPlayer; }
        public PlayerMemeber HostilePlayer { get => hostilePlayer; }
        public Camp NeutralCamp { get => neutralCamp; }
        public Camp HostileCamp { get => hostileCamp; }
        public PlayerMemeber Player1 { get => player1; }
        public PlayerMemeber EastPlayer { get => eastPlayer; }
        public Camp PlayerCamp1 { get => playerCamp1; }
        public Camp EastCamp { get => eastCamp; }

        public static AStarPathfinding2D MainMap => NewInstance.gridMap;
        public static WorldBase MainWorld=>NewInstance.world;

        public PlayerEnum MyPlayerEnum { get => myPlayerEnum; }

        private PlayerMemeber GetPlayerMemeber(PlayerEnum playerEnum)
        {
            switch (playerEnum)
            {
                case PlayerEnum.monster:
                    return eastPlayer;
                case PlayerEnum.player:
                    return player1;
                default:
                    return null;
            }
        }
        private PlayerEnum GetPlayerEnum(PlayerMemeber player)
        {
            if (player == player1) return PlayerEnum.player;
            else if (player == eastPlayer) return PlayerEnum.monster;
            else return default;
        }
        void GiveGameResult(PlayerMemeber playerMemeber)
        {
            UnitMainSystem unitMainSystem = FindSystem<UnitMainSystem>();
            PlayerMemeber[] playerMemebers = playerMemeber.BelongCamp.GetAllPlayers();
            foreach(var v in playerMemebers)
            {
                if (unitMainSystem.GetNumberUnitOfMine(v) > 0) return;
            }
            FightServerManager.ConsoleWrite_Saber("Tigger Game over ，reason:UnitDead", System.ConsoleColor.DarkYellow);
            ChangeGameStatus(GameStatus.Finish);
        }
        protected override void Start()
        {
            base.Start();
            FightServerManager.ConsoleWrite_Saber("GameManager Init Over,StartGame", System.ConsoleColor.DarkYellow);

            if (CampManager.Instance != null)
            {
                CampManager.Instance.GetPlayerFunc = GetPlayerMemeber;
                CampManager.Instance.GetPlayerEnumFunc = GetPlayerEnum;

            }
            if (gridMap == null) { gridMap = FindObjectOfType<AStarPathfinding2D>(); }
            hostilePlayer = new PlayerMemeber(null, Color.black, true);
            hostileCamp = new Camp(hostilePlayer, CampRelation.hostile);
            neutralPlayer = new PlayerMemeber(null, Color.grey, true);
            neutralCamp = new Camp(neutralPlayer, CampRelation.neutral);
            player1 = new PlayerMemeber(null, Color.green, false);
            playerCamp1 = new Camp(player1, CampRelation.hostile);
            eastPlayer = new PlayerMemeber(null, Color.red, true);
            eastCamp = new Camp(eastPlayer, CampRelation.hostile);
            myPlayerEnum = GetPlayerEnum(player1);
            FindSystem<UnitMainSystem>().OnPlayerFailEvent += GiveGameResult;


            InstanceFinder.GetInstance<TimingSystemUI>().OnGameOver += () => ChangeGameStatus(GameStatus.Finish);

            InstanceFinder.GetInstance<NormalUtility>().OnStartAfterNetwork += StartAfterNetwork;
            OnGameOverEvent += (r) =>
            {
                if ( world != null && isPasuedWhenFinishGame)
                {
                    FindSystem<UnitAttackSystem>()?.SetEnable(false);
                    FindSystem<UnitSpellSystem>()?.SetEnable(false);
                    UnitMoveSystem unitMoveSystem = FindSystem<UnitMoveSystem>();
                    unitMoveSystem?.SetEnable(false);
                    unitMoveSystem?.StopAll();
                }
                InstanceFinder.GetInstance<TimingSystemUI>().StopTimer();
                TimerManager.instance.AddTimer(() => { FightServerManager.ConsoleWrite_Saber("GAME OVER"); InstanceFinder.ServerManager.StopConnection(true); }, 5f);
            };



#if UNITY_SERVER
            //Console.WriteLine("Saber:Enter GameScene");
#else

#endif

        }

        internal void InitFight(MainPack mainPack)
        {
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            UnitMainSystem mainSystem = world.FindSystem<UnitMainSystem>();
            PlayerEnum playerEnum = PlayerEnum.player;
            myUnitStartPos = new Dictionary<byte, int>();
            startPosManager = gridMap.GetComponent<StartPosManager>();
            roundEnemies = new Queue<HeroAndPosPack>();
            int id = 0;
            foreach (var v in mainPack.HeroAndPosList)
            {
                //0是自己，1是当前初始化的敌人
                if (id < 2)
                {
                    foreach (var u in v.List)
                    {
                        if (u != null)
                        {
                            PlayerEnum @enum = playerEnum;
                            //int levelId = u.Pos;
                            Debug.Log($"初始化{u.LevelID}单位-->{playerEnum}");
                            UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(u.LevelID, mainSystem);
                            if (unitModel != null)
                            {
                                Debug.Log($"初始化{unitModel.PrefabName}单位");

                                unitModel.Player = playerEnum;
                                GameObject go = normalUtility.Server_SpawnModel($"{ABUtility.UnitMainName}{unitModel.PrefabName}", unitModel.PrefabName, null, 1);
                                if (playerEnum == PlayerEnum.player)
                                {
                                    go.transform.rotation = Quaternion.Euler(Vector3.down * 180);
                                    myUnitStartPos.Add((byte)u.Pos, u.LevelID);
                                }
                                if (go != null)
                                    mainSystem.SwapnUnitInPos(unitModel, go, playerEnum, startPosManager.PlayerPosDict[playerEnum][(byte)u.Pos],(unit)=>
                                    {
                                        if(@enum==PlayerEnum.player)
                                            unit.FindOrganInBody<BodyOrgan>(ComponentType.body).PosID =(byte)u.Pos;
                                    });
                            }
                        }
                    }
                }
                //多余的就是之后的敌人
                else
                {
                    roundEnemies.Enqueue(v);
                }

                playerEnum = PlayerEnum.monster;
                id++;
            }
            world.FindSystem<UnitMainSystem>().Update();
            world.IsPaused = true;
        }
        /// <summary>
        /// 返回值为True时不触发终局
        /// </summary>
        /// <returns></returns>
        internal bool InitOtherFight()
        {
            Debug.Log("检测是否还有波次要打" + roundEnemies.Count);
            if (roundEnemies.Count <= 0) return false;
            if (isPreapareFight)
            {
                //正在准备下一场战斗
                //修正战斗结果和状态
                gameResult = GameResult.None;
                gameStatus = GameStatus.Starting;
                return true;
            }
            //失败了就不用开启下一场战斗了
            if (gameResult == GameResult.Fail) return false;
            isPreapareFight = true;
            gameResult = GameResult.None;
            gameStatus = GameStatus.Starting;
            InstanceFinder.GetInstance<TimingSystemUI>().StopTimer();

            //关闭一些UI，开启转场
            //隐藏所有存活单位
            //重新计算倒计时
            //蓝量继承，血量继承
            System.Collections.IEnumerator SpawnOtherFight()
            {
                float timer = 0;
                UnitMainSystem mainSystem = World.FindSystem<UnitMainSystem>();
                UnitSpellSystem spellSystem = World.FindSystem<UnitSpellSystem>();
                spellSystem.SetEnable(false);
                WaitUntil waitForSpellAfter = new WaitUntil(() =>
                {
                    timer += Time.deltaTime;
                    return mainSystem.GetAllSpellingUnits() == 0;
                });
                yield return waitForSpellAfter;
                yield return new WaitForSeconds(Mathf.Max(1, 3 - timer));

                world.IsPaused = true;
                NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
                UnitBase[] myUnits = mainSystem.GetAllOfMine(GetPlayerMemeber(PlayerEnum.player));
                List<(int, Vector2Int)> myUnitsStatus = new List<(int, Vector2Int)>();
                //记录棋子状态
                foreach (var v in myUnits)
                {
                    BodyOrgan body = v.FindOrganInBody<BodyOrgan>(ComponentType.body);
                    //不在标记里即可能是召唤物，不需要重置
                    if (body.PosID != 0 && myUnitStartPos.ContainsKey(body.PosID) == true)
                    {
                        MagicOrgan magicOrgan = v.FindOrganInBody<MagicOrgan>(ComponentType.magic);
                        myUnitsStatus.Add((body.PosID, new Vector2Int(body.Health_Curr, magicOrgan.MagicPoint_Curr)));

                    }
                    //回收棋子
                    mainSystem.RemoveUnitFromGame(v, 0);
                }
                //回收场景中的特效和投射物
                normalUtility.ORPC_ClearScene();

                //世界重置一定要在生成之前，不然会出Bug！！！
                //重置内容包括--》
                //点位重置，人物重置，地图重置
                yield return new WaitForSeconds(1f);
                world.WorldReset();
                yield return new WaitForSeconds(1f);
                //生成敌人
                HeroAndPosPack enemy = roundEnemies.Dequeue();
                foreach (var u in enemy.List)
                {
                    byte posId = (byte)u.Pos;
                    Debug.Log($"初始化{u.LevelID}单位-->{PlayerEnum.monster}");
                    UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(u.LevelID, mainSystem);
                    if (unitModel != null)
                    {
                        Debug.Log($"初始化{unitModel.PrefabName}单位");

                        unitModel.Player = PlayerEnum.monster;
                        GameObject go = normalUtility.Server_SpawnModel($"{ABUtility.UnitMainName}{unitModel.PrefabName}", unitModel.PrefabName, null, 1,null,(u)=>u.transform.position=AStarPathfinding2D.GetNodeWorldPositionV3(startPosManager.PlayerPosDict[PlayerEnum.monster][posId],gridMap));
                        if (go != null)
                            mainSystem.SwapnUnitInPos(unitModel, go, PlayerEnum.monster, startPosManager.PlayerPosDict[PlayerEnum.monster][posId]);
                    }
                }
                yield return new WaitForSeconds(1f);
                //生成自己单位
                foreach (var u in myUnitsStatus)
                {
                    byte posId = (byte)u.Item1;
                    int levelId = 0;
                    _ = myUnitStartPos.TryGetValue(posId, out levelId);
                    Vector2Int status = u.Item2;
                    Debug.Log($"初始化{levelId}单位-->{PlayerEnum.player}");
                    UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(levelId, mainSystem);
                    if (unitModel != null)
                    {
                        Debug.Log($"初始化{unitModel.PrefabName}单位");

                        unitModel.Player = PlayerEnum.player;
                        GameObject go = normalUtility.Server_SpawnModel($"{ABUtility.UnitMainName}{unitModel.PrefabName}", unitModel.PrefabName, null, 1, (u) => u.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(startPosManager.PlayerPosDict[PlayerEnum.player][posId], gridMap));
                        if (go != null)
                            mainSystem.SwapnUnitInPos(unitModel, go, PlayerEnum.player, startPosManager.PlayerPosDict[PlayerEnum.player][posId], (unit) =>
                            {
                                BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(ComponentType.body);
                                //继承血量和蓝量
                                bodyOrgan.PosID = posId;
                                bodyOrgan.Health_Curr = status.x;
                                ((IMagicPointRecover)unit.FindOrganInBody<MagicOrgan>(ComponentType.magic)).MagicPoint_Curr = status.y;
                            });
                        go.transform.rotation = Quaternion.Euler(Vector3.down * 180);
                    }
                }
                world.FindSystem<UnitMainSystem>().Update();
                yield return new WaitForSeconds(1f);
                spellSystem.SetEnable(true);
                StartFight();
                //TimerManager.instance.AddTimer(, 1f);
            }
                StartCoroutine(SpawnOtherFight());

            return true;

        }


        internal void StartFight()
        {
            world.IsPaused = false;
            //倒计时持续时间，以及警告时间
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            InstanceFinder.GetInstance<TimingSystemUI>().StartTimer(300, 60);
            //相机位移
            Action<AttackOrgan,BodyOrgan,Damage> action = (a, b, d) =>
             {

                 InstanceFinder.GetInstance<NormalUtility>().ORPC_StartDirectorToFight((a.OwnerUnit.transform.position + b.OwnerUnit.transform.position)/2, 1f);

             };
            foreach(var v in world.FindSystem<UnitMainSystem>().GetAllUnits())
            {
                if (v != null)
                    normalUtility.Server_SetUnitColor(v.gameObject, UnitUtility.GetUnitBelongPlayer(v).Color);
            }
            InstanceFinder.GetInstance<NormalUtility>().ORPC_StartDirector(2.5f);
            UnitUtility.FristFightListen(action);
            isPreapareFight = false;
        }

        protected override bool IsNotTimeToFinishGame() => InitOtherFight();
        public void StartAfterNetwork()
        {
            //联网后初始化
            FightServerManager.ConsoleWrite_Saber("MainWord Init After Network ");
            MainWorld.StartAfterNetwork();
        }

        public override GameResult SetGameResult()
        {
            //判断时间，时间走完了就输
            if (TimingSystemUI.TimeRemaining == 0)
            {
                FightServerManager.ConsoleWrite_Saber($"Because TimeRemaining is 0，player fail！", System.ConsoleColor.Yellow);
                return GameResult.Fail;
            }

            //判断还有没有人剩下
            UnitMainSystem unitMainSystem = FindSystem<UnitMainSystem>();
            PlayerMemeber[] playerMemebers = GetPlayerMemeber(myPlayerEnum).BelongCamp.GetAllPlayers();
            bool isAlive = false;
            foreach(var v in playerMemebers)
            {
                if (unitMainSystem.GetNumberUnitOfMine(v) > 0)
                {
                    isAlive = true;
                    break;
                }
            }
            if (isAlive)
            {
                FightServerManager.ConsoleWrite_Saber($"Because fight win，player win！",System.ConsoleColor.Yellow);

                return GameResult.Success;
            }
 
            else
            {
                FightServerManager.ConsoleWrite_Saber($"Because fight fail，player fail！", System.ConsoleColor.Yellow);
                return GameResult.Fail;

            }
        }

    }
}
