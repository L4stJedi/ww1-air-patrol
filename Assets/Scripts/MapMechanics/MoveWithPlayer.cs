using PlayerPlaneScripts;
using UnityEngine;

namespace MapMechanics
{
    public class MoveWithPlayer : MonoBehaviour
    {

        private ControlScript _playerSpeedRef;
        [SerializeField] private Rigidbody2D rb;
        private float _cameraSpeed;
        [SerializeField, Range(0, 1)] private float speedRatio = 1f;

        //finds player with the control script
        private void Awake()
        {
            _playerSpeedRef = FindAnyObjectByType<ControlScript>();
        }

        private void Start()
        {
            _cameraSpeed = _playerSpeedRef.moveSpeed;
        }

        private void FixedUpdate()
        {
            rb.linearVelocityX = _cameraSpeed * speedRatio;
        }
    }
}
