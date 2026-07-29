using System;
using System.Collections;
using SavedVariables.PilotUpgrades;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace PlayerPlaneScripts
{
    public class ControlScript : MonoBehaviour
    {
        [Header("Configuration")] 

        public float moveSpeed = 5f;

        [Header("References")] [SerializeField]
        private Rigidbody2D rb;

        private float _jumpSpeed;
        private Vector2 _jumpToY;
        private Xp _xpRef;
        private bool _shouldJump;
        private PlayerInputActions _inputActions;
        private InputAction _jumpAction;
        private InputAction _planeAbilityAction;
        private InputAction _pilotAbilityAction;
        private PilotStatData _pilotStatData;
        private PlaneAbility _planeAbility;
        private PilotAbilityScript _pilotAbilityScript;
        private bool _canUseAbility = true;
        private InitializePlaneProperties _initializePlaneProperties;
        [NonSerialized] public bool isAbilityActive;


        //sets up input actions, variables and references
        private void Start()
        {
            _planeAbility = GetComponent<PlaneAbility>();
            _xpRef = GetComponent<Xp>();
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;

           _planeAbility.abilityDuration += _pilotStatData.planeAbilityDurationBonus;
           _pilotAbilityScript = GetComponent<PilotAbilityScript>();

           _initializePlaneProperties = GetComponent<InitializePlaneProperties>();
           _jumpSpeed = _initializePlaneProperties.currentJumpStrength;
           
        }

        
        //Setting, Enabling and Disabling Input Actions
        #region Input Actions Handling
        private void OnEnable()
        {
            _inputActions = new PlayerInputActions();
            
            _jumpAction = _inputActions.PlaneControls.Jump;
            _jumpAction.Enable();

            _planeAbilityAction = _inputActions.PlaneControls.PlaneAbility;
            _planeAbilityAction.Enable();

            _pilotAbilityAction = _inputActions.PlaneControls.PilotAbility;
            _pilotAbilityAction.Enable();
        }

        private void OnDisable()
        {
            _jumpAction.Disable();
            _planeAbilityAction.Disable();
            _pilotAbilityAction.Disable();
        }

        #endregion

        
        //checking for inputs, and calls function
        #region ChekingInputs

        private void Update()
        {
            if (_jumpAction.WasPressedThisFrame() && !EventSystem.current.IsPointerOverGameObject())
            {
                _shouldJump = true;
            }

            if (_planeAbilityAction.WasPressedThisFrame())
            {
                PlaneAbility();
            }

            if (_pilotAbilityAction.WasPressedThisFrame())
            {
                PilotAbility();
            }
        }


        private void FixedUpdate()
        {
            Fly();
            if (!_shouldJump) return;
            Jump();
            _shouldJump = false;
        }
        
        #endregion
        
        //changing fire modes, when player uses ability
        #region Plane Ability

        private void PlaneAbility()
        {
            var xpCost = Mathf.Max(0, _planeAbility.xpCost);
            if (!_canUseAbility || _xpRef.xP < xpCost) return;
            if (_planeAbility.isAbilityFireType && isAbilityActive) return;

            _xpRef.xP -= xpCost;
            _canUseAbility = false;

            if (_planeAbility.isAbilityFireType)
            {
                isAbilityActive = true;
                StartCoroutine(PlaneAbilityDurationTimer());
            }
            else
            {
                _planeAbility.UseAbility();
            }

            if (_planeAbility.abilityCd <= 0)
            {
                _canUseAbility = true;
                return;
            }
            
            StartCoroutine(PlaneAbilityCooldownTimer());
        }

        //when fire mode isn't set to "Default" cooldown is running, at the end fire mode is set to "Default"
        private IEnumerator PlaneAbilityDurationTimer()
        {
            yield return new WaitForSeconds(_planeAbility.abilityDuration);
            isAbilityActive = false;
        }

        private IEnumerator PlaneAbilityCooldownTimer()
        {
            yield return new WaitForSeconds(_planeAbility.abilityCd);
            _canUseAbility = true;
        }
        #endregion

        #region PilotAbility

        private void PilotAbility()
        {
            _pilotAbilityScript.UseAbility();
        }

        #endregion
        
        //jumping and flying to the right
        #region player movement

        private void Fly()
        {
            rb.linearVelocityX = moveSpeed;
        }

        //when Jump is called plane is forced upwards
        private void Jump()
        {
            rb.linearVelocityY = _jumpSpeed;
        }

        #endregion
    }
}