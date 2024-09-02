using System;
using GameMain.Script.Consts;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Character_System.HFSM.Util;
using UnityEngine;

namespace Script.View_Controller.Particle_System
{
    public class ParticleController : ControllerBase
    {
        private AnimationTimer mTimer;
        
        public ParticleType type;
        public Animator animator;

        public override void OnAwake()
        {
            mTimer = new AnimationTimer(animator.GetAnimationLength());
        }

        private void OnEnable()
        {
            mTimer.Reset();
        }

        public override void OnUpdate(float elapse)
        {
            if (mTimer.IsAnimatorFinish)
            {
                LevelManager.Instance.DestroyController(this);
            }
        }
    }
}