using GameMain.Script.Consts;
using QFramework;

namespace GameMain.Script.Controller.Interface
{
    public interface IColor
    {
        public BindableProperty<ColorType> Color { get; }

        public void RegisterColor();
    }
}