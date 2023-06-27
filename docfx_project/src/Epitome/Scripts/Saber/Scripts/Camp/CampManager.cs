using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
//Saber阿尔托莉雅
namespace Saber.Camp
{
    public enum CampRelation
    {
        none,
        friendly,
        hostile,
        neutral
    }
    public enum PlayerEnum
    {
        player,
        monster
    }
    [Serializable]
    public class Camp
    {
        private static readonly List<Camp> allCamps = new List<Camp>(6);
    
        private List<PlayerMemeber> campMembers;
        public static List<Camp> AllCamps => allCamps;

        public byte CampId { get => (byte)allCamps.IndexOf(this); }

        public Camp(PlayerMemeber[] members)
        {
            campMembers = new List<PlayerMemeber>(members.Length);
            foreach (PlayerMemeber member in members)
            {
                Add(member);
            }
            allCamps.Add(this);
        }
        public Camp(PlayerMemeber memeber,CampRelation campRelation)
        {
            campMembers = new List<PlayerMemeber>(1);
            Add(memeber);
            allCamps.Add(this);
            CampManager.Instance.InitCamp(this, campRelation);
        }


        public void Add(PlayerMemeber member)
        {
            if (!campMembers.Contains(member)&&member.BelongCamp==null)
            {
                campMembers.Add(member);
                member.ChangeCamp(this);
            }
        }
        public bool Contains(PlayerMemeber member)
        {
            if (campMembers.Contains(member))
                return true;
            else
                return false;
        }
        public void Remove(PlayerMemeber member)
        {
            if (campMembers.Contains(member))
            {
                campMembers.Remove(member);
                member.ChangeCamp(null);
            }
        }
        public PlayerMemeber[] GetAllPlayers()
        {
            return campMembers.ToArray();
        }
    }
    public class CampManager : AutoSingleton<CampManager>
    {
        protected Dictionary<(Camp, Camp), CampRelation> campRelation;
        public Func< PlayerEnum,PlayerMemeber> GetPlayerFunc;
        public Func< PlayerMemeber, PlayerEnum> GetPlayerEnumFunc;

        [SerializeField]
        List<Color> colors = new List<Color>();
        public static Color PlayerGetColor()
        {
            if (instance.colors.Count <= 0) return Color.red;
            Color color = instance.colors[0];
            instance.colors.RemoveAt(0);
            return color;
        }
        protected byte CalculateRelationCount()
        {
            return (byte)(((byte)Mathf.Pow(GameDataManager.Instance.PlayerNum_Max+2, 2.0f) - GameDataManager.Instance.PlayerNum_Max+2)/2);
        }
        protected override void Init()
        {
            base.Init();
            campRelation = new Dictionary<(Camp, Camp), CampRelation>(CalculateRelationCount());
            //PlayerMemeber neutralPlayer = new PlayerMemeber();
            //Camp neutralCamp = new Camp(neutralPlayer,CampRelation.neutral);
            //PlayerMemeber hostilePlayer = new PlayerMemeber();
            //Camp hostileCamp = new Camp(hostilePlayer, CampRelation.hostile);

        }

        public void InitCamp(Camp camp,CampRelation campRelation=CampRelation.neutral)
        {
            foreach(Camp c in Camp.AllCamps)
            {
                if (c != camp)
                {
                    ChangeRealtion(camp, c, campRelation);
                }
            }
        }

        public CampRelation CampsRealtion(Camp left,Camp right)
        {
            if (left == right) return CampRelation.friendly;
            if (campRelation.ContainsKey((left, right)))
                return campRelation[(left, right)];
            else if (campRelation.ContainsKey((right, left)))
                return campRelation[(right, left)];
            else
            {
                campRelation.Add((left, right), CampRelation.neutral);
                return CampRelation.neutral;
            }
        }

        public void ChangeRealtion(Camp left, Camp right,CampRelation relation)
        {
            if (left == right) return;
            if (left.CampId != 0 && left.CampId != 1 && right.CampId != 0 && right.CampId != 1)
            {
                if (campRelation.ContainsKey((left, right)))
                    campRelation[(left, right)] = relation;
                else if (campRelation.ContainsKey((right, left)))
                    campRelation[(right, left)] = relation;
                else
                    campRelation.Add((left, right), relation);
            }
            else
            {
                if (left.CampId == 1 || right.CampId == 1)
                {
                    ChangeCampRelation(left, right, CampRelation.neutral);
                    return;
                }
                else if(left.CampId == 0 || right.CampId ==0)
                {
                    ChangeCampRelation(left, right, CampRelation.hostile);
                    return;
                }
                else
                {
                    ChangeCampRelation(left, right, relation);
                    return;
                }
            }
            //方法声明
             void ChangeCampRelation(Camp left, Camp right, CampRelation relation)
            {
                if (campRelation.ContainsKey((left, right)))
                    campRelation[(left, right)] = relation;
                else if (campRelation.ContainsKey((right, left)))
                    campRelation[(right, left)] = relation;
                else
                    campRelation.Add((left, right), relation);
            }
        }
        public static PlayerMemeber GetPlayerMemeber(PlayerEnum playerEnum)
        {
            return instance.GetPlayerFunc?.Invoke(playerEnum);
        }
        public static PlayerEnum GetPlayerEnum(PlayerMemeber player)
        {
            if (instance.GetPlayerEnumFunc != null) return instance.GetPlayerEnumFunc(player);
            else return default;
        }

        protected override void Awake()
        {
            base.Awake();
            //Init();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Camp.AllCamps.Clear();
        }
    }
}
