using DG.Tweening;
using QFramework;
using Script.View_Controller.Scene_System;
using UnityEngine;

namespace Script.Command
{
    public class LoadSceneCommand : AbstractCommand
    {
        public SceneConfig config;
        
        protected override void OnExecute()
        {
            SceneKit.Instance.LoadScene(config, this.SendCommand<SpawnPlayerCommand>);
        }
    }
}