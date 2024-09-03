using System;
using DG.Tweening;
using GameMain.Script.Controller.Character.HFSM.StateMachine;
using GameMain.Script.Controller.Character.Player.State;
using QFramework;
using Script;
using Script.Architecture;
using Script.Event;
using Script.Model;
using UniRx;
using UnityEngine;

namespace GameMain.Script.Controller.Environment_System.Collectible
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
