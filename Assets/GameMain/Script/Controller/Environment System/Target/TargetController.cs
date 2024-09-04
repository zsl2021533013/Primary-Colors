using System;
using GameMain.Script.Controller.Character.HFSM.StateMachine;
using QFramework;
using Script;
using Script.Architecture;
using Script.Event;
using Script.Model;
using UniRx;
using UnityEngine;

namespace GameMain.Script.Controller.Environment_System.Target
{
    public class TargetController : ControllerBase
    {
        public Animator animator;
        
        public StateMachine<Type, Type, Type> FSM { get; private set; }

        private void Awake()
        {
            FSM = new StateMachine<Type, Type, Type>();
        
            FSM.AddState<TargetIdleState>(
                animator,
                "Idle");
            
            FSM.AddState<TargetDisappearState>(
                animator,
                "Disappear");
            
            FSM.AddTriggerTransitionFromAny(typeof(StageClearEvent), typeof(TargetDisappearState), forceInstantly: true);
        }

        private void Start()
        {
            FSM.Init();
            
            Observable.EveryUpdate()
                .Subscribe(_ => FSM?.OnLogic())
                .AddTo(this);
        }
    }
}
