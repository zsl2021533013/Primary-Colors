using System;
using System.Collections.Generic;
using QFramework;
using Script.Architecture;
using UnityEngine;

namespace Script.Model
{
    public class ParticleModel : AbstractModel
    {
        private ResLoader mResLoader;
        
        public GameObject ParticleRoot { get; private set; }
        public Dictionary<ParticleType, SimpleObjectPool<GameObject>> PoolDic { get; private set; }

        protected override void OnInit()
        {
            mResLoader = ResLoader.Allocate();

            ParticleRoot = new GameObject("Particle Root");
            ParticleRoot.DontDestroyOnLoad();
            
            PoolDic = new Dictionary<ParticleType, SimpleObjectPool<GameObject>>();
            foreach (ParticleType type in Enum.GetValues(typeof(ParticleType)))
            {
                PoolDic.Add(type, new SimpleObjectPool<GameObject>(
                    () =>
                    {
                        var gameObject = mResLoader.LoadSync<GameObject>(type.ToString().InsertSpace()).Instantiate();
                        gameObject.Parent(ParticleRoot.transform);
                        return gameObject;
                    },
                    o =>
                    {
                        o.SetActive(false);
                    }));
            }
        }
    }
}