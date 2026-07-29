using System;
using System.Collections;
using SavedVariables.Abilities;
using SavedVariables.PilotUpgrades;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPlaneScripts
{
    public class PilotAbilityScript : MonoBehaviour
    {

        private Image _filter;
        private PilotStatData _pilotStatData;
        private bool _areReflexesActive;
        private float _reflexesLenght;
        private Xp _xp;
        private PlayerHp _playerHp;

        private int _repairPoints;
        private bool _canUseRepair = true;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            var filterObject = GameObject.Find("Filter");
            _filter = filterObject.GetComponent<Image>();
            _playerHp = GetComponent<PlayerHp>();
            
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;
            _xp = GetComponent<Xp>();
        }

        public void UseAbility()
        {
            switch (_pilotStatData.ability)
            {
                case ListOfPilotAbilities.PilotAbilities.None:
                    break;
                
                case ListOfPilotAbilities.PilotAbilities.Reflexes1:
                    Reflexes(1);
                    break;
                case ListOfPilotAbilities.PilotAbilities.Reflexes2:
                    Reflexes(2);
                    break;
                
                case ListOfPilotAbilities.PilotAbilities.PartialRepair:
                    StartCoroutine(Repair(1));
                    break;
                
                case ListOfPilotAbilities.PilotAbilities.Repair:
                    StartCoroutine(Repair(2));
                    break;

                case ListOfPilotAbilities.PilotAbilities.Dodge1:
                    StartCoroutine(Dodge(1));
                    break;

                case ListOfPilotAbilities.PilotAbilities.Dodge2:
                    StartCoroutine(Dodge(2));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Update()
        {
 
            if (!_areReflexesActive) return;
            Time.timeScale += (1f / _reflexesLenght) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            if (Time.timeScale >= 1)
            {
                _filter.color = new Color(0, 0, 0, 0);
                _areReflexesActive = false;
            }
        }

        private void Reflexes(int level)
        {
            if (_xp.xP < 1 || _areReflexesActive) return;
            _xp.xP -= 1;
            
            switch (level)
            {
                case 1:
                    _reflexesLenght = 15f;
                    Time.timeScale = 0.5f;
                    break;
                
                case 2:
                    _reflexesLenght = 20f;
                    Time.timeScale = 0.2f;
                    break;
            }
            _filter.color = new Color(0f, 0.26f, 1f, 0.22f);
            _areReflexesActive = true;
        }

        private IEnumerator Repair(int level)
        {
            if (_xp.xP < 1 || !_canUseRepair || _playerHp.Hp >= _playerHp.maxHp) yield break;
            _xp.xP -= 1;        
            _repairPoints++;
            switch (level)
            {
                case 1:
                    if (_repairPoints >= 3)
                    {
                        _playerHp.Heal();
                        _repairPoints = 0;
                    }
                    break;
                
                case 2:
                    if (_repairPoints >= 2)
                    {
                        _playerHp.Heal();
                        _repairPoints = 0;
                    }
                    break;
            }
            _canUseRepair = false;
            
            yield return new WaitForSeconds(2f);

            _canUseRepair = true;
        }

        private IEnumerator Dodge(int level) 
        {
            if (_playerHp.isInvincible || _xp.xP <= 0) yield break;
            _xp.xP--;
            
            
            _playerHp.isInvincible = true;
            _filter.color = new Color(1f, 0.89f, 0f, 0.22f);
            switch (level)
            {
                case 1: yield return new WaitForSeconds(0.5f); break;
                case 2: yield return new WaitForSeconds(0.75f); break;
            }

            _filter.color = new Color(0, 0, 0, 0);
            _playerHp.isInvincible = false;
        }
    }
}


