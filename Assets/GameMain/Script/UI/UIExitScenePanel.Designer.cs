using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Serialization;

namespace QFramework.Example
{
	// Generate Id:92587264-07b1-47da-ba7e-f5f8d6184be7
	public partial class UIExitScenePanel
	{
		public const string Name = "UIExitScenePanel";
		
		public Image image;
		
		private UIExitScenePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIExitScenePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIExitScenePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIExitScenePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
