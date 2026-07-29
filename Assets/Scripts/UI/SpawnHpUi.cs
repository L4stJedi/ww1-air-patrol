using System.Collections.Generic;
using PlayerPlaneScripts;
using UnityEngine;

namespace UI
{
    public class SpawnHpUi : MonoBehaviour
    {
        [SerializeField] private GameObject healthPointImageRef;
        [SerializeField] private GameObject shieldPointImageRef;
        private RectTransform _healthPointsContainerRef;
        private PlayerHp _hpRef;

        private List<GameObject> _spawnedHpImages = new List<GameObject>();
        private List<GameObject> _spawnedShieldImages = new List<GameObject>();
        private int _i;
        
        // Start sets references and creates HP ui elements (images)
        private void Start()
        {
            _healthPointsContainerRef = GetComponent<RectTransform>();
            _hpRef = FindAnyObjectByType<PlayerHp>();

            for (_i = 0; _i < _hpRef.maxHp; _i++)
            {
                InstantiateUi(_spawnedHpImages, healthPointImageRef, _i);
            }
            
            for (_i = 0; _i < _hpRef.shield; _i++)
            {
                InstantiateUi(_spawnedShieldImages, shieldPointImageRef, _i);
            }  
        }

        public void InstantiateUi(List<GameObject> list, GameObject imageRef, int i)
        {
            list.Add(Instantiate(imageRef, _healthPointsContainerRef));
            var imageRect = list[i].GetComponent<RectTransform>();
            
            if (imageRect == null) return;
            imageRect.SetSiblingIndex(i); 
            imageRect.localScale = Vector3.one;
            imageRect.localRotation = Quaternion.identity;
        }

        public void Heal()
        {
            // var imageRect //_spawnedHpImages.Add(Instantiate(healthPointImageRef, _healthPointsContainerRef));
            //  
            // 
            // if (imageRect == null) return;
            // imageRect.SetSiblingIndex(i); 
            // imageRect.localScale = Vector3.one;
            // imageRect.localRotation = Quaternion.identity;
        }
    }
}
