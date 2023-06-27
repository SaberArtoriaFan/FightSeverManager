using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Saber阿尔托莉雅
namespace Saber.Camp
{
    [Serializable]
    public class PlayerMemeber
    {
        private static  byte Id = 0;
        [SerializeField]
        private  byte playerId = 0;
        [SerializeField]
        private Camp belongCamp;
        [SerializeField]
        bool isAI=false;
        [SerializeField]
        Color color;
        public PlayerMemeber(Camp camp=null,Color color=default,bool isAI=false)
        {
            this.playerId = Id;
            Id++;
            //Debug.Log("!!!!");
            this.belongCamp = null;
            if(camp!=null)
                camp.Add(this);
            this.isAI = isAI;
            this.color = color;
            if(color==default) color = CampManager.PlayerGetColor();
        }

        public byte PlayerId{get => playerId;}
        public Camp BelongCamp { get => belongCamp;}
        public bool IsAI { get => isAI;  }
        public Color Color { get => color; }

        internal void ChangeCamp(Camp camp)
        {
            belongCamp = camp;
        }
    }
}
