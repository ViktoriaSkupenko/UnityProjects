﻿using System.Collections.Generic;
using Factorys;
using Gameplay.Builds;
using Gameplay.Map;
using Infrastructure.SceneStates;
using Pathfinding;
using Plugins.DIContainer;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(AstarPath))]
    public class PathfinderShell : MonoBehaviour
    {
        [Range(0,1f)][SerializeField] private float _nodeSize;
        [SerializeField] private GraphUpdateScene _graphUpdateScene;
        
        [DI] private GameSceneData _gameSceneData;
        private AstarPath _astarPath;

        private void Awake()
        {
            transform.position = Vector3.zero;
            var w = _gameSceneData.DataMap.ChunkSettings.ChunkSize * _gameSceneData.DataMap.ChunkSettings.SectorSize *
                    _gameSceneData.DataMap.MapSettings.Size.x;
            var h = _gameSceneData.DataMap.ChunkSettings.ChunkSize * _gameSceneData.DataMap.ChunkSettings.SectorSize *
                    _gameSceneData.DataMap.MapSettings.Size.y;
            _astarPath = GetComponent<AstarPath>();
             var graph = _astarPath.graphs[0] as GridGraph;
             graph.center = GetCenter(w, h);
             graph.nodeSize = _nodeSize;
             var mulNode = 1 / _nodeSize;
             graph.Width = (int)(w * mulNode);
             graph.depth = (int) (h * mulNode);
        }

        private void OnBuildCreated(Build obj) => Scan();

        public void Scan() => _astarPath.Scan();

        public void UpdateTag(WorldShell worldShell)
        {
            List<Vector3> points = new List<Vector3>();
            var xSize = _gameSceneData.DataMap.MapSettings.Size.x * _gameSceneData.DataMap.ChunkSettings.ChunkSize *
                        _gameSceneData.DataMap.ChunkSettings.SectorSize;
            var ySize = _gameSceneData.DataMap.MapSettings.Size.y * _gameSceneData.DataMap.ChunkSettings.ChunkSize *
                        _gameSceneData.DataMap.ChunkSettings.SectorSize;
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    var brick = worldShell.World.GetBlock(new Vector3(x, 0, y)).GetContent().GetUpestBrick();
                    if(brick.IsWater)
                        points.Add(brick.transform.position+Vector3.up*0.5f);
                }
            }

            _graphUpdateScene.points = points.ToArray();
        }

        private Vector3 GetCenter(int w, int h) => new Vector3((float)w/2-0.5f,0,(float)h/2-0.5f);
    }
}