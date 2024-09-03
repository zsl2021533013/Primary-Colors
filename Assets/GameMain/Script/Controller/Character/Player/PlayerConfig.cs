using UnityEngine;

namespace GameMain.Script.Controller.Character.Player
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "Scriptable Object/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Ground")]
        public float maxSpeed;
        public float coyoteTime;
        public float groundDamping;
        
        [Header("Jump")]
        public float jumpHeight;
        public float edgeJumpHeight;
        public float wallJumpHeight;
        public float wallJumpAngle;
        public float superJumpHeight;
        public float superJumpTime;
        public float bounceUpHeight;
        public float bounceSideHeight;
        public float orangeBounceUpHeight;
        public float orangeBounceSideHeight;
        public float bounceAngle;
        public float bounceTime;
        
        [Header("Air")] 
        public float floatSpeed;
        public float airDamping;
        
        [Header("Wall")]
        public float wallClimbOffset;
        public float wallClimbSpeed;

        [Header("Environment")] 
        public float gravity;
    }
}