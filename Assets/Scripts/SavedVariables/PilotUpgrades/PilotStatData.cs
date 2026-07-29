using System;
using System.Collections.Generic;
using SavedVariables.Abilities;

namespace SavedVariables.PilotUpgrades
{
    // Data that is saved as JSON in "PilotSaveSystem" separately for every pilot

    // !! Note that very variable should be saved as positive value since values that lower
    // !! cool-downs etc. are handled in the separate scripts

    [Serializable]
    public class PilotStatData
    {
        public string pilotName = "";
        public ListOfPilotAbilities.PilotAbilities ability = ListOfPilotAbilities.PilotAbilities.None;
        public int killsForXpModifier = 0;
        public float planeAbilityDurationBonus = 0f;

        public float rangeBonus = 0f;
        public float fireSpeedBonus = 0f;

        public int hpBonus = 0;
        public int shieldBonus = 0;
        public float invincibilityDurationBonus = 0f;
    }
}