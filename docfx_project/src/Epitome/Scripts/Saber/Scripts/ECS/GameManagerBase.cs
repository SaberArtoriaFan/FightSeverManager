using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Saber.ECS
{
    public enum GameStatus
    {
        None,
        Starting,
        Finish
    }
    public enum GameResult
    {
        None,
        Success,
        Fail
    }
    public interface IGameOverHandler
    {
        void AddGameSuccAndFailEvent(Action<GameResult> gameOverEvent, bool isOnce);

        void RemoveGameSuccAndFailEvent(Action<GameResult> gameOverEvent);
    }
    public abstract class GameManagerBase : Singleton<GameManagerBase>
    {


        [SerializeField]
        protected WorldBase world;
        [SerializeField]
        protected bool isPasuedWhenFinishGame = true;
        protected GameStatus gameStatus = GameStatus.None;
        protected GameResult gameResult = GameResult.None;



        public event Action<GameResult> OnGameOverEvent;

        public GameStatus _GameStatus { get => gameStatus;  }
        public GameResult _GameResult { get => gameResult;  }
        public WorldBase World { get => world;  }

        protected virtual bool IsNotTimeToFinishGame()
        {
            return false;
        }
        public abstract GameResult SetGameResult();
        public virtual void ChangeGameStatus(GameStatus gameStatus)
        {
            this.gameStatus = gameStatus;
            bool isFinish = true;
            if (this.gameStatus == GameStatus.Finish)
            {
                gameResult= SetGameResult();
                isFinish = !IsNotTimeToFinishGame();
                switch (gameResult)
                {
                    case GameResult.Success:
                        //还没结束，还有下拨
                        break;
                    case GameResult.Fail:
                        break;
                }
                if (!isFinish) return;
                Debug.Log("触发游戏结束事件");
                OnGameOverEvent?.Invoke(gameResult);
            }
        }

        public static T FindSystem<T>() where T : class, IMono, new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystem<T>();
        }
        public static NormalSystemBase<T> FindSystemByComponent<T>() where T : ComponentBase,new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystemByComponent<T>();
        }
        public static NormalSystemBase<T> FindSystemByComponent<T>(T component) where T : ComponentBase, new()
        {
            if (instance == null || instance.world == null) return null;
            return instance.world.FindSystemByComponent(component);
        }
        public static void DesotryComponent<T>(T component) where T : ComponentBase, new()
        {
            if (instance == null || instance.world == null) return;
            instance.world.DesotryComponent(component);
        }
        protected virtual void Start()
        {
            if (world == null) world = FindObjectOfType<WorldBase>();

        }
    }
}
