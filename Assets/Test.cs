using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Script.Architecture;
using Script.Command;
using Script.Model;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour, IController
{

    private void Update()
    {
        var value = Physics2D.OverlapBox(
            transform.position, 
            transform.localScale, 
            0f, 
            LayerMask.GetMask("Spike"));
        if (value)
        {
            var type = this.GetModel<TileModel>().GetTileType(value.transform);
            if (type == TileType.Spike)
            {
                Debug.Log(1);
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return PrimaryColors.Interface;
    }
}
