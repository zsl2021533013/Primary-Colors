using System;
using GameMain.Script.Controller.Character.HFSM.StateMachine;
using QFramework;
using Script;
using Script.Event;
using UnityEngine;

namespace GameMain.Script.Controller.Environment_System.Target
{
    public class TargetController : ControllerBase
    {
        public Animator animator;
        
        public StateMachine<Type, Type, Type> FSM { get; private set; }

        public override void OnAwake()
        {
            this.RegisterEvent<StageClearEvent>(e =>
            {
                FSM.Trigger(typeof(StageClearEvent));
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            FSM = new StateMachine<Type, Type, Type>();
        
            FSM.AddState<TargetIdleState>(
                animator,
                "Idle");
            
            FSM.AddState<TargetDisappearState>(
                animator,
                "Disappear");
            
            FSM.AddTriggerTransitionFromAny(typeof(StageClearEvent), typeof(TargetDisappearState), forceInstantly: true);
            
            FSM.Init();
        }

        public override void OnUpdate(float elapse)
        {
            FSM.OnLogic();
        }
    }
}
