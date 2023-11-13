using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Scene_System;
using UnityEngine;

public partial class LevelBtnController : MonoBehaviour, IController
{
    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            this.SendCommand(new LoadSceneCommand() { config = targetScene });
            UIKit.ClosePanel<UILevelSelectPanel>();
        });
    }

    public IArchitecture GetArchitecture()
    {
        return PrimaryColors.Interface;
    }
}
