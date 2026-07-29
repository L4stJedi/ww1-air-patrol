using SavedVariables.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class ToggleHapticFeedback : MonoBehaviour
    {
    
        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            _toggle.isOn = SavedSettings.IsHapticEnabled;
        }

        public void TriggerOnToggle()
        {
            SavedSettings.IsHapticEnabled = _toggle.isOn;
            if (_toggle.isOn)
            {
                HapticWrapper.Feedback(HapticType.Heavy);
            }
        }
        
    }
}
