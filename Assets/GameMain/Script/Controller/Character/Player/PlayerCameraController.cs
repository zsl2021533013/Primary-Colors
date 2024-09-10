using System.Collections.Generic;
using Cinemachine;
using GameMain.Script.Controller.Environment_System;
using QFramework;
using Script.Architecture;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain.Script.Controller.Character.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [BoxGroup("Follow")] public Transform follow;
        [BoxGroup("Follow")] public float followDamp;
        [BoxGroup("Follow")] public Vector2 followOffset;
        
        [BoxGroup("Camera")] public CinemachineVirtualCamera vCamera;
        [BoxGroup("Camera")] public CinemachineConfiner2D confine;
        
        [BoxGroup("Target Group")] public List<TargetGroupController> targetGroupList;
        
        public void OnAwake()
        {
            follow.Parent(null);
            vCamera.Parent(null);
            vCamera.Follow = follow;

            targetGroupList = new List<TargetGroupController>();
            targetGroupList.AddRange(FindObjectsOfType<TargetGroupController>());
            targetGroupList.ForEach(t => t.OnAwake());

            var edge = GameObject.Find("Edge").GetComponent<PolygonCollider2D>();
            confine.m_BoundingShape2D = edge;
        }

        public void OnUpdate(float elapse)
        {
            follow.position = Vector3.Lerp(follow.position, transform.position + (Vector3)followOffset, elapse * followDamp);

            vCamera.MoveToTopOfPrioritySubqueue();
            
            targetGroupList.ForEach(t =>
            {
                if (t.WithinArea(transform.position))
                {
                    t.playerFollow = follow;
                    t.vCamera.MoveToTopOfPrioritySubqueue();
                    t.OnUpdate(elapse);
                }
            });
        }
    }
}