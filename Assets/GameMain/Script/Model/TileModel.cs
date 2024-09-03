using System.Collections.Generic;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Interface;
using QFramework;
using Script.Architecture;
using UnityEngine;

namespace Script.Model
{
    public class TileModel : AbstractModel
    {
        private Dictionary<Transform, (IColor, TileType)> tileDic;
        
        protected override void OnInit()
        {
            tileDic = new Dictionary<Transform, (IColor, TileType)>();
        }

        public IColor GetTileController(Transform transform)
        {
            if (!transform)
            {
                return null;
            }
            
            return tileDic.TryGetValue(transform, out var tile) ? tile.Item1 : null;
        }
        
        public ColorType GetTileColor(Transform transform)
        {
            if (!transform)
            {
                return ColorType.White;
            }
            
            return tileDic.TryGetValue(transform, out var tile) ? 
                tile.Item1.Color.Value : ColorType.White;
        }
        
        public TileType GetTileType(Transform transform)
        {
            if (!transform)
            {
                return TileType.Unchangeable;
            }
            
            return tileDic.TryGetValue(transform, out var tile) ? 
                tile.Item2 : TileType.Unchangeable;
        }
        
        public IUnRegister RegisterTile(Transform transform, IColor color, TileType tileType)
        {
            tileDic[transform] = (color, tileType);
            return new CustomUnRegister(() => tileDic.Remove(transform));
        }
    }
}