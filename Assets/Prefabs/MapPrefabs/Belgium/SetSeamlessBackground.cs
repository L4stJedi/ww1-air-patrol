using UnityEngine;

namespace Prefabs.MapPrefabs.Belgium
{
    public  class SetSeamlessBackground : MonoBehaviour
    {
        [Header("Seamless Background Objects")]
        [SerializeField] private GameObject seamlessPlaceholder0; 
        [SerializeField] private GameObject seamlessPlaceholder1; 
        [SerializeField] private GameObject seamlessPlaceholder2;
        
        [Header("Objects")]
        public float seamlessPlaceHolder0YAxis;
        public float seamlessPlaceHolder1YAxis;
        public float seamlessPlaceHolder2YAxis;

        [Header("Sprites")]
        public Sprite sprite0;
        public Sprite sprite1;
        public Sprite sprite2;

        private void Start()
        {
            MoveBackgroundPartsY();
        }

        public void MoveBackgroundPartsY()
        {
            var pos0 = seamlessPlaceholder0.transform.localPosition;
            var pos1 = seamlessPlaceholder1.transform.localPosition;
            var pos2 = seamlessPlaceholder2.transform.localPosition;
            
            pos0.y = seamlessPlaceHolder0YAxis;
            pos1.y = seamlessPlaceHolder1YAxis;
            pos2.y = seamlessPlaceHolder2YAxis;
             
            
            seamlessPlaceholder0.transform.localPosition =  pos0;
            seamlessPlaceholder1.transform.localPosition =  pos1;
            seamlessPlaceholder2.transform.localPosition =  pos2;
        }
    }
}
