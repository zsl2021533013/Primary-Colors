using QFramework;
using Script.Architecture;
using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{
    public partial class PlayerGravityController : MonoSingleton<PlayerGravityController>
    {
        private void FixedUpdate()
        {
            if (rb.velocity.y < -PrimaryColorsAsset.MaxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -PrimaryColorsAsset.MaxFallSpeed);
            }
            else
            {
                rb.AddForce((rb.velocity.y >= 0f
                                ? PrimaryColorsAsset.JumpGravityFactor
                                : PrimaryColorsAsset.FallGravityFactor) *
                            PrimaryColorsAsset.Gravity * PrimaryColorsAsset.GravityScale * Vector2.down,
                    ForceMode2D.Force);
            }
        }
    }
}