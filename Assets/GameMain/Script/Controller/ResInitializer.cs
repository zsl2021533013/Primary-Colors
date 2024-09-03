using QFramework;
using Script.Architecture;
using UnityEngine;

namespace GameMain.Script.Controller
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