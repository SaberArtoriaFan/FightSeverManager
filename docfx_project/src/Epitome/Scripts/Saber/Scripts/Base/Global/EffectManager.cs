using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.Base
{
    public class EffectManager : AutoSingleton<EffectManager>
    {
        public const string effectABPackageName = "effect";

        private void Start()
        {
            
        }
        public ParticleSystem GetEffectInPool(string effectName)
        {
            if (!PoolManager.instance.IsPoolAlive(effectName))
                PoolManager.instance.AddPool<ParticleSystem>(() => { return Instantiate(ABManager.instance.LoadResource<GameObject>(effectABPackageName, effectName)).GetComponent<ParticleSystem>(); }, 
                    (u) => { u.gameObject.SetActive(false);u.transform.SetParent(this.transform); }, 
                    (u) => { u.gameObject.SetActive(false); },
                    effectName);
            return PoolManager.instance.GetObjectInPool<ParticleSystem>(effectName);
        }
        public void RecycleEffectToPool(ParticleSystem effect,string effectName)
        {
            if (!PoolManager.instance.IsPoolAlive(effectName)) return;
            PoolManager.instance.RecycleToPool(effect,effectName);
        }
    }
}
