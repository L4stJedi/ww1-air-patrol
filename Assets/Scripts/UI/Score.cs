using PlayerPlaneScripts;
using SavedVariables;
using SavedVariables.Planes;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Score : MonoBehaviour
    {
        public int CurrentScore { get; private set; }
        private TextMeshProUGUI _scoreText;
        private TopAceUI _topAceUI;
        private InitializePlaneProperties _initializePlaneProperties;
        private PlaneEnhanceSaveData _enhanceSaveData;

        private void Awake()
        {
            _initializePlaneProperties = FindAnyObjectByType<InitializePlaneProperties>();
        }

        private void Start()
        {
            _scoreText = GetComponent<TextMeshProUGUI>();
            _topAceUI = FindAnyObjectByType<TopAceUI>();
            CurrentScore = 0;
        }

        //Score is changing value in "EnemyHP" and "FlyByDetection"
        private void Update()
        {
            _scoreText.text = "Score: " + CurrentScore;
        }

        public void ChangeScoreValueBy(int newScore)
        {
            CurrentScore += newScore;
            if (CurrentScore <= BestScore.CurrentBestScore) return;
            BestScore.SetCurrentBestScore(CurrentScore);
            _enhanceSaveData = PlaneSaveSystem.LoadPlaneProperties(_initializePlaneProperties.basePropertiesData.planeType);
            _enhanceSaveData.bestScore = CurrentScore;
            PlaneSaveSystem.SavePilotData(_initializePlaneProperties.basePropertiesData.planeType, _enhanceSaveData);
            _topAceUI.UpdateTopAceText();
        }
    }
}