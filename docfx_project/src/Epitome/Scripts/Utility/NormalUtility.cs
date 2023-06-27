using FishNet;
using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting.Tugboat;
using Saber.Base;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
//using network
namespace XianXia
{
    public class NormalUtility : NetworkBehaviour, IGameOverHandler
    {
        static ulong id = 0;
        Utility_ShapeShiftManager shapeShiftManager;
#if UNITY_EDITOR||!UNITY_SERVER
        RisingSpaceUI risingSpaceUI;
        Client_ProjectileManager projectileManager;
        Client_EffectSystem client_Effect;
        private Action<GameResult> OnGameResultedEvent;
#endif
#if UNITY_EDITOR || UNITY_SERVER
        //[SerializeField]
        //IStartAfterNetwork gameWorld;
       public  bool clientLogin = false;
        [SerializeField]
        float timeScale = 2;
        public event Action OnStartAfterNetwork;
        PoolManager poolManager;
        public event Action<NetworkConnection> ClientOfflineEvent;
#endif

        //NetworkUtility networkUtility;

        public event Action<NetworkConnection> OnClientEnterEvent;

        public event Action<GameObject> UnitDeadClientAction;
        [SerializeField]
        bool isClientInitOver = false;
        public static ulong GetId()
        {
            if (id == ulong.MaxValue)
                id = 0;
            return id++;
        }
        private void Awake()
        {
            if (InstanceFinder.GetInstance<NormalUtility>() != null) { Debug.LogError("Cant Find"); return; }
            InstanceFinder.RegisterInstance(this, false);
            shapeShiftManager = new Utility_ShapeShiftManager();
#if UNITY_SERVER
            InstanceFinder.ServerManager.OnAuthenticationResult += OnClientEnter;
        }
        private void Start()
        {
#if UNITY_SERVER
            TimerManager.Instance.AddTimer(() => OnStartAfterNetwork?.Invoke(), Time.deltaTime);
#endif


        }
        private void OnDestroy()
        {
            shapeShiftManager?.OnDestroy();
            OnClientEnterEvent = null;
            UnitDeadClientAction = null;
#if UNITY_SERVER
            if (InstanceFinder.ServerManager != null)
                InstanceFinder.ServerManager.OnAuthenticationResult -= OnClientEnter;
#endif
        }
        [Server]
        void OnClientEnter(NetworkConnection conn, bool res)
        {
#if UNITY_SERVER

            if (res == true)
            {
                OnClientEnterEvent?.Invoke(conn);
            }
#endif
        }

        /// <summary>
        /// 客户端连接后向服务器请求更新数据
        /// </summary>
        /// <param name="conn"></param>
        [ServerRpc(RequireOwnership = false)]
        void InitClientWhenInited(NetworkConnection conn)
        {
#if UNITY_SERVER
            isClientInitOver = true;
#endif
        }
        [Server]
        private void InitServer()
        {
#if UNITY_SERVER 

            //InstanceFinder.ServerManager.OnAuthenticationResult += SynchronizeResources;

            GameManagerBase.Instance.OnGameOverEvent += (r) => ORPC_GameResultedEvent((int)r);
            //gameWorld.StartAfterNetwork();

            //Console.WriteLine($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
            Console.WriteLine($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
#else
            Debug.Log($"Saber:Enter GameScene,IP:{InstanceFinder.NetworkManager.TransportManager.Transport.GetServerBindAddress(FishNet.Transporting.IPAddressType.IPv4)};Port:{InstanceFinder.NetworkManager.TransportManager.Transport.GetPort()}");
#endif

        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            //Debug.Log("9999999");
#if UNITY_SERVER
            poolManager = PoolManager.Instance;
            TimerManager.Instance.AddTimer(InitServer, 0.5f);
            //InitServer();
            //networkUtility = InstanceFinder.GetInstance<NetworkUtility>();
            //Debug.Log(networkUtility.name + "");
            //GameManager.MainWorld.StartAfterNetwork();
#endif
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
#if UNITY_SERVER
            //if (InstanceFinder.ServerManager != null)
            //    InstanceFinder.ServerManager.OnAuthenticationResult -= SynchronizeResources;
            OnClientEnterEvent = null;
#endif
        }
        public override void OnStartClient()
        {
            base.OnStartClient();


#if !UNITY_SERVER

            TimerManager.Instance.AddTimer(InitClient, 0.5f);
#endif
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            ServerRpc_ClientOfflineEvent(InstanceFinder.ClientManager.Connection);
        }
        /// <summary>
        /// 在客户端上线时会调用该函数
        /// </summary>
        [Client]
        void InitClient()
        {
#if !UNITY_SERVER
            Client_InstanceFinder.Register<IGameOverHandler>(this,"NormalUtility");
            Debug.Log("注册normalutility" + Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility"));
            Client_InstanceFinder.GetInstance<XianXia.Unit.Client_TimeScaleUI>().OnSetTimeScale+= ServerRpc_SetTimeScale;
            //networkUtility = InstanceFinder.GetInstance<NetworkUtility>();
            Client_InstanceFinder.StartAfterNetwork();
            shapeShiftManager.StartAfterNetwork();
            //base.GiveOwnership(ClientManager.Connection);
            TimerManager.Instance.AddTimer(() => { 
                risingSpaceUI = Client_InstanceFinder.GetInstance<RisingSpaceUI>();
                projectileManager = Client_InstanceFinder.GetInstance<Client_ProjectileManager>();
                client_Effect = Client_InstanceFinder.GetInstance<Client_EffectSystem>();
                Debug.Log("Xxx");
                InitClientWhenInited(ClientManager.Connection);
            }, Time.deltaTime);
            Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
            foreach(var v in canvases)
            {
                if (v.name != "MainCanvas")
                    v.gameObject.SetActive(false);
            }
            ServerRpc_Login();
#endif
        }


        #region 人物属性
        #region 公共
        #region Spawn
        /// <summary>
        /// 保证安全的生产方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="initNum"></param>
        /// <returns></returns>
        [Server]
        public GameObject Server_SpawnModel(string path, string name, Transform parent, int initNum = 0, Action<GameObject> recycleAction = null, Action<GameObject> initAction = null)
        {

#if UNITY_SERVER
            _ = Server_InitSpawnPool(path, name, parent, initNum, recycleAction, initAction);
            return poolManager.GetObjectInPool<GameObject>(name);
#else
            return null;
#endif
        }
        [ObserversRpc(ExcludeServer =false,RunLocally =true)]
        public void ORPC_StrongSyncPosition(GameObject gameObject,Vector3 pos)
        {
            gameObject.transform.position = pos;
        }
        /// <summary>
        /// 非安全方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Server]
        public GameObject Server_SpawnModel(string name)
        {
            //if (!poolManager.IsPoolAlive(name))
            //{
            //    GameObject model = ABUtility.Load<GameObject>(path + name);
            //    poolManager.AddPool<GameObject>(() => { return GameObject.Instantiate(model); }, (u) => { Despawn(u, DespawnType.Pool); u.SetActive(false); }, (u) => { Spawn(u); }, name);
            //}
#if UNITY_SERVER
            return poolManager.GetObjectInPool<GameObject>(name);
#else
            return null;
#endif
        }
        [Server]
        public void Server_DespawnAndRecycleModel(GameObject go, string name)
        {
#if UNITY_SERVER
            //if (poolManager.IsPoolAlive(name))
            //    poolManager.RecycleToPool<GameObject>(go, name);
            //else
                Despawn(go, DespawnType.Destroy);
#endif
        }
        [Server]
        public Saber.Base.ObjectPool<GameObject> Server_InitSpawnPool(string path, string name, Transform parent, int initNum = 0, Action<GameObject> recycleAction = null, Action<GameObject> initAction = null)
        {
#if UNITY_SERVER
            if (!poolManager.IsPoolAlive(name))
            {
                GameObject go = AddSpawnablePrefabs(InstanceFinder.NetworkManager, path);
                //OnClientEnterEvent += (n) => { /*Debug.Log(*//*InstanceFinder.GetInstance<NetworkUtility>().*/TRPC_AddSpawnablePrefabs(n, path); } ;
                //ORPC_AddSpawnablePrefabs(path);
                return poolManager.AddPool<GameObject>(() => { GameObject model = GameObject.Instantiate(go); model.name = name; return model; }, (u) => { recycleAction?.Invoke(u);Despawn(u, DespawnType.Pool); u.SetActive(false); u.transform.SetParent(parent); }, (u) => {  Spawn(u); initAction?.Invoke(u); }, name);
            }
            else
                return poolManager.GetPool<GameObject>(name);
#else
            return null;
#endif
        }
        [Server]
        public void Server_UnitDead(GameObject go)
        {
#if UNITY_SERVER

            if (go == null) return;
            Client_UnitProperty client_UnitProperty = go.GetComponent<Client_UnitProperty>();
            if (client_UnitProperty != null) client_UnitProperty.HealthPointPer = 0;
            ORPC_UnitDeadClientAction(go);
#endif

        }
        //[Server]
        //public void Server_ShapeShift(GameObject go,string modelPath,string name)
        //{

        //}
        [ObserversRpc(RunLocally = true)]
        public void ORPC_ShapeShift(GameObject go, string modelPath, string name)
        {
            //因为需要同步动画器，所以服务器也要更新
            shapeShiftManager.ShapeShift(go, modelPath, name);
        }
        [ObserversRpc]
        private void ORPC_UnitDeadClientAction(GameObject go)
        {
#if !UNITY_SERVER
            //Client_UnitDead();
            UnitDeadClientAction?.Invoke(go);
#endif
        }



        [ObserversRpc]
        private void ORPC_AddSpawnablePrefabs(string path)
        {
#if !UNITY_SERVER

            _ = AddSpawnablePrefabs(InstanceFinder.NetworkManager,path);
#endif

        }
        //不能在这里发送，因为客户端验证后事件立即触发，而此时该脚本还没有加载
        [TargetRpc]
        public void TRPC_AddSpawnablePrefabs(NetworkConnection connection, string path)
        {
#if !UNITY_SERVER

            AddSpawnablePrefabs(connection.NetworkManager, path);
#endif

        }
        private GameObject AddSpawnablePrefabs(NetworkManager networkManager, string path)
        {
            NetworkObject networkObject = ABUtility.Load<GameObject>(path).GetComponent<NetworkObject>();

            //networkManager.SpawnablePrefabs.AddObject(networkObject, true);
            return networkObject.gameObject;
            //return networkUtility.AddSpawnablePrefabs(networkManager, path, name);
        }
        #endregion


        [Server]
        public Client_UnitProperty Server_AddUnitProperty(GameObject go)
        {
#if UNITY_SERVER
            //NetworkObject networkObject = go.GetComponent<NetworkObject>();
            //if (networkObject == null) return null;
            Client_UnitProperty unitProperty = go.GetComponent<Client_UnitProperty>();

            //Client_UnitProperty unitProperty =networkObject.AddAndSerialize<Client_UnitProperty>();
            //if (unitProperty == null) unitProperty = go.AddComponent<Client_UnitProperty>();
            ORPC_AddUnitProperty(unitProperty.NetworkObject);
            return unitProperty;
#else
            return null;
#endif
        }

        [Server]
        public void Server_RemoveUnitProperty(GameObject go)
        {
#if UNITY_SERVER
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null) return;
            ORPC_RemoveUnitProperty(networkObject);
#endif
        }
//        [ServerRpc(RequireOwnership =false)]
//        public void ServerRpc_StartGame()
//        {
//#if UNITY_SERVER
//            //GameManagerBase.Instance
//            //Servertest
//#endif
//        }
        [ServerRpc(RequireOwnership = false)]
        public void ServerRpc_SetTimeScale(float speed)
        {
#if UNITY_SERVER
            if (timeScale < 0) timeScale = 2;
            if (speed > timeScale) speed = timeScale;
            Time.timeScale = speed;
#endif
        }
        [Server]
        public void Server_SetUnitColor(GameObject go, Color color)
        {
#if UNITY_SERVER
            NetworkObject networkObject = go.GetComponent<NetworkObject>();
            if (networkObject == null) return;
            ORPC_ChangeUnitColor(networkObject, color);
#endif
        }
        //[Server]
        //public void Server_SetAnimatorParameter_Trigger(Client_UnitProperty c, int id)
        //{

        //    ORPC_SetAnimatorParameter_Trigger(c, id);
        //}

        [ObserversRpc]
        public void ORPC_StartDirector(float time)
        {
#if !UNITY_SERVER
            var client = Client_InstanceFinder.GetInstance<XianXia.Client.Client_InputController>();
            if (client == null) return;
            Vector3 y = Vector3.up * ((client.HightLimit.x + client.HightLimit.y) / 2 + client.y_offest);
            XianXia.Client.Client_InputController.StartDirector(client.cameraTarget.position, Vector3.right * (client.WidthLimit.x + client.WidthLimit.y) / 2 + y + client.cameraTarget.transform.position.z * Vector3.forward, time, false);
#endif

        }
        /// <summary>
        /// 这个pos的x，y分别代表对应点映射的比例
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="time"></param>
        [ObserversRpc]
        public void ORPC_StartDirectorToFight(Vector2 pos,float time)
        {


#if !UNITY_SERVER
              var client = Client_InstanceFinder.GetInstance<XianXia.Client.Client_InputController>();
            if (client == null) return;
            //XianXia.Client.Client_InputController.StartDirector(client.cameraTarget.position,pos, time, true);
            //Vector2 res = new Vector2();
            //res.x = (pos.x - client.BackGroundX.x) / (client.BackGroundX.y - client.BackGroundX.x);
            //res.x = (pos.y - client.BackGroundY.x) / (client.BackGroundY.y - client.BackGroundY.x);
            //pos = res;
            Vector3 y = Vector3.up * ((client.HightLimit.x + client.HightLimit.y) / 2 + client.y_offest);
            XianXia.Client.Client_InputController.StartDirector(client.cameraTarget.position, new Vector3(pos.x,pos.y,client.cameraTarget.position.z), time, true);
#endif

        }
        [ObserversRpc]
        public void ORPC_CreateUnitEffect(string effectName, string key, GameObject parent, Vector3 offestPos, bool isAutoRecycle)
        {

#if !UNITY_SERVER
            if (parent == null) return;
            client_Effect.CreateEffectInPool_Main(effectName, key,parent,offestPos, isAutoRecycle);
#endif
        }
        [ObserversRpc]
        public void ORPC_CreateEffect(string effectName, GameObject parent, Vector3 pos, bool isAutoRecycle)
        {

#if !UNITY_SERVER
            if (parent == null) return;
            parent.transform.position = pos;
            client_Effect.CreateEffectInPool_Main(effectName, parent,Vector3.zero, isAutoRecycle);
#endif
        }
        [ObserversRpc]
        public void ORPC_CreateEffect(string effectName, string key, Vector3 pos, Vector3 rotate = default, Vector3 scale = default, bool isAutoRecycle = false)
        {
#if !UNITY_SERVER
            client_Effect.CreateEffectInPool_Main(effectName, key, pos, rotate, scale, isAutoRecycle);
#endif
            //if (parent == null) return;
        }
        [ObserversRpc]
        public void ORPC_RecycleEffect(GameObject parent)
        {
#if !UNITY_SERVER
            if (parent == null) return;
            client_Effect.RecycleEffect(parent);
#endif
        }
        [ObserversRpc]
        public void ORPC_CreateProjectile(GameObject parent, Vector3 pos, string name)
        {
#if !UNITY_SERVER
            parent.transform.position = pos;
            projectileManager.InitProjectile(parent, name);
#endif
        }
        [ObserversRpc]
        public void ORPC_RecycleProjectile(GameObject parent)
        {
#if !UNITY_SERVER
            if (parent == null) return;
            projectileManager.RecycleProjectile(parent);
#endif
        }
        [ObserversRpc]
        public void ORPC_RecycleEffect(string key)
        {
#if !UNITY_SERVER
            client_Effect.RecycleEffect(key);
#endif
            //if (parent == null) return;
        }
        [ObserversRpc]
        public void ORPC_ShowRisingSpace(string s, Vector3 worldPos, Vector3 dir, Color color = default, int size = 24, FontStyles fontStyles = FontStyles.Normal, float speed = 1, float continueTime = 1.5f)
        {
#if !UNITY_SERVER
            risingSpaceUI.ShowRisingSpace(s, worldPos, dir, color, size, fontStyles, speed, continueTime);
#endif
        }
        [ObserversRpc]
        public void ORPC_SetRotateY(GameObject gameObject, float value)
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.up * value);
        }
        [ObserversRpc]
        public void ORPC_SetAnimatorParameter_Trigger(Client_UnitProperty c, int id)
        {
#if !UNITY_SERVER
            if (c != null) c.SetAnimatorParameter_Trigger(id);
#endif
        }
        [ObserversRpc]
        public void ORPC_SetAnimatorParameter_Bool(Client_UnitProperty c, int id, bool value)
        {

#if !UNITY_SERVER
            if (c != null) c.SetAnimatorParameter_Bool(id,value);
#endif
        }

        [ObserversRpc]
        public void ORPC_SpawnUnit(GameObject go)
        {
#if !UNITY_SERVER
            SortingGroup sortingGroup = go.GetComponent<SortingGroup>();
            if (sortingGroup == null)
                sortingGroup = go.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Unit";
            sortingGroup.sortingOrder = 0;
#endif
        }
        [ObserversRpc]
        public void ORPC_ClearScene()
        {
#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_EffectSystem>().Clear();
            Client_InstanceFinder.GetInstance<Client_ProjectileManager>().Clear();
#endif
        }
        //[ObserversRpc]

        #endregion
        #region 私人
        [ServerRpc(RequireOwnership = false)]
        private void ServerRpc_ClientOfflineEvent(NetworkConnection conn)
        {
#if UNITY_SERVER
            ClientOfflineEvent?.Invoke(conn);

#endif
        }
        [ObserversRpc]
        private void ORPC_GameResultedEvent(int value)
        {
            Debug.Log("ClientGAMEOVER");
#if !UNITY_SERVER
            OnGameResultedEvent?.Invoke((GameResult)value);
#endif
        }
        //添加属性
        [ObserversRpc]
        private void ORPC_AddUnitProperty(NetworkObject networkObject)
        {

#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().AddUnitProperty(networkObject);
#endif
        }
        [ObserversRpc]
        private void ORPC_RemoveUnitProperty(NetworkObject networkObject)
        {

#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().RemoveUnitProperty(networkObject);
#endif
        }
        [ObserversRpc]
        private void ORPC_ChangeUnitColor(NetworkObject networkObject, Color color)
        {

#if !UNITY_SERVER
            Client_InstanceFinder.GetInstance<Client_UnitPropertyUI>().SetUnitColor(networkObject, color);
#endif
            //if (go == null) return;
            //NetworkObject networkObject = go.GetComponent<NetworkObject>();
        }
        [Client]
        void IGameOverHandler.AddGameSuccAndFailEvent(Action<GameResult> gameOverEvent, bool isOnce)
        {
#if !UNITY_SERVER || UNITY_EDITOR

            Debug.Log("成功添加游戏结束事件");
            if (isOnce)
            {
                Action<GameResult> action = null;
                action = (r) =>
                {
                    gameOverEvent?.Invoke(r);
                    OnGameResultedEvent -= action;
                };
                OnGameResultedEvent += action;
            }
            else
                OnGameResultedEvent += gameOverEvent;
#endif
        }
        [Client]
        void IGameOverHandler.RemoveGameSuccAndFailEvent(Action<GameResult> gameOverEvent)
        {
#if !UNITY_SERVER || UNITY_EDITOR
            OnGameResultedEvent -= gameOverEvent;
#endif
        }

#endregion
#endregion
        [ServerRpc(RequireOwnership =false)]
        public void ServerRpc_Login()
        {
#if UNITY_EDITOR || UNITY_SERVER
            clientLogin = true;
#endif
        }
    }
}
