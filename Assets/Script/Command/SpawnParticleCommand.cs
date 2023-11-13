using QFramework;
using Script.Architecture;
using Script.Model;
using UnityEngine;

namespace Script.Command
{
    public class SpawnParticleCommand : AbstractCommand
    {
        public ParticleType type;
        public Vector2 pos;

        protected override void OnExecute()
        {
            var dic = this.GetModel<ParticleModel>().PoolDic;

            if (dic.TryGetValue(type, out var pool))
            {
                var particle = pool.Allocate();
                particle.transform.position = pos;
                particle.SetActive(true);
            }
        }
    }
}