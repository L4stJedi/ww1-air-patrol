using UnityEngine;

namespace SavedVariables.PilotUpgrades
{
    public static class CurrentPilot
    {

        private const string CurrentPilotKey = "CurrentPilotKey";

        public static int PilotID
        {
            get => PlayerPrefs.GetInt(CurrentPilotKey, 0);
            set
            {
                PlayerPrefs.SetInt(CurrentPilotKey, value);
                PlayerPrefs.Save();
            }
        }
        
        
    }
}