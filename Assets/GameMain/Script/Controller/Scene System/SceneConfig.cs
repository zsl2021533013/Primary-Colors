using UnityEngine;
using UnityEngine.Serialization;

namespace Script.View_Controller.Scene_System
{
    [CreateAssetMenu(fileName = "Scene Config", menuName = "Scriptable Object/Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        public string sceneNumber;
        public string sceneTitle;
    }
}