using QFramework;
using Script.Architecture;
using Script.View_Controller.Interface;
using UnityEngine;

namespace Script.View_Controller
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