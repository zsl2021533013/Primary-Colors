using DG.Tweening;
using GameMain.Script.Controller.Scene_System;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Command
{
    public class LoadSceneCommand : AbstractCommand
    {
        [FormerlySerializedAs("config")] public LevelConfigSO configSo;
        
        protected override void OnExecute()
        {
            SceneKit.Instance.LoadScene(configSo, this.SendCommand<SpawnPlayerCommand>);
        }
    }
}