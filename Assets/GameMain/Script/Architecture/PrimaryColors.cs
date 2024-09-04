using System;
using QFramework;
using Script.Model;
using UnityEngine;

namespace Script.Architecture
{
    public class PrimaryColors : Architecture<PrimaryColors>
    {
        protected override void Init()
        {
            RegisterModel(new PlayerModel());
            RegisterModel(new TileModel());
        }
    }

    
}
