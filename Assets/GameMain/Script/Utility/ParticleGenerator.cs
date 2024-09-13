using System.Collections.Generic;
using GameMain.Script.Controller.Environment_System;
using GameMain.Scripts.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameMain.Script.Utility
{
    public class ParticleGenerator
    {
        public Tilemap tilemap; // 引用你当前的 Tilemap
        private TileController controller;
        
        // 存储最外层 tiles 及其对应的发射方向
        private Dictionary<Vector3Int, Vector3Int> outerTiles = new Dictionary<Vector3Int, Vector3Int>();

        public ParticleGenerator(Tilemap tilemap, TileController controller)
        {
            this.tilemap = tilemap;
            this.controller = controller;
        }

        #region Green Grass
        
        public void GenerateGreenGrass(List<SpriteRenderer> greenGrassRenderer)
        {
            var container = new GameObject("Green Grass Container");
            FindOuterTiles();
            CastRaysAndGenerateObjects(container.transform, greenGrassRenderer);
        }

        // 找出最外层的 tiles 并记录发射方向
        void FindOuterTiles()
        {
            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(tilePos))
                    {
                        Vector3Int direction = GetOuterTileDirection(tilePos);
                        if (direction != Vector3Int.zero)
                        {
                            outerTiles.Add(tilePos, direction);
                        }
                    }
                }
            }
        }

        // 判断一个 tile 是否为外层 tile，并返回发射射线的方向
        Vector3Int GetOuterTileDirection(Vector3Int pos)
        {
            Vector3Int[] directions = new Vector3Int[]
            {
                Vector3Int.up,
                Vector3Int.down,
                Vector3Int.left,
                Vector3Int.right
            };

            foreach (var dir in directions)
            {
                if (!tilemap.HasTile(pos + dir))
                {
                    return dir; // 如果某个方向是空的，返回这个方向
                }
            }

            return Vector3Int.zero; // 如果四周都有 tile，则返回零向量
        }

        // 发射射线并生成 GameObject
        void CastRaysAndGenerateObjects(Transform container, List<SpriteRenderer> greenGrassRenderer)
        {
            foreach (var entry in outerTiles)
            {
                Vector3Int tilePos = entry.Key;
                Vector3Int direction = entry.Value;

                Vector3 worldPos = tilemap.CellToWorld(tilePos);
                var randomOffset = Random.Range(-0.5f, 0.5f);
                if (direction == Vector3Int.right)
                {
                    worldPos.x += tilemap.cellSize.x;
                    worldPos.y += tilemap.cellSize.y / 2f + randomOffset;
                }
                else if (direction == Vector3Int.left)
                {
                    worldPos.y += tilemap.cellSize.y / 2f + randomOffset;
                }
                else if (direction == Vector3Int.up)
                {
                    worldPos.x += tilemap.cellSize.x / 2 + randomOffset;
                    worldPos.y += tilemap.cellSize.y;
                }
                else if (direction == Vector3Int.down)
                {
                    worldPos.x += tilemap.cellSize.x / 2 + randomOffset;
                }
                
                Vector2 rayOrigin = new Vector2(worldPos.x, worldPos.y);
                Vector2 rayDirection = new Vector2(direction.x, direction.y);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 1f, LayerMask.GetMask("Ground"));
                RaycastHit2D innerHit = Physics2D.Raycast(rayOrigin, -rayDirection, 1f, LayerMask.GetMask("Ground"));

                if (!hit && innerHit)
                {
                    var randomIndex = Random.Range(1, 5);
                    var prefab =
                        Resources.Load<GameObject>(PathManager.GetEnvironmentAsset($"Grass_{randomIndex}"));
                    
                    // 计算 z 轴旋转角度
                    float rotationZ = 0f;

                    if (direction == Vector3Int.right)
                    {
                        rotationZ = -90f;  // 顺时针旋转 90 度
                    }
                    else if (direction == Vector3Int.left)
                    {
                        rotationZ = 90f; // 逆时针旋转 90 度
                    }
                    else if (direction == Vector3Int.up)
                    {
                        rotationZ = 0f;   // 无旋转，上方默认
                    }
                    else if (direction == Vector3Int.down)
                    {
                        rotationZ = 180f; // 旋转 180 度
                    }

                    // 使用旋转角度实例化 prefab
                    var go = prefab.Instantiate(innerHit.point, Quaternion.Euler(0f, 0f, rotationZ));
                    greenGrassRenderer.Add(go.GetComponent<SpriteRenderer>());
                    go.Parent(container);
                }
            }
        }

        #endregion

        #region Purple Particle

        private const float RaycastDistance = 100f; // 射线检测的距离
        
        public void GeneratePurpleParticle(List<MeshRenderer> purpleParticleRenderer)
        {
            if (tilemap == null || controller == null)
            {
                Debug.Log("Tilemap or TileController Don't Exist!");
                return;
            }
            
            // 1. 获取所有的连通块
            List<List<Vector3Int>> connectedBlocks = GetConnectedBlocks();

            var container = new GameObject("Purple Particle Container");
            // 2. 对每个连通块执行射线检测，并生成对应的 Mesh
            foreach (var block in connectedBlocks)
            {
                List<Vector3> collisionPoints = PerformRaycastOnBlock(block);
                GenerateMeshFromBlock(container.transform, block, collisionPoints, purpleParticleRenderer);
            }
        }

        // 获取所有连通块
        private List<List<Vector3Int>> GetConnectedBlocks()
        {
            List<List<Vector3Int>> connectedBlocks = new List<List<Vector3Int>>();
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            var allPos = tilemap.cellBounds.allPositionsWithin;
            // 遍历 Tilemap 中的每个 tile
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos) && !visited.Contains(pos))
                {
                    // 执行 BFS 或 DFS 来获取该连通块
                    List<Vector3Int> block = new List<Vector3Int>();
                    GetConnectedBlock(pos, block, visited);
                    connectedBlocks.Add(block);
                }
            }
            
            return connectedBlocks;
        }

        // 获取一个连通块，使用 DFS 或 BFS
        private void GetConnectedBlock(Vector3Int startPos, List<Vector3Int> block, HashSet<Vector3Int> visited)
        {
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(startPos);
            visited.Add(startPos);

            // 用于存储相同 x 和 z 坐标下的最高 y 的 tile
            Dictionary<Vector2Int, Vector3Int> highestTiles = new Dictionary<Vector2Int, Vector3Int>();

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();

                // 获取当前 tile 的 x 和 z 坐标
                Vector2Int xzPos = new Vector2Int(current.x, current.z);

                // 检查当前 x 和 z 下的最高 y 坐标
                if (highestTiles.ContainsKey(xzPos))
                {
                    // 如果当前 tile 的 y 坐标比已记录的 y 坐标大，则更新
                    if (current.y > highestTiles[xzPos].y)
                    {
                        highestTiles[xzPos] = current;
                    }
                }
                else
                {
                    // 如果是新的 x 和 z 坐标，直接添加
                    highestTiles[xzPos] = current;
                }

                // 检查当前 tile 的四个方向
                Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
                foreach (var dir in directions)
                {
                    Vector3Int neighbor = current + dir;
                    if (tilemap.HasTile(neighbor) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            // 将具有最高 y 坐标的 tile 添加到 block 中
            foreach (var tile in highestTiles.Values)
            {
                block.Add(tile);
            }

            block.Sort((v1, v2) => v1.x.CompareTo(v2.x));
        }

        // 对每个连通块的上表面发射射线，并记录碰撞点
        private List<Vector3> PerformRaycastOnBlock(List<Vector3Int> block)
        {
            List<Vector3> collisionPoints = new List<Vector3>();

            for (var index = 0; index < block.Count; index++)
            {
                var tilePos = block[index];
                
                Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.cellSize; // 获取上表面的中心点
                if (index == 0)
                {
                    worldPos.x -= tilemap.cellSize.x;
                }
                worldPos.y += 0.1f;
                
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.up, RaycastDistance);

                if (!hit)
                {
                    collisionPoints.Add(worldPos + RaycastDistance * Vector3.up);
                }
                else
                {
                    collisionPoints.Add(hit.point); // 记录碰撞点
                }
            }

            return collisionPoints;
        }

        // 根据连通块和碰撞点生成 Mesh
        private void GenerateMeshFromBlock(Transform container, List<Vector3Int> block, List<Vector3> collisionPoints, List<MeshRenderer> purpleParticleRenderer)
        {
            GameObject blockObject = new GameObject("BlockMesh");
            MeshFilter meshFilter = blockObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = blockObject.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            blockObject.Parent(container);

            // 为连通块的顶面和碰撞点添加顶点
            for (int i = 0; i < block.Count; i++)
            {
                Vector3 worldPos = tilemap.CellToWorld(block[i]) + tilemap.cellSize; // 获取上表面的中心点
                if (i == 0)
                {
                    worldPos.x -= tilemap.cellSize.x;
                }
                vertices.Add(worldPos); // 顶面顶点
                vertices.Add(collisionPoints[i]); // 碰撞点顶点

                // 添加 UV 坐标，简单地将 x 和 z 作为 UV
                uvs.Add(new Vector2(worldPos.x, worldPos.y));
                uvs.Add(new Vector2(collisionPoints[i].x, collisionPoints[i].y));
            }

            // 为顶点创建三角形
            for (int i = 0; i < block.Count - 1; i++)
            {
                // 创建两个三角形以连接顶面和碰撞点
                triangles.Add(i * 2);
                triangles.Add(i * 2 + 1);
                triangles.Add(i * 2 + 2);

                triangles.Add(i * 2 + 1);
                triangles.Add(i * 2 + 3);
                triangles.Add(i * 2 + 2);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray(); // 设置 UV 坐标
            mesh.RecalculateNormals(); // 重新计算法线

            meshFilter.mesh = mesh;
            meshRenderer.material = Resources.Load<Material>(PathManager.GetMaterialAsset("PurpleParticleMat")).Instantiate();
            purpleParticleRenderer.Add(meshRenderer);
        }

        #endregion
    }
}