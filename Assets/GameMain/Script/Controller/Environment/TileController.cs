using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Interface;
using GameMain.Script.Utility;
using GameMain.Scripts.Utility;
using QFramework;
using Script.Architecture;
using Script.Event;
using Script.Model;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Timer = GameMain.Script.Controller.Character.HFSM.Util.Timer;

namespace GameMain.Script.Controller.Environment_System
{
    public partial class TileController : ControllerBase, IColor
    {
        public Tilemap tilemap;
        public TilemapRenderer tilemapRenderer;
        public CompositeCollider2D collider;
        
        [SerializeField] private ColorType color;
        [SerializeField] private TileType tileType;
        
        private Timer timer;
        
        private static readonly int Outline = Shader.PropertyToID("_OutlineEnable");
        private static readonly int ColorfulOutline = Shader.PropertyToID("_ColorfulOutlineEnable");

        public BindableProperty<ColorType> Color { get; private set; }

        private List<MeshRenderer> purpleParticleRenderer = new List<MeshRenderer>();
        private List<SpriteRenderer> greenGrassRenderer = new List<SpriteRenderer>();
        
        private static readonly int Duration = Shader.PropertyToID("_Duration");

        private bool pending2Reset = false;

        private Transform playerTransform;

        private Transform PlayerTransform
        {
            get
            {
                if (playerTransform == null)
                {
                    playerTransform = this.GetModel<PlayerModel>().Transform;
                }

                return playerTransform;
            }
        }

        private void Awake()
        {
            Color = new BindableProperty<ColorType>(color);

            timer = new Timer();
            
            tilemap.color = UnityEngine.Color.white;

            Color.Register(value =>
            {
                timer.Reset();
                
                tilemapRenderer.material.DOColor(value.ColorType2Color(), "_Color", PrimaryColorsAsset.ColorChangeDuration);
            }).UnRegisterWhenGameObjectDestroyed(this);

            if (tileType == TileType.Changeable)
            {
                Color.Register(value =>
                {
                    if (value == ColorType.Black)
                    {
                        LevelManager.Instance.blackTileList.Add(this);
                    }
                    else
                    {
                        EnableCollision();
                        LevelManager.Instance.blackTileList.Remove(this);
                    }
                }).UnRegisterWhenGameObjectDestroyed(this);
            }

            RegisterColor();
            
            tilemapRenderer.material.DOColor(color.ColorType2Color(), "_Color", 0f);

            tilemapRenderer.material.SetInt(Outline, tileType == TileType.Touchable ? 0 : 1);
            tilemapRenderer.material.SetInt(ColorfulOutline, tileType == TileType.Changeable ? 1 : 0);

            if (tileType == TileType.Changeable || (color == ColorType.Green && tileType != TileType.Spike))
            {
                var p = new ParticleGenerator(tilemap, this);
                p.GenerateGreenGrass(greenGrassRenderer);
                
                if (greenGrassRenderer.FirstOrDefault())
                {
                    greenGrassRenderer.ForEach(r => r.material.SetInt(Duration, 1));
                }
            }

            if (tileType == TileType.Changeable || (color == ColorType.Purple && tileType != TileType.Spike))
            {
                var p = new ParticleGenerator(tilemap, this);
                p.GeneratePurpleParticle(purpleParticleRenderer);
                
                if (purpleParticleRenderer.FirstOrDefault())
                {
                    purpleParticleRenderer.ForEach(r => r.material.SetInt(Duration, 1));
                }
            }
            
            if (tileType == TileType.Changeable)
            {
                purpleParticleRenderer.ForEach(i => i.material.SetFloat(Duration, color == ColorType.Purple ? 1f : 0f));
                greenGrassRenderer.ForEach(r => r.material.SetInt(Duration, color == ColorType.Green ? 1 : 0));
                
                Color.Register(value =>
                {
                    if (value == ColorType.Purple)
                    {
                        purpleParticleRenderer.ForEach(i => i.material.SetFloat(Duration, 0f));
                        purpleParticleRenderer.ForEach(i => i.material.DOKill());
                        purpleParticleRenderer.ForEach(i => i.material.DOFloat(1f, Duration, 1f));
                    }
                    else
                    {
                        purpleParticleRenderer.ForEach(i => i.material.DOKill());
                        purpleParticleRenderer.ForEach(i => i.material.DOFloat(0f, Duration, 1f));
                    }
                    
                    if (value == ColorType.Green)
                    {
                        greenGrassRenderer.ForEach(r => r.material.SetInt(Duration, 0));
                        greenGrassRenderer.ForEach(r => r.material.DOKill());
                        greenGrassRenderer.ForEach(r => r.material.DOFloat(1f, Duration, 1f));
                    }
                    else
                    {
                        greenGrassRenderer.ForEach(r => r.material.DOKill());
                        greenGrassRenderer.ForEach(r => r.material.DOFloat(0f, Duration, 1f));
                    }
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            
            
        }

        private void Start()
        {
            if (tileType == TileType.Changeable)
            {
                this.RegisterEvent<PlayerDieEvent>(e =>
                {
                    Color.Value = color;
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
        }

        private void Update()
        {
            if (pending2Reset && !CheckPlayerCollision())
            {
                pending2Reset = false;
                Color.Value = color;
                timer.Reset();
            }
            
            if (tileType == TileType.Changeable && timer.Elapsed > PrimaryColorsAsset.TileResetTime)
            {
                if (CheckPlayerCollision())
                {
                    pending2Reset = true;
                }
                else
                {
                    Color.Value = color;
                    timer.Reset();
                }
            }
        }

        public void RegisterColor()
        {
            this.GetModel<TileModel>()
                .RegisterTile(transform, this, tileType)
                .UnRegisterWhenGameObjectDestroyed(this);
        }
        
        private bool CheckPlayerCollision()
        {
            // 如果有碰撞，返回 true
            return tilemap.HasTile(Vector3Int.FloorToInt(PlayerTransform.position));
        }
        
        public void EnableCollision()
        {
            collider.enabled = true;
        }

        public void DisableCollision()
        {
            collider.enabled = false;
        }
    }
}