using System;
using SavedVariables.Planes;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class InitializePlaneProperties : MonoBehaviour
    {
        [Header("Data")] public PlaneBaseProperties basePropertiesData;
        
        
        [NonSerialized] public float currentFallSpeed;
        [NonSerialized] public float currentJumpStrength;
        [NonSerialized] public float currentFireSpeed;
        [NonSerialized] public float currentFireRange;
        
        private PlaneEnhanceSaveData _enhanceData;

        
        
        private void Awake()
        {
            if (basePropertiesData == null)
            {
                Debug.Log("Plane misses base properties data");
                return;
            }

            ApplyProperties(basePropertiesData);
        }

        public void ConfigureRuntimePlane(
            ListOfPlanes.Planes planeType,
            float baseFallSpeed,
            float baseJumpStrength,
            float baseFireSpeed,
            float baseFireRange,
            int baseKillsForXp)
        {
            var runtimeData = ScriptableObject.CreateInstance<PlaneBaseProperties>();
            runtimeData.planeType = planeType;
            runtimeData.baseFallSpeed = baseFallSpeed;
            runtimeData.baseJumpStrength = baseJumpStrength;
            runtimeData.baseFireSpeed = baseFireSpeed;
            runtimeData.baseFireRange = baseFireRange;
            runtimeData.baseKillsForXp = baseKillsForXp;

            basePropertiesData = runtimeData;
            ApplyProperties(runtimeData);
        }

        private void ApplyProperties(PlaneBaseProperties data)
        {
            _enhanceData = PlaneSaveSystem.LoadPlaneProperties(data.planeType);

            currentFallSpeed = data.baseFallSpeed * (1 - _enhanceData.enhanceFallSpeed * 0.05f);
            currentJumpStrength = data.baseJumpStrength * (1 + _enhanceData.enhanceJumpStrength * 0.05f);
            currentFireSpeed = data.baseFireSpeed * (1 + _enhanceData.enhanceFireSpeed * 0.1f);
            currentFireRange = data.baseFireRange * (1 + _enhanceData.enhanceFireRange * 0.1f);
        }
    }
}
