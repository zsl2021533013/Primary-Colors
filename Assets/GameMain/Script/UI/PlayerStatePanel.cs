using System;
using DG.Tweening;
using GameMain.Script.Consts;
using GameMain.Scripts.Utility;
using JetBrains.Annotations;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Script.UI
{
    public class PlayerStatePanel : UIPanel
    {
        public Image currentState;
        public Image ArrowA;
        public Image ArrowB;
        public Image ArrowC;
        public Image NextStateA;
        public Image NextStateB;
        public Image NextStateC;
        
        protected override void OnClose()
        {
        }
        
        

        public void ChangeState(ColorType color)
        {
            switch (color)
            {
                case ColorType.Null:
                    break;
                case ColorType.White:
                    ArrowA.gameObject.SetActive(false);
                    ArrowB.gameObject.SetActive(false);
                    ArrowC.gameObject.SetActive(false);
                    NextStateA.gameObject.SetActive(false);
                    NextStateB.gameObject.SetActive(false);
                    NextStateC.gameObject.SetActive(false);
                    currentState.DOColor(color.ColorType2Color(), 1f);
                    currentState.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("WhiteState"));
                    break;
                case ColorType.Black:
                    ArrowA.gameObject.SetActive(false);
                    ArrowB.gameObject.SetActive(false);
                    ArrowC.gameObject.SetActive(false);
                    NextStateA.gameObject.SetActive(false);
                    NextStateB.gameObject.SetActive(false);
                    NextStateC.gameObject.SetActive(false);
                    currentState.DOColor(color.ColorType2Color(), 1f);
                    currentState.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("WhiteState"));
                    break;
                case ColorType.Red:
                    ArrowA.gameObject.SetActive(true);
                    ArrowB.gameObject.SetActive(true);
                    ArrowC.gameObject.SetActive(false);
                    NextStateA.gameObject.SetActive(true);
                    NextStateB.gameObject.SetActive(true);
                    NextStateC.gameObject.SetActive(false);
                    currentState.DOColor(color.ColorType2Color(), 1f);
                    ArrowA.DOColor(ColorType.Blue.ColorType2Color(), 1f);
                    ArrowB.DOColor(ColorType.Yellow.ColorType2Color(), 1f);
                    NextStateA.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("PurpleState"));
                    NextStateB.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("OrangeState"));
                    break;
                case ColorType.Yellow:
                    ArrowA.gameObject.SetActive(true);
                    ArrowB.gameObject.SetActive(true);
                    ArrowC.gameObject.SetActive(false);
                    NextStateA.gameObject.SetActive(true);
                    NextStateB.gameObject.SetActive(true);
                    NextStateC.gameObject.SetActive(false);
                    currentState.DOColor(color.ColorType2Color(), 1f);
                    ArrowA.DOColor(ColorType.Red.ColorType2Color(), 1f);
                    ArrowB.DOColor(ColorType.Blue.ColorType2Color(), 1f);
                    NextStateA.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("OrangeState"));
                    NextStateB.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("GreenState"));
                    break;
                case ColorType.Blue:
                    ArrowA.gameObject.SetActive(true);
                    ArrowB.gameObject.SetActive(true);
                    ArrowC.gameObject.SetActive(false);
                    NextStateA.gameObject.SetActive(true);
                    NextStateB.gameObject.SetActive(true);
                    NextStateC.gameObject.SetActive(false);
                    currentState.DOColor(color.ColorType2Color(), 1f);
                    ArrowA.DOColor(ColorType.Red.ColorType2Color(), 1f);
                    ArrowB.DOColor(ColorType.Yellow.ColorType2Color(), 1f);
                    NextStateA.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("PurpleState"));
                    NextStateB.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("GreenState"));
                    break;
                case ColorType.Orange:
                    ArrowA.gameObject.SetActive(false);
                    ArrowB.gameObject.SetActive(false);
                    ArrowC.gameObject.SetActive(true);
                    NextStateA.gameObject.SetActive(false);
                    NextStateB.gameObject.SetActive(false);
                    NextStateC.gameObject.SetActive(true);
                    currentState.DOColor(ColorType.White.ColorType2Color(), 1f);
                    currentState.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("OrangeState"));
                    ArrowC.DOColor(ColorType.Blue.ColorType2Color(), 1f);
                    NextStateC.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("WhiteState"));
                    NextStateC.DOColor(ColorType.Black.ColorType2Color(), 1f);
                    break;
                case ColorType.Purple:
                    ArrowA.gameObject.SetActive(false);
                    ArrowB.gameObject.SetActive(false);
                    ArrowC.gameObject.SetActive(true);
                    NextStateA.gameObject.SetActive(false);
                    NextStateB.gameObject.SetActive(false);
                    NextStateC.gameObject.SetActive(true);
                    currentState.DOColor(ColorType.White.ColorType2Color(), 1f);
                    currentState.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("PurpleState"));
                    ArrowC.DOColor(ColorType.Yellow.ColorType2Color(), 1f);
                    NextStateC.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("WhiteState"));
                    NextStateC.DOColor(ColorType.Black.ColorType2Color(), 1f);
                    break;
                case ColorType.Green:
                    ArrowA.gameObject.SetActive(false);
                    ArrowB.gameObject.SetActive(false);
                    ArrowC.gameObject.SetActive(true);
                    NextStateA.gameObject.SetActive(false);
                    NextStateB.gameObject.SetActive(false);
                    NextStateC.gameObject.SetActive(true);
                    currentState.DOColor(ColorType.White.ColorType2Color(), 1f);
                    currentState.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("GreenState"));
                    ArrowC.DOColor(ColorType.Red.ColorType2Color(), 1f);
                    NextStateC.sprite = Resources.Load<Sprite>(PathManager.GetSpriteAsset("WhiteState"));
                    NextStateC.DOColor(ColorType.Black.ColorType2Color(), 1f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
    }
}
