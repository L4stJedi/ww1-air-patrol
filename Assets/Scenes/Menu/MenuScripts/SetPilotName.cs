using SavedVariables.PilotUpgrades;
using TMPro;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class SetPilotName : MonoBehaviour
    {
        private TMP_InputField _textRef;
        private PilotStatData _pilotStats;
        private PilotSkillData _pilotSkills;

        private void Start()
        {

            var profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            _pilotStats = profile.statData;
            _pilotSkills = profile.skillData;
            
            _textRef = GetComponent<TMP_InputField>();
            
            if (_pilotStats.pilotName != null)
                _textRef.text = _pilotStats.pilotName;
        }
    
        public void TriggerSetPilotName()
        {
            _pilotStats.pilotName = _textRef.text;
            PilotSaveSystem.SavePilotData(CurrentPilot.PilotID, _pilotSkills, _pilotStats);
        }
    }
}
