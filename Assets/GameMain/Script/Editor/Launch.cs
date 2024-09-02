using GameMain.Scripts.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMain.Scripts.Editor
{
#if UNITY_EDITOR
    
    public class Launch
    {
        [MenuItem("Tools/Launch Game _F5")]
        public static void LaunchGame()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // 打开指定的启动场景
                EditorSceneManager.OpenScene(PathManager.GetSceneAsset("Launch"));

                // 播放模式开始
                EditorApplication.isPlaying = true;
            }
        }
    }
    
#endif
}