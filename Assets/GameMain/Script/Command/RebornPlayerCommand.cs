using DG.Tweening;
using GameMain.Script.Controller;
using GameMain.Scripts.Utility;
using QFramework;
using Script.Architecture;
using Script.Event;
using Script.Model;
using UnityEngine;

namespace Script.Command
{
    public class RebornPlayerCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var bornPoint = GameObject.Find("Player Spawn Point");

            var hit = Physics2D.Raycast(
                bornPoint.transform.position, 
                Vector2.down, 
                float.PositiveInfinity,
                LayerMask.GetMask("Ground"));

            var pos = hit.point + Vector2.up;
            
            var model = this.GetModel<PlayerModel>();
            model.Transform.position = pos;
            Physics2D.SyncTransforms(); // 强制更新，进而更新 cc2D 中 capsuleCollider.bounds 的值，否则会有延迟
            model.Controller.cc2D.warpToGrounded();
            
            this.SendEvent<PlayerBornEvent>();
        }
        
    }
}