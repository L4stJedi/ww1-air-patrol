using MapMechanics;
using System.Collections;
using UnityEngine;

namespace EnemyPlaneScripts
{
    public class EnemyShooting : MonoBehaviour
    {
        [SerializeField] private GameObject bullet;
        public bool shootToPlayerPos;
        private PlayerInstantiate _playerSpawner;
        private bool _isSpawnedPlaneNull;
        private Rigidbody2D _playerRb;

        

        private void OnEnable()
        {

            StartCoroutine(Shooting());
        }

        //Shoots 8 shots to the left or one at the player
        private IEnumerator Shooting()
        {
            if (PlayerInstantiate.PlayerSpawnerInstance == null) yield break;
            _playerRb = PlayerInstantiate.PlayerSpawnerInstance.spawnedPlane.GetComponent<Rigidbody2D>();
            if (shootToPlayerPos)
            {
                var dir = _playerRb.transform.position  - transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                var spawnedRotation = Quaternion.Euler(0, 0, angle);
                
                Instantiate(bullet, gameObject.transform.position, spawnedRotation);
            }
            else
            {
                for (var i = 0; i < 8; i++)
                {
                    Instantiate(bullet, gameObject.transform.position, Quaternion.Euler(0, 0, 180));
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
