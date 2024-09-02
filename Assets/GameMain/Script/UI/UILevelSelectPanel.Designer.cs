using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:8f3e75cf-311b-444b-a90c-a3311b053440
	public partial class UILevelSelectPanel
	{
		public const string Name = "UILevelSelectPanel";
		
		
		private UILevelSelectPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UILevelSelectPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelSelectPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelSelectPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
