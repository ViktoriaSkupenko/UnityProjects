using System;
using System.Drawing;
using Extension;
using Factorys;
using Gameplay.Builds.Beh;
using Gameplay.Builds.Data;
using Gameplay.GameSceneScript;
using Gameplay.Map;
using Gameplay.Map.ChunkMaster.Scripts.ChunkMaster.ChunkMaster.src.Unity.Framework;
using Gameplay.Units;
using Plugins.DIContainer;
using Plugins.HabObject;
using Plugins.HabObject.GeneralProperty;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Builds
{
    [RequireComponent(typeof(FixedRotateble))]
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private GameObject Cube;
        
        private HabObject _hab;
        private FixedRotateble _fixedRotateble;

        private FixedRotateble FixedRotateble => _fixedRotateble!=null?_fixedRotateble : _fixedRotateble = GetComponent<FixedRotateble>();
        
        [DI] private WorldShell _worldShell;
        [DI] private FactoryBuild _factoryBuild;
        [DI] private FactoryUnit _factoryUnit;
        [DI] private GridBuild _grid;
        private float _lenghtBeforeCamera = 12;


        private void OnDisable()
        {
            _grid.HideAll();
        }

        public void Init(HabObject hab)
        {
            _hab = hab;
            FixedRotateble.SetZero();
            var size = hab.MainDates.GetOrNull<SizeOnMap>();
            SetPositionCube(size);
            var scale = size.Size.ToVector3XZ();
            scale.y = Cube.transform.localScale.y;
            Cube.transform.localScale = scale;
            GridView(_hab);
        }

        private void GridView(HabObject hab)
        {
            if (hab is Build)
                _grid.VieaArenaAsync(hab as Build);
            else
                _grid.VieaArenaAsync(hab as Unit);
        }

        private void SetPositionCube(SizeOnMap size)
        {
            if (size == null) return;
            Cube.transform.position = transform.position + size.Offset.ToVector3XZ()-SizeOnMap.SingModifacateXZ(transform.eulerAngles.y)/2;
        }

        public void TurnTo(bool toActive)
        {
            if (!toActive)
                _hab = null;
            gameObject.SetActive(toActive);
        }

        public void StandBeforeCamera()
        {
            Camera camera = Camera.main;
            var forwardCamera = camera.transform.forward;
            forwardCamera.y = 0;
            forwardCamera *= _lenghtBeforeCamera;
            var resultPosCamera = camera.transform.position + forwardCamera;
            MoveAtPosition(resultPosCamera);
        }

        public void Rotate(bool toright)
        {
            if(toright) FixedRotateble.RotateRight();
            else FixedRotateble.RotateLeft();
            MoveAtPosition(transform.position);
        }

        public void MoveAtPosition(Vector3 getPositionForGhost)
        {
            Vector3 newPos;
            getPositionForGhost.y = 0;
            getPositionForGhost.x = Mathf.Floor(getPositionForGhost.x);
            getPositionForGhost.z = Mathf.Floor(getPositionForGhost.z);
            if (_worldShell.World.IsOccupied(getPositionForGhost))
                newPos = GetPositionFromBlockWrold(getPositionForGhost);
            else
                newPos = GetPositionFromBlockWrold(Vector3.zero);
            ChangePosition(newPos);
        }

        public void MoveToDiraction(Vector2Int diraction)
        {
            var modXZ = SizeOnMap.GetModifacateXZ(transform.eulerAngles.y);
            var posBlock = transform.position + diraction.ToVector3XZ();
            posBlock.y = 0;
            ChangePosition(GetPositionFromBlockWrold(posBlock));
        }

        private Vector3 GetPositionFromBlockWrold(Vector3 getPositionForGhost)
        {
            getPositionForGhost.y = 0;
            var block = _worldShell.World.GetBlock(getPositionForGhost);
            Brick upestBrick = block.GetContent().GetUpestBrick();
            return upestBrick.transform.position + (Vector3.up * 0.5f);
        }

        private void ChangePosition(Vector3 newPos)
        {
            transform.position = newPos;
        }

        public void SetBuild()
        {
            if(!_hab)
                return;
            if (_hab is Build)
                _factoryBuild.Spawn((Build)_hab, transform.position, transform.eulerAngles);
            else
                _factoryUnit.Spawn((Unit)_hab, transform.position, transform.eulerAngles);
            GridView(_hab);
        }
    }
}