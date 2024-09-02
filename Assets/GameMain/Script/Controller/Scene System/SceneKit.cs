using System;
using DG.Tweening;
using QFramework;
using QFramework.Example;
using Script.Architecture;
using UnityEngine;

namespace Script.View_Controller.Scene_System
{
    public class SceneKit : Singleton<SceneKit>
    {
        private ResLoader mResLoader;

        private SceneKit() {}

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
        
            mResLoader = ResLoader.Allocate();
        }

        public void LoadScene(SceneConfig config, Action onLoadFinish = null)
        {
            DOTween.Sequence()
                .AppendCallback(() => UIKit.OpenPanel<UIExitScenePanel>())
                .AppendInterval(PrimaryColorsAsset.UIFadeDuration)
                .AppendCallback(UIKit.ClosePanel<UIExitScenePanel>)
                .AppendCallback(() => UIKit.OpenPanel<UIEnterScenePanel>(new UIEnterScenePanelData() { config = config }))
                .AppendCallback(() => mResLoader.LoadSceneSync(config.sceneNumber))
                .AppendInterval(1f)
                .AppendCallback(() => onLoadFinish?.Invoke());
        }
    }
}
