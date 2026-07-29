using SavedVariables.Planes;
using UnityEngine;
using UnityEngine.UI;


namespace Scenes.Menu.MenuScripts
{
    public class PlaneShowcaseProperties : MonoBehaviour
    {
        [Header("Base info")]
        [SerializeField] private string airplaneName;
        [SerializeField] private string description;
        [SerializeField] private string bio;
        [SerializeField] private Image planeImage;

        [Header("properties")]
        [SerializeField] private string abilityText;
        public ListOfPlanes.Planes planeType;
        public EnhanceUIGetCurrentPlaneShowcase uiGetCurrentPlaneShowcase;
        
        private int _bestScore;
        private UpdateThumbnail _updateThumbnail;

        [SerializeField] private bool shouldCallOnStart;

        [Header("Other references")] [SerializeField]
        private EquipButton equipButton;
        


        private void OnEnable()
        {
           
            _updateThumbnail = FindAnyObjectByType<UpdateThumbnail>();
            if (!shouldCallOnStart) return;
            CallSwapThumbnail();
            CallSwapEnhanceTarget();
        }


        public void CallSwapThumbnail()
        {
            _updateThumbnail.SetThumbnail(airplaneName, description, bio, abilityText, planeImage, planeType);
            equipButton.UpdateEquipButtonUI();
        }

        public void CallSwapEnhanceTarget()
        {
            uiGetCurrentPlaneShowcase.GetCurrentPlaneShowcaseEnhanceData(planeType);
        }
    }
}
