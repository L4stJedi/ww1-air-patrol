using System.Collections;
using UnityEngine;

namespace Prefabs.MapPrefabs.Belgium
{
    public class SwapSeamlessBackground : MonoBehaviour
    {
        private float _moveBy;

        private Sprite _sprite0;
        private Sprite _sprite1;
        private Sprite _sprite2;
        
        private SpriteRenderer _spriteRendererSelf;



        private void Start()
        {
            _spriteRendererSelf = GetComponent<SpriteRenderer>();
        }


        private void OnTriggerExit2D(Collider2D other)
        {

            if (!other.CompareTag("Player")) return;
            StartCoroutine(SeamlessMove());
        }

        
        
        private IEnumerator SeamlessMove()
        {
            yield return new WaitForSeconds(1f);
            var newTransform = transform;
            var newLocalPos = newTransform.localPosition;
            

            _moveBy = (_spriteRendererSelf.bounds.size.x * 1.5f);  
            

            
            newLocalPos.x += _moveBy - 0.2f;
            newLocalPos.x = Mathf.Round(newLocalPos.x * 1000) / 1000;
            
            transform.localPosition = newLocalPos;
        }
    }
}
