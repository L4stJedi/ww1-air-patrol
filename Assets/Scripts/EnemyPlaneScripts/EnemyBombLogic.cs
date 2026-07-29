using PlayerPlaneScripts;
using UnityEngine;

namespace EnemyPlaneScripts
{
    public class EnemyBombLogic : MonoBehaviour
    {
        [Header("Configuration")]
        public int bombDamage = 2;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;

        //Forces bullet to fly to the right
        private void Start()

        {
            rb.AddForceX(1.5f);
        }

        //Checks if bullet collides with valid enemy, if yes destroys itself and deals damage
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;
    
            var playerHpRef = col.GetComponentInParent<PlayerHp>();

            if (playerHpRef == null) return;

            playerHpRef.TakeDamage(bombDamage);
            Destroy(gameObject);
        }
    }
}
