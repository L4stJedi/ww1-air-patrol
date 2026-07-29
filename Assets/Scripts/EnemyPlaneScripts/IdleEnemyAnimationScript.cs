using UnityEngine;

namespace EnemyPlaneScripts
{
    public class IdleEnemyAnimationScript : MonoBehaviour
    {
  

        [Header("Configuration")]
        public float animStrength = 2f;
        public float animSpeed = 0.5f;
        [SerializeField] private Rigidbody2D rb;
        private bool _firstFrame = true;
        private float _startY;


        private void OnEnable()
        {
            _startY = transform.position.y;
        }
        
        private void FixedUpdate()
        {
            if (_firstFrame)
            {
                _startY = transform.position.y;
                _firstFrame = false;
            }
            var newY = _startY + Mathf.Sin(Time.time * animSpeed) * animStrength;
            rb.MovePosition(new Vector3(rb.position.x, newY));
        }
    }
}