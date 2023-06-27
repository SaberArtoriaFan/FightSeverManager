using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia.Client
{
    public class ClientSkeletonAnimationHandle : MonoBehaviour, XianXia.Client.IAnimationHandle
	{
		IAnimationStateComponent IASC;
		ISkeletonComponent ISC;


		//public SkeletonAnimation skeletonAnimation;
		public string[] triggerAnimations;
		HashSet<int> triggerAnimationHashSet;
		//public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
		//public List<AnimationTransition> transitions = new List<AnimationTransition>(); // Alternately, an AnimationPair-Animation Dictionary (commented out) can be used for more efficient lookups.
		Dictionary<int, Spine.Animation> animationHashNameDict;

		Dictionary<string, Spine.Animation> animationNameDict;
		Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation> transDict;
		//readonly Dictionary<Spine.AnimationStateData.AnimationPair, Spine.Animation> transitionDictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>(Spine.AnimationStateData.AnimationPairComparer.Instance);

		public Spine.Animation TargetAnimation { get; private set; }

		void Awake()
		{
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

		private void Start()
		{
			InitAnimation();
		}
		public void InitAnimation()
		{
			triggerAnimationHashSet = new HashSet<int>();
			foreach(var v in triggerAnimations)
            {
				int i= Animator.StringToHash(v);
				if (!triggerAnimationHashSet.Contains(i))
					triggerAnimationHashSet.Add(i);
            }
			transDict = new Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation>();

			//skeletonAnimation = GetComponent<SkeletonAnimation>();
			//if (skeletonAnimation == null)
			//	skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
			this.ISC = GetComponentInChildren<ISkeletonComponent>();
            if (ISC == null)
            {
				Debug.LogError("该组件无法寻找到ISkeletonComponent");
				return;
            }
			if (ISC is IAnimationStateComponent va) IASC = va;
			else IASC= GetComponentInChildren<IAnimationStateComponent>();
			if (IASC == null)
			{
				Debug.LogError("该组件无法寻找到IAnimationStateComponent");
				return;
			}

			List<Spine.Animation> animations =ISC.SkeletonDataAsset.GetSkeletonData(false).Animations.ToList();
			animationNameDict = new Dictionary<string, Spine.Animation>();
			animationHashNameDict = new Dictionary<int, Spine.Animation>();
			List<Spine.Animation> transitions = new List<Spine.Animation>();
			foreach (var v in animations)
			{

				Debug.Log(v.Name + "_" + StringToHash(v.Name));
				animationNameDict.Add(v.Name, v);
				animationHashNameDict.Add(StringToHash(v.Name), v);
				if (v.Name.Contains('-'))
					transitions.Add(v);
			}
			foreach (var v in transitions)
			{
				//Debug.Log("Tran__" + v.Name);
				int index = v.Name.IndexOf('-');
				if (index + 1 >= v.Name.Length) continue;
				string from = v.Name.Substring(0, index);
				string to = v.Name.Substring(index + 1, v.Name.Length - from.Length - 1);
				if (animationNameDict.TryGetValue(from, out var fromAnimation) && animationNameDict.TryGetValue(to, out var toAnimation))
					ClientSkeletonAnimationManager.Instance.RegisterCondition((fromAnimation, toAnimation), v);
			}
		}
		public void AddAnimationEvent(string name,Action action)
        {
			IASC.AnimationState.End += (res) =>
			{
				if (res.Animation.Name != name) return;
				action?.Invoke();
			};
        }
		/// <summary>Sets the horizontal flip state of the skeleton based on a nonzero float. If negative, the skeleton is flipped. If positive, the skeleton is not flipped.</summary>
		public void SetFlip(float horizontal)
		{
			if (horizontal != 0)
			{
				ISC.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
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
			Debug.Log("play_ " + shortNameHash);
			if (foundAnimation == null)
				return;
			if (shortNameHash == FSM.AnimatorParameters.Death)
				PlayDeathAnimation(foundAnimation, layerIndex);
			else if (triggerAnimationHashSet.Contains(shortNameHash))
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

			current = GetCurrentAnimation(layerIndex);
			if (current != null)
				transition = TryGetTransition(current, target);

			if (transition != null)
			{
				IASC.AnimationState.SetAnimation(layerIndex, transition, false);
				IASC.AnimationState.AddAnimation(layerIndex, target, true, 0f);
			}
			else
			{
				IASC.AnimationState.SetAnimation(layerIndex, target, true);
			}

			this.TargetAnimation = target;
		}

		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		public void PlayOneShot(Spine.Animation oneShot, int layerIndex)
		{
			var state = IASC.AnimationState;
			state.SetAnimation(0, oneShot, false);

			var transition = TryGetTransition(oneShot, TargetAnimation);
			if (transition != null)
				state.AddAnimation(0, transition, false, 0f);

			state.AddAnimation(0, this.TargetAnimation, true, 0f);
		}
		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		public void PlayDeathAnimation(Spine.Animation oneShot, int layerIndex)
		{
			var state = IASC.AnimationState;
			var entry = state.SetAnimation(0, oneShot, false);
			entry.End += (v) =>
			{
				state.TimeScale = 0;
			};
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
			return ClientSkeletonAnimationManager.Instance.TryGetCondition((from, to));

			//Spine.Animation foundTransition = null;
			//transitionDictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out foundTransition);
			//return foundTransition;
		}

		Spine.Animation GetCurrentAnimation(int layerIndex)
		{
			var currentTrackEntry = IASC.AnimationState.GetCurrent(layerIndex);
			return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
		}

		int StringToHash(string s)
		{
			return Animator.StringToHash(s);
		}
	}
}
