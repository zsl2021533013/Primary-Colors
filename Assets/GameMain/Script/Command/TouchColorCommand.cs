using GameMain.Script.Consts;
using QFramework;
using Script.Architecture;
using Script.Model;

namespace Script.Command
{
    public class TouchColorCommand : AbstractCommand
    {
        public ColorType color = ColorType.White;

        protected override void OnExecute()
        {
            var playerColor = this.GetModel<PlayerModel>().PlayerColor.Value;
            this.GetModel<PlayerModel>().PlayerColor.Value = playerColor.Add(color);
        }
    }
}