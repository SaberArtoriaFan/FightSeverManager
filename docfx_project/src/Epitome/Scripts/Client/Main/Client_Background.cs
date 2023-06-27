using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    //[RequireComponent(typeof(SpriteRenderer))]
    public class Client_Background : Client_SingletonBase<Client_Background>
    {
        [SerializeField]
        SpriteRenderer backGround;

        public SpriteRenderer BackGround { get => backGround; }

        protected override void Awake()
        {
            base.Awake();
            foreach(var v in GameObject.FindObjectsOfType<SpriteRenderer>())
            {
                if (v.sortingLayerName=="Terrain"&& v.name == "BackGround")
                {
                    backGround = v;
                    break;
                }
            }
            if (backGround == null)
            {
                Debug.LogError("Œ¥’“µΩ±≥æ∞Õº");
            }
            else
                Debug.Log("’“µΩ±≥æ∞Õº");
        }
    }
}
