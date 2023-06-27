using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public static partial class BuffDictionary
    {
        
        public class Frozen : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit, SkillUtility.World.FindSystem<UnitMainSystem>(), false);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit, SkillUtility.World.FindSystem<UnitMainSystem>(), true);
            }
        }
        public class Enwind : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit, SkillUtility.World.FindSystem<UnitMainSystem>(), false);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit, SkillUtility.World.FindSystem<UnitMainSystem>(), true);
            }
        }
        public class Dizziness : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit,SkillUtility.World.FindSystem<UnitMainSystem>(),false);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Dizziness(statusBar.OwnerUnit, SkillUtility.World.FindSystem<UnitMainSystem>(),true);
            }
        }
        public class Disarm : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Disarm(statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack), SkillUtility.World.FindSystem<UnitMainSystem>(), false);
                if(effectName!=string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Disarm(statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack), SkillUtility.World.FindSystem<UnitMainSystem>(), true);
            }
        }
        public class Slient : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Slient(statusBar.OwnerUnit.FindOrganInBody<MagicOrgan>(Saber.ECS.ComponentType.magic), SkillUtility.World.FindSystem<UnitMainSystem>(), false);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Slient(statusBar.OwnerUnit.FindOrganInBody<MagicOrgan>(Saber.ECS.ComponentType.magic), SkillUtility.World.FindSystem<UnitMainSystem>(), true);
            }
        }
        public class Fetter : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BuffUtility.Fetter(statusBar.OwnerUnit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg), SkillUtility.World.FindSystem<UnitMainSystem>(), false);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.Fetter(statusBar.OwnerUnit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg), SkillUtility.World.FindSystem<UnitMainSystem>(), true);
            }
        }
        public class Invincible : InherentBuff
        {
            Action<StatusOrgan, Buff> action;
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                action= BuffUtility.Invincible(statusBar, false, SkillUtility.World.FindSystem<EventSystem>(), action);
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                _=BuffUtility.Invincible(statusBar, true, SkillUtility.World.FindSystem<EventSystem>(),  action);
            }
        }
        public class AddAttack : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                AttackOrgan attackOrgan;
                if ((attackOrgan = statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= attackOrgan.OriginAttackVal;
                    BuffUtility.AddAttack(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.ReduceAttack(statusBar, effectValue);
            }
        }
        public class ReduceAttack : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                AttackOrgan attackOrgan;
                if ((attackOrgan = statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= attackOrgan.OriginAttackVal;
                    BuffUtility.ReduceAttack(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.AddAttack(statusBar, effectValue);
            }
        }
        public class AddDefence : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BodyOrgan bodyOrgan;
                if ((bodyOrgan = statusBar.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= bodyOrgan.Origin_def;
                    BuffUtility.AddDefence(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.ReduceDefence(statusBar, effectValue);
            }
        }
        public class ReduceDefence : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                BodyOrgan bodyOrgan;
                if ((bodyOrgan = statusBar.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= bodyOrgan.Origin_def;
                    BuffUtility.ReduceDefence(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.AddDefence(statusBar, effectValue);
            }
        }
        //public class AddAttackRange : InherentBuff
        //{
        //    string key = string.Empty;
        //    public override void OnEnterEvent()
        //    {
        //        //Debug.Log("TTT—£‘Œbuff");
        //        base.OnEnterEvent();
        //        AttackOrgan bodyOrgan;
        //        if ((bodyOrgan = statusBar.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body)) != null)
        //        {
        //            effectValue *= bodyOrgan.Origin_def;
        //            BuffUtility.AddDefence(statusBar, effectValue);
        //        }
        //        if (effectName != string.Empty)
        //            key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
        //    }
        //    public override void OnUpdateEvent()
        //    {
        //        base.OnUpdateEvent();
        //    }
        //    public override void OnExitEvent()
        //    {
        //        base.OnExitEvent();
        //        BuffUtility.ReduceDefence(statusBar, effectValue);
        //        if (key != string.Empty)
        //        {
        //            BuffUtility.RecycleEffect(key);
        //            key = string.Empty;
        //        }
        //    }
        //}
        public class AddAttackSpeed : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                AttackOrgan attackOrgan;
                if ((attackOrgan = statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack)) != null)
                {
                    effectValue -= 100;
                    effectValue = effectValue/ 100;
                    //Debug.Log("π•ª˜º”≥…" + effectValue);
                    effectValue *= attackOrgan.Origin_AttackSpeed;
                    BuffUtility.AddAttackSpeed(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.ReduceAttackSpeed(statusBar, effectValue);
            }
        }
        public class ReduceAttackSpeed : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                AttackOrgan attackOrgan;
                if ((attackOrgan = statusBar.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= attackOrgan.Origin_AttackSpeed;
                    BuffUtility.ReduceAttackSpeed(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.AddAttackSpeed(statusBar, effectValue); 
                }
            }
        }
        public class AddMoveSpeed : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                LegOrgan legOrgan;
                if ((legOrgan = statusBar.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= legOrgan.MoveSpeed;
                    BuffUtility.AddMoveSpeed(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.ReduceMoveSpeed(statusBar, effectValue);
            }
        }
        public class ReduceMoveSpeed : InherentBuff
        {
            public override void OnEnterEvent()
            {
                //Debug.Log("TTT—£‘Œbuff");
                base.OnEnterEvent();
                LegOrgan legOrgan;
                if ((legOrgan = statusBar.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg)) != null)
                {
                    effectValue -= 100;
                    effectValue /= 100;
                    effectValue *= legOrgan.MoveSpeed;
                    BuffUtility.ReduceMoveSpeed(statusBar, effectValue);
                }
                if (effectName != string.Empty)
                    key = BuffUtility.LoadEffectForUnit(statusBar.OwnerUnit, effectName, effectOffectPos);
            }
            public override void OnUpdateEvent()
            {
                base.OnUpdateEvent();
            }
            public override void OnExitEvent()
            {
                base.OnExitEvent();
                BuffUtility.AddMoveSpeed(statusBar, effectValue);

            }
        }
    }



