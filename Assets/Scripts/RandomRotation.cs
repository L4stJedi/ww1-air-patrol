using UnityEngine;

[AddComponentMenu("Utilities/SpawnWithRandomRotation")]
public class RandomRotation : MonoBehaviour
{
    [SerializeField, Range(0f, 359f)] private float minRotation;
    [SerializeField, Range(0f, 360f)] private float maxRotation = 360;
    
    private void Start()
    {
        if (minRotation >= maxRotation) return;

        
        var rotationRange= Random.Range(minRotation, maxRotation);
        transform.rotation = Quaternion.Euler(0, 0, rotationRange);
    }
}
