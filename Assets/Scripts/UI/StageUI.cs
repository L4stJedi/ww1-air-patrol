using MapMechanics;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StageUI : MonoBehaviour
    {
        private TextMeshProUGUI _stageText;
        private StageProgression _stageProgression;

        private void Awake()
        {
            _stageText = GetComponent<TextMeshProUGUI>();
            _stageProgression = FindAnyObjectByType<StageProgression>();
        }

        private void OnEnable()
        {
            if (_stageProgression == null)
                _stageProgression = FindAnyObjectByType<StageProgression>();

            if (_stageProgression == null)
                return;

            _stageProgression.StageStarted += HandleStageStarted;
            _stageProgression.BossEncounterStarted += HandleBossEncounterStarted;
            _stageProgression.CampaignCompleted += HandleCampaignCompleted;
        }

        private void OnDisable()
        {
            if (_stageProgression == null)
                return;

            _stageProgression.StageStarted -= HandleStageStarted;
            _stageProgression.BossEncounterStarted -= HandleBossEncounterStarted;
            _stageProgression.CampaignCompleted -= HandleCampaignCompleted;
        }

        private void Start()
        {
            UpdateStageUI();
        }

        public void UpdateStageUI()
        {
            if (_stageText == null || _stageProgression == null)
                return;

            var profile = _stageProgression.CurrentProfile;
            _stageText.text = profile == null
                ? $"Stage: {_stageProgression.CurrentStage}"
                : $"Stage: {profile.displayName}";
        }

        private void HandleStageStarted(StageProfile profile)
        {
            UpdateStageUI();
        }
        private void HandleBossEncounterStarted()
        {
            if (_stageText != null)
                _stageText.text = "ACE PATROL SIGHTED";
        }


        private void HandleCampaignCompleted()
        {
            if (_stageText != null)
                _stageText.text = "CAMPAIGN CLEARED";
        }
    }
}
