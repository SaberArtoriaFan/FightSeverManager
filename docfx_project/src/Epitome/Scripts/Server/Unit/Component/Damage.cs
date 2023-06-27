using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class Damage
    {
       private static Damage godDamage=new Damage(null,0,false,false); 
        UnitBase source;
        int val;
        bool isAttack;
        bool isCriticalStrike = false;
        public Damage(UnitBase source, int val, bool isAttack, bool isCriticalStrike=false)
        {
            this.Source = source;
            this.Val = val;
            this.IsAttack = isAttack;
            this.isCriticalStrike = isCriticalStrike;
        }

        public static Damage GodDamage { get => godDamage; }
        public UnitBase Source { get => source; set => source = value; }
        public int Val { get => val; set => val = value; }
        public bool IsAttack { get => isAttack; set => isAttack = value; }
        public bool IsCriticalStrike { get => isCriticalStrike; set => isCriticalStrike = value; }
    }
}
