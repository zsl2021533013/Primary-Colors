using Cinemachine;
using UnityEngine;

namespace GameMain.Script.Controller.Character.Player
{
    public partial class PlayerCameraController
    {
        public Transform defaultTarget;
        public CinemachineVirtualCamera camera;
        public CinemachineConfiner2D confiner;
    }
}