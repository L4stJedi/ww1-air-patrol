using System.Collections.Generic;

namespace SavedVariables.PilotUpgrades
{
    // List of skills from which devs can choose when creating skill tree nodes
    // M,T,A are types of skills and their numbers are counted from top to bottom

    public enum ListOfExistingSkills
    {
        M0_PartialRepair,
        M1_InvincibilityAftetHit,
        M2_Range,
        M3_Repair,

        T0_Reflexes,
        T1_KillsForXp,
        T2_PlaneAbilityDuration,
        T3_Reflexes,
        
        A0_Dodge,
        A1_PlaneAbilityDuration,
        A2_FireSpeed,
        A3_Dodge,
        
        SA1_Shield,
        SB1_WhoKnows // work in progress
    }
}