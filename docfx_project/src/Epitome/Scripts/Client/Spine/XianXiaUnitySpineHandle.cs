using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Saber.ECS;
using FishNet.Component.Animating;

namespace XianXia.Client
{
    public class XianXiaUnitySpineHandle : MonoBehaviour
    {
        public string path;
        public SkeletonDataAsset skeletonDataAsset;
        SkeletonMecanim skeletonMecanim;
		void CreateInstance()
		{

			if (skeletonDataAsset == null)
				skeletonDataAsset = ABUtility.Load<SkeletonDataAsset>(path);
			foreach (var v in skeletonDataAsset.atlasAssets)
			{
				v.PrimaryMaterial.shader = Shader.Find(v.PrimaryMaterial.shader.name);
				foreach (var a in v.Materials)
					a.shader = Shader.Find(a.shader.name);
			}
			skeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(skeletonDataAsset.skeletonJSON, skeletonDataAsset.atlasAssets, true);
			
			skeletonMecanim = SkeletonMecanim.AddSpineComponent<SkeletonMecanim>(gameObject, skeletonDataAsset);
			
			//skeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(sda);
			//skeletonMecanim.transform.SetParent(this.gameObject.transform);
			//skeletonMecanim.transform.localPosition = Vector3.zero;
			//skeletonMecanim.transform.localScale = Vector3.one;
			//skeletonMecanim.transform.localRotation = Quaternion.identity;
		}
        private void Awake()
        {
			CreateInstance();
			Animator animator = GetComponent<Animator>();
			Debug.Log("¶¯»­¿ØÖÆÆ÷" + skeletonDataAsset.controller.name);
			animator.runtimeAnimatorController = skeletonDataAsset.controller;
			NetworkAnimator networkAnimator = GetComponent<NetworkAnimator>();
			if (networkAnimator != null) networkAnimator.SetAnimator(animator);
        }
    }
}
