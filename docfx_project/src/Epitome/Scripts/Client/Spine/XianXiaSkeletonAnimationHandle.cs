using FishNet.Component.Animating;
using Saber.ECS;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace XianXia.Client
{
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	public class XianXiaSkeletonAnimationHandle : MonoBehaviour,XianXia.Client.IAnimationHandle
    {
		//Animator animator;
		[HideInInspector]
		public SkeletonAnimation skeletonAnimation;
		//public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
		//public List<AnimationTransition> transitions = new List<AnimationTransition>(); // Alternately, an AnimationPair-Animation Dictionary (commented out) can be used for more efficient lookups.
		Dictionary<int, Spine.Animation> animationHashNameDict;

		Dictionary<string, Spine.Animation> animationNameDict;
		SpineAnimationTransitionManager transitionManager;

		//readonly Dictionary<Spine.AnimationStateData.AnimationPair, Spine.Animation> transitionDictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>(Spine.AnimationStateData.AnimationPairComparer.Instance);
		//[ShowInInspector]
		[SerializeField]
		string ABPackage;
		[SerializeField]
		//[ShowInInspector]
		string path;

		[AssetsOnly]
		public SkeletonDataAsset skeletonDataAsset;
		public Spine.Animation TargetAnimation { get; private set; }
		public float Speed { get => skeletonAnimation.AnimationState.TimeScale; set { skeletonAnimation.AnimationState.TimeScale = value; } }
		public bool IsActiveAnimation => skeletonAnimation?.AnimationState?.TimeScale != 0;

		void Awake()
		{
			CreateInstance();
			InitAnimation();
			//animator = GetComponent<Animator>();
			//animator.enabled = false;
			// Initialize AnimationReferenceAssets
			//foreach (var entry in statesAndAnimations)
			//{
			//	entry.animation.Initialize();
			//}
			//foreach (var entry in transitions)
			//{
			//	entry.from.Initialize();
			//	entry.to.Initialize();
			//	entry.transition.Initialize();
			//}

			// Build Dictionary
			//foreach (var entry in transitions) {
			//	transitionDictionary.Add(new AnimationStateData.AnimationPair(entry.from.Animation, entry.to.Animation), entry.transition.Animation);
			//}
		}
		void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
				if (skeletonDataAsset != null)
				{
					path = UnityEditor.AssetDatabase.GetAssetPath(skeletonDataAsset);
					int len = path.Length;
					path = path.Replace(ABUtility.ABPackageDataPath, "");
					if (len == path.Length)
					{
						Debug.LogError(path + "所选资产似乎并不是位于合法路径");
						path = "";
						skeletonDataAsset = null;
						ABPackage = "";
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 1; i < path.Length; i++)
						{

							if (path[i] == '/' || path[i] == '\\')
							{
								ABPackage = stringBuilder.ToString();
								return;
							}
							stringBuilder.Append(path[i]);
						}
						NetworkAnimator networkAnimator = GetComponentInParent<NetworkAnimator>();
						networkAnimator?.SetAnimator(GetComponent<Animator>());
					}
				}
				else
				{
					path = "";
					ABPackage = "";
				}
			}

#endif
		}

		private void Start()
        {
			//animator.enabled = true;
			//InitAnimation();
        }
        void OnEnable()
        {
            if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
                skeletonAnimation.AnimationState.TimeScale = 1;
        }
        void CreateInstance()
        {

			if (skeletonDataAsset == null)
				skeletonDataAsset = ABUtility.Load<SkeletonDataAsset>(path,ABPackage);
            if (!SpineAnimationTransitionManager.Instance.RegisterAlready(Animator.StringToHash(skeletonDataAsset.name)))
            {
				foreach (var v in skeletonDataAsset.atlasAssets)
				{
					v.PrimaryMaterial.shader = Shader.Find(v.PrimaryMaterial.shader.name);
					foreach (var a in v.Materials)
						a.shader = Shader.Find(a.shader.name);
				}
			}
			
			SkeletonDataAsset sda = SkeletonDataAsset.CreateRuntimeInstance(skeletonDataAsset.skeletonJSON, skeletonDataAsset.atlasAssets, true);
			skeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(sda);
			skeletonAnimation.transform.SetParent(this.gameObject.transform);
			skeletonAnimation.transform.localPosition = Vector3.zero;
			skeletonAnimation.transform.localScale = Vector3.one;
			skeletonAnimation.transform.localRotation = Quaternion.identity;

        }
        public void InitAnimation()
        {
			transitionManager = SpineAnimationTransitionManager.Instance;
			//skeletonAnimation = GetComponent<SkeletonAnimation>();
			if(skeletonAnimation==null)
				skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
			List<Spine.Animation> animations = skeletonAnimation.skeletonDataAsset.GetSkeletonData(false).Animations.ToList();
			animationNameDict = new Dictionary<string, Spine.Animation>();
			animationHashNameDict = new Dictionary<int, Spine.Animation>();
			List<Spine.Animation> transitions = new List<Spine.Animation>();
			foreach (var v in animations)
			{

				Debug.Log("Spine动画名记录"+v.Name+"_"+ StringToHash(v.Name));
				animationNameDict.Add(v.Name, v);
				animationHashNameDict.Add(StringToHash(v.Name), v);
				if (v.Name.Contains('-'))
					transitions.Add(v);
			}
			foreach (var v in transitions)
			{
				Debug.Log("Tran__" + v.Name);
				int index = v.Name.IndexOf('-');
				if (index + 1 >= v.Name.Length) continue;
				string from = v.Name.Substring(0, index);
				string to = v.Name.Substring(index + 1, v.Name.Length - from.Length - 1);
				if (animationNameDict.TryGetValue(from, out var fromAnimation) && animationNameDict.TryGetValue(to, out var toAnimation))
					transitionManager.RegisterCondition((fromAnimation, toAnimation), v);
			}
		}
		/// <summary>Sets the horizontal flip state of the skeleton based on a nonzero float. If negative, the skeleton is flipped. If positive, the skeleton is not flipped.</summary>
		public void SetFlip(float horizontal)
		{
			if (horizontal != 0)
			{
				skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
			}
		}

		/// <summary>Plays an animation based on the state name.</summary>
		public void PlayAnimationForState(string stateShortName, int layerIndex)
		{
			PlayAnimationForState(StringToHash(stateShortName), layerIndex);
		}

		/// <summary>Plays an animation based on the hash of the state name.</summary>
		public void PlayAnimationForState(int shortNameHash, int layerIndex)
		{
			var foundAnimation = GetAnimationForState(shortNameHash);
			Debug.Log("play_ " + foundAnimation ==null?"":foundAnimation.Name);
			if (foundAnimation == null)
				return;
			if (shortNameHash == FSM.AnimatorParameters.Death)
				PlayDeathAnimation(foundAnimation, layerIndex);
			else if (transitionManager.IsTriggerAnimation(shortNameHash))
				PlayOneShot(foundAnimation, layerIndex);
			else
				PlayNewAnimation(foundAnimation, layerIndex);
		}

		/// <summary>Gets a Spine Animation based on the state name.</summary>
		public Spine.Animation GetAnimationForState(string stateShortName)
		{
			return GetAnimationForState(StringToHash(stateShortName));
		}

		/// <summary>Gets a Spine Animation based on the hash of the state name.</summary>
		public Spine.Animation GetAnimationForState(int shortNameHash)
		{
			//var foundState = statesAndAnimations.Find(entry => StringToHash(entry.stateName) == shortNameHash);
			//return (foundState == null) ? null : foundState.animation;
			if (animationHashNameDict.TryGetValue(shortNameHash, out var res)) return res;
			else return null;
		}

		/// <summary>Play an animation. If a transition animation is defined, the transition is played before the target animation being passed.</summary>
		public void PlayNewAnimation(Spine.Animation target, int layerIndex)
		{
			Spine.Animation transition = null;
			Spine.Animation current = null;
			//skeletonAnimation.AnimationState.TimeScale = 1;

			current = GetCurrentAnimation(layerIndex);
			if (current != null)
				transition = TryGetTransition(current, target);

			if (transition != null)
			{
				skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false);
				skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, true, 0f);
			}
			else
			{
				skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true);
			}

			this.TargetAnimation = target;
		}

		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		public void PlayOneShot(Spine.Animation oneShot, int layerIndex)
		{
			var state = skeletonAnimation.AnimationState;
			var entry = state.SetAnimation(0, oneShot, false);
            //Debug.Log("播放一次" + oneShot.Name);
            var transition = TryGetTransition(oneShot, TargetAnimation);
            //float delay = Time.deltaTime;
            if (transition != null)
            {
                state.AddAnimation(0, transition, false, 0f);
                //delay = 0;
            }

            state.AddAnimation(0, this.TargetAnimation, true, 0);
        }
		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		public void PlayDeathAnimation(Spine.Animation oneShot, int layerIndex)
		{
			var state = skeletonAnimation.AnimationState;
			var  entry= state.SetAnimation(0, oneShot, false);
			//Spine.AnimationState.TrackEntryDelegate action = null;
			//action = (u) =>
			//{
			//	state.TimeScale = 0;
			//	entry.End -= action;
			//};
			//entry.End += action;
			var transition = TryGetTransition(oneShot, TargetAnimation);
			//float delay = Time.deltaTime;
			if (transition != null)
			{
				state.AddAnimation(0, transition, false, 0f);
				//delay = 0;
			}

			state.AddAnimation(0, this.TargetAnimation, true, 0);
		}
		Spine.Animation TryGetTransition(Spine.Animation from, Spine.Animation to)
		{
			//foreach (var transition in transitions)
			//{
			//	if (transition.from.Animation == from && transition.to.Animation == to)
			//	{
			//		return transition.transition.Animation;
			//	}
			//}
			//return null;
			return transitionManager.TryGetCondition((from, to));

			//Spine.Animation foundTransition = null;
			//transitionDictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out foundTransition);
			//return foundTransition;
		}

		Spine.Animation GetCurrentAnimation(int layerIndex)
		{
			var currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
			return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
		}

		int StringToHash(string s)
		{
			return Animator.StringToHash(s);
		}
	}
}
