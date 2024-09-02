using System;
using QFramework;
using Script.Architecture;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.View_Controller.Environment_System
{
    public class BackgroundController : MonoBehaviour, IController
    { 
        /*private ResLoader mResLoader;
        
        private void Awake()
        {
            mResLoader = ResLoader.Allocate();
            
            var type = Random.Range(0, 10000) % 5 + 1;
            mResLoader.LoadSync<GameObject>("Background " + type).Instantiate();
        }

        private void OnDestroy()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }*/

        public IArchitecture GetArchitecture()
        {
            return PrimaryColors.Interface;
        }
    }
}