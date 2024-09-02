using System;
using QFramework;
using Script.Architecture;
using UnityEngine;

namespace Script.View_Controller
{
    public class ResInitializer : MonoBehaviour, IController
    {
        private void Awake()
        {
            ResKit.Init();
        }

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}