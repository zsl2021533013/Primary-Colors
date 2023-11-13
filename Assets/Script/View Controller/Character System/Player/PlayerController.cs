using System;
using DG.Tweening;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Event;
using Script.Model;
using Script.View_Controller.Character_System.HFSM.StateMachine;
using Script.View_Controller.Character_System.Player.State;
using Script.View_Controller.Character_System.Player.State.Air_State;
using Script.View_Controller.Character_System.Player.State.Ground_State;
using Script.View_Controller.Character_System.Player.State.Jump_State;
using Script.View_Controller.Input_System;
using Script.View_Controller.Scene_System;
using UniRx;
using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{
	public partial class PlayerController : MonoBehaviour, IController
	{
		public StateMachine<Type, Type, Type> FSM { get; private set; }
		
		private void Awake()
		{
			FSM = new StateMachine<Type, Type, Type>();

			this.RegisterEvent<PlayerRebornEvent>(e =>
			{
				FSM.Trigger(typeof(PlayerRebornEvent));
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			this.RegisterEvent<PlayerDieEvent>(e =>
			{
				FSM.Trigger(typeof(PlayerDieEvent));
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			this.RegisterEvent<StageClearEvent>(e =>
			{
				FSM.Trigger(typeof(StageClearEvent));
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			FSM.AddState<PlayerBornState>(
				animator,
				"Born",
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerDieState>(
				animator,
				"Die",
				onEnter: state =>
				{
					rb.velocity = Vector2.zero;
					PlayerGravityController.Instance.enabled = false;
				},
				onLogic: state =>
				{
					if (state.timer.IsAnimatorFinish)
					{
						this.SendCommand<RebornPlayerCommand>();
					}
				},
				onExit: state =>
				{
					PlayerGravityController.Instance.enabled = true;
					this.GetModel<PlayerModel>().ColorType.Value = ColorType.White;
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerStageClearState>(
				animator,
				"Stage Clear",
				onEnter: state =>
				{
					PlayerGravityController.Instance.enabled = false;
					rb.velocity = Vector2.zero;
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
			
			#region Ground FSM

			var groundFSM = new StateMachine<Type, Type, Type>();
			
			FSM.AddState(typeof(PlayerGroundSubFSM), groundFSM);
			
			groundFSM.AddState<PlayerGroundSelectState>(
				animator,
				"Idle",
				onEnter: state =>
				{
					InputKit.Instance.jump.Reset();
				},
				isGhostState: true);
			
			groundFSM.AddState<PlayerIdleState>(
				animator,
				"Idle",
				onEnter: state =>
				{
					rb.velocity = Vector2.zero;
					moveCollider.sharedMaterial = config.fullFriction;
				});
			
			groundFSM.AddState<PlayerMoveState>(
				animator,
				"Move",
				onEnter: state =>
				{
					moveCollider.sharedMaterial = config.zeroFriction;
				},
				onLogic: state =>
				{
					var inputX = InputKit.Instance.move.Value.x;
					inputX = inputX == 0 ? 0 : inputX > 0 ? 1 : -1;
					
					var currentSpeed = rb.velocity.x;
					var targetSpeed = inputX * config.maxSpeed;

					if (currentSpeed * targetSpeed < 0f)
					{
						currentSpeed = 0f;
					}

					var newSpeed = currentSpeed;
					if (currentSpeed > targetSpeed)
					{
						newSpeed = Mathf.Clamp(currentSpeed - config.accelerate * Time.deltaTime, 
							targetSpeed,  config.maxSpeed);
					}
					if (currentSpeed < targetSpeed)
					{
						newSpeed = Mathf.Clamp(currentSpeed + config.accelerate * Time.deltaTime, 
							-config.maxSpeed,  targetSpeed);
					}

					rb.velocity = new Vector2(newSpeed, rb.velocity.y);
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				});
			
			groundFSM.AddTransition<PlayerGroundSelectState, PlayerIdleState>
				(transition => InputKit.Instance.move.Value.x == 0);
			
			groundFSM.AddTransition<PlayerGroundSelectState, PlayerMoveState>
				(transition => InputKit.Instance.move.Value.x != 0);
			
			groundFSM.AddTransition<PlayerMoveState, PlayerIdleState> 
				(transition => InputKit.Instance.move.Value.x == 0f && rb.velocity.x == 0f);
			
			groundFSM.AddTransition<PlayerIdleState, PlayerMoveState> 
				(transition => InputKit.Instance.move.Value.x != 0f);

			#endregion
			
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
					
					rb.AddForce(new Vector2(0, config.jumpForce), ForceMode2D.Impulse);
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddState<PlayerEdgeJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					rb.AddForce(new Vector2(0, config.edgeJumpForce), ForceMode2D.Impulse);
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
					
					rb.AddForce(new Vector2(transform.Direction() * config.wallJumpForce * Mathf.Cos(config.wallJumpAngle * Mathf.Deg2Rad), 
							config.wallJumpForce * Mathf.Sin(config.wallJumpAngle * Mathf.Deg2Rad)), ForceMode2D.Impulse);
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddState<PlayerSuperJumpBeginState>(
				animator,
				"Super Jump Begin",
				onEnter: state =>
				{
					PlayerGravityController.Instance.enabled = false;
					
					var velocity = rb.velocity;
					velocity = new Vector2(velocity.x / 10f, velocity.y) ;
					rb.velocity = velocity;
				},
				onExit: state =>
				{
					PlayerGravityController.Instance.enabled = true;
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerSuperJumpState>(
				animator,
				"Air",
				onEnter: state =>
				{
					PlayerGravityController.Instance.enabled = false;
					
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
					
					var currentSpeedX = rb.velocity.x;
					var targetSpeedX = inputX * config.maxSpeed;

					var newSpeedX = currentSpeedX;
					if (currentSpeedX > targetSpeedX)
					{
						newSpeedX = Mathf.Clamp(currentSpeedX - config.accelerate * Time.deltaTime, 
							targetSpeedX,  config.maxSpeed);
					}
					if (currentSpeedX < targetSpeedX)
					{
						newSpeedX = Mathf.Clamp(currentSpeedX + config.accelerate * Time.deltaTime, 
							-config.maxSpeed,  targetSpeedX);
					}
					
					rb.velocity = new Vector2(newSpeedX, config.superJumpSpeed);
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				},
				onExit: state =>
				{
					PlayerGravityController.Instance.enabled = true;
				},
				canExit: state => state.timer > config.superJumpTime,
				needsExitTime: true);

			var isBounce = false;
			Tween bounceTween = null;
			FSM.AddState<PlayerBounceState>(
				animator,
				"Air",
				onEnter: state =>
				{
					this.SendCommand(new SpawnParticleCommand()
						{ type = ParticleType.Bounce, pos = sensorController.orangeSensor.Value.Item1.point });
					
					if (this.GetModel<PlayerModel>().ColorType == ColorType.Purple ||
					    sensorController.purpleSensor)
					{
						animator.Play("Float",0 , 0);
					}
					
					var tile = sensorController.orangeSensor.Value.Item1.transform;
					if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Touchable)
					{
						var color = this.GetModel<TileModel>()
							.GetTileColor(sensorController.groundSensor.Value.transform);
						this.SendCommand(new TouchColorCommand { color = color });
					}
					
					rb.velocity = Vector2.zero;
					if (sensorController.orangeSensor.Value.Item2 == Vector2.right)
					{
						transform.Flip();
						rb.velocity = Vector2.zero;

						if (this.GetModel<PlayerModel>().ColorType.Value == ColorType.Orange)
						{
							rb.AddForce(
								new Vector2(
									config.orangeBounceSideForce * transform.Direction() * Mathf.Cos(config.bounceAngle * Mathf.Deg2Rad),
									config.orangeBounceSideForce * Mathf.Sin(config.bounceAngle * Mathf.Deg2Rad)),
								ForceMode2D.Impulse);
						}
						else
						{
							rb.AddForce(
								new Vector2(
									config.bounceSideForce * transform.Direction() * Mathf.Cos(config.bounceAngle * Mathf.Deg2Rad),
									config.bounceSideForce * Mathf.Sin(config.bounceAngle * Mathf.Deg2Rad)),
								ForceMode2D.Impulse);
						}

						bounceTween?.Kill();
						isBounce = true;
						bounceTween = DOVirtual.DelayedCall(config.bounceTime, () => isBounce = false);
					}
					if (sensorController.orangeSensor.Value.Item2 == Vector2.down)
					{
						if (this.GetModel<PlayerModel>().ColorType.Value == ColorType.Orange)
						{
							rb.AddForce(new Vector2(0, config.orangeBounceUpForce), ForceMode2D.Impulse);
						}
						else
						{
							rb.AddForce(new Vector2(0, config.bounceUpForce), ForceMode2D.Impulse);
						}
					}
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);
			
			FSM.AddTransition<PlayerJumpSelectState, PlayerJumpState>
				(transition => this.GetModel<PlayerModel>().ColorType != ColorType.Orange);
			
			FSM.AddTransition<PlayerJumpSelectState, PlayerSuperJumpBeginState>
				(transition => this.GetModel<PlayerModel>().ColorType == ColorType.Orange);
			
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

					var currentSpeed = rb.velocity.x;
					var targetSpeed = inputX * config.maxSpeed;

					var newSpeed = currentSpeed;
					if (!isBounce)
					{
						if (currentSpeed > targetSpeed)
						{
							newSpeed = Mathf.Clamp(currentSpeed - config.accelerate * Time.deltaTime, 
								targetSpeed,  config.maxSpeed);
						}
						if (currentSpeed < targetSpeed)
						{
							newSpeed = Mathf.Clamp(currentSpeed + config.accelerate * Time.deltaTime, 
								-config.maxSpeed,  targetSpeed);
						}
					}

					rb.velocity = new Vector2(newSpeed, rb.velocity.y);
					
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
					
					var currentSpeedX = rb.velocity.x;
					var targetSpeedX = inputX * config.maxSpeed;

					var newSpeedX = currentSpeedX;
					if (!isBounce)
					{
						if (currentSpeedX > targetSpeedX)
						{
							newSpeedX = Mathf.Clamp(currentSpeedX - config.accelerate * Time.deltaTime, 
								targetSpeedX,  config.maxSpeed);
						}
						if (currentSpeedX < targetSpeedX)
						{
							newSpeedX = Mathf.Clamp(currentSpeedX + config.accelerate * Time.deltaTime, 
								-config.maxSpeed,  targetSpeedX);
						}
					}

					var newSpeedY = rb.velocity.y;
					if (this.GetModel<PlayerModel>().ColorType == ColorType.Purple && 
					    sensorController.purpleSensor)
					{
						if (newSpeedY < config.floatSpeed)
						{
							PlayerGravityController.Instance.enabled = false;
							newSpeedY = config.floatSpeed;
						}
					}
					else
					{
						PlayerGravityController.Instance.enabled = true;
						if (newSpeedY <= -config.floatSpeed)
						{
							PlayerGravityController.Instance.enabled = false;
							newSpeedY = -config.floatSpeed;
						}
					}
					
					rb.velocity = new Vector2(newSpeedX, newSpeedY);
					
					if (inputX != 0)
					{
						transform.Flip(inputX);
					}
				},
				onExit: state =>
				{
					PlayerGravityController.Instance.enabled = true;
				},
				canExit: state => state.timer > 0.1f,
				needsExitTime: true);

			airFSM.AddTransition<PlayerAirSelectState, PlayerFloatState>
			(_ => this.GetModel<PlayerModel>().ColorType == ColorType.Purple ||
			      sensorController.purpleSensor);

			airFSM.AddTransition<PlayerAirSelectState, PlayerAirState>
			(_ => this.GetModel<PlayerModel>().ColorType != ColorType.Purple && 
			      !sensorController.purpleSensor);

			airFSM.AddTransition<PlayerAirState, PlayerFloatState>
				(transition => sensorController.purpleSensor);
			
			airFSM.AddTransition<PlayerFloatState, PlayerAirState>
				(transition => InputKit.Instance.reset ||
				               !(this.GetModel<PlayerModel>().ColorType == ColorType.Purple ||
				                 sensorController.purpleSensor));
			
			#endregion
			
			FSM.AddState<PlayerWallState>(
				animator,
				"Wall",
				onEnter: state =>
				{
					PlayerGravityController.Instance.enabled = false;
					rb.velocity = Vector2.zero;
					
					transform.position =
						sensorController.wallSensor.Value.point + 
						Vector2.right * (config.wallClimbOffset * transform.Direction());
				},
				onLogic: state =>
				{
					if (transform.IsSameDirection(InputKit.Instance.move.Value.x) ||
					    InputKit.Instance.move.Value.y > 0f)
					{
						rb.velocity = new Vector2(0, config.wallClimbSpeed);
					}
					else if (InputKit.Instance.move.Value.y < 0f)
					{
						rb.velocity = new Vector2(0, -config.wallClimbSpeed);
					}
					else if (InputKit.Instance.move.Value.y == 0f)
					{
						rb.velocity = Vector2.zero;
					}
					
				},
				onExit: state =>
				{
					PlayerGravityController.Instance.enabled = true;
				});
			
			FSM.AddState<PlayerLandState>(
				animator,
				"Land",
				onEnter: state =>
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
					
					if (sensorController.groundSensor)
					{
						var tile = sensorController.groundSensor.Value.transform;
						
						if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Touchable)
						{
							var color = this.GetModel<TileModel>()
								.GetTileColor(sensorController.groundSensor.Value.transform);
							this.SendCommand(new TouchColorCommand { color = color });
						}

						if (this.GetModel<TileModel>().GetTileType(tile) == TileType.Changeable)
						{
							this.SendCommand(new TouchChangeableTileCommand() { tile = tile });
						}
					}

					if (InputKit.Instance.move.Value.x == 0)
					{
						rb.velocity = Vector2.zero;
					}
				},
				canExit: state => state.timer.IsAnimatorFinish,
				needsExitTime: true);
			
			FSM.AddState<PlayerShakeState>(
				animator,
				"Shake",
				onEnter: state =>
				{
					rb.velocity = Vector2.zero;
					
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
			
			FSM.AddTriggerTransitionFromAny(typeof(PlayerRebornEvent), typeof(PlayerBornState), forceInstantly: true);
			FSM.AddTriggerTransitionFromAny(typeof(PlayerDieEvent), typeof(PlayerDieState), forceInstantly: true);
			FSM.AddTriggerTransitionFromAny(typeof(StageClearEvent), typeof(PlayerStageClearState), forceInstantly: true);

			FSM.AddTransition<PlayerBornState, PlayerGroundSubFSM>
				(transition => true);
			
			FSM.AddTransition<PlayerGroundSubFSM, PlayerCoyoteState>
			(transition =>
			{
				if (this.GetModel<PlayerModel>().ColorType == ColorType.Black)
				{
					return !(sensorController.groundSensor || sensorController.blackGroundSensor);
				}
				else
				{
					return !sensorController.groundSensor;
				}
			});
			
			FSM.AddTransition<PlayerGroundSubFSM, PlayerJumpSelectState> 
				(transition => InputKit.Instance.jump);
			
			FSM.AddTransition<PlayerGroundSubFSM, PlayerShakeState>
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
			
			FSM.AddTransition<PlayerAirSubFSM, PlayerLandState> 
				(transition =>
				{
					if (this.GetModel<PlayerModel>().ColorType == ColorType.Black)
					{
						return sensorController.groundSensor || sensorController.blackGroundSensor;
					}
					else
					{
						return sensorController.groundSensor;
					}
				});

			FSM.AddTransition<PlayerAirSubFSM, PlayerWallState>
			(transition => sensorController.wallSensor &&
			               sensorController.edgeSensor &&
			               !sensorController.groundSensor && 
			               (this.GetModel<PlayerModel>().ColorType == ColorType.Green
			                || this.GetModel<TileModel>().GetTileColor(sensorController.wallSensor?.Value.transform) == ColorType.Green));

			FSM.AddTransition<PlayerLandState, PlayerGroundSubFSM>
				(transition => InputKit.Instance.move.Value.x != 0, true);
			
			FSM.AddTransition<PlayerLandState, PlayerGroundSubFSM> 
				(transition => true);

			FSM.AddTransition<PlayerWallState, PlayerAirSubFSM>
				(transition => !sensorController.groundSensor && !sensorController.wallSensor);
			
			FSM.AddTransition<PlayerWallState, PlayerWallJumpState>
				(transition => InputKit.Instance.jump);

			FSM.AddTransition<PlayerWallState, PlayerEdgeJumpState>
			(transition => !sensorController.edgeSensor &&
			               (transform.IsSameDirection(InputKit.Instance.move.Value.x) ||
			                InputKit.Instance.move.Value.y > 0f));
			
			FSM.AddTransition<PlayerWallState, PlayerLandState>
				(transition => sensorController.groundSensor);

			FSM.AddTransition<PlayerWallState, PlayerWallJumpState>
				(transition => transform.IsOppositeDirection(InputKit.Instance.move.Value.x), true);

			FSM.AddTransition<PlayerShakeState, PlayerAirSubFSM>
				(transition => !sensorController.groundSensor, true);
			
			FSM.AddTransition<PlayerShakeState, PlayerGroundSubFSM>
				(transition => true);

			FSM.AddTransition<PlayerCoyoteState, PlayerLandState>
				(transition => 
					{
						if (this.GetModel<PlayerModel>().ColorType == ColorType.Black)
						{
							return sensorController.groundSensor || sensorController.blackGroundSensor;
						}
						else
						{
							return sensorController.groundSensor;
						} 
					}, 
					forceInstantly: true);
			
			FSM.AddTransition<PlayerCoyoteState, PlayerJumpSelectState>
				(transition => InputKit.Instance.jump, forceInstantly: true);
			
			FSM.AddTransition<PlayerCoyoteState, PlayerAirSubFSM>
				(transition => true);
		}

		private void Start()
		{
			FSM.Init();

			Observable.EveryUpdate()
				.Subscribe(_ => FSM.OnLogic())
				.AddTo(this);
			
			Observable.EveryUpdate()
				.Subscribe(_ =>
				{
					animator.SetFloat("SpeedX", rb.velocity.x.Abs());
					animator.SetFloat("SpeedY", rb.velocity.y);
				})
				.AddTo(this);
			
			Observable.EveryUpdate()
				.Subscribe(_ =>
				{
					if (InputKit.Instance.reset)
					{
						this.GetModel<PlayerModel>().ColorType.Value = ColorType.White;
						InputKit.Instance.reset.Reset();
					}
				})
				.AddTo(this);

			var spikeEnable = true;
			var spikeFlag = true;
			Observable.EveryUpdate()
				.Subscribe(_ =>
				{
					if (spikeEnable && spikeFlag &&
					    sensorController.spikeSensor && 
					    this.GetModel<TileModel>().GetTileColor(sensorController.spikeSensor.Value.transform) 
					    != this.GetModel<PlayerModel>().ColorType)
					{
						spikeFlag = false;
						DOVirtual.DelayedCall(PrimaryColorsAsset.SpikeSensorDelay, () => spikeFlag = true);
						this.SendCommand<KillPlayerCommand>();
					}
				})
				.AddTo(this);
			
			var targetFlag = true;
			Observable.EveryUpdate()
				.Subscribe(_ =>
				{
					if (targetFlag && sensorController.targetSensor)
					{
						targetFlag = false;
						spikeEnable = false;
						
						DOVirtual.DelayedCall(PrimaryColorsAsset.TargetSensorDelay, () => targetFlag = true);
						
						FSM.Trigger(typeof(StageClearEvent));
						this.GetModel<PlayerModel>().ColorType.Value = ColorType.White; 
						
						var controller = this.GetModel<TargetModel>()
							.GetController(sensorController.targetSensor.Value.transform);
						this.SendCommand(new ClearStageCommand() { controller = controller });
					}
				})
				.AddTo(this);
			
			var collectibleFlag = true;
			Observable.EveryUpdate()
				.Subscribe(_ =>
				{
					if (collectibleFlag && sensorController.collectibleSensor)
					{
						collectibleFlag = false;
						
						DOVirtual.DelayedCall(PrimaryColorsAsset.CollectibleSensorDelay, () => collectibleFlag = true);
						
						var controller = this.GetModel<CollectibleModel>()
							.GetController(sensorController.collectibleSensor.Value.transform);
						controller.isFollowing.Value = true;
					}
				})
				.AddTo(this);
		}

		public IArchitecture GetArchitecture()
		{
			return PrimaryColors.Interface;
		}
	}
}
