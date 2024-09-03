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
    public partial class TargetController : MonoBehaviour, IController
    {
        public StateMachine<Type, Type, Type> FSM { get; private set; }

        private void Awake()
        {
            this.GetModel<TargetModel>()
                .RegisterTarget(transform, this)
                .UnRegisterWhenGameObjectDestroyed(this);

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

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}
