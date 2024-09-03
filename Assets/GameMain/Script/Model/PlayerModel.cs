using System;
using DG.Tweening;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Character.Player;
using QFramework;
using QFramework.Example;
using Script.Architecture;
using UnityEngine;

namespace Script.Model
{
    public class PlayerModel : AbstractModel
    {
        public Transform Transform { get; private set; }
        public Transform CollectibleTarget { get; private set; }
        public GameObject GameObject { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public PlayerController Controller { get; private set; }
        public BindableProperty<ColorType> PlayerColor { get; private set; }

        protected override void OnInit()
        {
            PlayerColor = new BindableProperty<ColorType>();
            
            PlayerColor.Register(value =>
            {
                if (SpriteRenderer)
                {
                    SpriteRenderer.material
                        .DOColor(value.ColorType2Color(), "_Color", PrimaryColorsAsset.ColorChangeDuration);
                }
                if (GameObject)
                {
                    GameObject.Layer(value == ColorType.Black ? "Player Black" : "Player");
                }
            });
            
            PlayerColor.SetValueWithoutEvent(ColorType.White);
        }

        public void RegisterPlayer(Transform transform)
        {
            Transform = transform;
            CollectibleTarget = transform.Find("Collectible Target");
            GameObject = transform.gameObject;
            SpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
            Controller = transform.GetComponentInChildren<PlayerController>();
        }
    }
}