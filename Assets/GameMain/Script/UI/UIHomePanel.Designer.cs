using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:17f6ef84-5452-44ed-bdd6-ebd48462486b
	public partial class UIHomePanel
	{
		public const string Name = "UIHomePanel";

		public CanvasGroup canvasGroup;
		public Button levelSelectBtn;
		public Button exitBtn;
		
		private UIHomePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			levelSelectBtn = null;
			exitBtn = null;
			
			mData = null;
		}
		
		public UIHomePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIHomePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIHomePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
