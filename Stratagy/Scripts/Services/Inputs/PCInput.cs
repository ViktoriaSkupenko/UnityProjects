using System;
using System.Collections;
using Interface;
using Plugins.DIContainer;
using Plugins.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services.Inputs
{
    public class PCInput : IInput
    {
        [DI] private ICoroutineRunner _coroutineRunner;
        
        private Camera _main;
        private Camera MainCamera => _main!=null?_main:_main=Camera.main;

        public event Action<Vector3> MoveCameraAtDiraction;

        [DI]
        private void Init()
        {
            _coroutineRunner.StartCoroutine(LateUpdate());
        }

        private IEnumerator LateUpdate()
        {
            while (true)
            {
                if(CheckMoveCamera(out var dir))
                    MoveCameraAtDiraction?.Invoke(dir);
                yield return new WaitForEndOfFrame();
            }
        }

        private bool CheckMoveCamera(out Vector3 dir)
        {
            dir = Vector3.zero;
            bool result = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
                          Input.GetKey(KeyCode.S);
            if (!result)
                return result;
            if (Input.GetKey(KeyCode.A))
                dir.x = -1;
            else if (Input.GetKey(KeyCode.D))
                dir.x = 1;
            else
                dir.x = 0;
            
            if (Input.GetKey(KeyCode.W))
                dir.z = 1;
            else if (Input.GetKey(KeyCode.S))
                dir.z = -1;
            else
                dir.z = 0;
            
            return result;
        }

        public bool TryRaycstFromPoisitonInput(float lenght, LayerMask laeyrMask, out RaycastHit raycastHit)
        {
            raycastHit = new RaycastHit();
            if (!UnityEngine.Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject())
                return false;
            Ray ray = MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            return Physics.Raycast(ray, out raycastHit, lenght, laeyrMask.value);
        }
    }
}