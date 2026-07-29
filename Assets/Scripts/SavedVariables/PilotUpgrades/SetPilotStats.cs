using System;
using SavedVariables.Abilities;
using UnityEngine;


namespace SavedVariables.PilotUpgrades
{
    public class SetPilotStats
    {
        public static void SetPilotStatsValues(PilotSkillData skills, PilotStatData stats)
        {

            ListOfPilotAbilities.PilotAbilities ability = ListOfPilotAbilities.PilotAbilities.None;
            var killsForXpBonus = 0;
            var invincibilityBonus = 0f;
            var rangeBonus = 0f;
            var padBonus = 0f;
            var fireSpeedBonus = 0f;
            var hpBonus = 0;
            var shieldBonus = 0;

            foreach(ListOfExistingSkills skill in Enum.GetValues(typeof(ListOfExistingSkills)))
            {
                if (!skills.unlockedSkillsIds.Contains(skill.ToString())) continue;

                switch (skill)
                {

                    case ListOfExistingSkills.M1_InvincibilityAftetHit: invincibilityBonus += 1f; break;
                    case ListOfExistingSkills.M2_Range: rangeBonus += 1f; break;

                    case ListOfExistingSkills.T1_KillsForXp: killsForXpBonus += 1; break;
                    case ListOfExistingSkills.T2_PlaneAbilityDuration: padBonus += 1; break;

                    case ListOfExistingSkills.A1_PlaneAbilityDuration: padBonus+= 1f;  break;
                    case ListOfExistingSkills.A2_FireSpeed: fireSpeedBonus += 0.1f; break;
                    
                    case ListOfExistingSkills.SA1_Shield: shieldBonus += 1; break;
                    
                    //abilities must be here in order by level, because you never own two on the same level
                    case ListOfExistingSkills.M0_PartialRepair: ability = ListOfPilotAbilities.PilotAbilities.PartialRepair; break;
                    case ListOfExistingSkills.T0_Reflexes: ability = ListOfPilotAbilities.PilotAbilities.Reflexes1; break;
                    case ListOfExistingSkills.A0_Dodge: ability = ListOfPilotAbilities.PilotAbilities.Dodge1; break;
                    
                    case ListOfExistingSkills.M3_Repair: ability = ListOfPilotAbilities.PilotAbilities.Repair; break;
                    case ListOfExistingSkills.T3_Reflexes: ability = ListOfPilotAbilities.PilotAbilities.Reflexes2; break;
                    case ListOfExistingSkills.A3_Dodge: ability = ListOfPilotAbilities.PilotAbilities.Dodge2; break;
                    
                }
            }

            stats.ability = ability;
            stats.killsForXpModifier = killsForXpBonus;
            stats.invincibilityDurationBonus = invincibilityBonus;
            stats.rangeBonus = rangeBonus;
            stats.planeAbilityDurationBonus = padBonus;
            stats.fireSpeedBonus = fireSpeedBonus;
            stats.hpBonus = hpBonus;
            stats.shieldBonus = shieldBonus;

            Debug.Log(ability);
        }
    }
}