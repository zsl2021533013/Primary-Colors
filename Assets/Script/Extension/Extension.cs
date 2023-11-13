using System;
using System.Linq;
using System.Text.RegularExpressions;
using QFramework;
using Script.View_Controller.Character_System.HFSM.Base;
using Script.View_Controller.Character_System.HFSM.StateMachine;
using Script.View_Controller.Character_System.HFSM.States;
using Script.View_Controller.Character_System.HFSM.Transitions;
using UnityEngine;

namespace Script
{
    public static class AnimationStateExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <param name="onEnter"></param>
        /// <param name="onLogic"></param>
        /// <param name="onExit"></param>
        /// <param name="canExit"></param>
        /// <param name="needsExitTime"></param>
        /// <param name="isGhostState"></param>
        /// <typeparam name="TState"></typeparam>
        public static void AddState<TState>(
            this StateMachine<Type, Type, Type> fsm,
            Animator animator = null,
            string animationName = null,
            Action<AnimationState<Type, Type>> onEnter = null,
            Action<AnimationState<Type, Type>> onLogic = null,
            Action<AnimationState<Type, Type>> onExit = null,
            Func<AnimationState<Type, Type>, bool> canExit = null,
            bool needsExitTime = false,
            bool isGhostState = false)
        {
            if (animator == null || animationName == null)
            {
                fsm.AddState(typeof(TState), new StateBase<Type>(needsExitTime));
                return;
            }
            
            fsm.AddState(typeof(TState), new AnimationState<Type, Type>
            (animator, animationName, animator.GetAnimationLength(animationName),
                onEnter, onLogic, onExit, canExit, needsExitTime, isGhostState));
        }
        
        /// <summary>
        /// Add transition from TState1 to TState2
        /// </summary>
        /// <param name="fsm">Your FSM</param>
        /// <param name="condition">Transition will happen when condition return true</param>
        /// <param name="forceInstantly">If true, it will translate immediately</param>
        /// <typeparam name="TState1">The "from" state</typeparam>
        /// <typeparam name="TState2">The "to" state</typeparam>
        public static void AddTransition<TState1, TState2>(
            this StateMachine<Type, Type, Type> fsm,
            Func<Transition<Type>, bool> condition = null,
            bool forceInstantly = false)
        {
            fsm.AddTransition(new Transition<Type>(typeof(TState1), typeof(TState2), condition, forceInstantly));
        }
    }
    
    public static class AnimatorExtension
    {
        /// <summary>
        /// Get target animation length
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public static float GetAnimationLength(this Animator animator, string animationName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            return clips.Where(clip => clip.name
                    .Equals(animationName))
                .Select(clip => clip.length)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Get the first animation length
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public static float GetAnimationLength(this Animator animator)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            return clips[0].length;
        }
    }
    
    public static class TransformExtension
    {
        /// <summary>
        /// Flip transform direction by changing it's scale
        /// </summary>
        /// <param name="transform">Target transform</param>
        public static void Flip(this Transform transform)
        {
            var scale = transform.localScale;
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
        
        /// <summary>
        /// Flip transform by direction, if direction &gt; 0, turn right, if direction &lt; 0, turn left
        /// </summary>
        /// <param name="transform">Target transform</param>
        /// <param name="direction">Input direction</param>
        public static void Flip(this Transform transform, float direction)
        {
            var scale = transform.localScale;
            var scaleX = Mathf.Abs(scale.x);
            switch (direction)
            {
                case > 0:
                    transform.localScale = new Vector3(scaleX, scale.y, scale.z);
                    break;
                case < 0:
                    transform.localScale = new Vector3(-scaleX, scale.y, scale.z);
                    break;
                case 0:
                    break;
            }
        }

        /// <summary>
        /// Find weather direction is same to the transform
        /// </summary>
        /// <param name="transform">Target transform</param>
        /// <param name="direction">Input direction</param>
        /// <returns></returns>
        public static bool IsSameDirection(this Transform transform, float direction)
        {
            return transform.localScale.x * direction > 0;
        }
        
        /// <summary>
        /// Find weather direction is opposite to the transform
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static bool IsOppositeDirection(this Transform transform, float direction)
        {
            return transform.localScale.x * direction < 0;
        }

        /// <summary>
        /// Return 1 when the transform face right, return -1 when the transform face left
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static int Direction(this Transform transform)
        {
            return transform.lossyScale.x > 0 ? 1 : -1;
        }
    }
    
    public static class StringExtension
    {
        /// <summary>
        /// 按照大写字母与数字插入空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InsertSpace(this string s)
        {
            var newString = s.Replace(" ", "").Replace("\t", "");
            newString = Regex.Replace(newString, @"(?<!^)(?=([A-Z]|\d))", " $0");
            return newString;
        }
    }
}
