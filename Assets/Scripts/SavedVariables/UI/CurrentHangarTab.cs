using PlayerPlaneScripts;
using UnityEngine;

namespace SavedVariables
{
    public class CurrentHangarTab : MonoBehaviour
    {
        private const string CurrentHangarTabKey = "CurrentHangarTab";

        public static int CurrentHangarTabValue
        {
            get => PlayerPrefs.GetInt(CurrentHangarTabKey, 0);
            set => PlayerPrefs.SetInt(CurrentHangarTabKey, value);
        }
    }
}
