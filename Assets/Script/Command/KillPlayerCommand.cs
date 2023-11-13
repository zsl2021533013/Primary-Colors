using QFramework;
using Script.Event;

namespace Script.Command
{
    public class  KillPlayerCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<PlayerDieEvent>();
        }
    }
}