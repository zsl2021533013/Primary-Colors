using System;
using QFramework;
using Script.Architecture;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.View_Controller
{
    public class ParallaxController : MonoBehaviour, IController
    {
        public float cameraStartPositionX;

        private Camera _cam;
        
        private Vector3 _newPosition;
        private float _startX;
        private float _startY;
        private float _startZ;
        
        private float Travel => _cam.transform.position.x - cameraStartPositionX;
        
        private float DistanceFromSubject => _startZ; // _startZ 为负数时仍有意义
        
        private float ClippingPlane =>
            (_cam.transform.position.z + (DistanceFromSubject > 0 ? _cam.farClipPlane : _cam.nearClipPlane));
        private float ParallaxFactor => DistanceFromSubject / Mathf.Abs(ClippingPlane);
        
        private void Awake()
        {
            _cam = Camera.main;

            cameraStartPositionX = _cam.transform.position.x;
            
            var position = transform.position;
            _startX = position.x;
            _startY = position.y;
            _startZ = position.z;
        }

        private void Start()
        {
            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    var targetX = _startX + Travel * ParallaxFactor;
                    transform.position = new Vector3(targetX, _startY, _startZ);
                })
                .AddTo(this);
        }
        
        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}