using SavedVariables;
using SavedVariables.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    public class ToggleLeftHandedMode : MonoBehaviour
    {

        private Toggle _toggle;

        private void Awake()
        {
           _toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            _toggle.isOn = SavedSettings.IsLeftHanded;
        }

        public void TriggerOnToggle()
        {
            SavedSettings.IsLeftHanded = _toggle.isOn;
        }
    }
}
