using QFramework;
using Script.Architecture;
using Script.Model;
using UnityEngine;

namespace Script.Command
{
    public class RecycleParticleCommand : AbstractCommand
    {
        public ParticleType type;
        public GameObject gameObject;

        protected override void OnExecute()
        {
            var dic = this.GetModel<ParticleModel>().PoolDic;

            if (dic.TryGetValue(type, out var pool))
            {
                pool.Recycle(gameObject);
            }
        }
    }
}