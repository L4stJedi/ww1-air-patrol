using System.Collections;
using MapMechanics;
using UnityEngine;

namespace EnemyPlaneScripts
{
    public class FlyPassEnemy : MonoBehaviour
    {

        public float swoopForce = 5f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private IdleEnemyAnimationScript idleEnemyAnimationScript;
        [SerializeField] private GameObject bullet;
        private PlayerInstantiate _playerSpawner;
        private GameObject _spawnedBullet;
        private Rigidbody2D _playerRb;
        
        private void OnEnable()
        {
            StartCoroutine(FlyBy());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator FlyBy()
        {
            _playerSpawner = FindAnyObjectByType<PlayerInstantiate>();
            _playerRb = _playerSpawner.spawnedPlane.GetComponent<Rigidbody2D>();
            
            idleEnemyAnimationScript.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForceX(14, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
            
            rb.AddForceY(transform.position.y < 2f ? swoopForce : -swoopForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
            
            
            var dir = _playerRb.transform.position  - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var spawnedRotation = Quaternion.Euler(0, 0, angle);
            
            Instantiate(bullet, gameObject.transform.position, spawnedRotation);
            
            
            yield return new WaitForSeconds(0.4f);
            
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            idleEnemyAnimationScript.enabled = true;
        }


    }
}
