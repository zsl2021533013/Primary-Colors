using DG.Tweening;
using GameMain.Script.Controller.Environment_System.Target;
using QFramework;
using Script.Event;
using UnityEngine;

namespace Script.Command
{
    public class StageClearCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<StageClearEvent>();
            DOVirtual.DelayedCall(1f, this.SendEvent<NextLevelEvent>);
        }
    }
}