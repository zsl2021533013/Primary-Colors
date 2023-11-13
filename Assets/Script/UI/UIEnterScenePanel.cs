using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Script.Architecture;
using Script.View_Controller.Scene_System;

namespace QFramework.Example
{
	public class UIEnterScenePanelData : UIPanelData
	{
		public SceneConfig config;
	}
	
	public partial class UIEnterScenePanel : UIPanel
	{
		private Sequence mSequence;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIEnterScenePanelData ?? new UIEnterScenePanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mData = uiData as UIEnterScenePanelData ?? new UIEnterScenePanelData();
			
			image.material.SetFloat("_Slider", 2f);
			
			mSequence = DOTween.Sequence()
				.Append(sceneNumber.DOText(mData.config.sceneNumber, 
						mData.config.sceneNumber.Length * PrimaryColorsAsset.UICharDuration)
					.SetEase(Ease.Linear))
				.Append(sceneTitle.DOText(mData.config.sceneTitle, 
						mData.config.sceneNumber.Length * PrimaryColorsAsset.UICharDuration)
					.SetEase(Ease.Linear))
				.AppendInterval(PrimaryColorsAsset.UISceneEnterTextKeepTime)
				.Append(canvasGroup.DOFade(0f, PrimaryColorsAsset.UISceneEnterTextFadeDuration))
				.Append(image.material.DOFloat(0, "_Slider", PrimaryColorsAsset.UIFadeDuration))
				.AppendInterval(0.5f)
				.AppendCallback(CloseSelf);
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
