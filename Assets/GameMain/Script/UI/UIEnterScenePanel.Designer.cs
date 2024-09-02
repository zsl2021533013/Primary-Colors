using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using UnityEngine.Serialization;

namespace QFramework.Example
{
	// Generate Id:79c4d325-75fb-4362-b904-d7de033ef046
	public partial class UIEnterScenePanel
	{
		public const string Name = "UIEnterScenePanel";

		public Image image;
		public CanvasGroup canvasGroup;
		public TMP_Text sceneNumber;
		public TMP_Text sceneTitle;
		
		private UIEnterScenePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIEnterScenePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIEnterScenePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIEnterScenePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
