using QFramework;
using Script.Architecture;
using Script.Model;
using UnityEngine;

namespace Script.Command
{
    public class TouchChangeableTileCommand : AbstractCommand
    {
        public Transform tile;
        
        protected override void OnExecute()
        {
            var playerColor = this.GetModel<PlayerModel>().ColorType.Value;
            var tileColor = this.GetModel<TileModel>().GetTileColor(tile);

            var newColor = tileColor.Add(playerColor);

            this.GetModel<TileModel>().GetTileController(tile).Color.Value = newColor;
        }
    }
}