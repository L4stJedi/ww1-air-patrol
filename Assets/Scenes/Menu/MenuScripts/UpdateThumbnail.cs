using System;
using SavedVariables.Planes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class UpdateThumbnail : MonoBehaviour
    {
        [SerializeField] private GameObject unlockButton;
        [SerializeField] private GameObject levelButton;

        [SerializeField] private TextMeshProUGUI airplaneName;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI bio;
        [SerializeField] private Image planeImage;
        [SerializeField] private TextMeshProUGUI abilityText;
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI bestScore;
        [SerializeField] private TextMeshProUGUI costText;
        [NonSerialized] public ListOfPlanes.Planes planeType;


        public void SetThumbnail(string planeName, string descriptionText, string bioText, string abilityName, Image image, ListOfPlanes.Planes newPlaneType)
        {
            airplaneName.text = planeName;
            description.text = descriptionText;
            bio.text = bioText;
        

            abilityText.text = abilityName;
            planeImage.sprite = image.sprite;

            planeType = newPlaneType;
            var planeProperties = PlaneSaveSystem.LoadPlaneProperties(planeType);
            if (planeProperties.isUnlocked)
            {
                unlockButton.SetActive(false);
                levelButton.SetActive(true);
            }
            else
            {
                levelButton.SetActive(false);
                unlockButton.SetActive(true);
            }
        }
    }
}
