using System;
using System.Collections;
using System.Collections.Generic;
using GameMain.Script.Consts;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Model;
using Script.View_Controller;
using Unity.VisualScripting;
using UnityEngine;

public class Test : ControllerBase
{
    private void Start()
    {
        Debug.Log(ParticleType.Bounce.ToString());
    }
}
