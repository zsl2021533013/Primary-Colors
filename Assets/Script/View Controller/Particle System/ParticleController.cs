using System;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Character_System.HFSM.Util;
using UnityEngine;

namespace Script.View_Controller.Particle_System
{
    public partial class ParticleController : MonoBehaviour, IController
    {
        private AnimationTimer mTimer;

        private void Awake()
        {
            mTimer = new AnimationTimer(animator.GetAnimationLength());
        }

        private void OnEnable()
        {
            mTimer.Reset();
        }

        private void Update()
        {
            if (mTimer.IsAnimatorFinish)
            {
                this.SendCommand(new RecycleParticleCommand { type = type, gameObject = gameObject });
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}