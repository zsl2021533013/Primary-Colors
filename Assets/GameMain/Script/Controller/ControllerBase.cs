using GameMain.Script.Controller.Interface;
using QFramework;
using Script.Architecture;
using UnityEngine;

namespace GameMain.Script.Controller
{
    public class ControllerBase : MonoBehaviour, IPrimaryColorsController, IController
    {
        public virtual void OnAwake()
        {
        }

        public virtual void OnUpdate(float elapse)
        {
        }

        public virtual void OnFixedUpdate(float elapse)
        {
        }

        public virtual void OnGameShutdown()
        {
        }
        
        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}