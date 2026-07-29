using SavedVariables;
using SavedVariables.Planes;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class UnlockPlaneButton : MonoBehaviour
    {
        [SerializeField] private UpdateThumbnail updateThumbnail;
        [SerializeField] private GameObject enhanceButton;

        public void TriggerBuyPlane()
        {
            var planeData = PlaneSaveSystem.LoadPlaneProperties(updateThumbnail.planeType);
            
            if (planeData.isUnlocked || !BankedXp.SpendBankedXp(10)) return;
            
            planeData.isUnlocked = true;
            PlaneSaveSystem.SavePilotData(updateThumbnail.planeType, planeData);

            enhanceButton.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
