using System;
using UnityEngine;

namespace SavedVariables.Abilities
{
    public class ListOfPilotAbilities
    {
        public enum PilotAbilities
        {
            None,

            Reflexes1,
            PartialRepair,
            Dodge1,

            Reflexes2,
            Repair,
            Dodge2
        }
    }

    public class ListOfPlaneAbilities
    {
        public enum PlaneAbilities
        {
            None,
            Burst,
            Rapid
        }
    }
}