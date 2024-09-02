using DG.Tweening;
using QFramework;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.Scripts.UI
{
    public class CommonButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const float FadeTime = 0.6f;
        
        private CanvasGroup canvasGroup = null;

        protected override void Awake()
        {
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            
            onClick.AsObservable().Subscribe(unit => DoStateTransition(SelectionState.Highlighted, false));
        }

        public void Show()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, FadeTime);
        }

        public void Hide()
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, FadeTime);
        }

        public void ShowImmediately()
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 1f;
        }
        
        public void HideImmediately()
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0f;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            if (interactable)
            {
                DoStateTransition(SelectionState.Highlighted, true);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            
            if (interactable)
            {
                DoStateTransition(SelectionState.Normal, true);
            }
        }
    }
}