using System;
using System.Collections;
using UI;
using UnityEngine;

namespace MapMechanics
{
    [Serializable]
    public class StageProfile
    {
        [Header("Identity")]
        public string id = "stage";
        public string displayName = "Stage";
        [TextArea] public string introMessage;

        [Header("Timing")]
        [Min(0f)] public float duration = 30f;
        public bool advanceAutomatically = true;

        [Header("Enemy spawning")]
        public GameObject enemyPrefab;
        [Min(0.1f)] public float enemySpawnInterval = 0.8f;
        [Min(1)] public int enemiesPerSpawn = 1;
        [Range(0, 100)] public int enemySpawnChance = 40;

        [Header("Obstacle spawning")]
        public bool spawnObstacles;
        public GameObject obstaclePrefab;
        [Min(0.1f)] public float obstacleSpawnInterval = 2f;
        [Min(1)] public int obstaclesPerSpawn = 1;
        [Range(0, 100)] public int obstacleSpawnChance = 60;

        [Header("Boss encounter")]
        public GameObject bossPrefab;
        public GameObject escortPrefab;
        public bool requireBossDefeat;

        [Header("Encounter flags")]
        public bool isBossStage;
        public bool completesCampaign;
    }

    public class StageProgression : MonoBehaviour
    {
        public enum Stage
        {
            FirstStage = 1,
            SecondStage,
            ThirdStage,
            FourthStage,
            FifthStage
        }

        public enum BattleState
        {
            Preparing,
            Playing,
            StageTransition,
            Boss,
            Victory,
            Defeat,
            Paused
        }

        [Header("Campaign configuration")]
        [Tooltip("Used when no stage profile supplies a duration.")]
        public float timeUntilNextStage = 30f;
        [Tooltip("A reusable campaign asset. If empty, stageProfiles is used.")]
        public CampaignDefinition campaignDefinition;
        [Tooltip("Profiles are reusable stage data. Leave empty to generate a Belgium-compatible five-stage fallback from Spawner.")]
        public StageProfile[] stageProfiles;
        public bool autoAdvanceFinalStage;
        public bool completeCampaignWhenFinalStageEnds;

        [Header("References")]
        [SerializeField] private StageUI stageUI;
        [SerializeField] private Spawner spawnerRef;

        private Coroutine _stageTimerRoutine;

        public Stage CurrentStage { get; private set; } = Stage.FirstStage;
        public BattleState CurrentState { get; private set; } = BattleState.Preparing;
        public StageProfile CurrentProfile => GetProfile(CurrentStage);
        public StageProfile[] ActiveStageProfiles => campaignDefinition != null && campaignDefinition.StageCount > 0 ? campaignDefinition.stages : stageProfiles;
        public int TotalStages => ActiveStageProfiles == null ? 0 : ActiveStageProfiles.Length;
        public bool IsCampaignComplete => CurrentState == BattleState.Victory;
        public bool BossSpawned { get; private set; }
        public bool BossDefeated { get; private set; }

        public event Action<StageProfile> StageStarted;
        public event Action<StageProfile> StageChangedEvent;
        public event Action CampaignCompleted;
        public event Action<BattleState> BattleStateChanged;
        public event Action BossEncounterStarted;
        public event Action BossEncounterDefeated;

        private void Awake()
        {
            if (stageUI == null)
                stageUI = FindAnyObjectByType<StageUI>();

            if (spawnerRef == null)
                spawnerRef = GetComponent<Spawner>();

            EnsureStageProfiles();
        }

        private void Start()
        {
            BeginStage(Stage.FirstStage, false);
        }

        private void EnsureStageProfiles()
        {
            if (stageProfiles == null || stageProfiles.Length == 0)
                stageProfiles = new StageProfile[5];

            var legacyEnemies = spawnerRef == null ? null : spawnerRef.enemyReference;
            var legacyObstacle = spawnerRef == null ? null : spawnerRef.obstacleReference;
            var legacyObstacleStart = spawnerRef == null ? 2 : spawnerRef.firstStageObstaclesSpawn;

            for (var i = 0; i < stageProfiles.Length; i++)
            {
                if (stageProfiles[i] == null)
                    stageProfiles[i] = new StageProfile();

                var profile = stageProfiles[i];
                if (string.IsNullOrWhiteSpace(profile.id) || profile.id == "stage")
                    profile.id = $"stage-{i + 1}";

                if (string.IsNullOrWhiteSpace(profile.displayName) || profile.displayName == "Stage")
                    profile.displayName = $"Stage {i + 1}";

                if (profile.duration <= 0f)
                    profile.duration = timeUntilNextStage;

                if (profile.enemyPrefab == null && legacyEnemies != null && i < legacyEnemies.Length)
                    profile.enemyPrefab = legacyEnemies[i];

                if (profile.enemySpawnInterval <= 0f)
                    profile.enemySpawnInterval = spawnerRef == null ? 0.8f : spawnerRef.enemySpawnCd;

                if (profile.enemiesPerSpawn <= 0)
                    profile.enemiesPerSpawn = spawnerRef == null ? 1 : Mathf.Max(1, spawnerRef.enemiesPerSpawn);

                if (profile.enemySpawnChance <= 0)
                    profile.enemySpawnChance = spawnerRef == null ? 40 : spawnerRef.chanceForTheEnemyToSpawn;

                if (profile.obstaclePrefab == null)
                    profile.obstaclePrefab = legacyObstacle;

                if (i == 0 && !profile.spawnObstacles)
                    profile.spawnObstacles = false;
                else if (i >= legacyObstacleStart - 1 && legacyObstacle != null)
                    profile.spawnObstacles = true;

                if (profile.obstacleSpawnInterval <= 0f)
                    profile.obstacleSpawnInterval = spawnerRef == null ? 2f : spawnerRef.obstacleSpawnCd;

                if (profile.obstaclesPerSpawn <= 0)
                    profile.obstaclesPerSpawn = spawnerRef == null ? 1 : Mathf.Max(1, spawnerRef.obstaclesPerSpawn);

                if (profile.obstacleSpawnChance <= 0)
                    profile.obstacleSpawnChance = spawnerRef == null ? 60 : spawnerRef.chanceForTheObstacleToSpawn;

                if (i == stageProfiles.Length - 1)
                    profile.isBossStage = true;
            }
        }

        private StageProfile GetProfile(Stage stage)
        {
            var profiles = ActiveStageProfiles;
            if (profiles == null || profiles.Length == 0)
                return null;

            var index = Mathf.Clamp((int)stage - 1, 0, profiles.Length - 1);
            return profiles[index];
        }

        private void BeginStage(Stage stage, bool isTransition)
        {
            CurrentStage = stage;
            BossSpawned = false;
            BossDefeated = false;

            var profile = CurrentProfile;
            SetBattleState(profile != null && profile.isBossStage
                ? BattleState.Boss
                : (isTransition ? BattleState.StageTransition : BattleState.Playing));

            if (stageUI != null)
                stageUI.UpdateStageUI();

            StageStarted?.Invoke(profile);
            if (isTransition)
            if (isTransition && (profile == null || !profile.isBossStage))
                StartCoroutine(ReturnToPlayingState());
                StageChangedEvent?.Invoke(profile);

            if (_stageTimerRoutine != null)
                StopCoroutine(_stageTimerRoutine);

            _stageTimerRoutine = StartCoroutine(StageTimer());
        }
        private IEnumerator ReturnToPlayingState()
        {
            yield return null;
            if (CurrentState == BattleState.StageTransition)
                SetBattleState(BattleState.Playing);
        }


        private IEnumerator StageTimer()
        {
            while (CurrentState != BattleState.Victory && CurrentState != BattleState.Defeat)
            {
                var profile = CurrentProfile;
                if (profile == null)
                    yield break;

                if (!profile.advanceAutomatically || profile.duration <= 0f)
                    yield break;

                if ((int)CurrentStage >= TotalStages && !autoAdvanceFinalStage)
                {
                    if (CanCompleteFinalStage(profile))
                        CompleteCampaign();
                    yield break;
                }

                yield return new WaitForSeconds(profile.duration);

                if (CurrentState == BattleState.Victory || CurrentState == BattleState.Defeat)
                    yield break;

                if ((int)CurrentStage >= TotalStages)
                {
                    if (CanCompleteFinalStage(profile))
                        CompleteCampaign();
                    yield break;
                }

                AdvanceStage();
            }
        }

        private bool CanCompleteFinalStage(StageProfile profile)
        {
            if (completeCampaignWhenFinalStageEnds)
                return true;

            if (!profile.completesCampaign)
                return false;

            return !profile.requireBossDefeat || BossDefeated;
        }

        public void AdvanceStage()
        {
            if (CurrentState == BattleState.Victory || CurrentState == BattleState.Defeat)
                return;

            var nextStageIndex = (int)CurrentStage + 1;
            if (nextStageIndex > TotalStages)
            {
                if (CurrentProfile != null && CanCompleteFinalStage(CurrentProfile))
                    CompleteCampaign();
                return;
            }

            BeginStage((Stage)nextStageIndex, true);
        }

        public void NotifyBossSpawned()
        {
            if (BossSpawned)
                return;

            BossSpawned = true;
            SetBattleState(BattleState.Boss);
            BossEncounterStarted?.Invoke();
        }

        public void NotifyBossDefeated()
        {
            if (!BossSpawned || BossDefeated)
                return;

            BossDefeated = true;
            BossEncounterDefeated?.Invoke();

            if (CurrentProfile != null && CurrentProfile.completesCampaign && CurrentProfile.requireBossDefeat)
                CompleteCampaign();
        }

        public void SetDefeat()
        {
            if (CurrentState == BattleState.Victory)
                return;

            SetBattleState(BattleState.Defeat);
            if (_stageTimerRoutine != null)
                StopCoroutine(_stageTimerRoutine);
        }

        public void CompleteCampaign()
        {
            if (IsCampaignComplete)
                return;

            SetBattleState(BattleState.Victory);
            if (_stageTimerRoutine != null)
                StopCoroutine(_stageTimerRoutine);

            if (spawnerRef != null)
                spawnerRef.StopSpawning();

            CampaignCompleted?.Invoke();
        }

        public void StageChanged()
        {
            if (spawnerRef != null)
                spawnerRef.StageChanged();
        }

        private void SetBattleState(BattleState state)
        {
            if (CurrentState == state)
                return;

            CurrentState = state;
            BattleStateChanged?.Invoke(state);
        }
    }
}
