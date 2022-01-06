using System;
using Infrastructure.SceneStates;
using Interface;
using Plugins.DIContainer;
using UnityEngine;

namespace Gameplay
{
    public class CustomCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new Vector3(1,0,1);
        [Min(0)][SerializeField] private float _addHeightForCamera = 4;
        [SerializeField] private float _speed;

        [DI] private IInput _input;
        [DI] private GameSceneData _gameSceneData;

        private Vector3 _minPoint;
        private Vector3 _maxPoint;

        private void OnEnable() => _input.MoveCameraAtDiraction += OnMoveEvent;

        private void OnDisable() => _input.MoveCameraAtDiraction -= OnMoveEvent;

        private void Awake()
        {
            _minPoint = Vector3.zero + _offset;
            var chunkSet = _gameSceneData.DataMap.ChunkSettings;
            var maxX = chunkSet.ChunkSize * chunkSet.SectorSize * _gameSceneData.DataMap.MapSettings.Size.x;
            var maxz = chunkSet.ChunkSize * chunkSet.SectorSize * _gameSceneData.DataMap.MapSettings.Size.x;
            _maxPoint = new Vector3(maxX, 0, maxz) - _offset;
            Vector3 startPostion = new Vector3(maxX/2, _gameSceneData.DataMap.MapSettings.HeightModule.Max+_addHeightForCamera, maxz/2);
            transform.position = startPostion;
        }

        private void OnMoveEvent(Vector3 dir)
        {
            dir *= _speed;
            dir *= Time.deltaTime;
            dir = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * dir;
            dir = transform.InverseTransformDirection(dir);

            transform.Translate(dir, Space.Self);

            transform.position = ClampPosition();
        }

        private Vector3 ClampPosition()
        {
            return new Vector3(
                Mathf.Clamp(transform.position.x, _minPoint.x, _maxPoint.x), 
                transform.position.y, 
                Mathf.Clamp(transform.position.z, _minPoint.z, _maxPoint.z));
        }

        private void OnValidate()
        {
            _offset.y = 0;
            if (_offset.x < 0)
                _offset.x = 0;
            if (_offset.z < 0)
                _offset.z = 0;
        }
    }
}