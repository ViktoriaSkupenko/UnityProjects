using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Timer : MonoBehaviour
    {
        public bool IsReach => _CurrentTime >= _richTime;
        
        [SerializeField] private float _richTime;
        [SerializeField]private float _CurrentTime;

        private void Update()
        {
            _CurrentTime += Time.deltaTime;
        }

        public void Zero() => _CurrentTime = 0;
    }
}