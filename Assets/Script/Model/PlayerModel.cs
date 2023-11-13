using System;
using DG.Tweening;
using QFramework;
using QFramework.Example;
using Script.Architecture;
using Script.View_Controller.Character_System.HFSM.StateMachine;
using Script.View_Controller.Character_System.Player;
using UnityEngine;

namespace Script.Model
{
    public class PlayerModel : AbstractModel
    {
        private ResLoader mResLoader;
        
        public Transform Transform { get; private set; }
        public Transform CollectibleTarget { get; private set; }
        public GameObject GameObject { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public PlayerController Controller { get; private set; }
        public BindableProperty<ColorType> ColorType { get; private set; }

        protected override void OnInit()
        {
            mResLoader = ResLoader.Allocate();
            
            ColorType = new BindableProperty<ColorType>();
            
            ColorType.Register(value =>
            {
                if (SpriteRenderer)
                {
                    SpriteRenderer.material
                        .DOColor(value.ColorType2Color(), "_Color", PrimaryColorsAsset.ColorChangeDuration);
                }
                if (GameObject)
                {
                    GameObject.Layer(value == Architecture.ColorType.Black ? "Player Black" : "Player");
                }
            });
            
            ColorType.SetValueWithoutEvent(Architecture.ColorType.White);
        }

        private void RegisterPlayer(Transform transform)
        {
            Transform = transform;
            CollectibleTarget = transform.Find("Collectible Target");
            GameObject = transform.gameObject;
            SpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
            Controller = transform.GetComponentInChildren<PlayerController>();
        }

        public void InstantiatePlayer()
        {
            var player = mResLoader.LoadSync<GameObject>("White").Instantiate();
            
            RegisterPlayer(player.transform);

            ColorType.Value = Architecture.ColorType.White;
        }
    }
}