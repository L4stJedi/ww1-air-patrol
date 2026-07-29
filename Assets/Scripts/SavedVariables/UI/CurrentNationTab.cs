using PlayerPlaneScripts;
using UnityEngine;

namespace SavedVariables
{
    public class CurrentNationTab : MonoBehaviour
    {
        private const string CurrentNationTabKey = "CurrentNationTab";

        public static int CurrentNationTabValue
        {
            get => PlayerPrefs.GetInt(CurrentNationTabKey, 0);
            set => PlayerPrefs.SetInt(CurrentNationTabKey, value);
        }
    }
}
