using SavedVariables;
using SavedVariables.Settings;
using UnityEngine;

namespace MapMechanics
{
    public class SwapButtonPosition : MonoBehaviour
    {
        [SerializeField] private RectTransform cachedTransform;
    
        // If player is left handed inverts buttons !(and text and icons)!
        private void Start()
        {
            if (!SavedSettings.IsLeftHanded) return;
            
            var newPosition = cachedTransform.localPosition;
            newPosition.x *= -1;
            cachedTransform.localPosition = newPosition;
            
        }


    }
}
