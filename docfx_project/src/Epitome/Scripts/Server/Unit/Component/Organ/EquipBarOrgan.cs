using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class EquipBarOrgan : StatusOrganBase<EquipItem>
    {
        protected override ComponentType componentType => ComponentType.equip;


    }
}
