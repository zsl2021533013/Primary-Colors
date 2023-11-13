using System;
using DG.Tweening;
using QFramework;
using Script.Architecture;
using Script.Event;
using Script.Model;
using Script.View_Controller.Character_System.HFSM.StateMachine;
using Script.View_Controller.Character_System.Player.State;
using Script.View_Controller.Character_System.Player.State.Ground_State;
using Script.View_Controller.Environment_System.Target;
using UniRx;
using UnityEngine;

namespace Script.View_Controller.Environment_System
{
    public partial class CollectibleController : MonoBehaviour, IController
    {
        public ReactiveProperty<bool> isFollowing = new ReactiveProperty<bool>(false);

        public StateMachine<Type, Type, Type> FSM { get; private set; }
    
        private Vector3 mStartPos;

        private void Awake()
        {
            this.GetModel<CollectibleModel>()
                .RegisterCollectible(transform, this)
                .UnRegisterWhenGameObjectDestroyed(this);
            
            mStartPos = transform.position;

            this.RegisterEvent<PlayerDieEvent>(e =>
            {
                isFollowing.Value = false;
                transform.DOMove(mStartPos, PrimaryColorsAsset.CollectibleResetDuration);
            }).UnRegisterWhenGameObjectDestroyed(this);
            
            FSM = new StateMachine<Type, Type, Type>();
        
            FSM.AddState<CollectibleIdleState>(
                animator,
                "Idle");
            
            FSM.AddState<CollectibleDisappearState>(
                animator,
                "Disappear",
                onLogic: state =>
                {
                    if (state.timer.IsAnimatorFinish)
                    {
                        gameObject.SetActive(false);
                    }
                });
            
            FSM.AddTriggerTransitionFromAny(typeof(CollectibleDisappearEvent), typeof(CollectibleDisappearState), forceInstantly: true);
        }

        private void Start()
        {
            FSM.Init();
            
            Observable.EveryUpdate()
                .Subscribe(_ => FSM?.OnLogic())
                .AddTo(this);

            isFollowing
                .First(_ => isFollowing.Value)
                .Subscribe(_ => Wake())
                .AddTo(this);
        }

        public void Wake()
        {
            var collectibleFlag = true;
            var target = this.GetModel<PlayerModel>().CollectibleTarget;
            var playerFSM = this.GetModel<PlayerModel>().Controller.FSM;
            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    if (isFollowing.Value && collectibleFlag)
                    {

                        if (playerFSM.ActiveStateName == typeof(PlayerStageClearState))
                        {
                            collectibleFlag = false;
                            FSM.Trigger(typeof(CollectibleDisappearEvent));
                        }

                        transform.position = Vector3.Lerp(transform.position, target.position, PrimaryColorsAsset.CollectibleChasingDuration);
                    }
                })
                .AddTo(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}
