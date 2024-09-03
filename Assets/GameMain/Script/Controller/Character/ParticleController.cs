using GameMain.Script.Consts;
using GameMain.Script.Controller.Character.HFSM.Util;
using Script;
using UnityEngine;

namespace GameMain.Script.Controller.Character
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

        public override void OnUpdate(float elapse)
        {
            if (mTimer.IsAnimatorFinish)
            {
                LevelManager.Instance.DestroyController(this);
            }
        }
    }
}