using System.Linq;
using SavedVariables;
using SavedVariables.PilotUpgrades;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class BuySkill : MonoBehaviour
    {
        private DescriptionUI _des;
        private PilotSaveSystem.PilotSaveWrapper _profile;
        private PilotSkillData _skillData;


        
        // called on button click, checks if player has previous skills/nodes unlocked
        public void Buy()
        {
            _des = GetComponentInParent<DescriptionUI>();
            _profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            _skillData = _profile.skillData;
            Unlock();
        }

        private void Unlock()
        {
            _profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            _skillData = _profile.skillData;

            if (_skillData.unlockedSkillsIds.Contains(_des.ownedSkill.ToString())) return;

            if (_des.prohibitedSkills.Any(prohibitedSkills =>
                    _skillData.unlockedSkillsIds.Contains(prohibitedSkills.ToString())))
            {
                return;
            }

            if (_des.requiredSkills.Length > 0)
            {
                if (_des.doesNeedAllOfReqSkills)
                {
                    if (_des.requiredSkills.Any(skill => !_skillData.unlockedSkillsIds.Contains(skill.ToString())))
                    {
                        return;
                    }
                }
                else
                {
                    var isAnyOfTheSkillsUnlocked = _des.requiredSkills.Any(skill =>
                        _skillData.unlockedSkillsIds.Contains(skill.ToString()));
                    if (!isAnyOfTheSkillsUnlocked) return;
                }
            }

            if (!BankedXp.SpendBankedXp(_des.cost)) return;

            _skillData.unlockedSkillsIds.Add(_des.ownedSkill.ToString());
            PilotSaveSystem.SavePilotData(CurrentPilot.PilotID, _skillData, _profile.statData);
            _des.skl.SetToUnlocked();
        }
    }
}
