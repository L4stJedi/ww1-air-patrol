using SavedVariables.Planes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class EquipButton : MonoBehaviour
    {

        [SerializeField] private UpdateThumbnail updateThumbnail;
        [SerializeField] private TextMeshProUGUI equipText;
        [SerializeField] private Image background;
        [SerializeField] private Outline outline;
        private ListOfPlanes.Planes _planeType;

        private void OnEnable()
        {
            _planeType = updateThumbnail.planeType;

            if (_planeType != CurrentPlane.EquippedPlane) return;
            UpdateEquipButtonUI();
        }

        public void OnTriggerEquip()
        {
            CurrentPlane.EquippedPlane = _planeType;
            UpdateEquipButtonUI();
        }


        public void UpdateEquipButtonUI()
        {
            if (_planeType == CurrentPlane.EquippedPlane)
            {
                background.color = new Color(0.1f, 0.07f, 0.05f);
                equipText.color = new Color(1f, 0.83f, 0.53f);
                outline.enabled = true;
            }
            else
            {
                background.color = new Color(0.16f, 0.13f, 0.06f);
                equipText.color = Color.white;
                outline.enabled = false;
            }
        }
    }
}
