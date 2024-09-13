using QFramework;
using Script.Event;

namespace Script.Command
{
    public class Return2MenuCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<Return2MenuEvent>();
        }
    }
}