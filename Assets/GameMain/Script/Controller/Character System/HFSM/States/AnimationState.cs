using System;
using Script.View_Controller.Character_System.HFSM.Util;
using UnityEngine;

namespace Script.View_Controller.Character_System.HFSM.States
{
    public class AnimationState<TStateId, TEvent> : ActionState<TStateId, TEvent>
    {
		private Action<AnimationState<TStateId, TEvent>> onEnter;
		private Action<AnimationState<TStateId, TEvent>> onLogic;
		private Action<AnimationState<TStateId, TEvent>> onExit;
		private Func<AnimationState<TStateId, TEvent>, bool> canExit;

		private Animator mAnimator;
		private string mAnimationName;
		private AnimationCurve mCurve;

		public AnimationTimer timer;

		/// <summary>
		/// Initialises a new instance of the State class
		/// </summary>
		/// <param name="animator">The character's animator</param>
		/// <param name="animationName">Animation name in the character animator</param>
		/// <param name="animationLength">The animation length in the animator</param>
		/// <param name="onEnter">A function that is called when the state machine enters this state</param>
		/// <param name="onLogic">A function that is called by the logic function of the state machine if this state is active</param>
		/// <param name="onExit">A function that is called when the state machine exits this state</param>
		/// <param name="canExit">(Only if needsExitTime is true):
		/// 	Called when a state transition from this state to another state should happen.
		/// 	If it can exit, it should call fsm.StateCanExit()
		/// 	and if it can not exit right now, later in OnLogic() it should call fsm.StateCanExit()</param>
		/// <param name="needsExitTime">Determines if the state is allowed to instantly
		/// 	exit on a transition (false), or if the state machine should wait until the state is ready for a
		/// 	state change (true)</param>
		/// <param name="isGhostState">If true, this state becomes a ghost state, a
		/// 	state the state machine does not want to stay in. That means that if the
		/// 	fsm transitions to this state, it will test all outgoing transitions instantly
		/// 	and not wait until the next OnLogic call.</param>
		public AnimationState(
			Animator animator,
			string animationName,
			float animationLength,
			Action<AnimationState<TStateId, TEvent>> onEnter = null,
			Action<AnimationState<TStateId, TEvent>> onLogic = null,
			Action<AnimationState<TStateId, TEvent>> onExit = null,
			Func<AnimationState<TStateId, TEvent>, bool> canExit = null,
			bool needsExitTime = false,
			bool isGhostState = false) : base(needsExitTime, isGhostState)
		{
			mAnimator = animator;
			mAnimationName = animationName;
			this.onEnter = onEnter;
			this.onLogic = onLogic;
			this.onExit = onExit;
			this.canExit = canExit;
			
			this.timer = new AnimationTimer(animationLength);
		}

		public override void OnEnter()
		{
			timer.Reset();

			if (!isGhostState)
			{
				mAnimator.Play(mAnimationName);
			}

			// Debug.Log(name);

			onEnter?.Invoke(this);
		}

		public override void OnLogic()
		{
			onLogic?.Invoke(this);
		}

		public override void OnExit()
		{
			onExit?.Invoke(this);
		}

		public override void OnExitRequest()
		{
			if (!needsExitTime || canExit != null && canExit(this))
			{
				fsm.StateCanExit();
			}
		}
	}
}