using FishNet;
using FishNet.Object;
using Proto;
using Saber.Camp;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;
using XianXia.Unit;

namespace XianXia
{
    public class ServerTest : Singleton<ServerTest>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
        void AddHero(int i,int pos,HeroAndPosPack heroAndPosPack)
        {
            NGridPack nGridPack = new NGridPack();
            nGridPack.LevelID = i;
            nGridPack.Pos = pos;
            heroAndPosPack.List.Add(nGridPack);
        }
        [Button]
       public  void Test()
        {
            Debug.Log("SERVERtEST1");
            MainPack mainPack = new MainPack();
            HeroAndPosPack heroAndPosPack1 = new HeroAndPosPack();

            //AddHero(3000170, 16, heroAndPosPack1);
            //AddHero(3000170, 5, heroAndPosPack1);
            //AddHero(3000170, 15, heroAndPosPack1);

            //AddHero(4000250, 8, heroAndPosPack1);
            //AddHero(4000250, 1, heroAndPosPack1);
            //AddHero(4000250, 12, heroAndPosPack1);
            AddHero(4000229,1, heroAndPosPack1);
            AddHero(3000129, 12, heroAndPosPack1);
            AddHero(2000129, 5, heroAndPosPack1);
            AddHero(4000229, 2, heroAndPosPack1);
            AddHero(3000129, 8, heroAndPosPack1);
            mainPack.HeroAndPosList.Add(heroAndPosPack1);

            HeroAndPosPack heroAndPosPack2 = new HeroAndPosPack();
            //AddHero(3000170, , heroAndPosPack2);
            //AddHero(3000170, 1, heroAndPosPack2);
            //AddHero(3000170, 12, heroAndPosPack2);
            AddHero(4000229, 1, heroAndPosPack2);
            AddHero(4000229, 12, heroAndPosPack2);
            HeroAndPosPack heroAndPosPack3 = new HeroAndPosPack();
            //AddHero(3000170, , heroAndPosPack2);
            //AddHero(3000170, 1, heroAndPosPack2);
            //AddHero(3000170, 12, heroAndPosPack2);
            AddHero(3000129, 1, heroAndPosPack3);
            AddHero(3000129, 12, heroAndPosPack3);
            HeroAndPosPack heroAndPosPack4 = new HeroAndPosPack();

            AddHero(4000229, 1, heroAndPosPack4);
            AddHero(4000229, 12, heroAndPosPack4);
            //AddHero(4000250, 16, heroAndPosPack2);
            //AddHero(4000250, 5, heroAndPosPack2);
            //AddHero(4000250, 15, heroAndPosPack2);

            mainPack.HeroAndPosList.Add(heroAndPosPack2);
            mainPack.HeroAndPosList.Add(heroAndPosPack3);
            mainPack.HeroAndPosList.Add(heroAndPosPack4);
            GameManager.NewInstance.InitFight(mainPack);

        }
        [Button]
        public void Test2()
        {
            Debug.Log("SERVERtEST2");
            GameManager.NewInstance.StartFight();
        }
        [Button]
        void Test3()
        {
            Test();
            Test2();
        }
        [Button]
        void Test4()
        {
            MainPack mainPack = new MainPack();
            HeroAndPosPack heroAndPosPack1 = new HeroAndPosPack();

            //AddHero(3000170, 16, heroAndPosPack1);
            //AddHero(3000170, 5, heroAndPosPack1);
            //AddHero(3000170, 15, heroAndPosPack1);

            //AddHero(4000250, 8, heroAndPosPack1);
            //AddHero(4000250, 1, heroAndPosPack1);
            //AddHero(4000250, 12, heroAndPosPack1);
            AddHero(4000229, 1, heroAndPosPack1);
            AddHero(3000129, 12, heroAndPosPack1);

           
            mainPack.HeroAndPosList.Add(heroAndPosPack1);

            HeroAndPosPack heroAndPosPack2 = new HeroAndPosPack();
            //AddHero(3000170, , heroAndPosPack2);
            //AddHero(3000170, 1, heroAndPosPack2);
            //AddHero(3000170, 12, heroAndPosPack2);
            AddHero(4000229, 1, heroAndPosPack2);
            AddHero(3000129, 12, heroAndPosPack2);

            //AddHero(4000250, 16, heroAndPosPack2);
            //AddHero(4000250, 5, heroAndPosPack2);
            //AddHero(4000250, 15, heroAndPosPack2);

            mainPack.HeroAndPosList.Add(heroAndPosPack2);
            mainPack.IpAndPortPack = new IPAndPortPack();
            mainPack.IpAndPortPack.Ip = " ";
            mainPack.IpAndPortPack.Port = 7070;

            XianXiaControllerInit.StartFightFishNetServer(mainPack);
        }
    }

}
