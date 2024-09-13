using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Script.Controller.Scene_System
{
    [Serializable]
    public class GameData
    {
        public List<LevelConfig> levelList = new List<LevelConfig>();
    }
}