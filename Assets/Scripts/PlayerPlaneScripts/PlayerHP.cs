using System;
using System.Collections;
using EnemyPlaneScripts;
using SavedVariables.PilotUpgrades;
using UnityEngine;

namespace PlayerPlaneScripts
{
    public class PlayerHp : MonoBehaviour
    {
        [Header("Configuration")]
        public int maxHp = 3;
        public int Hp { get; private set; }
        public float invincibilityTime = 2f;
        public float shield;

        private PlayerDeath _deathRef;
        [NonSerialized] public bool isInvincible;
        private PilotStatData _pilotStatData;

        // Adds pilot skill bonuses to vars and sets current hp to max hp value
        private void Start()
        {
            _deathRef = GetComponent<PlayerDeath>();
            _pilotStatData = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID).statData;
                
            maxHp = maxHp + _pilotStatData.hpBonus;
            invincibilityTime += _pilotStatData.invincibilityDurationBonus;
            shield = _pilotStatData.shieldBonus;
            
            Hp = maxHp;
        }

        // Constantly checks, if player hp didnt exceed the max value and if yes than clamps it
        private void Update()
        {
            Hp = Mathf.Min(Hp, maxHp);
        }

        // On collision with different object player TakeDamage() is called
        private void OnTriggerEnter2D(Collider2D col)
        {
            switch (col.gameObject.tag)
            {
                case "Enemy":
                    TakeDamage(1);
                    var enemyHp = col.GetComponent<EnemyHp>();
                    if (enemyHp == null) return;
                    enemyHp.Hp--;
                    break;
                case "Obstacle":
                    TakeDamage(1);
                    break;
                case "Ground":
                    _deathRef.Die();
                    break;
                case "Roof":
                    TakeDamage(1);
                    break;

            }
        }

        // If player isn't invincible, he takes damage and becomes invincible for short period
        public void TakeDamage(int dmg)
        {
            if (isInvincible) return;

            Hp -= dmg;
            isInvincible = true;
            StartCoroutine(InvincibilityCountdown());
            HapticWrapper.Feedback(HapticType.Light);

            if (Hp <= 0)
            {
                _deathRef.Die();
                return;
            }
            
            StartCoroutine(DoHitStop(0.075f));
        }

        public void Heal()
        {
            Hp ++;
        }
        
        // Timer handling when player becomes vincible again
        private IEnumerator InvincibilityCountdown()
        {
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
        }

        private IEnumerator DoHitStop(float duration)
        {
            var previousTimeScale = Time.timeScale;
            Time.timeScale = Mathf.Min(previousTimeScale, 0.1f);
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = previousTimeScale;
        }
    }
}
