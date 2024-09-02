using System.Collections.Generic;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Interface;
using UnityEngine;

namespace Script.View_Controller
{
    public class LevelManager : MonoSingleton<LevelManager>, IController, IPrimaryColorsController
    {
        private List<IPrimaryColorsController> controllers = new List<IPrimaryColorsController>();
        
        private LevelManager() {}
        
        public void Initialize()
        {
            this.SendCommand<SpawnPlayerCommand>();
        }

        public void OnAwake()
        {
            controllers = new List<IPrimaryColorsController>();
        }

        public void OnUpdate(float elapse)
        {
            controllers.ForEach(controller => controller.OnUpdate(elapse));
        }

        public void OnFixedUpdate(float elapse)
        {
            controllers.ForEach(controller => controller.OnFixedUpdate(elapse));
        }

        public void OnGameShutdown()
        {
            controllers.ForEach(controller => controller.OnGameShutdown());
            controllers = null;
        }

        public ControllerBase InstantiateController(GameObject mGameObject)
        {
            return InstantiateController(mGameObject, Vector3.zero, Quaternion.identity);
        }
        
        public ControllerBase InstantiateController(GameObject mGameObject, Vector3 position, Quaternion rotation)
        {
            var instance = mGameObject.Instantiate(position, rotation);
            var controller = instance.GetComponentInChildren<ControllerBase>();
            controllers.Add(controller);
            return controller;
        }
        
        public void DestroyController(ControllerBase controller)
        {
            controllers.Remove(controller);
            controller.gameObject.DestroySelf();
        }
        
        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}