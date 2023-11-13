using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Script.Architecture;
using Script.View_Controller.Scene_System;

namespace QFramework.Example
{
	public class UIExitScenePanelData : UIPanelData
	{
	}
	
	public partial class UIExitScenePanel : UIPanel
	{
		private Sequence mSequence;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIExitScenePanelData ?? new UIExitScenePanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			image.material.SetFloat("_Slider", 0f);
			
			mSequence = DOTween.Sequence()
				.Append(image.material.DOFloat(2, "_Slider", PrimaryColorsAsset.UIFadeDuration));
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			mSequence.Kill();
		}
	}
}
