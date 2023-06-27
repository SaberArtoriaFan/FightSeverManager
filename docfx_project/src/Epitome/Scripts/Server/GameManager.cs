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
        #region ���ε���
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
            FightServerManager.ConsoleWrite_Saber("Tigger Game over ��reason:UnitDead", System.ConsoleColor.DarkYellow);
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
                //0���Լ���1�ǵ�ǰ��ʼ���ĵ���
                if (id < 2)
                {
                    foreach (var u in v.List)
                    {
                        if (u != null)
                        {
                            PlayerEnum @enum = playerEnum;
                            //int levelId = u.Pos;
                            Debug.Log($"��ʼ��{u.LevelID}��λ-->{playerEnum}");
                            UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(u.LevelID, mainSystem);
                            if (unitModel != null)
                            {
                                Debug.Log($"��ʼ��{unitModel.PrefabName}��λ");

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
                //����ľ���֮��ĵ���
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
        /// ����ֵΪTrueʱ�������վ�
        /// </summary>
        /// <returns></returns>
        internal bool InitOtherFight()
        {
            Debug.Log("����Ƿ��в���Ҫ��" + roundEnemies.Count);
            if (roundEnemies.Count <= 0) return false;
            if (isPreapareFight)
            {
                //����׼����һ��ս��
                //����ս�������״̬
                gameResult = GameResult.None;
                gameStatus = GameStatus.Starting;
                return true;
            }
            //ʧ���˾Ͳ��ÿ�����һ��ս����
            if (gameResult == GameResult.Fail) return false;
            isPreapareFight = true;
            gameResult = GameResult.None;
            gameStatus = GameStatus.Starting;
            InstanceFinder.GetInstance<TimingSystemUI>().StopTimer();

            //�ر�һЩUI������ת��
            //�������д�λ
            //���¼��㵹��ʱ
            //�����̳У�Ѫ���̳�
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
                //��¼����״̬
                foreach (var v in myUnits)
                {
                    BodyOrgan body = v.FindOrganInBody<BodyOrgan>(ComponentType.body);
                    //���ڱ���Ｔ�������ٻ������Ҫ����
                    if (body.PosID != 0 && myUnitStartPos.ContainsKey(body.PosID) == true)
                    {
                        MagicOrgan magicOrgan = v.FindOrganInBody<MagicOrgan>(ComponentType.magic);
                        myUnitsStatus.Add((body.PosID, new Vector2Int(body.Health_Curr, magicOrgan.MagicPoint_Curr)));

                    }
                    //��������
                    mainSystem.RemoveUnitFromGame(v, 0);
                }
                //���ճ����е���Ч��Ͷ����
                normalUtility.ORPC_ClearScene();

                //��������һ��Ҫ������֮ǰ����Ȼ���Bug������
                //�������ݰ���--��
                //��λ���ã��������ã���ͼ����
                yield return new WaitForSeconds(1f);
                world.WorldReset();
                yield return new WaitForSeconds(1f);
                //���ɵ���
                HeroAndPosPack enemy = roundEnemies.Dequeue();
                foreach (var u in enemy.List)
                {
                    byte posId = (byte)u.Pos;
                    Debug.Log($"��ʼ��{u.LevelID}��λ-->{PlayerEnum.monster}");
                    UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(u.LevelID, mainSystem);
                    if (unitModel != null)
                    {
                        Debug.Log($"��ʼ��{unitModel.PrefabName}��λ");

                        unitModel.Player = PlayerEnum.monster;
                        GameObject go = normalUtility.Server_SpawnModel($"{ABUtility.UnitMainName}{unitModel.PrefabName}", unitModel.PrefabName, null, 1,null,(u)=>u.transform.position=AStarPathfinding2D.GetNodeWorldPositionV3(startPosManager.PlayerPosDict[PlayerEnum.monster][posId],gridMap));
                        if (go != null)
                            mainSystem.SwapnUnitInPos(unitModel, go, PlayerEnum.monster, startPosManager.PlayerPosDict[PlayerEnum.monster][posId]);
                    }
                }
                yield return new WaitForSeconds(1f);
                //�����Լ���λ
                foreach (var u in myUnitsStatus)
                {
                    byte posId = (byte)u.Item1;
                    int levelId = 0;
                    _ = myUnitStartPos.TryGetValue(posId, out levelId);
                    Vector2Int status = u.Item2;
                    Debug.Log($"��ʼ��{levelId}��λ-->{PlayerEnum.player}");
                    UnitModel unitModel = UnitUtility.GetUnitModelByHeroLevelID(levelId, mainSystem);
                    if (unitModel != null)
                    {
                        Debug.Log($"��ʼ��{unitModel.PrefabName}��λ");

                        unitModel.Player = PlayerEnum.player;
                        GameObject go = normalUtility.Server_SpawnModel($"{ABUtility.UnitMainName}{unitModel.PrefabName}", unitModel.PrefabName, null, 1, (u) => u.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(startPosManager.PlayerPosDict[PlayerEnum.player][posId], gridMap));
                        if (go != null)
                            mainSystem.SwapnUnitInPos(unitModel, go, PlayerEnum.player, startPosManager.PlayerPosDict[PlayerEnum.player][posId], (unit) =>
                            {
                                BodyOrgan bodyOrgan = unit.FindOrganInBody<BodyOrgan>(ComponentType.body);
                                //�̳�Ѫ��������
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
            //����ʱ����ʱ�䣬�Լ�����ʱ��
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            InstanceFinder.GetInstance<TimingSystemUI>().StartTimer(300, 60);
            //���λ��
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
            //�������ʼ��
            FightServerManager.ConsoleWrite_Saber("MainWord Init After Network ");
            MainWorld.StartAfterNetwork();
        }

        public override GameResult SetGameResult()
        {
            //�ж�ʱ�䣬ʱ�������˾���
            if (TimingSystemUI.TimeRemaining == 0)
            {
                FightServerManager.ConsoleWrite_Saber($"Because TimeRemaining is 0��player fail��", System.ConsoleColor.Yellow);
                return GameResult.Fail;
            }

            //�жϻ���û����ʣ��
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
                FightServerManager.ConsoleWrite_Saber($"Because fight win��player win��",System.ConsoleColor.Yellow);

                return GameResult.Success;
            }
 
            else
            {
                FightServerManager.ConsoleWrite_Saber($"Because fight fail��player fail��", System.ConsoleColor.Yellow);
                return GameResult.Fail;

            }
        }

    }
}
