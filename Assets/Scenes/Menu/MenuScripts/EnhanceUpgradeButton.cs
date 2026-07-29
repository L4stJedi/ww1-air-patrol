using System;
using SavedVariables;
using SavedVariables.Planes;
using TMPro;
using UnityEngine;

namespace Scenes.Menu.MenuScripts
{
    public class EnhanceUpgradeButton : MonoBehaviour
    {
        [SerializeField] private EnhanceUIGetCurrentPlaneShowcase uiGetCurrentPlaneShowcase;
        [SerializeField] private IncreaseOrDecrease action;
        [SerializeField] private GetStatTypeScript getStatTypeScript;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI enhancePointsText;

        private int _currentEnhancePoints;
        private PlaneEnhanceSaveData _planeSaveData;
        private PlaneEnhanceableStatTypes _statType;
        private ListOfPlanes.Planes _plane;

        private enum IncreaseOrDecrease
        {
            Increase,
            Decrease
        } 
        
        private void OnEnable()
        {            
            _plane = uiGetCurrentPlaneShowcase.plane;
            _planeSaveData = PlaneSaveSystem.LoadPlaneProperties(_plane);
            _statType = getStatTypeScript.statType;
            _currentEnhancePoints = _planeSaveData.enhanceFallSpeed + _planeSaveData.enhanceJumpStrength +
                                    _planeSaveData.enhanceFireSpeed + _planeSaveData.enhanceFireRange;
            UpdateText();
        }

        public void CallEnhanceOnClick()
        {
            _plane = uiGetCurrentPlaneShowcase.plane;
            _planeSaveData = PlaneSaveSystem.LoadPlaneProperties(_plane);
            
            _currentEnhancePoints = _planeSaveData.enhanceFallSpeed + _planeSaveData.enhanceJumpStrength +
                                    _planeSaveData.enhanceFireSpeed + _planeSaveData.enhanceFireRange;
            
            switch (action)
            {
                case IncreaseOrDecrease.Increase:
                    Increase();
                    break;
                case IncreaseOrDecrease.Decrease:
                    Decrease();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            PlaneSaveSystem.SavePilotData(_plane, _planeSaveData);
            UpdateText();
        }
        
        private void Increase()
        {
            if (_currentEnhancePoints >= _planeSaveData.enhancePointMaximum || !BankedXp.SpendBankedXp(1)) return;
            switch (_statType)
            {
                case PlaneEnhanceableStatTypes.FallSpeed:
                   if (_planeSaveData.enhanceFallSpeed >= 5) return;
                   _planeSaveData.enhanceFallSpeed++;
                   break;
                case PlaneEnhanceableStatTypes.JumpStrength:
                    if (_planeSaveData.enhanceJumpStrength >= 5) return;
                    _planeSaveData.enhanceJumpStrength++;
                    break;
                case PlaneEnhanceableStatTypes.FireSpeed:
                    if (_planeSaveData.enhanceFireSpeed >= 5) return;
                    _planeSaveData.enhanceFireSpeed++;
                    break;
                case PlaneEnhanceableStatTypes.FireRange: 
                    if (_planeSaveData.enhanceFireRange >= 5) return;
                    _planeSaveData.enhanceFireRange++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private void Decrease()
        {
            switch (_statType)
            {
                case PlaneEnhanceableStatTypes.FallSpeed:
                    if (_planeSaveData.enhanceFallSpeed < 1) return;
                    _planeSaveData.enhanceFallSpeed--;
                    break;
                case PlaneEnhanceableStatTypes.JumpStrength:
                    if (_planeSaveData.enhanceJumpStrength < 1) return;
                    _planeSaveData.enhanceJumpStrength--;
                    break;
                case PlaneEnhanceableStatTypes.FireSpeed:
                    if (_planeSaveData.enhanceFireSpeed < 1) return;
                    _planeSaveData.enhanceFireSpeed--;
                    break;
                case PlaneEnhanceableStatTypes.FireRange:
                    if (_planeSaveData.enhanceFireRange < 1) return;
                    _planeSaveData.enhanceFireRange--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void UpdateText()
        {
            _currentEnhancePoints = _planeSaveData.enhanceFallSpeed + _planeSaveData.enhanceJumpStrength +
                                    _planeSaveData.enhanceFireSpeed + _planeSaveData.enhanceFireRange;
            
            currentLevelText.text = _statType switch
            {
                PlaneEnhanceableStatTypes.FallSpeed => ($"{100 - (_planeSaveData.enhanceFallSpeed * 5)} / 75%"),
                PlaneEnhanceableStatTypes.JumpStrength => ($"{100 + (_planeSaveData.enhanceJumpStrength * 5)} / 125%"),
                PlaneEnhanceableStatTypes.FireSpeed => ($"{100 + (_planeSaveData.enhanceFireSpeed) * 10} / 150%"),
                PlaneEnhanceableStatTypes.FireRange => ($"{100 + (_planeSaveData.enhanceFireRange) * 10} / 150%"),
                _ => throw new ArgumentOutOfRangeException()
            };

            enhancePointsText.text = "Enhances:" + Environment.NewLine +
                                     $"{_currentEnhancePoints}/{_planeSaveData.enhancePointMaximum}";
        }
    }
}
