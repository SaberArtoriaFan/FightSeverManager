using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;

namespace XianXia.Unit
{
    public class UnitStatusSystem : NormalSystemBase<StatusOrgan>
    {
        BuffSystem buffSystem;
        float buffRefreshTime=1f;
        float timer = 0;
        public override void Start()
        {
            base.Start();
            buffSystem=world.FindSystem<BuffSystem>();
        }
        protected override void InitializeBeforeRecycle(StatusOrgan t)
        {
            UnitUtility.ClearStatusOrgan(t);
            base.InitializeBeforeRecycle(t);
        }
        public override void Update()
        {
            base.Update();
            timer += Time.deltaTime;
            if (timer >= buffRefreshTime)
            {
                timer = 0;
                
                foreach (var v in allComponents)
                {
                    if (v != null && v.Owner != null && v.Enable)
                    {
                        for(int i=0;i<v.StatusList.Count;i++)
                        {
                            if(buffSystem.UpdateBuff(v.StatusList[i]))
                            {
                                //v.StatusList.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
            }

        }
    }
}
