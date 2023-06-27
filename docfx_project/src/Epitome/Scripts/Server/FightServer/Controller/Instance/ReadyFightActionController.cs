using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadyFightActionController : BaseController
    {
        public override ActionCode ActionCode => ActionCode.ReadyFightAction;

        protected override MainPack FailHandlePack(MainPack mainPack)
        {
            return mainPack;
        }

        protected override MainPack SuccessHandlePack(MainPack mainPack)
        {
            return mainPack;
        }
    }
}
