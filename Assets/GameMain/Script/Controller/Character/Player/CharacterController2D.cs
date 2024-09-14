#define DEBUG_CC2D_RAYS

using System;
using System.Collections.Generic;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Interface;
using QFramework;
using Script;
using Script.Architecture;
using Script.Model;
using UnityEngine;

namespace GameMain.Script.Controller.Character.Player
{
	[RequireComponent( typeof( CapsuleCollider2D ), typeof( Rigidbody2D ) )]
	public class CharacterController2D : MonoBehaviour, IController
	{
		#region internal types

		struct CharacterRaycastOrigins
		{
			public Vector3 topLeft;
			public Vector3 bottomRight;
			public Vector3 bottomLeft;
		}

		public class CharacterCollisionState2D
		{
			public bool right;
			public bool left;
			public bool above;
			public bool below;
			public bool becameGroundedThisFrame;
			public bool wasGroundedLastFrame;
			public bool movingDownSlope;
			public float slopeAngle;

			public bool hasCollision()
			{
				return below || right || left || above;
			}
			
			public void reset()
			{
				right = left = above = below = becameGroundedThisFrame = movingDownSlope = false;
				slopeAngle = 0f;
			}
			
			public override string ToString()
			{
				return string.Format( "[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
				                     right, left, above, below, movingDownSlope, slopeAngle, wasGroundedLastFrame, becameGroundedThisFrame );
			}
		}

		#endregion

		#region events, properties and fields

		public event Action<RaycastHit2D> onControllerCollidedEvent;
		public event Action<Collider2D> onTriggerEnterEvent;
		public event Action<Collider2D> onTriggerStayEvent;
		public event Action<Collider2D> onTriggerExitEvent;

		[SerializeField]
		[Range( 0.001f, 0.3f )]
		float _skinWidth = 0.02f;

		/// <summary>
		/// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
		/// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
		/// </summary>
		public float skinWidth
		{
			get { return _skinWidth; }
			set
			{
				_skinWidth = value;
				recalculateDistanceBetweenRays();
			}
		}

		/// <summary>
		/// mask with all layers that the player should interact with
		/// </summary>
		public LayerMask platformMask = 0;

		/// <summary>
		/// mask with all layers that trigger events should fire when intersected
		/// </summary>
		public LayerMask triggerMask = 0;

		[Range( 2, 20 )]
		public int totalHorizontalRays = 8;
		[Range( 2, 20 )]
		public int totalVerticalRays = 4;
		
		public Rigidbody2D rigidBody2D;
		public CapsuleCollider2D capsuleCollider;

		[NonSerialized]
		public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();

		public Vector3 velocity { get; private set; }
		public bool isGrounded { get { return collisionState.below; } }
		
		#endregion

		/// <summary>
		/// holder for our raycast origin corners (TR, TL, BR, BL)
		/// </summary>
		CharacterRaycastOrigins _raycastOrigins;

		/// <summary>
		/// stores our raycast hit during movement
		/// </summary>
		RaycastHit2D _raycastHit;

		/// <summary>
		/// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
		/// horizontally and vertically so that we can send the events after all collision state is set
		/// </summary>
		[HideInInspector] public List<RaycastHit2D> raycastHitVerticle = new List<RaycastHit2D>();
		[HideInInspector] public List<RaycastHit2D> raycastHitHorizontal = new List<RaycastHit2D>();

		// horizontal/vertical movement data
		float _verticalDistanceBetweenRays;
		float _horizontalDistanceBetweenRays;
		
		public Transform groundSensorTransform;
		public Transform wallSensorTransform;
		public Transform edgeSensorTransform;
		
		public SensorProperty<Collider2D> groundSensor;
		public SensorProperty<Collider2D> wallSensor;
		public SensorProperty<Collider2D> edgeSensor;
        
		public SensorProperty<(RaycastHit2D, Vector2)> orangeSensor;
		public SensorProperty<RaycastHit2D> purpleSensor;
		
		#region Monobehaviour

		public void OnAwake()
		{
			// here, we trigger our properties that have setters with bodies
			skinWidth = _skinWidth;

			// we want to set our CC2D to ignore all collision layers except what is in our triggerMask
			for( var i = 0; i < 32; i++ )
			{
				// see if our triggerMask contains this layer and if not ignore it
				if( ( triggerMask.value & 1 << i ) == 0 )
				{
					Physics2D.IgnoreLayerCollision(gameObject.layer, i);
				}
			}
			
			groundSensor = new SensorProperty<Collider2D>(
				() => Physics2D.OverlapBox(
					groundSensorTransform.position, 
					groundSensorTransform.localScale,
					0f,
					LayerMask.GetMask("Ground")),
				value => value != null);
			
			wallSensor = new SensorProperty<Collider2D>(
				() => Physics2D.OverlapBox(
					wallSensorTransform.position, 
					wallSensorTransform.localScale,
					0f,
					LayerMask.GetMask("Ground")),
				value => value != null);
			
			edgeSensor = new SensorProperty<Collider2D>(
                () => Physics2D.OverlapBox(
                    edgeSensorTransform.position, 
                    edgeSensorTransform.localScale,
                    0f,
                    LayerMask.GetMask("Ground")),
                value => value != null);
            
            orangeSensor = new SensorProperty<(RaycastHit2D, Vector2)>(() =>
                {
                    var ans = Physics2D.Raycast(
                        transform.position + (Vector3)capsuleCollider.offset,
                        Vector2.down,
                        capsuleCollider.bounds.extents.y + 0.1f,
                        LayerMask.GetMask("Ground"));

                    if (ans.collider)
                    {
                        return (ans, Vector2.down);
                    }

                    ans = Physics2D.Raycast(
                        transform.position + (Vector3)capsuleCollider.offset,
                        new Vector2(transform.Direction(), 0f),
                        capsuleCollider.bounds.extents.x + 0.1f,
                        LayerMask.GetMask("Ground"));
                    
                    if (ans.collider)
                    {
                        return (ans, Vector2.right * transform.Direction());
                    }

                    return (ans, Vector2.zero);
                },
                value =>
                {
                    var color = this.GetModel<TileModel>().GetTileColor(value.Item1.transform);
                    return color == ColorType.Orange;
                });
            
            purpleSensor = new SensorProperty<RaycastHit2D>(
                () => Physics2D.Raycast(
	                transform.position + (Vector3)capsuleCollider.offset, 
                    Vector2.down,
                    float.PositiveInfinity,
                    LayerMask.GetMask("Ground")),
                value =>
                {
                    var color = this.GetModel<TileModel>().GetTileColor(value.transform);
                    return color == ColorType.Purple;
                });
		}

		public void OnTriggerEnter2D( Collider2D col )
		{
			if( onTriggerEnterEvent != null )
			{
				onTriggerEnterEvent(col);
			}
		}

		public void OnTriggerStay2D( Collider2D col )
		{
			if( onTriggerStayEvent != null )
			{
				onTriggerStayEvent(col);
			}
		}

		public void OnTriggerExit2D( Collider2D col )
		{
			if( onTriggerExitEvent != null )
			{
				onTriggerExitEvent(col);
			}
		}

		public void TriggerOnTriggerEnter(Collider2D col)
		{
			onTriggerEnterEvent?.Invoke(col);
		}

		#endregion

		[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
		void DrawRay( Vector3 start, Vector3 dir, Color color )
		{
			Debug.DrawRay( start, dir, color );
		}
		
		#region Public

		public void OnUpdate(float elapse)
		{
			groundSensor.Detect();
			wallSensor.Detect();
			edgeSensor.Detect();
			orangeSensor.Detect();
			purpleSensor.Detect();
		}
		
		/// <summary>
		/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
		/// stop when run into.
		/// </summary>
		/// <param name="deltaMovement">Delta movement.</param>
		public void move( Vector3 deltaMovement )
		{
			// save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
			collisionState.wasGroundedLastFrame = collisionState.below;

			// clear our state
			collisionState.reset();
			raycastHitVerticle.Clear();
			raycastHitHorizontal.Clear();

			primeRaycastOrigins();

			// now we check movement in the horizontal dir
			if( deltaMovement.x != 0f )
			{
				moveHorizontally(ref deltaMovement);
			}

			// next, check movement in the vertical dir
			if( deltaMovement.y != 0f )
			{
				moveVertically(ref deltaMovement);
			}

			// move then update our state
			deltaMovement.z = 0;
			transform.Translate( deltaMovement, Space.World );

			// only calculate velocity if we have a non-zero deltaTime
			if( Time.deltaTime > 0f )
			{
				velocity = deltaMovement / Time.deltaTime;
			}

			// set our becameGrounded state based on the previous and current collision state
			if( !collisionState.wasGroundedLastFrame && collisionState.below )
			{
				collisionState.becameGroundedThisFrame = true;
			}

			// send off the collision events if we have a listener
			if( onControllerCollidedEvent != null )
			{
				for( var i = 0; i < raycastHitVerticle.Count; i++ )
				{
					onControllerCollidedEvent(raycastHitVerticle[i]);
				}
				for( var i = 0; i < raycastHitHorizontal.Count; i++ )
				{
					onControllerCollidedEvent(raycastHitHorizontal[i]);
				}
			}
		}

		/// <summary>
		/// moves directly down until grounded
		/// </summary>
		public void warpToGrounded()
		{
			do
			{
				move( new Vector3( 0, -1f, 0 ) );
			} while( !isGrounded );
		}

		/// <summary>
		/// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
		/// It is also used in the skinWidth setter in case it is changed at runtime.
		/// </summary>
		public void recalculateDistanceBetweenRays()
		{
			// figure out the distance between our rays in both directions
			// horizontal
			var colliderUseableHeight = capsuleCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
			_verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

			// vertical
			var colliderUseableWidth = capsuleCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
			_horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
		}

		#endregion

		#region Movement Methods

		/// <summary>
		/// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
		/// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
		/// </summary>
		/// <param name="futurePosition">Future position.</param>
		/// <param name="deltaMovement">Delta movement.</param>
		void primeRaycastOrigins()
		{
			// our raycasts need to be fired from the bounds inset by the skinWidth
			var modifiedBounds = capsuleCollider.bounds;
			modifiedBounds.Expand( -2f * _skinWidth );

			_raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
			_raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
			_raycastOrigins.bottomLeft = modifiedBounds.min;
		}

		/// <summary>
		/// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
		/// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
		/// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
		/// actually moving the player
		/// </summary>
		void moveHorizontally( ref Vector3 deltaMovement )
		{
			var isGoingRight = deltaMovement.x > 0;
			var rayDistance = Mathf.Abs( deltaMovement.x ) + _skinWidth;
			var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
			var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
			
			for( var i = 0; i < totalHorizontalRays; i++ )
			{
				var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );

				DrawRay( ray, rayDirection * rayDistance, Color.red );
				
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask);

				if( _raycastHit )
				{
					// set our new deltaMovement and recalculate the rayDistance taking it into account
					deltaMovement.x = _raycastHit.point.x - ray.x;
					rayDistance = Mathf.Abs( deltaMovement.x );

					// remember to remove the skinWidth from our deltaMovement
					if( isGoingRight )
					{
						deltaMovement.x -= _skinWidth;
						collisionState.right = true;
					}
					else
					{
						deltaMovement.x += _skinWidth;
						collisionState.left = true;
					}

					raycastHitHorizontal.Add( _raycastHit );
				}
			}
		}
		
		void moveVertically( ref Vector3 deltaMovement )
		{
			var isGoingUp = deltaMovement.y > 0;
			var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth; 
			var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
			var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

			// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
			initialRayOrigin.x += deltaMovement.x;
			
			// if we are moving up, we should ignore the layers in oneWayPlatformMask
			var mask = platformMask;
			for( var i = 0; i < totalVerticalRays; i++ )
			{
				var ray = new Vector2( initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y );
				DrawRay( ray, rayDirection * rayDistance, Color.yellow );
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, mask );
				
				if( _raycastHit )
				{
					// set our new deltaMovement and recalculate the rayDistance taking it into account
					deltaMovement.y = _raycastHit.point.y - ray.y;
					rayDistance = Mathf.Abs( deltaMovement.y );
					
					// remember to remove the skinWidth from our deltaMovement
					if( isGoingUp )
					{
						deltaMovement.y -= _skinWidth;
						collisionState.above = true;
					}
					else
					{
						deltaMovement.y += _skinWidth;
						collisionState.below = true;
					}
					
					raycastHitVerticle.Add( _raycastHit );
				}
			}
		}

		#endregion

		public IArchitecture GetArchitecture()
		{
			return PrimaryColors.Interface;
		}
	}
}