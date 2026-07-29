using System;
using System.Collections;
using SavedVariables.Abilities;
using SavedVariables.PilotUpgrades;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class PlaneAbility : MonoBehaviour
    {
        [Header("General Plane Ability Settings")]
        public ListOfPlaneAbilities.PlaneAbilities ability;
        public int xpCost = 1;
        public float abilityCd = 0.2f;
        public bool isAbilityFireType = true;
        public float abilityDuration = 10f;
        
        [Header("Burst")]
        public float burstFireCd = 1.5f;
        public int numberOfBurstShots= 3;
        [SerializeField] private GameObject burstBullet;
        private WaitForSeconds _waitBurst;
        
        [Header("Rapid")]
        public float rapidFireCd = 0.2f;
        private int _burstShots;
        private WaitForSeconds _waitRapid;
        
        private PilotStatData _pilotStatData;
        private PlayerFire _playerFire;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;
            _playerFire = GetComponent<PlayerFire>();

            _burstShots = numberOfBurstShots + PlaneAbilityUpgrades.CurrentBurstUpgradeLevel;
        
            burstFireCd -= _pilotStatData.fireSpeedBonus;
            rapidFireCd -= (PlaneAbilityUpgrades.CurrentRapidUpgradeLevel * 0.4f) + _pilotStatData.fireSpeedBonus;
            
                
            burstFireCd = Mathf.Clamp(burstFireCd, 0.2f, 10);
            rapidFireCd = Mathf.Clamp(rapidFireCd, 0.05f, 10);

            _waitBurst = new WaitForSeconds(burstFireCd);
            _waitRapid = new WaitForSeconds(rapidFireCd);
        }

        public void UseAbility()
        {
            switch (ability)
            {
                case ListOfPlaneAbilities.PlaneAbilities.None:
                    break;
                case ListOfPlaneAbilities.PlaneAbilities.Burst:
                    Burst();
                    break;
                case ListOfPlaneAbilities.PlaneAbilities.Rapid:
                    Rapid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Burst()
        {
            for (var i = 0; i < _burstShots; i++)
            { 
                Instantiate(burstBullet, _playerFire.spawnPosition, burstBullet.transform.rotation);
            }
            StartCoroutine(BurstCdTimer());
        }

        private IEnumerator BurstCdTimer()
        {
            yield return _waitBurst;
           _playerFire.isOnCooldown = false;
        }
    
        private void Rapid()
        {
            Instantiate(_playerFire.bullet, _playerFire.spawnPosition, quaternion.identity);
            StartCoroutine(RapidCdTimer());
        }
        private IEnumerator RapidCdTimer()
        {
            yield return _waitRapid;
            _playerFire.isOnCooldown = false;
        }
    }
}
