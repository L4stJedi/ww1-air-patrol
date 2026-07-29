using System.Collections;
using EnemyPlaneScripts;
using SavedVariables.Planes;
using UnityEngine;

namespace MapMechanics
{
    public class Spawner : MonoBehaviour
    {
        [Header("Legacy fallback configuration")]
        public float enemySpawnCd = 5f;
        public int enemiesPerSpawn;
        [Range(0, 100)] public int chanceForTheEnemyToSpawn;

        public int firstStageObstaclesSpawn = 2;
        public float obstacleSpawnCd = 5f;
        public int obstaclesPerSpawn;
        [Range(0, 100)] public int chanceForTheObstacleToSpawn;

        [Header("Legacy fallback references")]
        public GameObject[] enemyReference;
        public GameObject obstacleReference;

        [Header("Faction-aware encounter overrides")]
        [Tooltip("Used for the opening wave when the player flies the Albatros. Leave empty to use the configured stage enemy.")]
        [SerializeField] private GameObject albatrosFirstWaveEnemyPrefab;

        [Header("Runtime references")]
        [SerializeField] private EnemyPool objectPool;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private StageProgression stageProgression;

        private Coroutine _enemySpawnRoutine;
        private Coroutine _obstacleSpawnRoutine;
        private StageProfile _activeProfile;
        private EnemyHp _bossHp;
        private bool _isSpawning;

        private void Awake()
        {
            if (objectPool == null)
                objectPool = GetComponent<EnemyPool>();

            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            if (stageProgression == null)
                stageProgression = GetComponent<StageProgression>();

            _isSpawning = true;
        }

        private void Start()
        {
            if (stageProgression == null)
                stageProgression = FindAnyObjectByType<StageProgression>();

            if (stageProgression != null)
                stageProgression.StageStarted += HandleStageStarted;

            _activeProfile = stageProgression == null ? null : stageProgression.CurrentProfile;
            HandleStageStarted(_activeProfile);

            _enemySpawnRoutine = StartCoroutine(EnemySpawnLoop());
        }

        private void OnDestroy()
        {
            if (stageProgression != null)
                stageProgression.StageStarted -= HandleStageStarted;

            if (_bossHp != null)
                _bossHp.Died -= HandleBossDefeated;
        }

        private void HandleStageStarted(StageProfile profile)
        {
            _activeProfile = profile;

            if (_obstacleSpawnRoutine != null)
            {
                StopCoroutine(_obstacleSpawnRoutine);
                _obstacleSpawnRoutine = null;
            }

            var shouldSpawnObstacles = profile != null
                ? profile.spawnObstacles
                : stageProgression != null && (int)stageProgression.CurrentStage >= firstStageObstaclesSpawn;

            if (shouldSpawnObstacles && _isSpawning)
                _obstacleSpawnRoutine = StartCoroutine(ObstacleSpawnLoop());

            if (profile != null && profile.isBossStage && profile.bossPrefab != null && _isSpawning)
                SpawnBossEncounter(profile);
        }

        private void SpawnBossEncounter(StageProfile profile)
        {
            if (objectPool == null || rb == null || stageProgression == null)
                return;

            var bossPosition = new Vector2(rb.position.x, 3f);
            var boss = SpawnOne(profile.bossPrefab, bossPosition);
            if (boss == null)
                return;

            _bossHp = boss.GetComponent<EnemyHp>();
            if (_bossHp != null)
                _bossHp.Died += HandleBossDefeated;

            stageProgression.NotifyBossSpawned();

            if (profile.escortPrefab != null)
                SpawnOne(profile.escortPrefab, new Vector2(rb.position.x, -3f));
        }

        private void HandleBossDefeated(EnemyHp defeatedBoss)
        {
            if (_bossHp != null)
                _bossHp.Died -= HandleBossDefeated;

            _bossHp = null;
            if (stageProgression != null)
                stageProgression.NotifyBossDefeated();
        }

        public void StageChanged()
        {
            HandleStageStarted(stageProgression == null ? null : stageProgression.CurrentProfile);
        }

        public void StopSpawning()
        {
            _isSpawning = false;

            if (_enemySpawnRoutine != null)
                StopCoroutine(_enemySpawnRoutine);

            if (_obstacleSpawnRoutine != null)
                StopCoroutine(_obstacleSpawnRoutine);

            _enemySpawnRoutine = null;
            _obstacleSpawnRoutine = null;
        }

        public void ResumeSpawning()
        {
            if (_isSpawning)
                return;

            _isSpawning = true;
            _activeProfile = stageProgression == null ? null : stageProgression.CurrentProfile;
            HandleStageStarted(_activeProfile);
            _enemySpawnRoutine = StartCoroutine(EnemySpawnLoop());
        }

        private IEnumerator EnemySpawnLoop()
        {
            while (_isSpawning)
            {
                var profile = stageProgression == null ? _activeProfile : stageProgression.CurrentProfile;
                var enemyPrefab = ResolveEnemyPrefab(profile);

                var interval = profile != null ? profile.enemySpawnInterval : enemySpawnCd;
                var amount = profile != null ? profile.enemiesPerSpawn : enemiesPerSpawn;
                var chance = profile != null ? profile.enemySpawnChance : chanceForTheEnemyToSpawn;

                if (enemyPrefab != null && Roll(chance))
                    Spawn(Mathf.Max(1, amount), enemyPrefab);

                yield return new WaitForSeconds(Mathf.Max(0.1f, interval));
            }
        }

        private GameObject ResolveEnemyPrefab(StageProfile profile)
        {
            var enemyPrefab = profile != null && profile.enemyPrefab != null
                ? profile.enemyPrefab
                : GetLegacyEnemyPrefab();

            var isOpeningStage = stageProgression != null
                && stageProgression.CurrentStage == StageProgression.Stage.FirstStage;

            if (isOpeningStage
                && CurrentPlane.EquippedPlane == ListOfPlanes.Planes.AlbatrosDV
                && albatrosFirstWaveEnemyPrefab != null)
            {
                return albatrosFirstWaveEnemyPrefab;
            }

            return enemyPrefab;
        }

        private IEnumerator ObstacleSpawnLoop()
        {
            while (_isSpawning)
            {
                var profile = stageProgression == null ? _activeProfile : stageProgression.CurrentProfile;
                var obstaclePrefab = profile != null && profile.obstaclePrefab != null
                    ? profile.obstaclePrefab
                    : obstacleReference;

                var interval = profile != null ? profile.obstacleSpawnInterval : obstacleSpawnCd;
                var amount = profile != null ? profile.obstaclesPerSpawn : obstaclesPerSpawn;
                var chance = profile != null ? profile.obstacleSpawnChance : chanceForTheObstacleToSpawn;

                if (obstaclePrefab != null && Roll(chance))
                    Spawn(Mathf.Max(1, amount), obstaclePrefab);

                yield return new WaitForSeconds(Mathf.Max(0.1f, interval));
            }
        }

        private GameObject GetLegacyEnemyPrefab()
        {
            if (stageProgression == null || enemyReference == null || enemyReference.Length == 0)
                return null;

            var index = Mathf.Clamp((int)stageProgression.CurrentStage - 1, 0, enemyReference.Length - 1);
            return enemyReference[index];
        }

        private static bool Roll(int chance)
        {
            return chance > 0 && Random.Range(1, 101) <= Mathf.Clamp(chance, 0, 100);
        }

        private void Spawn(int objectsPerSpawn, GameObject prefab)
        {
            if (objectPool == null || rb == null || prefab == null)
                return;

            for (var i = 0; i < objectsPerSpawn; i++)
            {
                var randomY = Random.Range(-8f, 11f);
                var safetyNet = 0;

                while (Mathf.Abs(randomY - transform.position.y) <= 2f && safetyNet < 10)
                {
                    randomY = Random.Range(-8f, 11f);
                    safetyNet++;
                }

                SpawnOne(prefab, new Vector2(rb.position.x, randomY));
            }
        }

        private GameObject SpawnOne(GameObject prefab, Vector2 spawnPosition)
        {
            if (objectPool == null || prefab == null)
                return null;

            return objectPool.GetObjectFromPool(prefab, spawnPosition);
        }
    }
}
