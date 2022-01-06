using System;
using UnityEngine;

namespace Interface
{
    public interface IInput
    {
        event Action<Vector3> MoveCameraAtDiraction; 
        bool TryRaycstFromPoisitonInput(float lenght, LayerMask laeyrMask, out RaycastHit raycastHit);
    }
}