using System.Collections.Generic;
using DG.Tweening;
using GameMain.Script.Controller.Interface;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Event;
using Script.Model;
using UnityEngine;

namespace GameMain.Script.Controller
{
    public class LevelManager : MonoSingleton<LevelManager>, IController, IPrimaryColorsController
    {
        private List<IPrimaryColorsController> controllers;

        private List<IPrimaryColorsController> addCache;
        private List<IPrimaryColorsController> removeCache;
        
        private LevelManager() {}

        public void OnAwake()
        {
            controllers = new List<IPrimaryColorsController>();
            addCache = new List<IPrimaryColorsController>();
            removeCache = new List<IPrimaryColorsController>();
            
            this.SendCommand<SpawnPlayerCommand>();

            DOVirtual.DelayedCall(2f, () =>
            {
                this.GetModel<PlayerModel>().Controller.FSM.Trigger(typeof(PlayerBornEvent));
            });
        }

        public void OnUpdate(float elapse)
        {
            addCache.ForEach(controller => controllers.Add(controller));
            addCache.Clear();
            
            removeCache.ForEach(controller => controllers.Remove(controller));
            removeCache.Clear();
            
            controllers.ForEach(controller => controller.OnUpdate(elapse));
        }

        public void OnFixedUpdate(float elapse)
        {
            controllers.ForEach(controller => controller.OnFixedUpdate(elapse));
        }

        public void OnGameShutdown()
        {
            controllers.ForEach(controller => controller.OnGameShutdown());
            
            controllers.Clear();
            addCache.Clear();
            removeCache.Clear();
            
            controllers = null;
            addCache = null;
            removeCache = null;
        }

        public ControllerBase InstantiateController(GameObject mGameObject)
        {
            return InstantiateController(mGameObject, Vector3.zero, Quaternion.identity);
        }
        
        public ControllerBase InstantiateController(GameObject mGameObject, Vector3 position, Quaternion rotation)
        {
            var instance = mGameObject.Instantiate(position, rotation);
            var controller = instance.GetComponentInChildren<ControllerBase>();
            
            addCache.Add(controller);
            controller.OnAwake();
            
            return controller;
        }
        
        public void DestroyController(ControllerBase controller)
        {
            removeCache.Add(controller);
            controller.gameObject.DestroySelf();
        }
        
        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}