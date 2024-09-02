using System;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Scripts.UI
{
    public class SceneChangePanel : UIPanel
    {
        private const float FadeTime = 2f;

        [SerializeField] 
        private Image backgroundImage;

        public bool IsFading { get; private set; }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);
        }
        
        protected override void OnClose()
        {
            
        }

        public void FadeIn(Action endAction = null)
        {
            IsFading = true;
            backgroundImage.material.DOKill();
            
            backgroundImage.material.SetFloat("_Slider", 1f);
            backgroundImage.material.DOFloat(0, "_Slider", FadeTime).OnComplete(() =>
            {
                endAction?.Invoke();
                IsFading = false;
            });;
        }

        public void FadeOut(Action endAction = null)
        {
            IsFading = true;
            backgroundImage.material.DOKill();

            backgroundImage.material.SetFloat("_Slider", 0f);
            backgroundImage.material.DOFloat(1, "_Slider", FadeTime).OnComplete(() =>
            {
                endAction?.Invoke();
                IsFading = false;
            });
        }
        
        public void FadeInImmediately()
        {
            backgroundImage.material.SetFloat("_Slider", 0f);
        }

        public void FadeOutImmediately()
        {
            backgroundImage.material.SetFloat("_Slider", 1f);
        }
    }
}