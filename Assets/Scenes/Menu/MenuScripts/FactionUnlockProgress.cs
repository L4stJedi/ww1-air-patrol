using SavedVariables;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    /// <summary>
    /// Persistent faction availability used by the main-menu carousel.
    /// Campaign rewards can call Unlock from their completion flow.
    /// </summary>
    public static class FactionUnlockProgress
    {
        private const string UnlockKeyPrefix = "FactionUnlocked_";

        public static NationTabs.Nations SelectedFaction
        {
            get
            {
                var saved = (NationTabs.Nations)CurrentNationTab.CurrentNationTabValue;
                return System.Enum.IsDefined(typeof(NationTabs.Nations), saved) && IsUnlocked(saved)
                    ? saved
                    : NationTabs.Nations.Germany;
            }
        }

        public static bool IsUnlocked(NationTabs.Nations faction)
        {
            var unlockedByDefault = faction is NationTabs.Nations.Germany or NationTabs.Nations.France;
            return PlayerPrefs.GetInt(GetUnlockKey(faction), unlockedByDefault ? 1 : 0) == 1;
        }

        public static bool TrySelect(NationTabs.Nations faction)
        {
            if (!IsUnlocked(faction))
            {
                return false;
            }

            CurrentNationTab.CurrentNationTabValue = (int)faction;
            PlayerPrefs.Save();
            return true;
        }

        public static void Unlock(NationTabs.Nations faction)
        {
            PlayerPrefs.SetInt(GetUnlockKey(faction), 1);
            PlayerPrefs.Save();
        }

        private static string GetUnlockKey(NationTabs.Nations faction)
        {
            return UnlockKeyPrefix + (int)faction;
        }
    }
}
