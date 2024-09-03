using System;
using System.Collections;
using System.Collections.Generic;
using GameMain.Script.Consts;
using GameMain.Script.Controller;
using GameMain.Scripts.Utility;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : ControllerBase
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadSceneAsync(PathManager.GetLevelAsset("1-2"));
        }
    }
}
