using QFramework;
using UnityEngine.UI;

namespace GameMain.Scripts.UI
{
    public class MainMenuPanelData : UIPanelData
    {
    }
    
    public class MenuPanel : UIPanel
    {
        public Button startGameBtn;

        protected override void OnClose()
        {
        }
    }
}