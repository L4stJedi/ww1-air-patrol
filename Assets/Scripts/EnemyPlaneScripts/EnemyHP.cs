using System;
using PlayerPlaneScripts;
using UI;
using UnityEngine;
using UnityEngine.Pool;

namespace EnemyPlaneScripts
{
    public class EnemyHp : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int maxHp = 1;

        public int scoreBonus = 3;
        public int killCountBonus = 1;

        private IObjectPool<GameObject> _poolRef;
        private Score _scoreRef;
        private Xp _xpRef;
        private int _hp;

        public int Hp
        {
            get => _hp;
            set
            {
                _hp = Mathf.Clamp(value, 0, maxHp);
                if (_hp <= 0)
                    EnemyDeath();
            }
        }

        public event Action<EnemyHp> Died;

        private void OnEnable()
        {
            _hp = maxHp;
        }

        public void SetPool(IObjectPool<GameObject> pool)
        {
            _poolRef = pool;
        }

        private void Awake()
        {
            _scoreRef = FindAnyObjectByType<Score>();
            _xpRef = FindAnyObjectByType<Xp>();
        }

        private void EnemyDeath()
        {
            Died?.Invoke(this);

            if (_scoreRef != null)
                _scoreRef.ChangeScoreValueBy(scoreBonus);

            if (_xpRef != null)
                _xpRef.killCount += killCountBonus;

            if (_poolRef != null)
                _poolRef.Release(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
