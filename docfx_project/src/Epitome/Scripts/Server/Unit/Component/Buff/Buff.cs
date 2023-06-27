using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Saber.ECS;
using cfg.buff;
namespace XianXia.Unit
{
    public enum BuffPriorityType
    {
        weak,
        strong,
        forever
    }
    public interface IAllowOwnNum 
    {
        public int AllowOwnMaxNum { get; }

    }

    public class Buff : IComponentBase, IAllowOwnNum, IRealName
    {


        public static bool IsCanDisperse(BuffPriorityType buffPriorityType, BuffPriorityType disperseType)
        {
            bool res = false;
            if (((ushort)buffPriorityType) <= ((ushort)disperseType))
                res = true;
            else
                res = false;

            return res;
        }

        private UnitBase source;

        protected bool isOver = false;
        protected StatusOrgan statusBar;
        private bool isForever = false;
        protected string effectName;
        protected int effectOffectPos;
        protected bool enable=true;
        public int AllowOwnMaxNum => 1;

        public int Curr_ContinueRoundNum { get; internal set; }
        public virtual Sprite BuffIcon { get; }
        public uint BuffId { get; }
        public virtual string BuffDescription { get; }
        public virtual bool IsDeBuff { get; internal set; }
        public virtual BuffPriorityType DispelPriority { get; protected set; }
        public virtual string BuffShowName { get; }
        public virtual string RealName { get; internal set; }
        public bool IsForever { get => isForever; set => isForever = value; }
        public bool Enable { get => enable; set => enable = value; }
        public IContainerEntity Owner { get => statusBar; set => statusBar = (StatusOrgan)value; }

        public ComponentType ComponentType => ComponentType.none;

        public UnitBase Source { get => source; set => source = value; }



        //private Action<StatusHeartOrgan> enterAction;
        //private Action<StatusHeartOrgan> roundAction;
        //private Action<StatusHeartOrgan> exitAction;
        //const string buffABPackage = "fightinfo";
        public Buff()
        {

        }
        //protected Buff(string buffName, BuffEventInfo buffEventInfo)
        //{
        //    buffInfo = ABManager.Instance.LoadResource<BuffInfo>(buffABPackage, buffName);
        //    if (buffInfo == null)
        //    {
        //        buffInfo = ABManager.Instance.LoadResource<BuffInfo>(buffABPackage, "DefaultBuffInfo");
        //        Debug.LogWarning("BuffInfoError");
        //    }
        //    //EnterAction = enterAction;
        //    //RoundAction = roundAction;
        //    //ExitAction = exitAction;
        //    this.buffEventInfo = buffEventInfo;
        //    isOver = false;
        //    //effectDict = new Dictionary<ParticleSystem, string>();
        //}


        //public virtual void SetEffect(string effectName, CharaterPartPoint placePos, Vector3 errorV3)
        //{
        //    ParticleSystem effect = EffectManager.Instance.GetEffectInPool(effectName);
        //    if (effect != null)
        //    {
        //        Transform place = chess.GetEquipPart(placePos);
        //        effect.transform.SetParent(place);
        //        effect.transform.localPosition = Vector3.zero;
        //        effect.transform.localPosition += errorV3;
        //        effect.gameObject.SetActive(true);
        //        effect.Play();
        //        if (effectDict == null) effectDict = new Dictionary<ParticleSystem, string>();
        //        effectDict.Add(effect, effectName);
        //    }
        //}
        //public virtual void RemoveEffect()
        //{
        //    if (effectDict == null) return;
        //    foreach (var v in effectDict)
        //    {
        //        EffectManager.Instance.RecycleEffectToPool(v.Key, v.Value);
        //    }
        //    effectDict.Clear();
        //    effectDict = null;
        //}
        public void Init(IContainerEntity owner)
        {
        }

        public virtual void Destory()
        {

        }

        public void ClearEnable()
        {
            Enable = false;
        }
        public virtual void ClearBuffEffect()
        {

        }
    }

    public class InherentBuff : Buff
    {
        protected string key;
        protected float effectValue;
        public virtual void InitData(BuffData buffData)
        {
            DispelPriority = BuffUtility.CalculateBuffDisperseType(buffData.EffectDisperseType);
            RealName = buffData.BuffName;
            IsDeBuff = buffData.BuffType == 1 ? false : true;
            effectName = buffData.EffectPrefab;
            effectOffectPos = buffData.EffectLocation;
        }
        public virtual void InitParameter(float val,StatusOrgan statusOrgan,UnitBase source)
        {
            effectValue = val;
            this.statusBar = statusOrgan;
            this.Source = source;
        }
        public virtual void OnEnterEvent()
        {

        }
        public virtual void OnUpdateEvent()
        {

        }
        public virtual void OnExitEvent()
        {
            ClearBuffEffect();
        }
        public override void Destory()
        {
            base.Destory();

        }
        public override void ClearBuffEffect()
        {
            if (!string.IsNullOrEmpty(key))
            {
                BuffUtility.RecycleEffect(key);
                key = string.Empty;
            }
        }
    }
    //public class TempRoundBuff : RoundBuff
    //{
    //    string buffRealName;
    //    string buffShowName;
    //    string buffDescription;
    //    Sprite buffIcon;
    //    bool isDeBuff;
    //    BuffPriorityType dispelPriority;
    //    public override Sprite BuffIcon => buffIcon;
    //    public override string BuffRealName => buffRealName;
    //    public override string BuffShowName => buffShowName;
    //    public override string BuffDescription => buffDescription;
    //    public override bool IsDeBuff => isDeBuff;
    //    public override BuffPriorityType DispelPriority => dispelPriority;
    //    public TempRoundBuff(string buffShowName, string buffDescription, bool isDeBuff, BuffPriorityType dispelPriority, Sprite buffIcon, BuffEventInfo buffEventInfo, int max = 1, int cur = -1, bool isShow = true) : base(buffShowName, max, cur, buffEventInfo)
    //    {
    //        this.buffRealName = buffShowName;
    //        this.buffShowName = buffShowName;
    //        this.buffIcon = buffIcon;
    //        this.buffDescription = buffDescription;
    //        this.isDeBuff = isDeBuff;
    //        this.dispelPriority = dispelPriority;
    //    }
    //}
    //public class RoundBuff : Buff
    //{
    //    protected int max_ContinueRoundNum = 1;

    //    protected int curr_ResidueRoundNum = 1;

    //    internal RoundBuff(string buffName, int max = 1, int curr = -1, BuffEventInfo buffEventInfo = null) : base(buffName, buffEventInfo)
    //    {
    //        max_ContinueRoundNum = max;
    //        if (curr != -1 && curr < max_ContinueRoundNum)
    //            curr_ResidueRoundNum = curr;
    //        else
    //            curr_ResidueRoundNum = max_ContinueRoundNum;
    //    }

    //    public override int Curr_ContinueRoundNum { get => curr_ResidueRoundNum; }
    //    public int Max_ContinueRoundNum { get => max_ContinueRoundNum; }

    //    public override void OnEnter(StatusBar statusBar)
    //    {
    //        base.OnEnter(statusBar);
    //        //curr_ContinueRoundNum = max_ContinueRoundNum;
    //    }
    //    public override void OnRound()
    //    {
    //        base.OnRound();
    //        curr_ResidueRoundNum--;
    //    }
    //    //public RoundBuff Copy()
    //    //{
    //    //    if (isOver == false)
    //    //        return new RoundBuff(BuffRealName, max_ContinueRoundNum, curr_ResidueRoundNum, EnterAction, RoundAction, ExitAction);
    //    //    else
    //    //        return new RoundBuff("");
    //    //}
    //}
    //public class ForeverBuff : Buff
    //{
    //    internal ForeverBuff(string buffName, BuffEventInfo buffEventInfo) : base(buffName, buffEventInfo)
    //    {
    //    }

    //    public override int Curr_ContinueRoundNum => 1;
    //    //public ForeverBuff Copy()
    //    //{
    //    //    if (isOver == false)
    //    //        return new ForeverBuff(BuffRealName, EnterAction, RoundAction, ExitAction);
    //    //    else
    //    //        return new ForeverBuff("");
    //    //}
    //}
}

