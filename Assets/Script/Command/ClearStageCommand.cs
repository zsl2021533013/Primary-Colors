using DG.Tweening;
using QFramework;
using Script.Event;
using Script.View_Controller.Environment_System.Target;
using Script.View_Controller.Scene_System;
using UnityEngine;

namespace Script.Command
{
    public class ClearStageCommand : AbstractCommand
    {
        public TargetController controller;
        
        protected override void OnExecute()
        {
            controller.FSM.Trigger(typeof(StageClearEvent));

            DOVirtual.DelayedCall(1f, () => this.SendCommand(new LoadSceneCommand() { config = controller.targetScene }));
        }
    }
}