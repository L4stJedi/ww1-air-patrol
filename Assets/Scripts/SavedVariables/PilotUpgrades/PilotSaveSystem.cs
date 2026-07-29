using UnityEngine;

namespace SavedVariables.PilotUpgrades
   
{
    public static class PilotSaveSystem
    {
        private static string GetSaveKey(int pilotID)
        {
            return $"Pilot_{pilotID}_Data";
        }

        [System.Serializable]
        public class PilotSaveWrapper
        {
            public PilotSkillData skillData = new PilotSkillData();
            public PilotStatData statData = new PilotStatData();
        }
        
        public static void SavePilotData(int pilotID, PilotSkillData skillData, PilotStatData statData)
        {
            SetPilotStats.SetPilotStatsValues(skillData, statData);
            
            PilotSaveWrapper wrapper = new PilotSaveWrapper();
            wrapper.skillData = skillData;
            wrapper.statData = statData;
            
            var json = JsonUtility.ToJson(wrapper);
            
            PlayerPrefs.SetString(GetSaveKey(pilotID), json);
            PlayerPrefs.Save();
            
            
        }

        public static PilotSaveWrapper LoadPilotData(int pilotID)
        {
            if (!PlayerPrefs.HasKey(GetSaveKey(pilotID))) return new PilotSaveWrapper();
            
            var json = PlayerPrefs.GetString(GetSaveKey(pilotID));
            
            return JsonUtility.FromJson<PilotSaveWrapper>(json);
        }
    }
}
