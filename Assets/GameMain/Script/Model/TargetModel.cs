using System;
using System.Collections.Generic;
using GameMain.Script.Controller.Character.HFSM.StateMachine;
using GameMain.Script.Controller.Environment_System.Target;
using GameMain.Script.Controller.Scene_System;
using QFramework;
using UnityEngine;

namespace Script.Model
{
    public class TargetModel : AbstractModel
    {
        private Dictionary<Transform, TargetController> targetDic;

        protected override void OnInit()
        {
            targetDic = new Dictionary<Transform, TargetController>();
        }

        public TargetController GetController(Transform transform)
        {
            if (!transform)
            {
                return null;
            }
            
            return targetDic.TryGetValue(transform, out var controller) ? controller : null;
        }
        
        public SceneConfig GetSceneConfig(Transform transform)
        {
            if (!transform)
            {
                return null;
            }
            
            return targetDic.TryGetValue(transform, out var controller) ? controller.targetScene : null;
        }
        
        public StateMachine<Type, Type, Type> GetTargetFSM(Transform transform)
        {
            if (!transform)
            {
                return null;
            }
            
            return targetDic.TryGetValue(transform, out var controller) ? controller.FSM : null;
        }
        
        public IUnRegister RegisterTarget(Transform transform, TargetController controller)
        {
            targetDic[transform] = controller;
            return new CustomUnRegister(() => targetDic.Remove(transform));
        }
    }
}