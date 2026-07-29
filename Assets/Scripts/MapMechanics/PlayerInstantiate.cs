using System;
using System.Collections.Generic;
using PlayerPlaneScripts;
using SavedVariables.Planes;
using Unity.Mathematics;
using UnityEngine;

namespace MapMechanics
{
    public class PlayerInstantiate : MonoBehaviour
    {
        [Serializable]
        private class RuntimePlaneDefinition
        {
            public ListOfPlanes.Planes planeType;
            public Sprite sprite;
            public float baseFallSpeed = 7f;
            public float baseJumpStrength = 10f;
            public float baseFireSpeed = 1f;
            public float baseFireRange = 10f;
            public int baseKillsForXp = 5;
        }

        public static PlayerInstantiate PlayerSpawnerInstance { get; private set;}
        
        [SerializeField] private GameObject[] planes;
        [SerializeField] private List<RuntimePlaneDefinition> runtimePlanes = new();

        [NonSerialized] public GameObject spawnedPlane; 
        
        
        private void Awake()
        {
            PlayerSpawnerInstance = this;
            foreach (var plane in planes)
            {
               if (plane.GetComponent<InitializePlaneProperties>().basePropertiesData.planeType !=
                    CurrentPlane.EquippedPlane) continue;
               
               spawnedPlane = Instantiate(plane, new Vector3(-4f, 0f, 0f), quaternion.identity);
               return;
            }

            var runtimePlane = runtimePlanes.Find(entry => entry.planeType == CurrentPlane.EquippedPlane);
            if (runtimePlane != null && planes.Length > 0)
            {
                spawnedPlane = Instantiate(planes[0], new Vector3(-4f, 0f, 0f), quaternion.identity);

                var spriteRenderer = spawnedPlane.GetComponentInChildren<SpriteRenderer>(true);
                if (spriteRenderer != null && runtimePlane.sprite != null)
                {
                    spriteRenderer.sprite = runtimePlane.sprite;
                }

                spawnedPlane.GetComponent<InitializePlaneProperties>().ConfigureRuntimePlane(
                    runtimePlane.planeType,
                    runtimePlane.baseFallSpeed,
                    runtimePlane.baseJumpStrength,
                    runtimePlane.baseFireSpeed,
                    runtimePlane.baseFireRange,
                    runtimePlane.baseKillsForXp);
                return;
            }

            Debug.LogError($"No playable prefab or runtime definition exists for {CurrentPlane.EquippedPlane}.");
        }
    }
}
