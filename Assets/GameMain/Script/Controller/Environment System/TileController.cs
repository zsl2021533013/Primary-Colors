using DG.Tweening;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Interface;
using QFramework;
using Script.Architecture;
using Script.Event;
using Script.Model;
using UniRx;
using UnityEngine;
using Timer = GameMain.Script.Controller.Character.HFSM.Util.Timer;

namespace GameMain.Script.Controller.Environment_System
{
    public partial class TileController : MonoBehaviour, IController, IColor
    {
        [SerializeField] private ColorType color;
        [SerializeField] private TileType tileType;

        private Timer timer;
        
        private static readonly int Outline = Shader.PropertyToID("_Outline");
        private static readonly int ColorfulOutline = Shader.PropertyToID("_ColorfulOutline");

        public BindableProperty<ColorType> Color { get; private set; }

        private void Awake()
        {
            Color = new BindableProperty<ColorType>(color);

            timer = new Timer();

            Color.Register(value =>
            {
                timer.Reset();
                
                tilemapRenderer.material
                    .DOColor(value.ColorType2Color(), "_Color", PrimaryColorsAsset.ColorChangeDuration);
                gameObject.Layer(value == ColorType.Black ? "Black" : "Ground");
            }).UnRegisterWhenGameObjectDestroyed(this);

            RegisterColor();
            
            tilemapRenderer.material.DOColor(color.ColorType2Color(), "_Color", 0f);

            tilemapRenderer.material.SetInt(Outline, tileType == TileType.Touchable ? 0 : 1);
            tilemapRenderer.material.SetInt(ColorfulOutline, tileType == TileType.Changeable ? 1 : 0);
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
            
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (timer.Elapsed > PrimaryColorsAsset.TileResetTime)
                    {
                        Color.Value = color;
                        timer.Reset();
                    }
                });
        }

        public void RegisterColor()
        {
            this.GetModel<TileModel>()
                .RegisterTile(transform, this, tileType)
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}