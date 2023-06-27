using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Server
{
    public class LoginRequest : BaseController
    {
        public override ActionCode ActionCode => ActionCode.Login;

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
