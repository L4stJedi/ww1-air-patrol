using System;
using System.Collections;
using SavedVariables.PilotUpgrades;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class PlayerFire : MonoBehaviour
    {

    
        [Header("BulletSpawn Information")]
        public float offset;
        [SerializeField] private LayerMask layerMask;
        

        [Header("References")]
        [SerializeField] private  Rigidbody2D rb;
        public GameObject bullet;

        private float _fireCd = 1f;
        private float _fireRange = 10;
        [NonSerialized] public bool isOnCooldown;
        
        
        [NonSerialized] public Vector2 spawnPosition;
        private InitializePlaneProperties _initializePlaneProperties;
        private PilotStatData _pilotStatData;

        private ControlScript _controlScript;
        private PlaneAbility _planeAbility;
        [NonSerialized] public WaitForSeconds cd;
        
        // Awake is used to add stat bonuses from "Pilot" to the variables of this script
        private void Start()
        {
            _planeAbility = GetComponent<PlaneAbility>();
            _controlScript = GetComponent<ControlScript>();
            
            
            _initializePlaneProperties = GetComponent<InitializePlaneProperties>();

            _fireCd = _initializePlaneProperties.currentFireSpeed;

            _fireRange = _initializePlaneProperties.currentFireRange;

            //Stat bonuses applying
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;

            _fireCd -= _pilotStatData.fireSpeedBonus;

            _fireRange += _pilotStatData.rangeBonus;

            _fireCd = Mathf.Clamp(_fireCd, 0.05f, 10);
            _fireRange = Mathf.Clamp(_fireRange, offset+1, 100);
            cd = new WaitForSeconds(_fireCd);
        }


        //Chooses which fire function will be called and calls it if enemy is right to the player 
        private void FixedUpdate()
        {
            if (isOnCooldown) return;
            
            var enemyInFront = Physics2D.Raycast(rb.position, Vector2.right, _fireRange, layerMask);
            
            if (!enemyInFront.collider) return;
        
            isOnCooldown = true;

            var position = rb.position;
            spawnPosition = new Vector2(position.x + offset, position.y);
            
            //FireMode is set in "ControlScript"
            if (!_controlScript.isAbilityActive)
            {
                Fire();
            }
            else
            {
                _planeAbility.UseAbility();
            }
        }
        
        // Fire function which instantiate bullets and timer for handling continuous fire
        #region Fire
        
        private void Fire()
        {
            Instantiate(bullet, spawnPosition, quaternion.identity);
            isOnCooldown = true;
            StartCoroutine(CdTimer());
        }

        public IEnumerator CdTimer()
        {
            yield return cd;
            isOnCooldown = false;
        }
    
        #endregion
        
    }
}