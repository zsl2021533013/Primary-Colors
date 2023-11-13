using System;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Model;
using Script.View_Controller.Interface;
using UnityEngine;

namespace Script.View_Controller.Environment_System
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