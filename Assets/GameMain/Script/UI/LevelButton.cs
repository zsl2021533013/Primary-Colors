using GameMain.Script.Controller.Scene_System;
using GameMain.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Script.UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button btn;
        private LevelConfig config;

        public string LevelNumber => config != null ? config.sceneIndex : null;
        public LevelConfig Config => config;
        public Button.ButtonClickedEvent onClick => btn.onClick;

        public void SetSceneConfig(LevelConfig config)
        {
            this.config = config;
            text.text = config.sceneIndex;
            if (config.hasCollectibleObject && !config.getCollectibleObject)
            {
                image.color = Color.red;
            }
        }
    }
}