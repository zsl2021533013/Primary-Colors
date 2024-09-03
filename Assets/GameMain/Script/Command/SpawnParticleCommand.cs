using GameMain.Script.Consts;
using GameMain.Script.Controller;
using GameMain.Scripts.Utility;
using QFramework;
using UnityEngine;

namespace Script.Command
{
    public class SpawnParticleCommand : AbstractCommand
    {
        public ParticleType type;
        public Vector2 pos;

        protected override void OnExecute()
        {
            LevelManager.Instance.InstantiateController(
                Resources.Load<GameObject>(PathManager.GetParticleAsset(type.ToString())),
                pos,
                Quaternion.identity);
        }
    }
}