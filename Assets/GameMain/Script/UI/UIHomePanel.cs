using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Scene_System;

namespace QFramework.Example
{
	public class UIHomePanelData : UIPanelData
	{
	}
	public partial class UIHomePanel : UIPanel, IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIHomePanelData ?? new UIHomePanelData();
			
			levelSelectBtn.onClick.AddListener(() =>
			{
				CloseSelf();
				UIKit.OpenPanel<UILevelSelectPanel>();
			});
			
			exitBtn.onClick.AddListener(Application.Quit);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
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

		public IArchitecture GetArchitecture()
		{
			return PrimaryColors.Interface;
		}
	}
}
