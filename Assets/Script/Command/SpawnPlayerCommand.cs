using QFramework;
using Script.Architecture;
using Script.Event;
using Script.Model;
using UnityEngine;

namespace Script.Command
{
    public class SpawnPlayerCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var bornPoint = GameObject.Find("Player Spawn Point");

            var hit = Physics2D.Raycast(
                bornPoint.transform.position, 
                Vector2.down, 
                float.PositiveInfinity,
                LayerMask.GetMask("Ground"));

            var pos = hit.point + PrimaryColorsAsset.PlayerSpawnOffset * Vector2.up;
            
            var model = this.GetModel<PlayerModel>();
            if (model.Transform == null)
            {
                model.InstantiatePlayer();
            }
            
            model.Transform.position = pos;
            model.Transform.localScale = Vector3.one;
        }
    }
}