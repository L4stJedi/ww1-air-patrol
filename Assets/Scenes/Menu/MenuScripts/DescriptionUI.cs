using System;
using SavedVariables.PilotUpgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class DescriptionUI : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [NonSerialized] public ListOfExistingSkills ownedSkill;
        [NonSerialized] public ListOfExistingSkills[] requiredSkills;
        [NonSerialized] public ListOfExistingSkills[] prohibitedSkills;
        [NonSerialized] public int cost;
        [NonSerialized] public bool doesNeedAllOfReqSkills;
        [NonSerialized] public SkillTreeNode skl;



        public void GetSkillInformation(string description, int skillCost, ListOfExistingSkills nodeSkill, ListOfExistingSkills[] requiredSkillsForNode,
                    ListOfExistingSkills[] prohibitedSkillsForNode, bool needsAllRequiredSkills, SkillTreeNode skillTreeNode)
        {
            descriptionText.text = description;
            costText.text = $"Cost: {skillCost}XP";

            doesNeedAllOfReqSkills = needsAllRequiredSkills;
            ownedSkill = nodeSkill;
            requiredSkills = requiredSkillsForNode;
            prohibitedSkills = prohibitedSkillsForNode;
            cost = skillCost;
            skl = skillTreeNode;

        }
    }
}
