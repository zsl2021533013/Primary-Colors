using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using Script.Architecture;
using Script.Command;
using Script.View_Controller.Input_System;
using UniRx;
using UnityEngine;

public class Starter : MonoBehaviour, IController
{
    private void Awake()
    {
        UIKit.Root.ScreenSpaceOverlayRenderMode();
        
        UIKit.Root.SetResolution(1920, 1080, 0.5f);
    }

    private void Start()
    {
        var homePanelFlag = true;
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (InputKit.Instance.esc)
                {
                    InputKit.Instance.esc.Reset();

                    if (homePanelFlag)
                    {
                        UIKit.OpenPanel<UIHomePanel>();
                    }
                    else
                    {
                        UIKit.ClosePanel<UIHomePanel>();
                        UIKit.ClosePanel<UILevelSelectPanel>();
                    }

                    homePanelFlag = !homePanelFlag;
                }
            })
            .AddTo(this);
        
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    this.SendCommand<SpawnPlayerCommand>();
                }
            })
            .AddTo(this);

    }

    public IArchitecture GetArchitecture()
    {
        return PrimaryColors.Interface;
    }
}
