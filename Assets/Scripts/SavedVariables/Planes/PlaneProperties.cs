using System;
using UnityEngine;

namespace SavedVariables.Planes
{
    [CreateAssetMenu(fileName = "NewPlaneData", menuName = "Planes/Plane Data")]
    public class PlaneBaseProperties : ScriptableObject
    {
        public ListOfPlanes.Planes planeType;
        
        public float baseFallSpeed = 7f;
        public float baseJumpStrength = 10f;
        public float baseFireSpeed = 1f;
        public float baseFireRange = 10f;
        public int   baseKillsForXp = 5;

        public bool startingPlane;
    }

    public class ExistingPlanes
    {
    }

    [Serializable]
    public class PlaneEnhanceSaveData
    {
        [Range(0, 5)] public int enhanceFallSpeed;
        [Range(0, 5)] public int enhanceJumpStrength;
        [Range(0, 5)] public int enhanceFireSpeed;
        [Range(0, 5)] public int enhanceFireRange;
        [Range(0, 5)] public int enhanceKillsForXp;

        [Range(0, 15)] public int enhancePointMaximum = 10;
        public int bestScore;
        public bool isUnlocked = false;

    }
    
    [Serializable]
    public enum PlaneEnhanceableStatTypes
    {
        FallSpeed,
        JumpStrength,
        FireSpeed,
        FireRange
    }
}
