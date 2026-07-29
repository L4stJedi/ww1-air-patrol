
using System;
using System.Collections.Generic;

namespace SavedVariables.PilotUpgrades
{
    //Skill data that is saved as JSON in "PilotSaveSystem" separately for every pilot

    //Note the list is empty and new "unlocked" skills are added in separate script,
    //which is attached to the skill tree notes

    [Serializable]
    public class PilotSkillData
    {
        public List<string> unlockedSkillsIds = new List<string>();
    }
}

