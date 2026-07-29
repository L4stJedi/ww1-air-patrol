using UnityEngine;

namespace SavedVariables
{
    public class BestScore : MonoBehaviour
    {
        private const string BestScoreKey = "BestScore";

        public static int CurrentBestScore
        {
            get => PlayerPrefs.GetInt(BestScoreKey, 0);
            private set
            {
                PlayerPrefs.SetInt(BestScoreKey, value);
                PlayerPrefs.Save();
            }
        }
        
        public static void SetCurrentBestScore(int newBestScore)
        {
            if (newBestScore > CurrentBestScore)
            {
                CurrentBestScore = newBestScore;
            }
        }
    }
}
