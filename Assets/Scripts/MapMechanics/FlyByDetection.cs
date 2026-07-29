using System;
using UI;
using UnityEngine;

namespace MapMechanics
{
    public class FlyByDetection : MonoBehaviour
    {

        [Header("Configuration")] 
        [SerializeField] private BonusOrPenalty giveBonusOrPenalty = BonusOrPenalty.Bonus;
        public int changeByValue = 1;
        
        private bool _scoreAlreadyChangedValue;
        private Score _scoreRef;

        private enum BonusOrPenalty
        {
            Bonus,
            Penalty
        }
        
        
        //set score references
        private void Start()
        {
            _scoreRef = FindAnyObjectByType<Score>();
        }

        //when player passes by and the enemy isn't destroyed his score is reduced
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            //if player has more colliders score is still decreased once
            if (_scoreAlreadyChangedValue) return;
            _scoreAlreadyChangedValue = true;

            switch (giveBonusOrPenalty)
            {
                case BonusOrPenalty.Bonus:
                    _scoreRef.ChangeScoreValueBy(changeByValue);
                    break;
                
                case BonusOrPenalty.Penalty:
                    _scoreRef.ChangeScoreValueBy(-changeByValue);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
