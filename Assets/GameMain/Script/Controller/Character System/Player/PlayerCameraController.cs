using System;
using System.Collections.Generic;
using Cinemachine;
using QFramework;
using Script.Architecture;
using UnityEngine;

namespace Script.View_Controller.Character_System.Player
{
    public partial class PlayerCameraController : MonoBehaviour, IController
    {
        private void Awake()
        {
            camera.Parent(null);
            InitConfiner();
        }

        private void InitConfiner()
        {
            var edge = GameObject.Find("Edge").GetComponent<PolygonCollider2D>();
            confiner.m_BoundingShape2D = edge;
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}