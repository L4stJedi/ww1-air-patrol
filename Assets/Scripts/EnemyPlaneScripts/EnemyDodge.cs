using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemyPlaneScripts
{
    public class EnemyDodge : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;

        [SerializeField]
        private IdleEnemyAnimationScript idleEnemyAnimationScript;

        private WaitForSeconds _waitForSeconds;

        private void OnEnable()
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            idleEnemyAnimationScript.enabled = true;
            
            rb.angularVelocity = 0f;
            _waitForSeconds = new WaitForSeconds(3f);
            StartCoroutine(Dodge());
        }



        private IEnumerator Dodge()
        {
            yield return _waitForSeconds;
            idleEnemyAnimationScript.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForceY((Random.value > 0.5f ? -1f : 1f) * 7.5f, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.5f);

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            idleEnemyAnimationScript.enabled = true;

        }
    }
}
