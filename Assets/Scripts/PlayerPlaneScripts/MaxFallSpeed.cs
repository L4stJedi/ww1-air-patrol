using System;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class MaxFallSpeed : MonoBehaviour
    {

        [SerializeField] private Rigidbody2D rb;
        private float _maxVelocitySpeed = 7.5f;
        private InitializePlaneProperties _initializePlaneProperties;
        
        private void Awake()
        {
            _initializePlaneProperties = GetComponent<InitializePlaneProperties>();
            _maxVelocitySpeed = _initializePlaneProperties.currentFallSpeed;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (rb.linearVelocityY < -_maxVelocitySpeed)
            {
                rb.linearVelocityY = -_maxVelocitySpeed;
            }

            if (_maxVelocitySpeed < 1)
            {
                _maxVelocitySpeed = _initializePlaneProperties.currentFallSpeed;
            }
        }
    }
}
