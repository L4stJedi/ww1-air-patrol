using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace EnemyPlaneScripts
{
    public class EnemyPool : MonoBehaviour
    {

        [Header("Pool settings for single prefab pool")] 
        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 20;

       private Dictionary<GameObject, IObjectPool<GameObject>> _pools = new Dictionary<GameObject, IObjectPool<GameObject>>();

       
        public GameObject GetObjectFromPool(GameObject prefab, Vector2 spawnPosition)
        {
            if (!_pools.ContainsKey(prefab))
            {
                _pools[prefab] = new ObjectPool<GameObject>(
                    createFunc: () => CreateObjectInstance(prefab),
                    actionOnGet: OnGetObject,
                    actionOnRelease: OnReleaseObject,
                    actionOnDestroy: OnDestroyObject,
                    collectionCheck: false,
                    defaultCapacity: defaultPoolSize,
                    maxSize: maxPoolSize
                );
            }
            
            var handledObject = _pools[prefab].Get();

            handledObject.transform.position = spawnPosition;
            if (handledObject.TryGetComponent(out Rigidbody2D rb))
            {
                rb.position = spawnPosition;
            }
            handledObject.SetActive(true);

            return handledObject;
        }
        private GameObject CreateObjectInstance(GameObject prefab)
        {
            var newObject = Instantiate(prefab);

            var currentPool = _pools[prefab];

            if (newObject.TryGetComponent(out EnemyHp enemyHp))
            {
               enemyHp.SetPool(currentPool);
            }

            if (newObject.TryGetComponent(out SelfDestroy selfDestroy))
            {
                selfDestroy.SetPool(currentPool);
            }
            
            return newObject;
        }
        
        private void OnGetObject(GameObject enemy){}
        private void OnReleaseObject(GameObject enemy) => enemy.SetActive(false);
        private void OnDestroyObject(GameObject enemy) => Destroy(enemy);
        
    }
}
