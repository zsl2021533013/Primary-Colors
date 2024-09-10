using System;
using Cinemachine;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain.Script.Controller.Environment_System
{
    public class TargetGroupController : MonoBehaviour
    {
        [BoxGroup("Camera")] public CinemachineVirtualCamera vCamera;
        [BoxGroup("Camera")] public CinemachineConfiner2D confine;
        [BoxGroup("Camera")] public Transform follow;
        [BoxGroup("Camera")] public Transform area;

        [BoxGroup("Scene")] public Transform target;
        [BoxGroup("Scene"), Range(0f, 1f)] public float weight = 0.5f;
        
        [HideInInspector] public Transform playerFollow;

        public void OnAwake()
        {
            vCamera.Follow = follow;
            
            var edge = GameObject.Find("Edge").GetComponent<PolygonCollider2D>();
            confine.m_BoundingShape2D = edge;
        }

        public void OnUpdate(float elapse)
        {
            follow.position = target.position * weight + playerFollow.position * (1 - weight);
        }

        public bool WithinArea(Vector3 pos)
        {
            var p = area.position;
            var l = area.localScale / 2f;
            return (pos.x - p.x).Abs() <= l.x && (pos.y - p.y).Abs() <= l.y;
        }

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            if (area)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(area.position, area.localScale);
            }
        }
        
#endif
    }
}