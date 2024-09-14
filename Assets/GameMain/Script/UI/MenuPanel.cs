using System.Collections.Generic;
using System.Linq;
using GameMain.Script.Controller.Scene_System;
using GameMain.Script.UI;
using GameMain.Scripts.Utility;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Scripts.UI
{
    public class MenuPanelData : UIPanelData
    {
        public List<LevelConfig> levelList;
    }
    
    public class MenuPanel : UIPanel
    {
        public Transform content;
        public TMP_Text collectibleObjectCount;
        public Button exitBtn;
        [HideInInspector] public List<LevelButton> levelBtnList;
        
        protected override void OnClose()
        {
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            base.OnOpen(uiData);

            var data = (MenuPanelData)uiData;

            var getCollectibleObject =
                data.levelList.Where(l => l.hasCollectibleObject && l.getCollectibleObject).ToList().Count;
            var hasCollectibleObject =
                data.levelList.Where(l => l.hasCollectibleObject).ToList().Count;
            collectibleObjectCount.text =
                getCollectibleObject == hasCollectibleObject ? "Perfect!" : $"{getCollectibleObject}/{hasCollectibleObject}";
            
            exitBtn.onClick.AddListener(Application.Quit);

            levelBtnList = new List<LevelButton>();
            
            var levelBtnTemplate = Resources.Load<GameObject>(PathManager.GetUIAsset("LevelBtn"));
            foreach (var config in data.levelList)
            {
                if (!config.enable)
                {
                    break;
                }
                
                var b = levelBtnTemplate.Instantiate();
                b.Parent(content);
                var lb = b.GetComponent<LevelButton>();
                levelBtnList.Add(lb);
                lb.SetSceneConfig(config);
            }
        }
    }
}