using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Server
{
    public class UpdateResources : BaseController
    {
        public override ActionCode ActionCode => ActionCode.UpdateResources;

        protected override MainPack FailHandlePack(MainPack mainPack)
        {
            return null;
        }

        protected override MainPack SuccessHandlePack(MainPack mainPack)
        {
            return null;
        }
    }
}
