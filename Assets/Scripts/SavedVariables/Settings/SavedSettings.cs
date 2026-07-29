using UnityEngine;

namespace SavedVariables.Settings
{
    public class SavedSettings : MonoBehaviour
    {

        private const string LeftHandedKey = "LeftHanded";

        public static bool IsLeftHanded
        {
            get
            {
                var savedValue = PlayerPrefs.GetInt(LeftHandedKey, 0);

                return savedValue == 1;
            }
            set
            {
                var valueToSave = value ? 1 : 0;
                
                PlayerPrefs.SetInt(LeftHandedKey, valueToSave);
                PlayerPrefs.Save();
            }
        }

        private const string HapticEnabledKey = "IsHapticEnabled";

        public static bool IsHapticEnabled
        {
            get
            {
                var savedValue = PlayerPrefs.GetInt(HapticEnabledKey, 1);

                return savedValue == 1;
            }
            set
            {
                var valueToSave = value ? 1 : 0;
                
                PlayerPrefs.SetInt(HapticEnabledKey, valueToSave);
                PlayerPrefs.Save();
            }
        }

    }
}
