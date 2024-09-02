using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{
	public partial class PlayerController
	{
		public PlayerConfig config;
		public Animator animator;
		public SpriteRenderer sprite;
		public Rigidbody2D rb;
		public CapsuleCollider2D moveCollider;
		public SensorController sensorController;
	}
}
