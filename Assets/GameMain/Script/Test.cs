using System;
using System.Collections.Generic;
using GameMain.Scripts.Utility;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameMain.Script
{
    [Serializable]
    class A : ScriptableObject
    {
        
    }
    
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            ES3.Save("A", ScriptableObject.CreateInstance<A>());
        }
    }
}
