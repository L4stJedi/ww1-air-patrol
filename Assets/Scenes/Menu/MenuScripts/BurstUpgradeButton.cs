using System;
using UnityEngine;
using SavedVariables;
using TMPro;
using UnityEngine.UIElements;

namespace Scenes.Menu.MenuScripts
{
    public class BurstUpgradeButton : MonoBehaviour
    {
        
        public int xpCost = 20;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private TextMeshProUGUI costText;
        private Button _button;

        private void Start()
        {
            buttonText.text = "Burst+" + Environment.NewLine + $"({SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel}-5)";
            costText.text = "Cost" + Environment.NewLine + $"{SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel}/5";

            if (SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel < 5) return;
            _button.SetEnabled(false);
            Destroy(costText);
        }

        public void TriggerUpgradeBurst()
            {
                if (BankedXp.BankedXpValue < xpCost || SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel >= 5) return;
                BankedXp.SpendBankedXp(xpCost);
                SavedVariables.Abilities.PlaneAbilityUpgrades.LevelUpBurst();
                buttonText.text = "Burst+" + Environment.NewLine + $"({SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel}-5)";
                if (SavedVariables.Abilities.PlaneAbilityUpgrades.CurrentBurstUpgradeLevel >= 5)
                {
                    _button.SetEnabled(false);
                    Destroy(costText);
                }
            }
    }
}