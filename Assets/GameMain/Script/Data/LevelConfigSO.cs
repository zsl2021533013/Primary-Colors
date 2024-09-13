using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameMain.Script.Controller.Scene_System
{
    [Serializable]
    public class LevelConfig
    {
        public string sceneIndex;
        public string sceneTitle;
        public bool hasCollectibleObject;
        
        public bool enable;
        public bool getCollectibleObject;
        

        public LevelConfig(LevelConfigSO so)
        {
            enable = false;
            sceneIndex = so.sceneIndex;
            sceneTitle = so.sceneTitle;
            hasCollectibleObject = so.hasCollectibleObject;
        }
    }
    
    [CreateAssetMenu(fileName = "Scene Config", menuName = "Scriptable Object/Scene Config")]
    public class LevelConfigSO : ScriptableObject
    {
        public string sceneIndex;
        public string sceneTitle;
        public bool hasCollectibleObject;
    }
}