using GameMain.Script.Consts;
using GameMain.Script.Controller.Interface;
using QFramework;
using Script.Architecture;
using Script.Model;
using UnityEngine;

namespace GameMain.Script.Controller.Environment_System
{
    public class MapEdgeController : MonoBehaviour, IController, IColor
    {
        public BindableProperty<ColorType> Color { get; } = new BindableProperty<ColorType>(ColorType.Null);

        private void Awake()
        {
            RegisterColor();
        }

        public void RegisterColor()
        {
            this.GetModel<TileModel>()
                .RegisterTile(transform, this, TileType.Spike)
                .UnRegisterWhenGameObjectDestroyed(this);
        }
        
        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}