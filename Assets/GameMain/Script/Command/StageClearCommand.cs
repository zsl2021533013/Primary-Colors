using DG.Tweening;
using GameMain.Script.Controller.Environment_System.Target;
using QFramework;
using Script.Event;
using UnityEngine;

namespace Script.Command
{
    public class StageClearCommand : AbstractCommand
    {
        public bool takeCollectible;
        
        protected override void OnExecute()
        {
            this.SendEvent<StageClearEvent>();
            DOVirtual.DelayedCall(1f, () => 
                this.SendEvent(new NextLevelEvent() {getCollectibleObjectInThisLevel = takeCollectible}));
        }
    }
}