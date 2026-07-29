using UnityEngine;

namespace SavedVariables.Abilities
{
    public static class PlaneAbilityUpgrades
    {
        #region Burst
        private const string BurstUpgradeKey = "BurstUpgradeLevel";

        public static int CurrentBurstUpgradeLevel
        {
            get => PlayerPrefs.GetInt(BurstUpgradeKey, 0);
            private set
            {
                PlayerPrefs.SetInt(BurstUpgradeKey, value);
                PlayerPrefs.Save();
            }
        }

        public static void LevelUpBurst()
        {
            if (CurrentBurstUpgradeLevel + 1 > 5) return;
            
            CurrentBurstUpgradeLevel++;
        }
        #endregion
        
        #region Rapid
        private const string RapidUpgradeKey = "RapidUpgradeLevel";

        public static int CurrentRapidUpgradeLevel
        {
            get => PlayerPrefs.GetInt(RapidUpgradeKey, 0);
            private set
            {
                PlayerPrefs.SetInt(RapidUpgradeKey, value);
                PlayerPrefs.Save();
            }
        }

        public static void LevelUpRapid()
        {
            if (CurrentRapidUpgradeLevel + 1 > 5) return;
            
            CurrentRapidUpgradeLevel++;
        }
        #endregion
    }
}
