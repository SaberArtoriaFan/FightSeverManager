using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Unit;

namespace XianXia
{
    public class TimeTriggeredPassiveSkill : PassiveSkill
    {
        protected float startTime = 20;
        /// <summary>
        /// True if 决定是计算游戏开始多少秒时
        /// else 计算游戏剩余时长低于多少秒时
        /// </summary>
        protected bool isCalcuPastTime = true;
        Timer timer = null;
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            if (Enable == false) return;
            Enable = false;
            //UnitDamagedBefore = SkillSystem.World.FindSystem<EventSystem>().GetEvent<BodyOrgan, Damage>("UnitDamagedBefore");
            //TimerManagerSystem timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();

            timer = TimerManager.Instance.AddTimer(Rage, 1, true);
        }
        void Rage()
        {
            //Debug.Log(TimingSystemUI.TimePast + "狂暴A");
            //Debug.Log(TimingSystemUI.TimeRemaining + "狂暴B");

            //if(Enable==false)
            if ((isCalcuPastTime && TimingSystemUI.TimePast >= startTime) || (!isCalcuPastTime && TimingSystemUI.TimeRemaining <= startTime))
            {
                TriggerMain();
                timer.Stop();
            }
        }
        protected virtual void TriggerMain()
        {

        }
        public override void LostSkill()
        {
            base.LostSkill();
            timer = null;
            Enable = true;
        }
    }
}
