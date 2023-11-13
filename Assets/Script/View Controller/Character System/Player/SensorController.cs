using System;
using QFramework;
using Script.Architecture;
using Script.Model;
using UniRx;
using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{

    public interface ISensorProperty<T>
    {
        public T Value { get; } 
        public bool IsDetected { get; }
        public void Detect();
    }
    
    public class SensorProperty<T> : ISensorProperty<T>
    {
        private Action detect;

        public T Value { get; private set; }

        public bool IsDetected { get; private set; }

        public SensorProperty(Func<T> detectFunc, Func<T, bool> resultSetter)
        {
            detect = () =>
            {
                Value = detectFunc();
                IsDetected = resultSetter(Value);
            };
        }

        public void Detect() => detect();

        public static implicit operator bool(SensorProperty<T> sensorProperty) => sensorProperty.IsDetected;
    }
    
    public partial class SensorController : MonoBehaviour, IController
    {
        public SensorProperty<RaycastHit2D> groundSensor;
        public SensorProperty<RaycastHit2D> wallSensor;
        public SensorProperty<RaycastHit2D> edgeSensor;
        public SensorProperty<Collider2D> spikeSensor;
        public SensorProperty<Collider2D> targetSensor;
        public SensorProperty<Collider2D> collectibleSensor;

        public SensorProperty<RaycastHit2D> blackGroundSensor;
        public SensorProperty<(RaycastHit2D, Vector2)> orangeSensor;
        public SensorProperty<RaycastHit2D> purpleSensor;

        private void Start()
        {
            groundSensor = new SensorProperty<RaycastHit2D>(
                () => Physics2D.Raycast(
                    groundSensorTransform.position, 
                    Vector2.down,
                    groundSensorTransform.localScale.y / 2f,
                    LayerMask.GetMask("Ground")),
                value => value.collider != null);
            
            wallSensor = new SensorProperty<RaycastHit2D>(
                () => Physics2D.Raycast(
                    wallSensorTransform.position, 
                    new Vector2(wallSensorTransform.Direction(), 0f),
                    wallSensorTransform.localScale.x / 2f,
                    LayerMask.GetMask("Ground")),
                value => value.collider != null);
            
            edgeSensor = new SensorProperty<RaycastHit2D>(
                () => Physics2D.Raycast(
                    edgeSensorTransform.position, 
                    new Vector2(edgeSensorTransform.Direction(), 0f),
                    edgeSensorTransform.localScale.x / 2f,
                    LayerMask.GetMask("Ground")),
                value => value.collider != null);

            spikeSensor = new SensorProperty<Collider2D>(
                () => Physics2D.OverlapBox(
                    spikeSensorTransform.position,
                    spikeSensorTransform.localScale,
                    0f,
                    LayerMask.GetMask("Spike")),
                value =>
                {
                    if (value == null)
                    {
                        return false;
                    }
                    
                    var type = this.GetModel<TileModel>().GetTileType(value.transform);
                    return type == TileType.Spike;
                });
            
            targetSensor = new SensorProperty<Collider2D>(
                () => Physics2D.OverlapBox(
                    spikeSensorTransform.position,
                    spikeSensorTransform.localScale,
                    0f,
                    LayerMask.GetMask("Target")),
                value => value);
            
            collectibleSensor = new SensorProperty<Collider2D>(
                () => Physics2D.OverlapBox(
                    spikeSensorTransform.position,
                    spikeSensorTransform.localScale,
                    0f,
                    LayerMask.GetMask("Collectible")),
                value => value);

            blackGroundSensor = new SensorProperty<RaycastHit2D>(
                () => Physics2D.Raycast(
                    groundSensorTransform.position, 
                    Vector2.down,
                    groundSensorTransform.localScale.y / 2f,
                    LayerMask.GetMask("Black")),
                value => value.collider != null);
            
            orangeSensor = new SensorProperty<(RaycastHit2D, Vector2)>(() =>
                {
                    var ans = Physics2D.Raycast(
                        groundSensorTransform.position,
                        Vector2.down,
                        groundSensorTransform.localScale.y / 2f,
                        LayerMask.GetMask("Ground"));

                    if (ans.collider)
                    {
                        return (ans, Vector2.down);
                    }

                    ans = Physics2D.Raycast(
                        wallSensorTransform.position,
                        new Vector2(wallSensorTransform.Direction(), 0f),
                        wallSensorTransform.localScale.x / 2f,
                        LayerMask.GetMask("Ground"));
                    
                    if (ans.collider)
                    {
                        return (ans, Vector2.right);
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
                    groundSensorTransform.position, 
                    Vector2.down,
                    float.PositiveInfinity,
                    LayerMask.GetMask("Ground") | LayerMask.GetMask("Black")),
                value =>
                {
                    var color = this.GetModel<TileModel>().GetTileColor(value.transform);
                    return color == ColorType.Purple;
                });

            Observable.EveryFixedUpdate()
                .Subscribe(_ => groundSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => wallSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => edgeSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => spikeSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => targetSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => collectibleSensor.Detect())
                .AddTo(this);

            Observable.EveryFixedUpdate()
                .Subscribe(_ => blackGroundSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => orangeSensor.Detect())
                .AddTo(this);
            
            Observable.EveryFixedUpdate()
                .Subscribe(_ => purpleSensor.Detect())
                .AddTo(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}