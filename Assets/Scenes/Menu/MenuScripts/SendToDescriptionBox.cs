
using System;
using SavedVariables.PilotUpgrades;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class SendToDescriptionBox : MonoBehaviour
    {

        [SerializeField] private bool needsAllOfRequiredSkills;
        [SerializeField] private DescriptionUI descriptionUI;
        [SerializeField] private string skillDescription = "This skill unlocks";
        private SkillTreeNode _skillTreeNode;
        public int cost = 1;
        public ListOfExistingSkills ownedSkill;
        public ListOfExistingSkills[] requiredSkills;
        public ListOfExistingSkills[] prohibitedSkills;

        public void Awake()
        {
            _skillTreeNode = GetComponent<SkillTreeNode>();
            

        }

        public void SendToDesBox()
        {
            if (PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).skillData.unlockedSkillsIds.Contains(ownedSkill.ToString())) return;
            
            descriptionUI.GetSkillInformation(skillDescription, cost, ownedSkill, requiredSkills, prohibitedSkills, needsAllOfRequiredSkills, _skillTreeNode);
        }
    }
}
