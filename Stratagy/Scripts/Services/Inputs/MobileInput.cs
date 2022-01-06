using System;
using System.Collections;
using Interface;
using Plugins.DIContainer;
using Plugins.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services.Inputs
{
    public class MobileInput : IInput
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
            if (Input.touchCount == 0)
                return false;
            if (EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId))
                return false;
            var delta = Input.GetTouch(0).deltaPosition;
            dir = new Vector3(delta.x, 0 , delta.y);
            dir = dir.normalized;
            return true;
        }

        public bool TryRaycstFromPoisitonInput(float lenght, LayerMask laeyrMask, out RaycastHit raycastHit)
        {
            raycastHit = new RaycastHit();
            if (UnityEngine.Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId))
                    return false;
                if (Input.GetTouch(0).phase != TouchPhase.Began)
                    return false;
                Ray ray = MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                return Physics.Raycast(ray, out raycastHit, lenght, laeyrMask.value);
            }
            return false;
        }
    }
}