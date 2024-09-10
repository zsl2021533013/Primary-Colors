using System;
using System.Collections.Generic;
using GameMain.Scripts.Utility;
using QFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameMain.Script
{
    public class Test : MonoBehaviour
    {
        public Collider2D collider;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log(CheckPlayerCollision());
            }
        }
        
        private bool CheckPlayerCollision()
        {
            // 如果 collider.enabled 为 false，则暂时的开启，检测后关闭
            var flag = collider.enabled;
            if (!flag)
            {
                collider.enabled = true;
                Physics.SyncTransforms();
            }
            
            // 创建一个 ContactFilter2D，用于过滤出 Player 层的碰撞
            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Player"));
            contactFilter.useLayerMask = true; // 确保使用 LayerMask 过滤
            contactFilter.useTriggers = true; // 根据是否想检测触发器调整

            // 获取与 TilemapCollider2D 碰撞的对象
            var results = new Collider2D[10]; // 一个数组来保存检测到的碰撞体，大小可以根据需求调整
            var collisionCount = collider.OverlapCollider(contactFilter, results);

            collider.enabled = flag;
            
            // 如果有碰撞，返回 true
            return collisionCount > 0;
        }
    }
}
