using System.Collections;
using EnemyPlaneScripts;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    [Header("Configuration")]
    public float bulletSpeed = 10;
    public int bulletDamage = 1;
    
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    private EnemyHp _enemyHpRef;
    public bool applyRotation;
    public int rotationAngle = 30;



    //Forces bullet to fly to the right
    private void Start()

    {

        if (applyRotation)

        {
            rb.freezeRotation = false;   

            transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-rotationAngle, rotationAngle));


        }
        
        rb.AddForce(transform.right * bulletSpeed);
    }
    
    //Checks if bullet collides with valid enemy, if yes destroys itself and deals damage
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Enemy")) return;
        
        _enemyHpRef = col.GetComponentInParent<EnemyHp>();

        if (_enemyHpRef == null) return;

        _enemyHpRef.Hp -= bulletDamage;
        Destroy(gameObject);
    }
    
    

    
}
