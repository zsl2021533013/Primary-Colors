using System.Collections.Generic;
using Script.View_Controller;
using Script.View_Controller.Interface;
using UnityEngine;

namespace GameMain.Scripts.Game
{
    public class PrimaryColorsGame : GameBase
    {
        private LevelManager levelManager;
        
        public override void Initialize()
        {
            levelManager  = Object.FindObjectOfType<LevelManager>();
            levelManager.Initialize();
        }

        public override void Shutdown()
        {
            levelManager.OnGameShutdown();
        }

        public override void Update(float elapse)
        {
            levelManager.OnUpdate(elapse);
        }

        public override void FixedUpdate(float elapse)
        {
            levelManager.OnFixedUpdate(elapse);
        }
    }
}