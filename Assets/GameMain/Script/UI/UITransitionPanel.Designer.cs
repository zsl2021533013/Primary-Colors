using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	// Generate Id:2e3f63da-e5e5-4fd2-aa22-4b5dc279c9f6
	public partial class UITransitionPanel
	{
		public const string Name = "UITranslationPanel";
		
		public Image image;
		public TMP_Text level;
		public TMP_Text title;
		
		private UITranslationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			image = null;
			
			mData = null;
		}
		
		public UITranslationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITranslationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITranslationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
