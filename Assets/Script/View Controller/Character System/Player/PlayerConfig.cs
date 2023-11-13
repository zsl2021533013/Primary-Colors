using UnityEngine;
using UnityEngine.Serialization;

namespace Script.View_Controller.Character_System.Player
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Scriptable Object/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Ground")]
        public float maxSpeed;
        public float accelerate;
        public float coyoteTime;
        
        [Header("Jump")]
        public float jumpForce;
        public float edgeJumpForce;
        public float wallJumpForce;
        public float wallJumpAngle;
        public float superJumpSpeed;
        public float superJumpTime;
        public float bounceUpForce;
        public float bounceSideForce;
        public float orangeBounceUpForce;
        public float orangeBounceSideForce;
        public float bounceAngle;
        public float bounceTime;
        
        [Header("Air")] 
        public float floatSpeed;
        
        [Header("Wall")]
        public float wallClimbOffset;
        public float wallClimbSpeed;
        
        
        [Header("Physical Material")]
        public PhysicsMaterial2D zeroFriction;
        public PhysicsMaterial2D fullFriction;
    }
}