using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[AddComponentMenu("Utilities/Self Destroy After Delay")]
public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float timeBeforeDestroyed;
    [SerializeField] private bool isPartOfPool;

    private IObjectPool<GameObject> _poolRef;

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _poolRef = pool;
    }
    
    private void OnEnable()
    {
        StartCoroutine(SelfDestroyAfterDelay());
    }

    private IEnumerator SelfDestroyAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeDestroyed);
        
        if (isPartOfPool)                               
        {                                               
            _poolRef.Release(gameObject);               
        }                                               
        else                                            
        {                                               
            Destroy(gameObject, timeBeforeDestroyed);   
        }                                               
    }
}


