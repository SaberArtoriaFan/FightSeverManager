using Sirenix.OdinInspector;
using UnityEngine;
using Saber.Camp;
using Saber.ECS;
using UnityEngine.UI;
using FishNet;
using FishNet.Object;
using Saber.Base;
namespace XianXia
{
    public class Test : NetworkBehaviour
    {
        //[SerializeField]
        //Vector3 startPos;
        [SerializeField]
        Transform enemysTR;
        [SerializeField]
        Transform playersTR;

        [SerializeField]
        UnityEngine.UI.Button button;
        [SerializeField]
        GameObject allButtonParent;
        private void Start()
        {
            //unit = FindObjectOfType<UnitBase>();
            //if (unit.FindOrganInBody<LegOrgan>(ComponentType.leg) == null)
            //if(button==null)
            //    button=FindObjectOfType<Button>();
            
#if !UNITY_SERVER
            Canvas canvas = BaseUtility.GetMainCanvas();
            Button[] trs = canvas.GetComponentsInChildren<Button>();
            foreach(var v in trs)
            {
                if (v.name == "BianShen")
                {
                    button = v;
                    allButtonParent = button.transform.parent.gameObject;
                    break;
                }
            }

            button.onClick.AddListener(() =>
            {
                Test1();
                allButtonParent.SetActive(false);
                button.gameObject.SetActive(false);
            });
#endif
            //AStarPathfinding2D map=FindObjectOfType<AStarPathfinding2D>();
            //Vector2Int mapInt= map.allNodeArray[99].Position;
            //UnityEngine.Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(mapInt, map);
            //Debug.Log("v2" + mapInt + "v3"+pos+"_v2"+AStarPathfinding2D.GetWorldPositionNodePos(pos,map));
            //Debug.Log(Application.persistentDataPath+"888");
            //#if UNITY_ANDROID
            //Test1();
            //#endif
        }
        [Button]
        [ServerRpc(RequireOwnership =false)]
        void Test1()
        {
#if UNITY_SERVER
            FightServerManager.ConsoleWrite_Saber("双方开始交战！！！");
            GameManager.Instance.ChangeGameStatus(GameStatus.Starting);
            InstanceFinder.GetInstance<XianXia.Unit.TimingSystemUI>().StartTimer(180, 30);
            XianXia.Unit.UnitMainSystem mainSystem = GameManager.MainWorld.FindSystem<XianXia.Unit.UnitMainSystem>();
            XianXia.Unit.Projectile projectile = new XianXia.Unit.Projectile("Cyclone", 2, 0);
            XianXia.Unit.Projectile temp;


            playersTR.gameObject.SetActive(true);
            enemysTR.gameObject.SetActive(true);

            XianXia.Unit.TestUnitModel[] players=playersTR.GetComponentsInChildren<XianXia.Unit.TestUnitModel>();
            XianXia.Unit.TestUnitModel[] enemy = enemysTR.GetComponentsInChildren<XianXia.Unit.TestUnitModel>();

            for (int i=0;i<enemy.Length;i++)
            {
                XianXia.Unit.TestUnitModel unit = enemy[i];
                unit.player = PlayerEnum.monster;
                mainSystem.Test_SwapnUnit(unit);
                
            }
            for (int i = 0; i < players.Length; i++)
            {
                XianXia.Unit.TestUnitModel unit = players[i];
                unit.player = PlayerEnum.player;

                mainSystem.Test_SwapnUnit(unit);
            }
#endif
        }
    }
}
