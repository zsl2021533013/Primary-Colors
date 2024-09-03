using DG.Tweening;
using GameMain.Script.Controller.Scene_System;
using QFramework;
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