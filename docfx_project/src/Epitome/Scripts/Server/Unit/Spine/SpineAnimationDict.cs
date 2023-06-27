using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Spine
{
    public class SpineAnimationDict : MonoBehaviour
    {
        public Dictionary<int, Dictionary<int, float>> animationDict = new Dictionary<int, Dictionary<int, float>>();
        public Dictionary<string, int> heroAnimationDict = new Dictionary<string, int>();
        private void Awake()
        {
            if (InstanceFinder.GetInstance<SpineAnimationDict>() != null) { Debug.LogError("Cant Find"); return; }
            InstanceFinder.RegisterInstance(this, false);
        }
        public bool IsContains(int id) => animationDict.ContainsKey(id);
        public void Register(int hashID, Dictionary<int, float> dict)
        {
            if (!animationDict.ContainsKey(hashID))
                animationDict.Add(hashID, dict);
        }
        public float GetAnimationLong(int hashID,int animationNameHashID)
        {
            if (animationDict.TryGetValue(hashID, out var v) && v.TryGetValue(animationNameHashID, out var res)) return res;
            else return -1;
        }

    }
}
