using PlayerPlaneScripts;
using UnityEngine;

namespace EnemyPlaneScripts
{
    public class EnemyBulletLogic : MonoBehaviour
    {

        [Header("Configuration")]
        public float bulletSpeed = 10;
        public int bulletDamage = 1;
    
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;

        //Forces bullet to fly to the right
        private void Start()

        {
            rb.linearVelocity =transform.right * bulletSpeed;
        }
    
        //Checks if bullet collides with valid enemy, if yes destroys itself and deals damage
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;
        
            var playerHpRef = col.GetComponentInParent<PlayerHp>();

            if (playerHpRef == null) return;

            playerHpRef.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}