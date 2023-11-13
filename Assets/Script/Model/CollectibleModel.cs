using System.Collections.Generic;
using QFramework;
using Script.View_Controller.Environment_System;
using Script.View_Controller.Environment_System.Target;
using UnityEngine;

namespace Script.Model
{
    public class CollectibleModel : AbstractModel
    {
        private Dictionary<Transform, CollectibleController> collectibleDic;

        protected override void OnInit()
        {
            collectibleDic = new Dictionary<Transform, CollectibleController>();
        }
        
        public CollectibleController GetController(Transform transform)
        {
            if (!transform)
            {
                return null;
            }
            
            return collectibleDic.TryGetValue(transform, out var controller) ? controller : null;
        }
        
        public IUnRegister RegisterCollectible(Transform transform, CollectibleController controller)
        {
            collectibleDic[transform] = controller;
            return new CustomUnRegister(() => collectibleDic.Remove(transform));
        }
    }
}