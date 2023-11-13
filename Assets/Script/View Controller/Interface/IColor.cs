using QFramework;
using Script.Architecture;

namespace Script.View_Controller.Interface
{
    public interface IColor
    {
        public BindableProperty<ColorType> Color { get; }

        public void RegisterColor();
    }
}