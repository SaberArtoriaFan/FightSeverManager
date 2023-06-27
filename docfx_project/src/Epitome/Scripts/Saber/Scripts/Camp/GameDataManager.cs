using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Saber.Base;
//Saber阿尔托莉雅
namespace Saber.Camp
{
    public class GameDataManager : AutoSingleton<GameDataManager>
    {
        [ShowInInspector]
        private byte playerNum_Max=12;

        public byte PlayerNum_Max { get => playerNum_Max; }
    }
}
