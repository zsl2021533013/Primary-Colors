using System;
using DG.Tweening;
using GameMain.Script.Consts;
using GameMain.Script.Controller.Character.HFSM.StateMachine;
using GameMain.Script.Controller.Character.Player.State;
using GameMain.Script.Controller.Character.Player.State.Air_State;
using GameMain.Script.Controller.Character.Player.State.Jump_State;
using GameMain.Script.Controller.Environment_System.Collectible;
using GameMain.Script.Controller.Input_System;
using QFramework;
using Script;
using Script.Command;
using Script.Event;
using Script.Model;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace GameMain.Script.Controller.Character.Player
{
	public class PlayerController : ControllerBase
	{
		[ReadOnly]
		public string playerState;
		
		public PlayerConfig config;
		public Animator animator;
		public CharacterController2D cc2D;
		public SensorController sensorController;
		
		public StateMachine<Type, Type, Type> FSM { get; private set; }

		private bool isLocked;
		private Vector2 _velocity;
		
		public override void OnAwake()
		{
			cc2D.OnAwake();
			cc2D.onTriggerEnterEvent += RegisterTriggers;

			#region FSM
			
			FSM = new StateMachine<Type, Type, Type>();

			this.RegisterEvent<PlayerBornEvent>(e =>
			{
				FSM.Trigger(typeof(PlayerBornEvent));
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			this.RegisterEvent<PlayerDieEvent>(e =>
			{
				FSM.Trigger(typeof(PlayerDieEvent));
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			FSM.AddState<PlayerSleepState>(
				animator,
				"Sleep");
			
			FSM.AddState<PlayerBornState>(
				animator,
				"Born",
				onEnter: state =>
				{
					_velocity = Vector2.zero;
					this.GetModel<PlayerModel>().PlayerColor.Value = ColorType.White; 
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerDieState>(
				animator,
				"Die",
				onEnter: state =>
				{
					_velocity = Vector2.zero;
					this.GetModel<PlayerModel>().PlayerColor.Value = ColorType.White;
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerStageClearState>(
				animator,
				"Stage Clear",
				onEnter: state =>
				{
					_velocity = Vector2.zero;
					this.GetModel<PlayerModel>().PlayerColor.Value = ColorType.White; 
				},
				onLogic: state =>
				{
					if (state.timer.IsAnimatorFinish && gameObject)
					{
						gameObject.SetActive(false);
					}
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerMoveState>(
				animator,
				"Move",
				onLogic: state =>
				{
					var inputX = InputKit.Instance.move.Value.x;
					inputX = inputX == 0 ? 0 : inputX > 0 ? 1 : -1;
					
					var smoothedMovementFactor = cc2D.isGrounded ? config.groundDamping : config.airDamping;
					_velocity.x = Mathf.Lerp( _velocity.x, inputX * config.maxSpeed, Time.deltaTime * smoothedMovementFactor );
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				});
			
			#region Jump 
			
			FSM.AddState<PlayerJumpSelectState>(
				animator,
				"Air",
				isGhostState: true);
			
			FSM.AddState<PlayerJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					this.SendCommand(new SpawnParticleCommand
					{
						type = ParticleType.JumpSmoke, 
						pos = transform.position
					});
					
					InputKit.Instance.jump.Reset();

					_velocity.y = Mathf.Sqrt((2f * config.jumpHeight * config.gravity).Abs());
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddState<PlayerEdgeJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					_velocity.y = Mathf.Sqrt((2f * config.edgeJumpHeight * config.gravity).Abs());
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddState<PlayerWallJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					InputKit.Instance.jump.Reset();
					
					transform.Flip();
					
					_velocity.x = config.wallJumpHeight * Mathf.Atan(config.wallJumpAngle * Mathf.Deg2Rad);
					_velocity.y = Mathf.Sqrt((2f * config.wallJumpHeight * config.gravity).Abs());
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddState<PlayerSuperJumpBeginState>(
				animator,
				"Super Jump Begin",
				onEnter: state =>
				{
					_velocity.y = Mathf.Sqrt((2f * config.wallJumpHeight * config.gravity).Abs());
				},
				onExit: state =>
				{
					_velocity.x /= 2;
					_velocity.y = Mathf.Sqrt((2f * config.superJumpHeight * config.gravity).Abs());
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerSuperJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					this.SendCommand(new SpawnParticleCommand
					{
						type = ParticleType.JumpSmoke, 
						pos = transform.position
					});
				},
				onLogic: state =>
				{
					var inputX = InputKit.Instance.move.Value.x;
					inputX = inputX == 0 ? 0 : inputX > 0 ? 1 : -1;
					
					var smoothedMovementFactor = cc2D.isGrounded ? config.groundDamping : config.airDamping;
					_velocity.x = Mathf.Lerp( _velocity.x, inputX * config.maxSpeed, Time.deltaTime * smoothedMovementFactor );
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				},
				canExit: state => state.timer > config.superJumpTime,
				needsExitTime: true);
			
			FSM.AddState<PlayerBounceState>(
				animator,
				"Air",
				onEnter: state =>
				{
					this.SendCommand(new SpawnParticleCommand()
						{ type = ParticleType.Bounce, pos = sensorController.orangeSensor.Value.Item1.point });
					
					if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Purple ||
					    sensorController.purpleSensor)
					{
						animator.Play("Float",0 , 0);
					}
					
					var tile = sensorController.orangeSensor.Value.Item1.transform;
					if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Touchable)
					{
						var color = this.GetModel<TileModel>()
							.GetTileColor(cc2D.raycastHitVerticle[0].transform);
						this.SendCommand(new TouchColorCommand { color = color });
					}
					
					_velocity = Vector2.zero;
					if (sensorController.orangeSensor.Value.Item2 == Vector2.right)
					{
						transform.Flip();
						
						if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Orange)
						{
							_velocity.x = config.orangeBounceSideHeight * Mathf.Atan(config.bounceAngle * Mathf.Deg2Rad);
							_velocity.y = Mathf.Sqrt((2f * config.orangeBounceSideHeight * config.gravity).Abs());
						}
						else
						{
							_velocity.x = config.bounceSideHeight * Mathf.Atan(config.bounceAngle * Mathf.Deg2Rad);
							_velocity.y = Mathf.Sqrt((2f * config.bounceSideHeight * config.gravity).Abs());
						}
					}
					if (sensorController.orangeSensor.Value.Item2 == Vector2.down)
					{
						if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Orange)
						{
							_velocity.y = Mathf.Sqrt((2f * config.orangeBounceUpHeight * config.gravity).Abs());
						}
						else
						{
							_velocity.y = Mathf.Sqrt((2f * config.bounceSideHeight * config.gravity).Abs());
						}
					}
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddTransition<PlayerJumpSelectState, PlayerJumpState>
				(transition => this.GetModel<PlayerModel>().PlayerColor.Value != ColorType.Orange);
			
			FSM.AddTransition<PlayerJumpSelectState, PlayerSuperJumpBeginState>
				(transition => this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Orange);
			
			FSM.AddTransition<PlayerSuperJumpBeginState, PlayerSuperJumpState>
				(transition => true);
			
			#endregion
			
			#region Air FSM

			var airFSM = new StateMachine<Type, Type, Type>();
			
			FSM.AddState(typeof(PlayerAirSubFSM), airFSM);

			airFSM.AddState<PlayerAirSelectState>(
				animator,
				"Air",
				isGhostState: true);
			
			airFSM.AddState<PlayerAirState>(
				animator,
				"Air",
				onLogic: state =>
				{
					var inputX = InputKit.Instance.move.Value.x;
					inputX = inputX == 0 ? 0 : inputX > 0 ? 1 : -1;
					
					var smoothedMovementFactor = cc2D.isGrounded ? config.groundDamping : config.airDamping;
					_velocity.x = Mathf.Lerp( _velocity.x, inputX * config.maxSpeed, Time.deltaTime * smoothedMovementFactor );
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			airFSM.AddState<PlayerFloatState>(
				animator,
				"Float",
				onLogic: state =>
				{
					var inputX = InputKit.Instance.move.Value.x;
					inputX = inputX == 0 ? 0 : inputX > 0 ? 1 : -1;
					
					var smoothedMovementFactor = cc2D.isGrounded ? config.groundDamping : config.airDamping;
					_velocity.x = Mathf.Lerp( _velocity.x, inputX * config.maxSpeed, Time.deltaTime * smoothedMovementFactor );
					
					var newSpeedY = _velocity.y;
					if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Purple && 
					    sensorController.purpleSensor)
					{
						_velocity.y = config.floatSpeed;
					}
					else
					{
						if (newSpeedY <= -config.floatSpeed)
						{
							_velocity.y = -config.floatSpeed;
						}
					}
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);

			airFSM.AddTransition<PlayerAirSelectState, PlayerFloatState>
			(_ => this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Purple ||
			      sensorController.purpleSensor);

			airFSM.AddTransition<PlayerAirSelectState, PlayerAirState>
			(_ => this.GetModel<PlayerModel>().PlayerColor.Value != ColorType.Purple && 
			      !sensorController.purpleSensor);

			airFSM.AddTransition<PlayerAirState, PlayerFloatState>
				(transition => sensorController.purpleSensor);
			
			airFSM.AddTransition<PlayerFloatState, PlayerAirState>
				(transition => InputKit.Instance.reset ||
				               !(this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Purple ||
				                 sensorController.purpleSensor));
			#endregion
			
			FSM.AddState<PlayerWallState>(
				animator,
				"Wall",
				onEnter: state =>
				{
					transform.position =
						cc2D.raycastHitHorizontal[0].point + 
						Vector2.right * (config.wallClimbOffset * transform.Direction());
				},
				onLogic: state =>
				{
					if (transform.IsSameDirection(InputKit.Instance.move.Value.x) ||
					    InputKit.Instance.move.Value.y > 0f)
					{
						_velocity = new Vector2(0, config.wallClimbSpeed);
					}
					else if (InputKit.Instance.move.Value.y < 0f)
					{
						_velocity = new Vector2(0, -config.wallClimbSpeed);
					}
					else if (InputKit.Instance.move.Value.y == 0f)
					{
						_velocity = Vector2.zero;
					}
					
				});
			
			FSM.AddState<PlayerShakeState>(
				animator,
				"Shake",
				onEnter: state =>
				{
					_velocity = Vector2.zero;
					this.SendCommand(new SpawnParticleCommand
					{
						type = ParticleType.Shake, 
						pos = transform.position
					});
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerCoyoteState>(
				animator,
				"Air",
				canExit: state => state.timer > config.coyoteTime,
				needsExitTime: true);
			
			FSM.AddTriggerTransitionFromAny(typeof(PlayerBornEvent), typeof(PlayerBornState), forceInstantly: true);
			FSM.AddTriggerTransitionFromAny(typeof(PlayerDieEvent), typeof(PlayerDieState), forceInstantly: true);
			FSM.AddTriggerTransitionFromAny(typeof(StageClearEvent), typeof(PlayerStageClearState), forceInstantly: true);

			FSM.AddTransition<PlayerBornState, PlayerMoveState>
				(transition => true);
			
			FSM.AddTransition<PlayerMoveState, PlayerCoyoteState>
			(transition =>
			{
				if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Black)
				{
					return !(cc2D.isGrounded || sensorController.blackGroundSensor);
				}
				else
				{
					return !cc2D.isGrounded;
				}
			});
			
			FSM.AddTransition<PlayerMoveState, PlayerJumpSelectState> 
				(transition => InputKit.Instance.jump);
			
			FSM.AddTransition<PlayerMoveState, PlayerShakeState>
				(transition => InputKit.Instance.reset);
			
			FSM.AddTransition<PlayerJumpState, PlayerAirSubFSM>
				(transition => true);
			
			FSM.AddTransition<PlayerEdgeJumpState, PlayerAirSubFSM>
				(transition => true);
			
			FSM.AddTransition<PlayerWallJumpState, PlayerAirSubFSM>
				(transition => true);
			
			FSM.AddTransition<PlayerSuperJumpState, PlayerAirSubFSM>
				(transition => true);

			FSM.AddTransition<PlayerBounceState, PlayerAirSubFSM>
				(transition => true);

			FSM.AddTransition<PlayerAirSubFSM, PlayerBounceState> 
				(transition => sensorController.orangeSensor);
			
			FSM.AddTransition<PlayerAirSubFSM, PlayerMoveState> 
				(transition =>
				{
					if (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Black)
					{
						return cc2D.isGrounded || sensorController.blackGroundSensor;
					}
					else
					{
						return cc2D.isGrounded;
					}
				},
				successAction: () =>
				{
					this.SendCommand(new SpawnParticleCommand
					{
						type = ParticleType.LandSmoke, 
						pos = transform.position
					});
					
					this.SendCommand(new SpawnParticleCommand
					{
						type = ParticleType.Land, 
						pos = transform.position
					});
					
					if (cc2D.isGrounded)
					{
						var tile = cc2D.raycastHitVerticle[0].transform;
						
						if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Touchable)
						{
							var color = this.GetModel<TileModel>()
								.GetTileColor(cc2D.raycastHitVerticle[0].transform);
							this.SendCommand(new TouchColorCommand { color = color });
						}

						if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Changeable)
						{
							this.SendCommand(new TouchChangeableTileCommand() { tile = tile });
						}
					}
				});

			FSM.AddTransition<PlayerAirSubFSM, PlayerWallState>
			(transition => (transform.Direction() == -1 ? cc2D.collisionState.left : cc2D.collisionState.right) &&
			               sensorController.edgeSensor &&
			               !cc2D.isGrounded && 
			               (this.GetModel<PlayerModel>().PlayerColor.Value == ColorType.Green
			                || this.GetModel<TileModel>().GetTileColor(cc2D.raycastHitHorizontal[0].transform) == ColorType.Green));

			FSM.AddTransition<PlayerWallState, PlayerAirSubFSM>
				(transition => !cc2D.isGrounded && !(transform.Direction() == -1 ? cc2D.collisionState.left : cc2D.collisionState.right));
			
			FSM.AddTransition<PlayerWallState, PlayerWallJumpState>
				(transition => InputKit.Instance.jump);

			FSM.AddTransition<PlayerWallState, PlayerEdgeJumpState>
			(transition => !sensorController.edgeSensor &&
			               (transform.IsSameDirection(InputKit.Instance.move.Value.x) ||
			                InputKit.Instance.move.Value.y > 0f));
			
			FSM.AddTransition<PlayerWallState, PlayerWallJumpState>
				(transition => transform.IsOppositeDirection(InputKit.Instance.move.Value.x), true);

			FSM.AddTransition<PlayerShakeState, PlayerAirSubFSM>
				(transition => !cc2D.isGrounded, true);
			
			FSM.AddTransition<PlayerShakeState, PlayerMoveState>
				(transition => true);

			FSM.AddTransition<PlayerCoyoteState, PlayerJumpSelectState>
				(transition => InputKit.Instance.jump, forceInstantly: true);
			
			FSM.AddTransition<PlayerCoyoteState, PlayerAirSubFSM>
				(transition => true);
			
			FSM.Init();
			
			#endregion
		}

		private void RegisterTriggers(Collider2D col)
		{
			if (isLocked)
			{
				return;
			}
			
			if (col.CompareTag("Target"))
			{
				isLocked = true;
				FSM.Trigger(typeof(StageClearEvent));
				DOVirtual.DelayedCall(1f, this.SendCommand<StageClearCommand>);
				
				return;
			}
			
			if (col.CompareTag("Spike") && 
			    this.GetModel<TileModel>().GetTileColor(col.transform) != this.GetModel<PlayerModel>().PlayerColor.Value)
			{
				isLocked = true;
				this.SendCommand<KillPlayerCommand>();
				DOVirtual.DelayedCall(1f, () =>
				{
					isLocked = false;
					this.SendCommand<RebornPlayerCommand>();
				});
				
				return;
			}
			
			if (col.CompareTag("Collectible"))
			{
				var controller = col.GetComponent<CollectibleController>();
				controller.isFollowing.Value = true;
				
				return;
			}
		}

		public override void OnUpdate(float elapse)
		{
			base.OnUpdate(elapse);

			playerState = FSM.ActiveStateName.Name;
			
			FSM.OnLogic();

			if (!isLocked)
			{
				_velocity.y -= config.gravity * elapse;
				cc2D.move(_velocity * elapse);
				
				animator.SetFloat("SpeedX", cc2D.velocity.x.Abs());
				animator.SetFloat("SpeedY", cc2D.velocity.y);
			}
			
			if (InputKit.Instance.reset)
			{
				this.GetModel<PlayerModel>().PlayerColor.Value = ColorType.White;
				InputKit.Instance.reset.Reset();
			}

			_velocity = cc2D.velocity;
		}
	}
}
