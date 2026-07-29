using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    /// <summary>
    /// Inspector-friendly campaign reward hook.
    /// Add this to a campaign completion object and call GrantUnlock from its UnityEvent.
    /// </summary>
    public sealed class FactionUnlockReward : MonoBehaviour
    {
        [SerializeField] private NationTabs.Nations faction = NationTabs.Nations.Britain;

        public void GrantUnlock()
        {
            FactionUnlockProgress.Unlock(faction);
        }
    }
}
