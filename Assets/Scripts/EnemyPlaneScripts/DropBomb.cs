using System.Collections;
using UnityEngine;

namespace EnemyPlaneScripts
{
    public class DropBomb : MonoBehaviour
    {
        public GameObject bomb;
        public float minTime = 0.5f, maxTime = 2f;
        public float offset;
        private WaitForSeconds _waitUntilDrop;

        private void OnEnable()
        {
            _waitUntilDrop = new WaitForSeconds(Random.Range(minTime, maxTime));
            StartCoroutine(Drop());
        }


        private IEnumerator Drop()
        {
            yield return _waitUntilDrop;
            var position = gameObject.transform.position;
            Instantiate(bomb, new Vector3(position.x, position.y - offset, 0), Quaternion.identity);
        }
    }
}
