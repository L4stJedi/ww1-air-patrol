using System;
using SavedVariables.PilotUpgrades;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class Xp : MonoBehaviour
    {
        [Header("Configuration")]
        public int xP;
        public int killCount;
        [NonSerialized] public int killsForXp = 5;
        
        private PilotStatData _pilotStatData;

        private void Start()
        {
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;

            killsForXp -= _pilotStatData.killsForXpModifier;
        }

        // Update is called once per frame
        private void Update()
        {
            if (killCount < killsForXp) return;
            killCount -= killsForXp;
            xP++;
        }
    }
}
