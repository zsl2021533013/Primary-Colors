using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Script.View_Controller.Scene_System;

namespace QFramework.Example
{
	public class UITranslationPanelData : UIPanelData
	{
		public SceneConfig config;
	}
	
	public partial class UITransitionPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UITranslationPanelData ?? new UITranslationPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			if (uiData != null)
			{
				
			}
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
